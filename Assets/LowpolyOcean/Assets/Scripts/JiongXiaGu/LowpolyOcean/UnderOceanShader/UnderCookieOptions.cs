using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(UnderOceanMode.CookieAddition)]
    public class UnderCookieOptions
    {
        public const string TextureShaderFieldName = "_LPOceanUnderCookieTexture";
        public static readonly int TextureShaderFieldID = Shader.PropertyToID(TextureShaderFieldName);
        public const string AttenuationShaderFieldName = "_LPOceanUnderCookieAtten";
        public const string IntensityShaderFieldName = "_LPOceanUnderCookieIntensity";
        public const string InShowIntensityShaderFieldName = "_LPOceanUnderCookieInShadowIntensity";
        public static readonly int WorldToCookieMatrixShaderID = Shader.PropertyToID("_LPOceanUnderWorldToCookie");

        public static UnderCookieOptions Default => new UnderCookieOptions()
        {
            Scale = new Vector3(20, 20, 20),
            Attenuation = 20,
            Intensity = 1f,
            InShowIntensity = 1f,
        };

        [ShaderField(TextureShaderFieldName)] public ScriptableFrameAnimation Texture;
        [ShaderFieldMark] public Vector3 Scale;
        [ShaderField(AttenuationShaderFieldName)] public float Attenuation;
        [ShaderField(IntensityShaderFieldName)] [Range(0, 2)] public float Intensity;
        [ShaderField(InShowIntensityShaderFieldName)] [Range(0, 5)] public float InShowIntensity;

        public void SetScale(float scale)
        {
            Scale = new Vector3(scale, scale, scale);
        }
    }
}
