using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{


    public enum WaveMode
    {
        None,

        /// <summary>
        /// One horizontal offset(BA) and one vertical offset(RG)
        /// </summary>
        Wave1 = OceanMode.Wave1,

        /// <summary>
        /// Two vertical offsets(RG, BA)
        /// </summary>
        Wave2 = OceanMode.Wave2,

        /// <summary>
        /// Three vertical offsets(R, G, B), one horizontal offset(A)
        /// </summary>
        Wave3 = OceanMode.Wave3,
    }
}
