using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// analyze scene ocean data and provide it to the camera
    /// </summary>
    public interface ICameraTaskController : ICameraTaskData, IDisposable
    {
        void OnPreOceanCull(OceanCameraTask oceanCamera);

        void OnPreOceanRender(OceanCameraTask oceanCamera);
        void OnPostOceanRender(OceanCameraTask oceanCamera);

        void OnPreRefractionRender(OceanCameraTask oceanCamera);
        void OnPostRefractionRender(OceanCameraTask oceanCamera);

        void OnPreReflectionRender(OceanCameraTask oceanCamera);
        void OnPostReflectionRender(OceanCameraTask oceanCamera);
    }
}
