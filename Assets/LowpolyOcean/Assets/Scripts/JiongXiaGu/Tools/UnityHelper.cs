using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{


    public static class UnityHelper
    {

        public static float GetRadiusScaleFactor(Vector3 lossyScale)
        {
            float num = 0f;
            num = Mathf.Max(num, Mathf.Abs(lossyScale.x));
            num = Mathf.Max(num, Mathf.Abs(lossyScale.y));
            num = Mathf.Max(num, Mathf.Abs(lossyScale.z));
            return num;
        }

    }
}
