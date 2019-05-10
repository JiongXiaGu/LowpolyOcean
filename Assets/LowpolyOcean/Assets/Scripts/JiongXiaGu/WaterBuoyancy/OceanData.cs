using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{


    public abstract class OceanData : ScriptableObject, IOceanData
    {
        [SerializeField] private float density = 1f;
        public float Density
        {
            get { return density; }
            set { density = value; }
        }

        /// <summary>
        /// Get the distance from the surface, the positive number is above the surface, and the negative number is below the surface;
        /// </summary>
        public abstract float DistanceToSurface(Vector3 postion);
    }
}
