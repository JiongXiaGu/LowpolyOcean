using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup]
    public sealed class ModeOptions
    {
        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(ModeOptions));

        public const string Wave1Keyword = "LPOCEAN_WAVE1";
        public const string Wave2Keyword = "LPOCEAN_WAVE2";
        public const string Wave3Keyword = "LPOCEAN_WAVE3";
        public const string RippleKeyword = "LPOCEAN_RIPPLE";
        public const string FresnelKeyword = "LPOCEAN_FRESNEL";
        public const string RefractionKeyworld = "LPOCEAN_REFRACTION";
        public const string RefractionFullKeyworld = "LPOCEAN_REFRACTION_FULL";
        public const string FoamSimpleEyeDepthKeyword = "LPOCEAN_FOAM_SIMPLE_EYE_DEPTH";
        public const string FoamEyeDepthKeyword = "LPOCEAN_FOAM_EYE_DEPTH";
        public const string FoamShpere8Keyword = "LPOCEAN_FOAM_SHPERE8";
        public const string FoamShpere16Keyword = "LPOCEAN_FOAM_SHPERE16";
        public const string FoamArea1Keyword = "LPOCEAN_FOAM_AREA1";
        public const string FoamArea4Keyword = "LPOCEAN_FOAM_AREA4";
        public const string CookieKeyword = "LPOCEAN_COOKIE";
        public const string ReflectionKeyworld = "LPOCEAN_REFLECTION";
        public const string BackLightingKeyword = "LPOCEAN_BACK_LIGHTING";
        public const string BackRefractionKeyword = "LPOCEAN_BACK_REFRACTION";
        public const string BackRefractionFullKeyword = "LPOCEAN_BACK_REFRACTION_FULL";
        public const string PointLightingKeyword = "LPOCEAN_POINT_LIGHTING";
        public const string ClipKeyword = "LPOCEAN_CLIP";
        public const string CullShaderFieldName = "_Cull";

        public static ModeOptions Default => new ModeOptions()
        {
        };

        [ShaderFieldEnumKeyword(null, WaveMode.None
            , Wave1Keyword, WaveMode.Wave1
            , Wave2Keyword, WaveMode.Wave2
            , Wave3Keyword, WaveMode.Wave3)]
        public WaveMode Wave;

        [ShaderFieldEnumKeyword(null, RippleMode.None
            , RippleKeyword, RippleMode.Max4)]
        public RippleMode Ripple;

        [ShaderFieldEnumKeyword(null, LightingMode.UnityPBS,
            FresnelKeyword, LightingMode.UnityPBSAndFresnel)]
        public LightingMode Lighting;

        [ShaderFieldEnumKeyword(null, RefractionMode.None
            , RefractionKeyworld, RefractionMode.Simple
            , RefractionFullKeyworld, RefractionMode.Full)]
        public RefractionMode Refraction;

        [ShaderFieldEnumKeyword(null, FoamMode.None
            , FoamSimpleEyeDepthKeyword, FoamMode.SimpleEyeDepth
            , FoamEyeDepthKeyword, FoamMode.EyeDepth)]
        public FoamMode Foam;

        [ShaderFieldEnumKeyword(null, FoamShpereMode.None
            , FoamShpere8Keyword, FoamShpereMode.Shpere8
            , FoamShpere16Keyword, FoamShpereMode.Shpere16)]
        public FoamShpereMode FoamShpere;

        [ShaderFieldEnumKeyword(null, FoamAreaMode.None
            , FoamArea1Keyword, FoamAreaMode.Area1)]
        public FoamAreaMode FoamArea;

        [ShaderFieldEnumKeyword(null, CookieMode.None
            , CookieKeyword, CookieMode.Addition)]
        public CookieMode Cookie;

        [ShaderFieldEnumKeyword(null, ReflectionMode.None
            , ReflectionKeyworld, ReflectionMode.SimpleOffset)]
        public ReflectionMode Reflection;

        [ShaderFieldEnumKeyword(null, BackLightingMode.None
            , BackLightingKeyword, BackLightingMode.Artistic)]
        public BackLightingMode BackLighting;

        [ShaderFieldEnumKeyword(null, BackRefractionMode.None
            , BackRefractionKeyword, BackRefractionMode.Simple
            , BackRefractionFullKeyword, BackRefractionMode.Full)]
        public BackRefractionMode BackRefraction;

        [ShaderFieldEnumKeyword(null, PointLightingMode.None
            , PointLightingKeyword, PointLightingMode.Simple)]
        public PointLightingMode PointLigting;

        [ShaderFieldEnumKeyword(null, ClipMode.None 
            , ClipKeyword, ClipMode.Normal)]
        public ClipMode Clip;

        [ShaderField(CullShaderFieldName)] public CullMode Cull = CullMode.Off;

        public ModeOptions Clone()
        {
            return (ModeOptions)MemberwiseClone();
        }

        public PreparedContent GetRenderContents()
        {
            var modeOptions = this;
            PreparedContent renderContents = PreparedContent.SunLight;

            if (modeOptions.Clip != ClipMode.None)
            {
                renderContents |= PreparedContent.ClipTexture;
            }


            if (modeOptions.Refraction == RefractionMode.Simple)
            {
                renderContents |= PreparedContent.RefractionTexture | PreparedContent.RefractionDepthTexture;
            }
            else if (modeOptions.Refraction == RefractionMode.Full)
            {
                renderContents |= PreparedContent.RefractionTexture | PreparedContent.RefractionDepthTexture | PreparedContent.UnderOceanMarkTexture;
            }


            if (modeOptions.BackRefraction == BackRefractionMode.Simple)
            {
                renderContents |= PreparedContent.RefractionTexture | PreparedContent.RefractionDepthTexture;
            }
            else if (modeOptions.BackRefraction == BackRefractionMode.Full)
            {
                renderContents |= PreparedContent.RefractionTexture | PreparedContent.RefractionDepthTexture | PreparedContent.UnderOceanMarkTexture;
            }


            if (modeOptions.FoamShpere == FoamShpereMode.Shpere8)
            {
                renderContents |= PreparedContent.RefractionDepthTexture | PreparedContent.Foam8;
            }
            else if (modeOptions.FoamShpere == FoamShpereMode.Shpere16)
            {
                renderContents |= PreparedContent.RefractionDepthTexture | PreparedContent.Foam16;
            }
            else if (modeOptions.Foam != FoamMode.None)
            {
                renderContents |= PreparedContent.RefractionDepthTexture;
            }


            if (modeOptions.FoamArea == FoamAreaMode.Area1)
            {
                renderContents |= PreparedContent.FoamArea1;
            }


            if (modeOptions.Reflection != ReflectionMode.None)
            {
                renderContents |= PreparedContent.ReflectionTexture;
            }


            if (modeOptions.Ripple != RippleMode.None)
            {
                renderContents |= PreparedContent.Ripple;
            }

            return renderContents;
        }
    }
}
