using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public interface ICameraTaskData
    {
        PreparedContent PreparedContents { get; }
        float Height { get; }
        Vector3 Normal { get; }
        Light SunLight { get; }
        float OceanTime { get; }
    }

}
