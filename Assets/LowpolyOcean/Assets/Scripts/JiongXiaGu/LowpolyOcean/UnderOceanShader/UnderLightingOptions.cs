using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(UnderOceanMode.LightingRuntimeAtten)]
    public class UnderLightingOptions
    {
        public const string LightAttenShaderFieldName = "_LPOceanUnderLightAtten";
        public const string LightAttenClampShaderFieldName = "_LPOceanUnderLightAttenClamp";
        public const string ShadowAttenShaderFieldName = "_LPOceanUnderShadowAtten";
        public const string ShadowAttenClampShaderFieldName = "_LPOceanUnderShadowAttenClamp";
        public const string ShadowIntensityShaderFieldName = "_LPOceanUnderShadowIntensity";

        public static UnderLightingOptions Default => new UnderLightingOptions()
        {
            LightAtten = 80,
            LightAttenClamp = new Vector2(0.3f, 0.9f),
            ShadowAtten = 80,
            ShadowAttenClamp = new Vector2(0, 0.9f),
            ShadowIntensity = 0.3f,
        };

        [ShaderField(LightAttenShaderFieldName)]  public float LightAtten;
        [ShaderField(LightAttenClampShaderFieldName)]  public Vector2 LightAttenClamp;
        [ShaderField(ShadowAttenShaderFieldName)]  public float ShadowAtten;
        [ShaderField(ShadowAttenClampShaderFieldName)] public Vector2 ShadowAttenClamp;
        [ShaderField(ShadowIntensityShaderFieldName)]  [Range(0, 1)] public float ShadowIntensity;
    }
}
