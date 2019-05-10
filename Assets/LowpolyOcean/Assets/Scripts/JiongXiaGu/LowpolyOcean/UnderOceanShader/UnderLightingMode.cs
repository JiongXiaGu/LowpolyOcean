using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{


    public enum UnderLightingMode
    {
        None = 0,

        /// <summary>
        /// Real-time light attenuation
        /// </summary>
        RuntimeAtten = UnderOceanMode.LightingRuntimeAtten,
    }
}
