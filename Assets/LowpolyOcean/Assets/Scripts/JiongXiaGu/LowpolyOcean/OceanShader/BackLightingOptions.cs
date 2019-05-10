using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(OceanMode.BackLightingArtistic)]
    public class BackLightingOptions
    {
        public const string ColorShaderFieldName = "_OceanBackArtisticColor";
        public const string SpecularPowShaderFieldName = "_OceanBackArtisticSpecularPow";
        public const string SpecularColorShaderFieldName = "_OceanBackArtisticSpecularColor";
        public const string FresnelTextureShaderFieldName = "_OceanBackArtisticFresnelTexture";
        public const string FresneIntensityShaderFieldName = "_OceanBackArtisticFresneIntensity";
        public const string FresnelColorShaderFieldName = "_OceanBackArtisticFresneColor";
        public const string ShadowAttenuationShaderFieldName = "_OceanBackArtisticShadowAttenuation";

        public static BackLightingOptions Default => new BackLightingOptions()
        {
            Color = new Color(0f, 0.59f, 0.62f, 1),
            SpecularPow = 20,
            SpecularColor = new Color(0.93f, 0.93f, 0.93f, 1),
            FresnelTexture = default(Texture),
            FresneIntensity = 1.7f,
            FresnelColor = new Color(0.1f, 0.34f, 0.47f, 1),
            ShadowAttenuation = 0.65f,
        };

        [ShaderField(ColorShaderFieldName)] public Color Color;
        [ShaderField(SpecularPowShaderFieldName)] public float SpecularPow;
        [ShaderField(SpecularColorShaderFieldName)] public Color SpecularColor;
        [ShaderField(FresnelTextureShaderFieldName)] public Texture FresnelTexture;
        [ShaderField(FresneIntensityShaderFieldName)] [Range(0, 3)] public float FresneIntensity;
        [ShaderField(FresnelColorShaderFieldName)] public Color FresnelColor;
        [ShaderField(ShadowAttenuationShaderFieldName)] [Range(0, 1)] public float ShadowAttenuation;
    }
}
