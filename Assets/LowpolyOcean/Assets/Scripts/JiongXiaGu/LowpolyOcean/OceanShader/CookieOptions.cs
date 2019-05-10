using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(OceanMode.CookieAddition)]
    public sealed class CookieOptions
    {
        public const string TextureShaderFieldName = "_OceanCookieTexture";
        public static readonly int TextureShaderFieldID = Shader.PropertyToID(TextureShaderFieldName);
        public const string ColorShaderFieldName = "_OceanCookieColor";
        public const string DepthAttenShaderFieldName = "_OceanCookieDepthAtten";
        public const string DepthAttenOffsetShaderFieldName = "_OceanCookieDepthAttenOffset";
        public const string LightAttenShaderFieldName = "_OceanCookieLightAtten";
        public const string IntensityShaderFieldName = "_OceanCookieIntensity";
        public static readonly int WorldToCookieMatrixShaderID = Shader.PropertyToID("_OceanWorldToCookie");

        public static CookieOptions Default { get; } = new CookieOptions()
        {
            Texture = null,
            Scale = new Vector3(12, 12, 1),
            DepthAtten = 0.15f,
            LightAtten = 1f,
            Intensity = 1f,
        };

        [ShaderField(TextureShaderFieldName)] public ScriptableFrameAnimation Texture;
        [ShaderField(ColorShaderFieldName)] public Color Color;
        [ShaderFieldMark] public Vector3 Scale;
        [ShaderField(DepthAttenShaderFieldName)] public float DepthAtten;
        [ShaderField(DepthAttenOffsetShaderFieldName)] public float DepthAttenOffset;
        [ShaderField(LightAttenShaderFieldName)] public float LightAtten;
        [ShaderField(IntensityShaderFieldName)] public float Intensity;
    }
}
