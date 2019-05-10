using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    public class FoamProjectSettings
    {
        public static FoamProjectSettings Default => new FoamProjectSettings()
        {
            EnabledFoamShpereCulling = true,
        };

        /// <summary>
        /// if you need any foamShpere mode, need set true
        /// </summary>
        [Tooltip("if you need any foamShpere mode, need set true")]
        public bool EnabledFoamShpereCulling;

    }
}
