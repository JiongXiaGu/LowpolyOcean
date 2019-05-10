using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// use to provide <see cref="RippleSystem"/> ripple data
    /// </summary>
    public interface IRippleData
    {
        Texture Texture { get; }
        Vector4 Rect { get; }
        Vector3 Position { get; }
        float Radian { get; }
        Vector4 HeightScale { get; }

        /// <summary>
        /// called when the filter accesses it, returns false if the current state is not available, otherwise returns true
        /// </summary>
        bool OnEnter(OceanCameraTask camera);

        /// <summary>
        /// called when it is selected, meaning it will be render
        /// </summary>
        void OnSelecte(OceanCameraTask camera);
    }
}
