using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JiongXiaGu
{

    /// <summary>
    /// frame animation base class, provide multiple convenient methods
    /// </summary>
    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(FrameAnimation))]
    public class FrameAnimation : ScriptableFrameAnimation
    {
        protected FrameAnimation()
        {
        }

        [SerializeField] [Range(-50, 50)] private float speed = 10;
        [SerializeField]
        protected TextureList textures;
        public List<Texture> Textures => textures;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public override Texture GetCurrentTexture()
        {
            return InternalGetCurrentTexture(textures, speed, Time.time);
        }

        internal static Texture InternalGetCurrentTexture(IReadOnlyList<Texture> textures, float speed, float time)
        {
            if (textures.Count <= 0)
                return null;

            time *= speed;
            var intTime = (int)time;
            int index = intTime % textures.Count;
            index = Mathf.Abs(index);
            return textures[index];
        }

        [ContextMenu(nameof(SortTexturesByName))]
        public void SortTexturesByName()
        {
#if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to sort textures?", "Yes", "No"))
                return;

            Undo.RegisterCompleteObjectUndo(this, nameof(SortTexturesByName));
#endif  
            Textures.Sort(new SortTexturesByNameComparer());
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(ReadFromCurrentDir))]
        public void ReadFromCurrentDir()
        {
            if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to auto read textures?", "Yes", "No"))
                return;

            Undo.RegisterCompleteObjectUndo(this, nameof(ReadFromCurrentDir));

            string thisPath = AssetDatabase.GetAssetPath(this);
            string dir = Path.GetDirectoryName(thisPath);
            foreach (var item in Directory.EnumerateFiles(dir))
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture>(item);
                if (texture != null)
                {
                    textures.Add(texture);
                }
            }
        }
#endif

        [Serializable]
        protected class TextureList : CustomReorderableList<Texture>
        {
        }

        private class SortTexturesByNameComparer : Comparer<Texture>
        {

            private float TryConvertToOrder(string name)
            {
                try
                {
                    float order = Convert.ToSingle(name);
                    return order;
                }
                catch
                {
                    try
                    {
                        int index = name.LastIndexOf('_');
                        if (index < 0)
                            return -1;

                        string oderStr = name.Remove(0, index + 1);
                        float order = Convert.ToSingle(oderStr);
                        return order;
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }

            public override int Compare(Texture x, Texture y)
            {
                if (x == y)
                    return 0;

                float xOrder = TryConvertToOrder(x.name);
                float yOrder = TryConvertToOrder(y.name);
                return Math.Sign(xOrder - yOrder);
            }
        }
    }
}
