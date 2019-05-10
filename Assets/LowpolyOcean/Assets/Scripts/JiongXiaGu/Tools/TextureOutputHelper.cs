using System;
using System.IO;
using UnityEngine;

namespace JiongXiaGu
{

    public enum TextureFileType
    {
        PNG,
        JPG,
    }

    public static class TextureOutputHelper
    {
        public const string PNGFileExtension = "png";
        public const string JPGFileExtension = "jpg";

        public static string GetFileExtension(TextureFileType fileType)
        {
            switch (fileType)
            {
                case TextureFileType.PNG:
                    return PNGFileExtension;

                case TextureFileType.JPG:
                    return JPGFileExtension;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Texture2D CreateTexture2D(this RenderTexture renderTexture)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            try
            {
                RenderTexture.active = renderTexture;
                Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();
                return texture;
            }
            finally
            {
                RenderTexture.active = currentActiveRT;
            }
        }

        #region Write

        private static void InternalWriteAsPNG(Texture2D texture, Stream stream)
        {
            byte[] data = texture.EncodeToPNG();
            stream.Write(data, 0, data.Length);
        }

        private static void InternalWriteAsJPG(Texture2D texture, Stream stream)
        {
            byte[] data = texture.EncodeToJPG();
            stream.Write(data, 0, data.Length);
        }

        private static void InternalWrite(Texture2D texture, Stream stream, TextureFileType fileType)
        {
            switch (fileType)
            {
                case TextureFileType.PNG:
                    InternalWriteAsPNG(texture, stream);
                    break;

                case TextureFileType.JPG:
                    InternalWriteAsJPG(texture, stream);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private static void ValidateWrite(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
        }

        private static void ValidateWrite(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new NotSupportedException("stream not writable");
        }

        private static void ValidateWrite(TextureFileType fileType)
        {
            if (!Enum.IsDefined(typeof(TextureFileType), fileType))
                throw new NotSupportedException();
        }

        public static void Write(this RenderTexture renderTexture, Stream stream, TextureFileType fileType)
        {
            if (renderTexture == null)
                throw new ArgumentNullException(nameof(renderTexture));
            ValidateWrite(stream);
            ValidateWrite(fileType);

            Texture2D texture = renderTexture.CreateTexture2D();
            try
            {
                InternalWrite(texture, stream, fileType);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        public static void Write(this Texture2D texture, Stream stream, TextureFileType fileType)
        {
            ValidateWrite(texture);
            ValidateWrite(stream);

            InternalWrite(texture, stream, fileType);
        }

        #endregion

        #region Read

        public const TextureFormat defaultTextureFormat = TextureFormat.RGBA32;
        public const bool defaultMipmap = false;
        public const bool defaultLinear = false;

        public static Texture2D Read(Stream stream, TextureFormat format = defaultTextureFormat, bool mipmap = defaultMipmap, bool linear = defaultLinear)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if(!stream.CanRead)
                throw new NotSupportedException("stream not readable");
            if (stream.Length > int.MaxValue)
                throw new ArgumentException("stream is too big");

            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return InternalRead(data, format, mipmap, linear);
        }

        private static Texture2D InternalRead(byte[] data, TextureFormat format, bool mipmap, bool linear)
        {
            Texture2D texture = new Texture2D(0, 0, format, mipmap, linear);
            if (ImageConversion.LoadImage(texture, data))
            {
                return texture;
            }
            else
            {
                UnityEngine.Object.Destroy(texture);
                throw new InvalidOperationException();
            }
        }

        #endregion

    }
}
