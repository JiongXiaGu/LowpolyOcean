using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{
    /// <summary>
    /// unity standard shader field
    /// </summary>
    public class ShaderField : ShaderFieldBase, IShaderStandardFiled, IShaderFieldGroup
    {
        private static readonly IShaderField[] formatAccessors = new IShaderField[]
        {
            new ColorAccessor(),
            new ColorArrayAccessor(),
            new Vector2Accessor(),
            new Vector3Accessor(),
            new Vector4Accessor(),
            new Vector4ArrayAccessor(),
            new FloatAccessor(),
            new FloatArrayAccessor(),
            new TextureAccessor(),
            new TextureOffsetAccessor(),
            new TextureScaleAccessor(),
            new MatrixAccessor(),
            new MatrixArrayAccessor(),
            new KeywordAccessor(),
            new IntAccessor(),
            new EnumAccessor(),
        };
        public static ShaderFieldFormat SupportFormats { get; } = GetShaderFieldFormat();

        public string ShaderFieldName { get; }
        public int ShderFieldID { get; }
        public IShaderField FormatAccessor { get; }
        public override ShaderFieldFormat Format => FormatAccessor.Format;
        List<IShaderFieldGroup> IShaderFieldGroup.Children => null;

        public ShaderField(string name, int mask, ShaderFieldFormat format, IReflectiveField fieldAccessor) : base(mask, fieldAccessor)
        {
            ShaderFieldName = name;
            ShderFieldID = Shader.PropertyToID(name);
            FormatAccessor = GetFormatAccessor(format);
        }

        public ShaderField(string name, int mask, IReflectiveField fieldAccessor, IShaderField formatAccessor) : base(mask, fieldAccessor)
        {
            ShaderFieldName = name;
            ShderFieldID = Shader.PropertyToID(name);
            FormatAccessor = formatAccessor;
        }

        private static ShaderFieldFormat GetShaderFieldFormat()
        {
            ShaderFieldFormat formats = ShaderFieldFormat.None;
            foreach (var item in formatAccessors)
            {
                formats |= item.Format;
            }
            return formats;
        }

        public override string ToString()
        {
            return "Name:" + ReflectiveField.Name + ", Mask:" + Mask + " ,Shader:" + ShaderFieldName + ", Format:" + Format;
        }

        public static IShaderField GetFormatAccessor(ShaderFieldFormat format)
        {
            foreach (var formatAccessor in formatAccessors)
            {
                if (formatAccessor.Format == format)
                {
                    return formatAccessor;
                }
            }

            throw new ArgumentOutOfRangeException(format.ToString());
        }

        public override object GetGlobalValues(object dest)
        {
            var value = FormatAccessor.GetGlobalValues(this);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void SetGlobalValues(object source)
        {
            var value = ReflectiveField.GetValue(source);
            FormatAccessor.SetGlobalValues(this, value);
        }

        public override object CopyWithoutKeywords(object source, object dest)
        {
            var value = ReflectiveField.GetValue(source);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void CopyWithoutKeywords(Material source, Material dest)
        {
            var value = FormatAccessor.GetValue(this, source);
            FormatAccessor.SetMaterial(this, dest, value);
        }

        public override object CopyWithoutKeywords(Material source, object dest)
        {
            var value = FormatAccessor.GetValue(this, source);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void CopyWithoutKeywords(object source, Material dest)
        {
            var value = ReflectiveField.GetValue(source);
            FormatAccessor.SetMaterial(this, dest, value);
        }

        public override object Copy(object source, object dest)
        {
            return CopyWithoutKeywords(source, dest);
        }

        public override void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
            CopyWithoutKeywords(source, dest);
        }

        public override object Copy(Material source, object dest)
        {
            return CopyWithoutKeywords(source, dest);
        }

        public override void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            CopyWithoutKeywords(source, dest);
        }

        public override object Lerp(object v0, object v1, object dest, float t)
        {
            var value0 = ReflectiveField.GetValue(v0);
            var value1 = ReflectiveField.GetValue(v1);
            var valueDest = FormatAccessor.Lerp(value0, value1, t);
            ReflectiveField.SetValue(dest, valueDest);
            return dest;
        }

        public override int GetEnabledKeywords()
        {
            return 0;
        }

        public override int GetEnabledKeywords(ICollection<string> keywords)
        {
            return 0;
        }

        public override int GetEnabledKeywords(object source)
        {
            return 0;
        }

        public override int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            return 0;
        }

        public override int GetEnabledKeywords(Material source)
        {
            return 0;
        }

        public override int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            return 0;
        }

        #region FormatAccessors

        public static object LerpArray<T>(T[] array0, T[] array1, Func<T, T, T> lerp)
        {
            int lenght = Math.Min(array0.Length, array1.Length);
            var dest = new T[lenght];
            for (int i = 0; i < lenght; i++)
            {
                dest[i] = lerp(array0[i], array1[i]);
            }
            return dest;
        }

        public static object LerpArray<T>(object value0, object value1, Func<T, T, T> lerp)
        {
            if (value0 == null || value1 == null)
                return null;

            var array0 = value0 as T[];
            var array1 = value1 as T[];
            return LerpArray(array0, array1, lerp);
        }

        public static object LerpClass(object v0, object v1, float t)
        {
            return v0;
        }

        public class ColorAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Color;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalColor(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalColor(info.ShderFieldID, (Color)value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetColor(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetColor(info.ShderFieldID, (Color)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var value0 = (Color)v0;
                var value1 = (Color)v1;
                return Color.Lerp(value0, value1, t);
            }
        }

        public class ColorArrayAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.ColorArray;

            public object GetGlobalValues(ShaderField info)
            {
                throw new NotSupportedException();
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                throw new NotSupportedException();
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetColorArray(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetColorArray(info.ShderFieldID, (Color[])value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpArray<Color>(v0, v1, (value0, value1) => Color.Lerp(value0, value1, t));
            }
        }

        public class Vector2Accessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Vector2;

            public object GetGlobalValues(ShaderField info)
            {
                Vector4 value = Shader.GetGlobalVector(info.ShderFieldID);
                return new Vector2(value.x, value.y);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                var vecto2 = (Vector2)value;
                Shader.SetGlobalVector(info.ShderFieldID, vecto2);
            }

            public object GetValue(ShaderField info, Material material)
            {
                Vector4 value = material.GetVector(info.ShderFieldID);
                return new Vector2(value.x, value.y);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                var vecto2 = (Vector2)value;
                material.SetVector(info.ShderFieldID, vecto2);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var value0 = (Vector2)v0;
                var value1 = (Vector2)v1;
                return Vector2.Lerp(value0, value1, t);
            }
        }

        public class Vector3Accessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Vector3;

            public object GetGlobalValues(ShaderField info)
            {
                Vector4 value = Shader.GetGlobalVector(info.ShderFieldID);
                return new Vector3(value.x, value.y, value.z);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Vector3 vector3 = (Vector3)value;
                Shader.SetGlobalVector(info.ShderFieldID, vector3);
            }

            public object GetValue(ShaderField info, Material material)
            {
                Vector4 value = material.GetVector(info.ShderFieldID);
                return new Vector3(value.x, value.y, value.z);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                Vector3 vector3 = (Vector3)value;
                material.SetVector(info.ShderFieldID, vector3);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var value0 = (Vector3)v0;
                var value1 = (Vector3)v1;
                return Vector3.Lerp(value0, value1, t);
            }
        }

        public class Vector4Accessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Vector4;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalVector(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalVector(info.ShderFieldID, (Vector4)value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetVector(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetVector(info.ShderFieldID, (Vector4)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var value0 = (Vector4)v0;
                var value1 = (Vector4)v1;
                return Vector4.Lerp(value0, value1, t);
            }
        }

        public class Vector4ArrayAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Vector4Array;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalVectorArray(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalVectorArray(info.ShderFieldID, (Vector4[])value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetVectorArray(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetVectorArray(info.ShderFieldID, (Vector4[])value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpArray<Vector4>(v0, v1, (t1, t2) => Vector4.Lerp(t1, t1, t));
            }
        }

        public class FloatAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Float;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalFloat(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalFloat(info.ShderFieldID, (float)value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetFloat(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetFloat(info.ShderFieldID, (float)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var t1 = (float)v0;
                var t2 = (float)v1;
                return Mathf.Lerp(t1, t2, t);
            }
        }

        public class FloatArrayAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.FloatArray;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalFloatArray(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalFloatArray(info.ShderFieldID, (float[])value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetFloatArray(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetFloatArray(info.ShderFieldID, (float[])value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpArray<float>(v0, v1, (t0, t1) => Mathf.Lerp(t0, t1, t));
            }
        }

        public class TextureAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Texture;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalTexture(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalTexture(info.ShderFieldID, (Texture)value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetTexture(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetTexture(info.ShderFieldID, (Texture)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpClass(v0, v1, t);
            }
        }

        public class TextureOffsetAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.TextureOffset;

            public object GetGlobalValues(ShaderField info)
            {
                return default(Vector2);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetTextureOffset(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetTextureOffset(info.ShderFieldID, (Vector2)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var t1 = (Vector2)v0;
                var t2 = (Vector2)v1;
                return Vector2.Lerp(t1, t2, t);
            }
        }

        public class TextureScaleAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.TextureScale;

            public object GetGlobalValues(ShaderField info)
            {
                return default(Vector2);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetTextureScale(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetTextureScale(info.ShderFieldID, (Vector2)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                var t1 = (Vector2)v0;
                var t2 = (Vector2)v1;
                return Vector2.Lerp(t1, t2, t);
            }
        }

        public class MatrixAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Matrix;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalMatrix(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalMatrix(info.ShderFieldID, (Matrix4x4)value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetMatrix(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetMatrix(info.ShderFieldID, (Matrix4x4)value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpClass(v0, v1, t);
            }
        }

        public class MatrixArrayAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.MatrixArray;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalMatrixArray(info.ShderFieldID);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                Shader.SetGlobalMatrixArray(info.ShderFieldID, (Matrix4x4[])value);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetMatrixArray(info.ShderFieldID);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                material.SetMatrixArray(info.ShderFieldID, (Matrix4x4[])value);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpClass(v0, v1, t);
            }
        }

        public class IntAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Int;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalInt(info.ShaderFieldName);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                int intValue = (int)value;
                Shader.SetGlobalInt(info.ShaderFieldName, intValue);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetInt(info.ShaderFieldName);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                var intValue = (int)value;
                material.SetInt(info.ShaderFieldName, intValue);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return v0;
            }
        }

        public class EnumAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Enum;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.GetGlobalInt(info.ShaderFieldName);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                int intValue = (int)value;
                Shader.SetGlobalInt(info.ShaderFieldName, intValue);
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.GetInt(info.ShaderFieldName);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                var intValue = (int)value;
                material.SetInt(info.ShaderFieldName, intValue);
            }

            public object Lerp(object v0, object v1, float t)
            {
                return v0;
            }
        }

        public class KeywordAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Keyword;

            public object GetGlobalValues(ShaderField info)
            {
                return Shader.IsKeywordEnabled(info.ShaderFieldName);
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                bool isEnable = (bool)value;
                if (isEnable)
                {
                    Shader.EnableKeyword(info.ShaderFieldName);
                }
                else
                {
                    Shader.DisableKeyword(info.ShaderFieldName);
                }
            }

            public object GetValue(ShaderField info, Material material)
            {
                return material.IsKeywordEnabled(info.ShaderFieldName);
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                bool isEnable = (bool)value;
                if (isEnable)
                {
                    material.EnableKeyword(info.ShaderFieldName);
                }
                else
                {
                    material.DisableKeyword(info.ShaderFieldName);
                }
            }

            public object Lerp(object v0, object v1, float t)
            {
                return LerpClass(v0, v1, t);
            }
        }

        #endregion
    }

}
