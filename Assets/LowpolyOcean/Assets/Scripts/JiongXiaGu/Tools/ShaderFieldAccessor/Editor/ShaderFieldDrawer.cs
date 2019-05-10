using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    public class ShaderFieldDrawer
    {
        public IShaderFieldGroup Group { get; }
        public List<IFieldDrawer> Drawers { get; }
        public bool IsKeywordChanged { get; private set; }
        public int Mask { get; private set; }
        private MaterialEditor materialEditor;
        private List<string> keywords;
        private bool isDrawUI;

        public ShaderFieldDrawer(IShaderFieldGroup group, MaterialProperty[] properties)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            Group = group;
            Drawers = new List<IFieldDrawer>(group.Children.Count);

            foreach (var child in Group.Children)
            {
                if (child is ShaderField)
                {
                    var field = (ShaderField)child;
                    IFieldDrawer drawer;
                    if (StandardFieldDrawer.TryCreate(field, properties, out drawer))
                    {
                        Drawers.Add(drawer);
                    }
                }
                if (child is ShaderKeyword)
                {
                    var field = (ShaderKeyword)child;
                    Drawers.Add(new ShaderKeywordDrawer(field));
                }
                else if (child is ShaderEnumKeyword)
                {
                    var field = (ShaderEnumKeyword)child;
                    Drawers.Add(new ShaderEnumKeywordDrawer(field));
                }
                else if (child is ShaderEnumFlagsKeyword)
                {
                    var field = (ShaderEnumFlagsKeyword)child;
                    Drawers.Add(new ShaderEnumFlagsKeywordDrawer(field));
                }
            }
        }

        public void Draw(MaterialEditor materialEditor, List<string> keywords, int mask)
        {
            this.materialEditor = materialEditor;
            Mask = 0;
            this.keywords = keywords;
            IsKeywordChanged = false;
            isDrawUI = true;

            foreach (var drawer in Drawers)
            {
                drawer.Draw(this, mask);
            }
        }

        public void Draw(MaterialEditor materialEditor, int mask)
        {
            Draw(materialEditor, null, mask);
        }

        public void Extract(MaterialEditor materialEditor, List<string> keywords, int mask)
        {
            this.materialEditor = materialEditor;
            Mask = 0;
            this.keywords = keywords;
            IsKeywordChanged = false;
            isDrawUI = false;

            foreach (var drawer in Drawers)
            {
                drawer.Draw(this, mask);
            }
        }


        public interface IFieldDrawer
        {
            void Draw(ShaderFieldDrawer parent, int mask);
        }

        private class StandardFieldDrawer : IFieldDrawer
        {
            public ShaderFieldBase Field { get; private set; }
            public MaterialProperty MaterialProperty { get; private set; }

            public StandardFieldDrawer(ShaderFieldBase field, MaterialProperty property)
            {
                Field = field;
                MaterialProperty = property;
            }

            public static bool TryCreate(ShaderField field, MaterialProperty[] properties, out IFieldDrawer drawer)
            {
                var materialProperty = ShaderDrawerHelper.PublicFindProperty(field.ShaderFieldName, properties);
                if (materialProperty != null)
                {
                    drawer = new StandardFieldDrawer(field, materialProperty);
                    return true;
                }
                else
                {
                    drawer = default(IFieldDrawer);
                    return false;
                }
            }

            public void Draw(ShaderFieldDrawer parent, int mask)
            {
                if ((mask & Field.Mask) == 0)
                    return;

                if (parent.isDrawUI)
                {
                    string displayName = Field.ReflectiveField.Name;
                    parent.materialEditor.ShaderProperty(MaterialProperty, displayName);
                }
            }
        }

        private class ShaderKeywordDrawer : IFieldDrawer
        {
            public ShaderKeyword Keyword { get; }

            public ShaderKeywordDrawer(ShaderKeyword keyword)
            {
                Keyword = keyword;
            }

            public void Draw(ShaderFieldDrawer parent, int mask)
            {
                if ((mask & Keyword.Mask) == 0)
                    return;

                Material targetMat = (Material)parent.materialEditor.target;
                string keyword = Keyword.Keyword;
                string displayName = Keyword.ReflectiveField.Name;

                bool isEnabled = targetMat.IsKeywordEnabled(keyword);

                if (parent.isDrawUI)
                {
                    bool newValue = EditorGUILayout.Toggle(displayName, isEnabled);
                    parent.IsKeywordChanged |= isEnabled != newValue;
                    isEnabled = newValue;
                }

                if (isEnabled)
                {
                    parent.keywords?.Add(keyword);
                    parent.Mask |= Keyword.Mask;
                }
            }
        }

        private class ShaderEnumKeywordDrawer : IFieldDrawer
        {
            public ShaderEnumKeyword Keyword { get; }

            public ShaderEnumKeywordDrawer(ShaderEnumKeyword keyword)
            {
                Keyword = keyword;
            }

            public void Draw(ShaderFieldDrawer parent, int mask)
            {
                if (parent.keywords == null)
                    return;
                if ((mask & Keyword.Mask) == 0)
                    return;

                var targetMat = (Material)parent.materialEditor.target;
                string keyword = null;
                int selecteIndex = -1;
                int emptyIndex = -1;

                for (int i = 0; i < Keyword.KeywordsAndMask.Length; i += 2)
                {
                    keyword = Keyword.GetKeyword(i);
                    if (keyword == null)
                    {
                        emptyIndex = i / 2;
                    }
                    else if (targetMat.IsKeywordEnabled(keyword))
                    {
                        selecteIndex = i / 2;
                        break;
                    }
                }

                if (selecteIndex < 0 && emptyIndex >= 0)
                {
                    selecteIndex = emptyIndex;
                }

                if (parent.isDrawUI)
                {
                    string displayName = Keyword.ReflectiveField.Name;
                    int newSelecteIndex = EditorGUILayout.Popup(displayName, selecteIndex, Keyword.SortNames);
                    parent.IsKeywordChanged |= selecteIndex != newSelecteIndex;
                    selecteIndex = newSelecteIndex;
                }

                int index = selecteIndex * 2;
                keyword = Keyword.GetKeyword(index);

                if (keyword != null)
                {
                    parent.keywords?.Add(keyword);
                    int itemMask = Keyword.GetMask(index);
                    parent.Mask |= itemMask;
                }
            }
        }

        private class ShaderEnumFlagsKeywordDrawer : IFieldDrawer
        {
            public ShaderEnumFlagsKeyword Keyword { get; }

            public ShaderEnumFlagsKeywordDrawer(ShaderEnumFlagsKeyword keyword)
            {
                Keyword = keyword;
            }

            public void Draw(ShaderFieldDrawer parent, int mask)
            {
                if (parent.keywords == null)
                    return;
                if ((mask & Keyword.Mask) == 0)
                    return;

                var targetMat = (Material)parent.materialEditor.target;
                int selecteMask = 0;

                for (int i = 0; i < Keyword.KeywordsAndMask.Length; i += 2)
                {
                    string keyword = Keyword.GetKeyword(i);
                    if (keyword != null && targetMat.IsKeywordEnabled(keyword))
                    {
                        int currentMask = 1 << i / 2;
                        selecteMask |= currentMask;
                    }
                }

                if (parent.isDrawUI)
                {
                    string displayName = Keyword.ReflectiveField.Name;
                    int newSelecteMask = EditorGUILayout.MaskField(displayName, selecteMask, Keyword.SortNames);
                    parent.IsKeywordChanged |= selecteMask != newSelecteMask;
                    selecteMask = newSelecteMask;
                }

                for (int i = 0; i < Keyword.KeywordsAndMask.Length; i += 2)
                {
                    int currentMask = 1 << i / 2;
                    if ((selecteMask & currentMask) != 0)
                    {
                        string keyword = Keyword.GetKeyword(i);
                        int itemMask = Keyword.GetMask(i);
                        if (keyword != null)
                        {
                            parent.keywords.Add(keyword);
                            parent.Mask |= itemMask;
                        }
                    }
                }
            }
        }
    }
}
