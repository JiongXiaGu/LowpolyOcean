using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    public class FoamCameraData : IDisposable
    {
        public CullingGroup CullingGroup { get; private set; }

        public FoamCameraData(Camera camera)
        {
            CullingGroup = new CullingGroup();
            CullingGroup.targetCamera = camera;
            CullingGroup.SetBoundingSpheres(FoamShpereSystem.Current.BoundingSpheres);
        }

        public void Dispose()
        {
            if (CullingGroup != null)
            {
                CullingGroup.Dispose();
                CullingGroup = null;
            }
        }
    }
}
