using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    public interface IFoamArea
    {
        Texture Texture { get; }
        float Intensity { get; }
        Vector4 Rect { get; }
        Vector3 Position { get; }
    }
}
