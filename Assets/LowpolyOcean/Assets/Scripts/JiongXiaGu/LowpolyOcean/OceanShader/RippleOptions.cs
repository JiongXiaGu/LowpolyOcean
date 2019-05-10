using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// ripple parameters, support 4 textures;
    /// the four color channels of the texture can each specify a height value, the height value is lower than gray (0.5) means height is reduced, otherwise height is increased.
    /// </summary>
    [Serializable]
    [ShaderFieldGroup(OceanMode.RippleMax4)]
    public class RippleOptions
    {
        public const string Texture0ShaderFieldName = "_OceanRippleTexture0";
        public const string Texture1ShaderFieldName = "_OceanRippleTexture1";
        public const string Texture2ShaderFieldName = "_OceanRippleTexture2";
        public const string Texture3ShaderFieldName = "_OceanRippleTexture3";
        public const string RectShaderFieldName = "_OceanRippleRect";
        public const string PositionShaderFieldName = "_OceanRipplePosition";
        public const string HeightScaleShaderFieldName = "_OceanRippleHeightScale";

        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(RippleOptions));
        public const int RippleCount = 4;

        [ShaderField(Texture0ShaderFieldName)] public Texture Texture0;
        [ShaderField(Texture1ShaderFieldName)] public Texture Texture1;
        [ShaderField(Texture2ShaderFieldName)] public Texture Texture2;
        [ShaderField(Texture3ShaderFieldName)] public Texture Texture3;
        [ShaderField(RectShaderFieldName)] public Vector4[] Rect;

        /// <summary>
        /// xyz:postion, w:radian
        /// </summary>
        [ShaderField(PositionShaderFieldName)] public Vector4[] Position;
        [ShaderField(HeightScaleShaderFieldName)] public Vector4[] HeightScale;

        public RippleOptions()
        {
            Rect = new Vector4[RippleCount];
            Position = new Vector4[RippleCount];
            HeightScale = new Vector4[RippleCount];
        }

        public Texture GetTexture(int index)
        {
            switch (index)
            {
                case 0:
                    return Texture0;

                case 1:
                    return Texture1;

                case 2:
                    return Texture2;

                case 3:
                    return Texture3;

                default:
                    throw new IndexOutOfRangeException(index.ToString());
            }
        }

        public void SetTexture(Texture texture, int index)
        {
            switch (index)
            {
                case 0:
                    Texture0 = texture;
                    break;

                case 1:
                    Texture1 = texture;
                    break;

                case 2:
                    Texture2 = texture;
                    break;

                case 3:
                    Texture3 = texture;
                    break;

                default:
                    throw new IndexOutOfRangeException(index.ToString());
            }
        }

        private void Clear(Vector4[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default(Vector4);
            }
        }

        public void Clear()
        {
            Texture0 = null;
            Texture1 = null;
            Texture2 = null;
            Texture3 = null;
            Clear(Rect);
            Clear(Position);
            Clear(HeightScale);
        }

        public void Clear(int index)
        {
            SetTexture(null, index);
            Rect[index] = default(Vector4);
            Position[index] = default(Vector4);
            HeightScale[index] = default(Vector4);
        }
    }
}
