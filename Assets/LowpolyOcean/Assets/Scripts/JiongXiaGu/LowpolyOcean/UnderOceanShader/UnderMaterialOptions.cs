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
    public class UnderMaterialOptions
    {
        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(UnderMaterialOptions));
        public static ShaderAccessor LightingAccessor { get; } = Accessor.CreateAccessor(nameof(Lighting));
        public static ShaderAccessor FogAccessor { get; } = Accessor.CreateAccessor(nameof(Fog));
        public static ShaderAccessor CookieAccessor { get; } = Accessor.CreateAccessor(nameof(Cookie));

        public static readonly int UnderOceanPositionShaderID = Shader.PropertyToID("_LPUnderOceanPosition");

        public UnderLightingOptions Lighting = UnderLightingOptions.Default;
        public UnderFogOptions Fog = UnderFogOptions.Default;
        public UnderCookieOptions Cookie = UnderCookieOptions.Default;
        public int Version { get; set; }

        public void Dirty()
        {
            Version++;
        }

        public static int CurrentHashCode { get; set; }
        public static int CurrentVersion { get; set; } = -1;

        public static bool UpdateShaderFields(UnderMaterialOptions options, int mask)
        {
            var hashCode = options.GetHashCode();
            if (hashCode != CurrentHashCode || CurrentVersion != options.Version)
            {
                Accessor.SetGlobalValues(options, group => ShaderAccessor.FilterByMask(group, mask));
                CurrentHashCode = hashCode;
                CurrentVersion = options.Version;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UpdateShaderFields(UnderMaterialOptions options)
        {
            var hashCode = options.GetHashCode();
            if (hashCode != CurrentHashCode || CurrentVersion != options.Version)
            {
                Accessor.SetGlobalValues(options);
                CurrentHashCode = hashCode;
                CurrentVersion = options.Version;
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
    }
}
