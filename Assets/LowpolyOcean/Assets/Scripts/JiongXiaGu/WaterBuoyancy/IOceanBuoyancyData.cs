using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{
    public interface IOceanData
    {
        float Density { get; }

        /// <summary>
        /// Get the distance from the surface, the positive number is above the surface, and the negative number is below the surface;
        /// </summary>
        float DistanceToSurface(Vector3 postion);
    }
}
