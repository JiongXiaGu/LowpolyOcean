using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    [Serializable]
    public class OceanClipProjectSetting
    {
        public static OceanClipProjectSetting Default => new OceanClipProjectSetting()
        {
            TextureScale = 1,
        };

        [Range(0.1f, 1)] public float TextureScale;
    }
}
