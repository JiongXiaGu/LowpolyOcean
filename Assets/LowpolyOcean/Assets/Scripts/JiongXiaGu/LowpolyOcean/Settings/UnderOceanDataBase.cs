using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    public abstract class UnderOceanDataBase : ScriptableObject
    {
        public abstract PreparedContent OnPreOceanRender(OceanCameraTask oceanCamera, OceanVolume ocean);
        public abstract void OnPostOceanRender(OceanCameraTask oceanCamera);
    }
}
