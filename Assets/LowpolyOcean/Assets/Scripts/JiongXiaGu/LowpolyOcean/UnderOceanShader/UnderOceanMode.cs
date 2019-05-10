using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{

    [Flags]
    public enum UnderOceanMode
    {
        LightingRuntimeAtten = 1 << 0,
        FogInPass = 1 << 1,
        CookieAddition = 1 << 2,
        Clip = 1 << 3,
    }
}
