using System.IO;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{


    public class TextureGeneratorHelper
    {

        public static string WriteTexture(Object sender, RenderTexture source, TextureFileType fileType = TextureFileType.PNG)
        {
            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sender));
            path = EditorUtility.SaveFilePanelInProject("Save to", "Texture", TextureOutputHelper.GetFileExtension(fileType), "", path);
            if (path.Length > 0)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    TextureOutputHelper.Write(source, stream, fileType);
                }
                AssetDatabase.ImportAsset(path);
                return path;
            }
            return null;
        }

        public static string WriteTexture(Object sender, Texture2D source, TextureFileType fileType = TextureFileType.PNG)
        {
            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sender));
            path = EditorUtility.SaveFilePanelInProject("Save to", "Texture", TextureOutputHelper.GetFileExtension(fileType), "", path);
            if (path.Length > 0)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    TextureOutputHelper.Write(source, stream, fileType);
                }
                AssetDatabase.ImportAsset(path);
                return path;
            }
            return null;
        }

    }
}
