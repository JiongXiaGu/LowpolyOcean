using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup]
    public sealed class UnderOceanModeOptions
    {
        public const string LightingKeyword = "LPUNDER_OCEAN_LIGHTING";
        public const string FogKeyword = "LPUNDER_OCEAN_EFFECT";
        public const string CookieKeyword = "LPUNDER_OCEAN_COOKIE";
        public const string ClipKeyword = "LPUNDER_OCEAN_CLIP";

        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(UnderOceanModeOptions));

        public static IReadOnlyList<string> KeywordAll { get; } = new string[]
        {
            LightingKeyword,
            FogKeyword,
            CookieKeyword,
            ClipKeyword,
        };

        public static UnderOceanModeOptions Default => new UnderOceanModeOptions()
        {
            Lighting = UnderLightingMode.RuntimeAtten,
            Cookie = UnderCookieMode.Addition,
            Fog = UnderFogMode.InPass,
            Clip = UnderClipMode.Normal,
        };

        [ShaderFieldEnumKeyword(null, UnderLightingMode.None,
           LightingKeyword, UnderLightingMode.RuntimeAtten)]
        public UnderLightingMode Lighting;

        [ShaderFieldEnumKeyword(null, UnderCookieMode.None,
           CookieKeyword, UnderCookieMode.Addition)]
        public UnderCookieMode Cookie;

        [ShaderFieldEnumKeyword(null, UnderFogMode.None,
            FogKeyword, UnderFogMode.InPass)]
        public UnderFogMode Fog;

        [ShaderFieldEnumKeyword(null, UnderClipMode.None,
            ClipKeyword, UnderClipMode.Normal)]
        public UnderClipMode Clip;

        public int Version { get; set; }

        public void Dirty()
        {
            Version++;
        }

        public int GetUpdateMask()
        {
            return (int)Lighting | (int)Cookie | (int)Fog | (int)Clip;
        }

        public static int CurrentHashCode { get; set; }
        public static int CurrentVersion { get; set; } = -1;

        public static bool UpdateKeywords(UnderOceanModeOptions mode)
        {
            var hashCode = mode.GetHashCode();
            if (hashCode != CurrentHashCode || CurrentVersion != mode.Version)
            {
                Accessor.SetGlobalValues(mode);
                CurrentHashCode = hashCode;
                CurrentVersion = mode.Version;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetCurrentDirty()
        {
            CurrentVersion = -1;
        }

        public static void DisableUnderOceanEffectAll()
        {
            foreach (var keyword in KeywordAll)
            {
                Shader.DisableKeyword(keyword);
            }
            SetCurrentDirty();
        }

        public static void DisableUnderOceanFogEffect()
        {
            Shader.DisableKeyword(FogKeyword);
            SetCurrentDirty();
        }

        public class DisableUnderOceanEffectScope : IDisposable
        {
            private bool isDisposed;
            private int original;

            public DisableUnderOceanEffectScope()
            {
                original = 0;
                for (int i = 0; i < KeywordAll.Count; i++)
                {
                    string keyword = KeywordAll[i];
                    if (Shader.IsKeywordEnabled(keyword))
                    {
                        original |= 1 << i;
                    }
                    Shader.DisableKeyword(keyword);
                }
            }

            public void Dispose()
            {
                if (!isDisposed)
                {
                    for (int i = 0; i < KeywordAll.Count; i++)
                    {
                        if ((original & (1 << i)) != 0)
                        {
                            string keyword = KeywordAll[i];
                            Shader.EnableKeyword(keyword);
                        }
                    }
                    isDisposed = true;
                }
            }
        }

        public class DisableUnderOceanFogScope : IDisposable
        {
            public string FogKeyword => UnderOceanModeOptions.FogKeyword;
            public bool OriginalFog { get; private set; }

            public DisableUnderOceanFogScope()
            {
                OriginalFog = Shader.IsKeywordEnabled(FogKeyword);
                if (OriginalFog)
                {
                    Shader.DisableKeyword(FogKeyword);
                }
            }

            public DisableUnderOceanFogScope(bool originalFog)
            {
                OriginalFog = originalFog;
                Shader.DisableKeyword(FogKeyword);
            }

            public void Dispose()
            {
                if (OriginalFog)
                    Shader.EnableKeyword(FogKeyword);
            }
        }
    }
}
