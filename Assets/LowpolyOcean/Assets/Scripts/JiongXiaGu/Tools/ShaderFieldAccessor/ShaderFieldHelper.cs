using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{
    public static class ShaderFieldHelper
    {
        #region Reflection

        public static List<IShaderFieldGroup> Find(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var parameters = new List<IShaderFieldGroup>();
            InternalFind(type, parameters);

            return parameters;
        }

        private static IEnumerable<IReflectiveField> EnumerateFields(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                if (!field.IsInitOnly)
                {
                    var fieldAccessor = new FieldMember(field);
                    yield return fieldAccessor;
                }
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                if (property.CanRead && property.GetMethod.IsPublic && property.CanWrite && property.SetMethod.IsPublic)
                {
                    var propertyAccessor = new PropertyMember(property);
                    yield return propertyAccessor;
                }
            }
        }

        private static void InternalFind(Type type, ICollection<IShaderFieldGroup> parameters)
        {
            var fields = EnumerateFields(type);
            foreach (var field in fields)
            {
                string name;
                ShaderFieldFormat format;
                Type fieldType = field.FieldType;
                string fieldName = field.Name;

                var attributes = field.MemberInfo.GetCustomAttributes();

                foreach (var attribute in attributes)
                {
                    if (attribute is ObsoleteAttribute)
                    {
                        goto NextField;
                    }
                }

                foreach (var attribute in attributes)
                {
                    if (attribute is ShaderFieldBaseAttribute)
                    {
                        if (attribute is ShaderFieldAttribute)
                        {
                            var att = (ShaderFieldAttribute)attribute;
                            name = att.Name;

                            var customClass = fieldType.GetCustomAttribute<ShaderCustomFieldClassAttribute>();
                            if (customClass != null)
                            {
                                var methods = fieldType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                                foreach (var method in methods)
                                {
                                    var customMethod = method.GetCustomAttribute<ShaderCustomFieldMethodAttribute>();
                                    if (customMethod != null)
                                    {
                                        IShaderField customField = (IShaderField)method.Invoke(null, null);
                                        if (customField == null)
                                            throw new ArgumentException("Custom method return null!" + method.ToString());

                                        var parameter = new ShaderCustomField(name, att.Mask, field, customField);
                                        parameters.Add(parameter);
                                    }
                                }
                            } 
                            else if (TryGetFormat(ref name, fieldType, out format))
                            {
                                var parameter = new ShaderField(name, att.Mask, format, field);
                                parameters.Add(parameter);
                            }
                        }
                        else if (attribute is ShaderFieldKeywordAttribute)
                        {
                            var att = (ShaderFieldKeywordAttribute)attribute;
                            if (typeof(bool) == fieldType)
                            {
                                ShaderKeyword parameter = new ShaderKeyword(att.Keyword, field, att.ShortName, att.Mask);
                                parameters.Add(parameter);
                            }
                        }
                        else if (attribute is ShaderFieldEnumKeywordAttribute)
                        {
                            var att = (ShaderFieldEnumKeywordAttribute)attribute;
                            if (typeof(Enum).IsAssignableFrom(fieldType))
                            {
                                var flagsAtt = fieldType.GetCustomAttribute<FlagsAttribute>();
                                if (flagsAtt == null)
                                {
                                    var item = new ShaderEnumKeyword(att.Mask, field, att.Keywords);
                                    parameters.Add(item);
                                }
                                else
                                {
                                    var item = new ShaderEnumFlagsKeyword(att.Mask, field, att.Keywords);
                                    parameters.Add(item);
                                }
                            }
                        }
                        else if (attribute is ShaderFieldMarkAttribute)
                        {
                            var att = (ShaderFieldMarkAttribute)attribute;
                            var item = new ShaderFieldMark(att.Mask, field);
                            parameters.Add(item);
                        }

                        goto NextField;
                    }
                }

                var structAttribute = field.FieldType.GetCustomAttribute<ShaderFieldGroupAttribute>();
                if (structAttribute != null)
                {
                    var group = new ShaderFieldGroup(field, structAttribute.Mask);
                    InternalFind(fieldType, group.Children);
                    if (group.Children.Count > 0)
                    {
                        parameters.Add(group);
                    }
                    goto NextField;
                }

                NextField:
                continue;
            }
        }

        #endregion

        #region Format

        public const string TextureOffsetKeyword = "Offset";
        public const string TextureScaleKeyword = "Scale";

        /// <summary>
        /// 支持的类型;
        /// </summary>
        public static IEnumerable<Type> SupportedTypes => formatTypes.Select(item => item.TargetType);

        /// <summary>
        /// 类型对应的着色器参数类型(判断顺序前往后);
        /// </summary>
        private static readonly IFormatConvert[] formatTypes = new IFormatConvert[]
        {
            new FormatByEquals(typeof(float), ShaderFieldFormat.Float),
            new FormatByEquals(typeof(Color), ShaderFieldFormat.Color),

            new FormatBySuffixGroup(typeof(Vector2),
                new FormatBySuffix(TextureOffsetKeyword, ShaderFieldFormat.TextureOffset),
                new FormatBySuffix(TextureScaleKeyword, ShaderFieldFormat.TextureScale),
                new FormatBySuffix(ShaderFieldFormat.Vector2)),

            new FormatByEquals(typeof(Vector3), ShaderFieldFormat.Vector3),
            new FormatByEquals(typeof(Vector4), ShaderFieldFormat.Vector4),
            new FormatByEquals(typeof(Texture), ShaderFieldFormat.Texture),
            new FormatByEquals(typeof(Matrix4x4), ShaderFieldFormat.Matrix),
            new FormatByEquals(typeof(bool), ShaderFieldFormat.Keyword),
            new FormatByEquals(typeof(float[]), ShaderFieldFormat.FloatArray),
            new FormatByEquals(typeof(Color[]), ShaderFieldFormat.ColorArray),
            new FormatByEquals(typeof(Vector4[]), ShaderFieldFormat.Vector4Array),
            new FormatByEquals(typeof(Matrix4x4[]), ShaderFieldFormat.MatrixArray),
            new FormatByEquals(typeof(int), ShaderFieldFormat.Int),
            new FormatByEquals(typeof(Enum), ShaderFieldFormat.Enum),
        };

        /// <summary>
        /// 获取到C#类型所表示的着色器参数类型,若未知格式则返回false,确定格式则返回true;
        /// </summary>
        private static bool TryGetFormat(ref string name, Type type, out ShaderFieldFormat format)
        {
            foreach (var pair in formatTypes)
            {
                if (pair.TryGetFromat(ref name, type, out format))
                {
                    if (format == ShaderFieldFormat.None)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            format = ShaderFieldFormat.None;
            return false;
        }

        private interface IFormatConvert
        {
            Type TargetType { get; }
            bool TryGetFromat(ref string name, Type type, out ShaderFieldFormat format);
        }

        /// <summary>
        /// 判断是否类型相同;
        /// </summary>
        private class FormatByEquals : IFormatConvert
        {
            public Type TargetType { get; }
            public ShaderFieldFormat Format { get; }

            public FormatByEquals(Type targetType, ShaderFieldFormat format)
            {
                TargetType = targetType;
                Format = format;
            }

            public bool TryGetFromat(ref string name, Type type, out ShaderFieldFormat format)
            {
                if(TargetType.IsAssignableFrom(type))
                {
                    format = Format;
                    return true;
                }
                format = ShaderFieldFormat.None;
                return false;
            }
        }

        /// <summary>
        /// 通过名称后缀判断属于的类型;
        /// </summary>
        private class FormatBySuffixGroup : IFormatConvert
        {
            public Type TargetType { get; }
            public FormatBySuffix[] KeywordPairs { get; }

            public FormatBySuffixGroup(Type targetType, params FormatBySuffix[] keywordPairs)
            {
                TargetType = targetType;
                KeywordPairs = keywordPairs;
            }

            public bool TryGetFromat(ref string name, Type type, out ShaderFieldFormat format)
            {
                if (type == TargetType)
                {
                    foreach (var pair in KeywordPairs)
                    {
                        if (name.EndsWith(pair.Suffix, StringComparison.Ordinal))
                        {
                            name = name.Remove(name.Length - pair.Suffix.Length, pair.Suffix.Length);
                            format = pair.Format;
                            return true;
                        }
                    }
                }

                format = ShaderFieldFormat.None;
                return false;
            }
        }

        private struct FormatBySuffix
        {
            public string Suffix;
            public ShaderFieldFormat Format;

            public FormatBySuffix(ShaderFieldFormat format)
            {
                Suffix = string.Empty;
                Format = format;
            }

            public FormatBySuffix(string keyword, ShaderFieldFormat format)
            {
                Suffix = keyword;
                Format = format;
            }
        }

        #endregion

        #region Expansions

        public static bool Contains(this IShaderFieldGroup group, string name)
        {
            if (group.Children != null)
            {
                int index = group.Children.FindIndex(item => item.ReflectiveField.Name == name);
                return index >= 0;
            }
            return false;
        }

        public static IShaderFieldGroup Find(this IShaderFieldGroup group, string name)
        {
            if (group.Children != null)
            {
                int index = group.Children.FindIndex(item => item.ReflectiveField.Name == name);
                if (index >= 0)
                {
                    var field = group.Children[index];
                    return field;
                }
            }
            throw new KeyNotFoundException(name);
        }

        public static ShaderAccessor CreateAccessor(this IShaderFieldGroup group)
        {
            if (group.Children == null)
                throw new InvalidOperationException(nameof(group) + " do not has any child");

            return new ShaderAccessor(group.ReflectiveField.FieldType, group.Children, group.Mask);
        }

        public static ShaderAccessor CreateAccessor(this IShaderFieldGroup group, string name)
        {
            return group.Find(name).CreateAccessor();
        }

        #endregion
    }

}
