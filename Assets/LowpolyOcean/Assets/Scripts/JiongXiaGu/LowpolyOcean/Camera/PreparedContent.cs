using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// before scene rendering stage preparation contents
    /// </summary>
    [Flags]
    public enum PreparedContent
    {
        None = 0,
        RefractionTexture = 1 << 0,
        RefractionDepthTexture = 1 << 1,
        ReflectionTexture = 1 << 4,
        ClipTexture = 1 << 5,
        UnderOceanMarkTexture = 1 << 6,
        Ripple = 1 << 8,
        SunLight = 1 << 9,
        Foam8 = 1 << 10,
        Foam16 = 1 << 11,
        FoamArea1 = 1 << 12,
    }
}
