using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{

    public enum ReflectionMode
    {
        None = 0,

        /// <summary>
        /// it has a good effect on calm ocean, but it is not so good on undulating ocean(in this case it is recommended to reflect only the distant view, set in <see cref="ProjectSettings.Reflection"/>)
        /// </summary>
        SimpleOffset = OceanMode.ReflectionSimpleOffset,
    }
}
