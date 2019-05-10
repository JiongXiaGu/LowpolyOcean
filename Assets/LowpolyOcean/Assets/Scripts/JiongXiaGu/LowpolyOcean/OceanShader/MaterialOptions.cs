using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    public class MaterialOptions
    {
        public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(MaterialOptions));
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

        public void Lerp(MaterialOptions v0, MaterialOptions v1, float t)
        {
            Accessor.Lerp(v0, v1, this, t);
        }
    }
}
