using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{


    public struct CameraTaskEvent
    {
        public Camera RenderCamera { get; set; }
        public OceanCameraTask OceanCamera { get; set; }
        public int Layer { get; set; }

        public CameraTaskEvent(Camera renderCamera, OceanCameraTask oceanCamera, int layer)
        {
            RenderCamera = renderCamera;
            OceanCamera = oceanCamera;
            Layer = layer;
        }
    }
}
