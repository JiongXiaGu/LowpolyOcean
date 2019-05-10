using System.Collections.Generic;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Filter the ripple texture that need to be displayed
    /// </summary>
    public interface IRippleFilter
    {
        void Filter(OceanCameraTask camera, CameraTaskRippleData rippleData, IReadOnlyList<IRippleData> source, IRippleData[] result);
    }
}
