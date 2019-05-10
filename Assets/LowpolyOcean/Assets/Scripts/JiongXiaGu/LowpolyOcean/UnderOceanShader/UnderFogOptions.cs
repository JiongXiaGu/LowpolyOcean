using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(UnderOceanMode.FogInPass)]
    public class UnderFogOptions
    {
        public const string ColorShaderFieldName = "_LPOceanUnderFogColor";
        public const string ClarityShaderFieldName = "_LPOceanUnderFogClarity";
        public const string DepthAttenuationShaderFieldName = "_LPOceanUnderFogDepthAtten";
        public const string LightAttenuationClampShaderFieldName = "_LPOceanUnderFogLightAttenClamp";

        public static UnderFogOptions Default => new UnderFogOptions()
        {
            Color = new Color(0.01f, 0.2f, 0.35f, 1),
            Clarity = 0.06f,
            DepthAtten = 150f,
            LightAttenuationClamp = new Vector2(0.3f, 0.8f),
        };

        [ShaderField(ColorShaderFieldName)] public Color Color;
        [ShaderField(ClarityShaderFieldName)] [Range(0, 1)] public float Clarity;
        [ShaderField(DepthAttenuationShaderFieldName)] public float DepthAtten;
        [ShaderField(LightAttenuationClampShaderFieldName)] public Vector2 LightAttenuationClamp;
    }
}
