using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{


    public enum LightingMode
    {

        /// <summary>
        /// use UnityPBS.
        /// </summary>
        UnityPBS = OceanMode.LightingUnityPBS,

        /// <summary>
        /// use UnityPBS and Fresnel, add another fresnel on the basis of UnityPBS.
        /// </summary>
        UnityPBSAndFresnel = OceanMode.LightingUnityPBSAndFresnel,
    }
}
