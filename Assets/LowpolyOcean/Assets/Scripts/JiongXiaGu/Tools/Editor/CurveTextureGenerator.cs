using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace JiongXiaGu
{

    /// <summary>
    /// use to create curve texture
    /// </summary>
    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(CurveTextureGenerator))]
    public sealed class CurveTextureGenerator : ScriptableObject
    {
        private CurveTextureGenerator()
        {
        }

        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        public Vector2Int TextureSize = new Vector2Int(256, 1);

        public Texture2D CreateTexture()
        {
            var texture = new Texture2D(TextureSize.x, TextureSize.y, TextureFormat.RGBA32, false, true);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.hideFlags = HideFlags.DontSave;

            for (int i = 0; i < 256; i++)
            {
                float v = Mathf.Clamp(Curve.Evaluate(i / 255f), 0f, 1f);

                for (int j = 0; j < texture.height; j++)
                {
                    texture.SetPixel(i, j, new Color(v, v, v, v));
                }
            }

            texture.Apply();
            return texture;
        }

        [ContextMenu(nameof(OuputTexture))]
        public void OuputTexture()
        {
            var texture = CreateTexture();
            string path = AssetDatabase.GetAssetPath(this);
            path = Path.ChangeExtension(path, TextureOutputHelper.PNGFileExtension);
            using (var stream = File.Open(path, FileMode.Create))
            {
                texture.Write(stream, TextureFileType.PNG);
            }
            AssetDatabase.ImportAsset(path);
            DestroyImmediate(texture);
        }

        [ContextMenu(nameof(OuputTextureTo))]
        public void OuputTextureTo()
        {
            var texture = CreateTexture();
            TextureGeneratorHelper.WriteTexture(this, texture);
            DestroyImmediate(texture);
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CurveTextureGenerator))]
    public class CurveTextureGeneratorDrawer : Editor
    {

        public override void OnInspectorGUI()
        {
            var target = this.target as CurveTextureGenerator;
            base.OnInspectorGUI();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(nameof(CurveTextureGenerator.OuputTexture)))
                {
                    target.OuputTexture();
                }
                if (GUILayout.Button(nameof(CurveTextureGenerator.OuputTextureTo)))
                {
                    target.OuputTextureTo();
                }
            }
        }
    }
}
