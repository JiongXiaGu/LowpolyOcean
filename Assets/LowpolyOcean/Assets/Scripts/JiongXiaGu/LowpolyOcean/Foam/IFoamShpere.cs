using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public interface IFoamShpere
    {
        Vector3 Position { get; }
        float Radius { get; }
        float Intensity { get; }
    }
}
