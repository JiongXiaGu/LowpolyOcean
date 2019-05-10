using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{

    [Flags]
    public enum OceanMode
    {
        Wave1 = 1 << 0,
        Wave2 = 1 << 1,
        Wave3 = 1 << 2,
        RippleMax4 = 1 << 3,
        RefractionSimple = 1 << 4,
        RefractionFull = 1 << 5,
        FoamSimpleEyeDepth = 1 << 6,
        ReflectionSimpleOffset = 1 << 7,
        BackLightingArtistic = 1 << 8,
        PointLightingSimple = 1 << 9,
        Tessellation = 1 << 10,
        BackRefractionSimple = 1 << 11,
        BackRefractionFull = 1 << 12,
        ClipNormal = 1 << 13,
        LightingUnityPBS = 1 << 14,
        CookieAddition = 1 << 15,
        LightingUnityPBSAndFresnel = 1 << 16,
        FoamEyeDepth = 1 << 17,
        FoamShpere8 = 1 << 18,
        FoamShpere16 = 1 << 19,
        FoamArea1 = 1 << 20,
        FoamArea4 = 1 << 21,
    }
}
