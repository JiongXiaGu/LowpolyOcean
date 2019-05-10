using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(WaveAll)]
    public class WaveOptions
    {
        public const string TextureShaderFieldName = "_OceanWaveTexture";
        public const string Rect0ShaderFieldName = "_OceanWaveRect0";
        public const string Rect1ShaderFieldName = "_OceanWaveRect1";
        public const string Rect2ShaderFieldName = "_OceanWaveRect2";
        public const string Rect3ShaderFieldName = "_OceanWaveRect3";
        public const string RadianShaderFieldName = "_OceanWaveRadian";
        public const string UniformRadianFieldName = "_OceanWaveUniformRadian";
        public const string HeightPowShaderFieldName = "_OceanWaveHeightPow";
        public const string HeightScaleShaderFieldName = "_OceanWaveHeightScale";
        public const string SpeedZShaderFieldName = "_OceanWaveSpeedZ";

        public static WaveOptions Default => new WaveOptions()
        {
            Texture = null,
            Rect0 = new Vector4(0.5f, 0.5f, 100, 100),
            Rect1 = new Vector4(0.5f, 0.5f, 100, 100),
            Rect2 = new Vector4(0.5f, 0.5f, 100, 100),
            Rect3 = new Vector4(0.5f, 0.5f, 100, 100),
            Radian = Vector4.zero,
            UniformRadian = 0,
            HeightPow = new Vector4(1, 1, 1, 1),
            HeightScale = new Vector4(1, 1, 1, 1),
            SpeedZ = new Vector4(0.01f, 0.01f, 0.01f, 0.01f),
        };

        public const OceanMode WaveAll = OceanMode.Wave1 | OceanMode.Wave2 | OceanMode.Wave3;

        [ShaderField(TextureShaderFieldName)] public Texture Texture;
        [ShaderField(Rect0ShaderFieldName)] public Vector4 Rect0;
        [ShaderField(Rect1ShaderFieldName)] public Vector4 Rect1;
        [ShaderField(Rect2ShaderFieldName)] public Vector4 Rect2;
        [ShaderField(Rect3ShaderFieldName)] public Vector4 Rect3;
        [ShaderField(RadianShaderFieldName)] public Vector4 Radian;
        [ShaderField(UniformRadianFieldName)] public float UniformRadian;
        [ShaderField(HeightPowShaderFieldName)] public Vector4 HeightPow;
        [ShaderField(HeightScaleShaderFieldName)] public Vector4 HeightScale;
        [ShaderField(SpeedZShaderFieldName)] public Vector4 SpeedZ;

        public Vector4 GetRect(int index)
        {
            switch (index)
            {
                case 0:
                    return Rect0;
                case 1:
                    return Rect1;
                case 2:
                    return Rect2;
                case 3:
                    return Rect3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Empty);
            }
        }
    }
}
