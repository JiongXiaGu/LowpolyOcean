using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    public abstract class OceanRendererBase : MonoBehaviour
    {
        public abstract PreparedContent GetRenderContents(OceanCameraTask oceanCamera);
    }
}
