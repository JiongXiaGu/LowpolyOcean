using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Override shader methods to provide ocean coordinate transformations
    /// </summary>
    public static class VertexHelper
    {
        public static float DefaultTime => Time.time;

        private static Vector2 Rotate(Vector2 localPos, float radian)
        {
            radian = Mathf.Atan2(localPos.x, localPos.y) - radian;
            float dis = Mathf.Sqrt(localPos.x * localPos.x + localPos.y * localPos.y);

            localPos.x = dis * Mathf.Sin(radian);
            localPos.y = dis * Mathf.Cos(radian);

            return localPos;
        }

        private static Vector2 Rotate(Vector2 pos, Vector2 origin, float radian)
        {
            Vector2 localPos = pos - origin;
            return Rotate(localPos, radian) + origin;
        }

        private static Vector2 GetUV(Vector2 localPos, Vector4 textureRect)
        {
            localPos.x += textureRect.z * textureRect.x;
            localPos.y += textureRect.w * textureRect.y;
            Vector2 uv = new Vector2();
            uv.x = localPos.x / textureRect.z;
            uv.y = localPos.y / textureRect.w;
            return uv;
        }

        private static Vector2 GetUV(Vector2 pos, Vector2 origin, Vector4 textureRect)
        {
            Vector2 localPos = pos - origin;
            return GetUV(localPos, textureRect);
        }

        private static bool UVClamp(Vector2 uv)
        {
            return uv.x > 0 && uv.x < 1 && uv.y > 0 && uv.y < 1;
        }

        private static float UVMirror(float value)
        {
            int f = Mathf.FloorToInt(value);
            value = Mathf.Abs(value - f);
            if ((f & 1) != 0)
            {
                return 1 - value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// because <see cref="Texture2D.GetPixelBilinear(float, float)"/> does not support <see cref="TextureWrapMode.Mirror"/>, use this method to get the color
        /// </summary>
        private static Color GetColor(Texture2D texture, float x, float y)
        {
            switch (texture.wrapMode)
            {
                case TextureWrapMode.Mirror:
                    x = UVMirror(x);
                    y = UVMirror(y);
                    return texture.GetPixelBilinear(x, y);

                default:
                    return texture.GetPixelBilinear(x, y);
            }
        }

        private static Color GetColorNormal(Texture2D texture, float x, float y)
        {
            return texture.GetPixelBilinear(x, y);
        }

        private static void EncodeFloatRG(float v, out float out0, out float out1)
        {
            out0 = 1f;
            out1 = 255f;
            float kEncodeBit = 1f / 255f;
            out0 = out0 * v;
            out1 = out1 * v;
            out0 = Mathf.Repeat(out0, 1);
            out1 = Mathf.Repeat(out1, 1);
            out0 -= out1 * kEncodeBit;
        }

        private static float DecodeFloatRG(Vector2 enc)
        {
            Vector2 decodeDot = new Vector2(1f, 1f / 255f);
            return Vector2.Dot(enc, decodeDot);
        }

        #region Wave

        private static Vector2 GetWorldWaveUV(Vector2 pos, float radian, Vector4 textureRect)
        {
            pos = Rotate(pos, radian);
            return GetUV(pos, textureRect);
        }

        public static float GetWaveHeight(WaveOptions wave, Texture2D waveTexture, Vector3 worldPos, int colorIndex, int index, float time)
        {
            Vector2 uv = GetWorldWaveUV(new Vector2(worldPos.x, worldPos.z), wave.Radian[index], wave.GetRect(index));
            uv.y -= wave.SpeedZ[index] * time;
            float height = GetColor(waveTexture, uv.x, uv.y)[colorIndex];
            height = Mathf.Pow(height, wave.HeightPow[index]) * wave.HeightScale[index];
            return height;
        }

        public static float GetWaveHeight(WaveOptions wave, Texture2D waveTexture, Vector3 worldPos, int colorIndex0, int colorIndex1, int index, float time)
        {
            Vector2 uv = GetWorldWaveUV(new Vector2(worldPos.x, worldPos.z), wave.Radian[index], wave.GetRect(index));
            uv.y -= wave.SpeedZ[index] * time;
            var c0 = GetColor(waveTexture, uv.x, uv.y)[colorIndex0];
            var c1 = GetColor(waveTexture, uv.x, uv.y)[colorIndex1];
            float height = DecodeFloatRG(new Vector2(c0, c1));
            height = Mathf.Pow(height, wave.HeightPow[index]) * wave.HeightScale[index];
            return height;
        }

        public static float GetWaveHeight(this WaveOptions wave, WaveMode waveMode, Vector3 worldPos, float time)
        {
            Texture2D waveTexture = wave.Texture as Texture2D;
            if (waveTexture == null)
                return 0;

            float height = 0;

            switch (waveMode)
            {
                case WaveMode.Wave1:
                    height += GetWaveHeight(wave, waveTexture, worldPos, 0, 1, 2, time);
                    break;

                case WaveMode.Wave2:
                    height += GetWaveHeight(wave, waveTexture, worldPos, 0, 1, 0, time);
                    height += GetWaveHeight(wave, waveTexture, worldPos, 2, 3, 1, time);
                    break;

                case WaveMode.Wave3:
                    height += GetWaveHeight(wave, waveTexture, worldPos, 0, 0, time);
                    height += GetWaveHeight(wave, waveTexture, worldPos, 1, 1, time);
                    height += GetWaveHeight(wave, waveTexture, worldPos, 2, 2, time);
                    break;
            }

            return height;
        }

        #endregion

        #region Ripple

        public static void GetRippleWorldPos(Vector2 center, Vector4 rect, float radian, ref Vector2[] worldPos)
        {
            float width = rect.x * rect.z;
            float height = rect.y * rect.w;
            var p0 = new Vector2(center.x - width, center.y - height);
            var p1 = new Vector2(center.x - width, center.y + height);
            var p2 = new Vector2(center.x + width, center.y + height);
            var p3 = new Vector2(center.x + width, center.y - height);

            worldPos[0] = Rotate(p0, center, radian);
            worldPos[1] = Rotate(p1, center, radian);
            worldPos[2] = Rotate(p2, center, radian);
            worldPos[3] = Rotate(p3, center, radian);
        }

        private static Vector2 GetWorldRippleUV(Vector2 pos, Vector2 origin, float radian, Vector4 textureRect)
        {
            pos = Rotate(pos, origin, radian);
            return GetUV(pos, origin, textureRect);
        }

        private static float GetRippleHeight(float color, float heightScale)
        {
            float height = (color - 0.5f) * heightScale;
            return height;
        }

        private static float GetRippleHeight(Color color, Vector4 heightScale)
        {
            float height = 0;
            for (int i = 0; i < 4; i++)
            {
                height += GetRippleHeight(color[i], heightScale[i]);
            }
            return height;
        }

        public static float GetRippleHeight(this RippleOptions data, Texture2D rippleTex, Vector3 worldPos, int index)
        {
            Vector2 uv = GetWorldRippleUV(new Vector2(worldPos.x, worldPos.z), new Vector2(data.Position[index].x, data.Position[index].z), data.Position[index].w, data.Rect[index]);

            if (UVClamp(uv))
            {
                Color color = GetColorNormal(rippleTex, uv.x, uv.y);
                Vector4 heightScale = data.HeightScale[index];
                float height = GetRippleHeight(color, heightScale);
                return height;
            }
            return 0;
        }

        public static float GetRippleHeight(Vector3 worldPos)
        {
            RippleOptions rippleOptions = RippleSystem.Current.FinalRippleOptions;
            float height = 0;
            for (int i = 0; i < RippleOptions.RippleCount; i++)
            {
                Texture tex = rippleOptions.GetTexture(i);
                if (tex != null)
                {
                    Texture2D rippleTex = (Texture2D)tex;
                    height += GetRippleHeight(rippleOptions, rippleTex, worldPos, i);
                }
            }
            return height;
        }

        public static float GetRippleHeight(RippleMode rippleMode, Vector3 worldPos)
        {
            if ((rippleMode & RippleMode.Max4) != 0)
            {
                return GetRippleHeight(worldPos);
            }
            return 0;
        }

        #endregion

        public static float GetVertexHeight(MaterialOptions data, ModeOptions mode, Vector3 worldPos, float time)
        {
            float height = GetWaveHeight(data.Wave, mode.Wave, worldPos, time);
            height += GetRippleHeight(mode.Ripple, worldPos);
            return height;
        }

        public static float GetVertexHeight(MaterialOptions data, ModeOptions mode, Vector3 worldPos)
        {
            return GetVertexHeight(data, mode, worldPos, DefaultTime);
        }
    }
}
