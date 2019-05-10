using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{


    public enum RefractionMode
    {
        None,

        /// <summary>
        /// Refraction bottom, add water depth effect
        /// </summary>
        Simple = OceanMode.RefractionSimple,

        /// <summary>
        /// Has the function of <see cref="Simple"/>, add refractive offset;
        /// This method requires use texture <see cref="PreparedContent.UnderOceanMarkTexture"/>;
        /// </summary>
        Full = OceanMode.RefractionFull,
    }
}
