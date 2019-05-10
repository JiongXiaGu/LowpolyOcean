using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup]
    public sealed class OceanShaderOptions
    {
        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(OceanShaderOptions));
        public static ShaderAccessor ModeAccessor { get; } = Accessor.CreateAccessor(nameof(Mode));
        public static ShaderAccessor TessellationAccessor { get; } = Accessor.CreateAccessor(nameof(Tessellation));
        public static ShaderAccessor WaveAccessor { get; } = Accessor.CreateAccessor(nameof(Wave));
        public static ShaderAccessor LightingAccessor { get; } = Accessor.CreateAccessor(nameof(Lighting));
        public static ShaderAccessor RefractionAccessor { get; } = Accessor.CreateAccessor(nameof(Refraction));
        public static ShaderAccessor FoamAccessor { get; } = Accessor.CreateAccessor(nameof(Foam));
        public static ShaderAccessor CookieAccessor { get; } = Accessor.CreateAccessor(nameof(Cookie));
        public static ShaderAccessor ReflectionAccessor { get; } = Accessor.CreateAccessor(nameof(Reflection));
        public static ShaderAccessor BackLightingAccessor { get; } = Accessor.CreateAccessor(nameof(BackLighting));
        public static ShaderAccessor BackRefractionAccessor { get; } = Accessor.CreateAccessor(nameof(BackRefraction));
        public static ShaderAccessor PointLightingAccessor { get; } = Accessor.CreateAccessor(nameof(PointLighting));

        public const string TransparentKeyword = "LPOCEAN_TRANSPARENT";
        public static readonly int RefractionTextureShaderID = Shader.PropertyToID("_LPOceanRefractionTexture");
        public static readonly int RefractionDepthTextureShaderID = Shader.PropertyToID("_LPOceanRefractionDepthTexture");
        public static readonly int ReflectionTextureShaderID = Shader.PropertyToID("_LPOceanReflectionTexture");
        public static readonly int ClipTextureShaderID = Shader.PropertyToID("_LPOceanClipTexture");
        public static readonly int ClipDepthTextureShaderID = Shader.PropertyToID("_LPOceanClipDepthTexture");
        public static readonly int UnderOceanMarkTextureShaderID = Shader.PropertyToID("_LPOceanMarkTexture");
        public static readonly int UnderOceanMarkDepthTextureShaderID = Shader.PropertyToID("_LPOceanMarkDepthTexture");

        /// <summary>
        /// float4, postion in xyz, Angle with the horizon(0, 1) in y.
        /// </summary>
        public static readonly int SunLightShaderID = Shader.PropertyToID("_LPOceanSunLight");

        /// <summary>
        /// float3,color
        /// </summary>
        public static readonly int SunLightColorShaderID = Shader.PropertyToID("_LPOceanSunLightColor");

        /// <summary>
        /// float
        /// </summary>
        public static readonly int TimeShaderID = Shader.PropertyToID("_LPOceanTime");

        public ModeOptions Mode = ModeOptions.Default;
        public TessellationOptions Tessellation = TessellationOptions.Default;
        public WaveOptions Wave = WaveOptions.Default;
        public LightingOptions Lighting = LightingOptions.Default;
        public RefractionOptions Refraction = RefractionOptions.Default;
        public FoamOptions Foam = FoamOptions.Default;
        public CookieOptions Cookie = CookieOptions.Default;
        public ReflectionOptions Reflection = ReflectionOptions.Default;
        public BackLightingOptions BackLighting = BackLightingOptions.Default;
        public BackRefractionOptions BackRefraction = BackRefractionOptions.Default;
        public PointLightingOptions PointLighting = PointLightingOptions.Default;
    }
}
