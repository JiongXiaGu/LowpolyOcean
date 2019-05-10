using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    public class SerializedPropertyDrawer
    {
        public IShaderFieldGroup Group { get; }
        public SerializedProperty SerializedProperty { get; }
        public List<Drawer> Drawers { get; }

        public SerializedPropertyDrawer(IShaderFieldGroup group, SerializedProperty property)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            Group = group;
            SerializedProperty = property;
            Drawers = new List<Drawer>(group.Children.Count);
            FindDrawers(Drawers, group, property);
        }

        public void FindDrawers(List<Drawer> drawers, IShaderFieldGroup group, SerializedProperty property)
        {
            foreach (var child in group.Children)
            {
                if (child is ShaderField)
                {
                    var field = (ShaderField)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new ShaderFieldDrawer(target, field);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderKeyword)
                {
                    var field = (ShaderKeyword)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new KeywordDrawer(target, field);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderEnumKeyword)
                {
                    var field = (ShaderEnumKeyword)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new EnumKeywordDrawer(target, field);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderEnumFlagsKeyword)
                {
                    var field = (ShaderEnumFlagsKeyword)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new EnumFlagsKeywordDrawer(target, field);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderFieldMark)
                {
                    var field = (ShaderFieldMark)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new Drawer(target);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderCustomField)
                {
                    var field = (ShaderCustomField)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new Drawer(target);
                        drawers.Add(drawer);
                    }
                }
                else if (child is ShaderFieldGroup)
                {
                    var field = (ShaderFieldGroup)child;
                    var target = property.FindPropertyRelative(field.ReflectiveField.Name);
                    if (target != null)
                    {
                        var drawer = new GroupDrawer(target, field);
                        drawers.Add(drawer);
                        FindDrawers(drawer.Drawers, field, target);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public void OnGUI(int mask)
        {
            SerializedProperty.isExpanded = EditorGUILayout.Foldout(SerializedProperty.isExpanded, SerializedProperty.displayName);

            if (SerializedProperty.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    foreach (var drawer in Drawers)
                    {
                        drawer.OnGUI(mask);
                    }
                }
            }
        }

        public void OnGUI(Rect position, int mask)
        {
            SerializedProperty.isExpanded = EditorGUI.Foldout(position, SerializedProperty.isExpanded, SerializedProperty.displayName);
            EditorHelper.AddOneLine(ref position);

            if (SerializedProperty.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    foreach (var drawer in Drawers)
                    {
                        drawer.OnGUI(position, mask);
                        position.y += drawer.GetPropertyHeight(mask);
                    }
                }
            }
        }

        public float GetPropertyHeight(int mask)
        {
            float height = EditorGUIUtility.singleLineHeight;
            foreach (var drawer in Drawers)
            {
                height += drawer.GetPropertyHeight(mask);
            }
            return height;
        }

        public int Extract()
        {
            var mask = 0;
            foreach (var drawer in Drawers)
            {
                drawer.Extract(ref mask);
            }
            return mask;
        }

        public Drawer Find(params string[] path)
        {
            List<Drawer> list = Drawers;
            Drawer drawer = null;

            foreach (var name in path)
            {
                var index = list.FindIndex(item => item.SerializedProperty.name == name);
                if (index >= 0)
                {
                    var current = list[index];
                    if (current is GroupDrawer)
                    {
                        list = ((GroupDrawer)current).Drawers;
                        drawer = current;
                        continue;
                    }
                }
            }

            return drawer;
        }

        public bool ChangeDrawer(Func<Drawer> getDrawer, params string[] path)
        {
            if (getDrawer == null)
                throw new ArgumentNullException(nameof(getDrawer));

            List<Drawer> list = Drawers;

            for (int i = 0; i < path.Length; i++)
            {
                var name = path[i];
                var index = list.FindIndex(item => item.SerializedProperty.name == name);
                if (index >= 0)
                {
                    if (i + 1 >= path.Length)
                    {
                        list[index] = getDrawer();
                        return true;
                    }

                    var current = list[index];
                    if (current is GroupDrawer)
                    {
                        list = ((GroupDrawer)current).Drawers;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public class Drawer
        {
            public SerializedProperty SerializedProperty { get; }

            public Drawer(SerializedProperty serializedProperty)
            {
                SerializedProperty = serializedProperty;
            }

            public virtual float GetPropertyHeight(int mask)
            {
                return EditorGUI.GetPropertyHeight(SerializedProperty, true);
            }

            public virtual void OnGUI(Rect position, int mask)
            {
                EditorGUI.PropertyField(position, SerializedProperty, true);
            }

            public virtual void OnGUI(int mask)
            {
                EditorGUILayout.PropertyField(SerializedProperty, true);
            }

            public virtual void Extract(ref int mask)
            {
            }
        }

        public class GroupDrawer : Drawer
        {
            public ShaderFieldGroup Group { get; private set; }
            public List<Drawer> Drawers { get; private set; }

            public GroupDrawer(SerializedProperty serializedProperty, ShaderFieldGroup group) : base(serializedProperty)
            {
                Group = group;
                Drawers = new List<Drawer>(group.Children.Count);
            }

            public override void OnGUI(int mask)
            {
                if ((mask & Group.Mask) != 0)
                {
                    SerializedProperty.isExpanded = EditorGUILayout.Foldout(SerializedProperty.isExpanded, SerializedProperty.displayName);

                    if (SerializedProperty.isExpanded)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            foreach (var drawer in Drawers)
                            {
                                drawer.OnGUI(mask);
                            }
                        }
                    }
                }
            }

            public override float GetPropertyHeight(int mask)
            {
                float height = 0;

                if ((mask & Group.Mask) != 0)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    if (SerializedProperty.isExpanded)
                    {
                        foreach (var drawer in Drawers)
                        {
                            height += drawer.GetPropertyHeight(mask);
                        }
                    }
                }

                return height;
            }

            public override void OnGUI(Rect position, int mask)
            {
                if ((mask & Group.Mask) != 0)
                {
                    SerializedProperty.isExpanded = EditorGUI.Foldout(position, SerializedProperty.isExpanded, SerializedProperty.displayName);
                    EditorHelper.AddOneLine(ref position);

                    if (SerializedProperty.isExpanded)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            foreach (var drawer in Drawers)
                            {
                                drawer.OnGUI(position, mask);
                                position.y += drawer.GetPropertyHeight(mask);
                            }
                        }
                    }
                }
            }

            public override void Extract(ref int mask)
            {
                foreach (var drawer in Drawers)
                {
                    drawer.Extract(ref mask);
                }
            }
        }

        public class ShaderFieldDrawer : Drawer
        {
            public ShaderFieldBase ShaderField { get; }

            public ShaderFieldDrawer(SerializedProperty serializedProperty, ShaderFieldBase shaderField) : base(serializedProperty)
            {
                ShaderField = shaderField;
            }

            public override float GetPropertyHeight(int mask)
            {
                if ((mask & ShaderField.Mask) != 0)
                {
                    return base.GetPropertyHeight(mask);
                }
                else
                {
                    return 0;
                }
            }

            public override void OnGUI(Rect position, int mask)
            {
                if ((mask & ShaderField.Mask) != 0)
                {
                    base.OnGUI(position, mask);
                }
            }

            public override void OnGUI(int mask)
            {
                if ((mask & ShaderField.Mask) != 0)
                {
                    base.OnGUI(mask);
                }
            }
        }

        public class KeywordDrawer : Drawer
        {
            public ShaderKeyword ShaderKeyword { get; }

            public KeywordDrawer(SerializedProperty serializedProperty, ShaderKeyword shaderKeyword) : base(serializedProperty)
            {
                ShaderKeyword = shaderKeyword;
            }

            public override void Extract(ref int mask)
            {
                if (SerializedProperty.boolValue)
                {
                    mask |= ShaderKeyword.Mask;
                }
            }
        }

        public class EnumKeywordDrawer : Drawer
        {
            public EnumKeywordDrawer(SerializedProperty serializedProperty, ShaderEnumKeyword field) : base(serializedProperty)
            {
            }

            public override void Extract(ref int mask)
            {
                if (SerializedProperty.intValue >= 0)
                {
                    mask |= SerializedProperty.intValue;
                }
            }
        }

        public class EnumFlagsKeywordDrawer : Drawer
        {
            public EnumFlagsKeywordDrawer(SerializedProperty serializedProperty, ShaderEnumFlagsKeyword field) : base(serializedProperty)
            {
            }

            public override void Extract(ref int mask)
            {
                if (SerializedProperty.intValue >= 0)
                {
                    mask |= SerializedProperty.intValue;
                }
            }
        }
    }
}
