using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    [Serializable]
    public class ReflectionProjectSetting
    {
        public static ReflectionProjectSetting Default => new ReflectionProjectSetting()
        {
            CullingMask = ~(ProjectSettings.DefaultOceanCullingMask),
            RenderingPath = RenderingPath.UsePlayerSettings,
            TextureScale = 0.3f,
            EnablePointLights = true,
        };

        public LayerMask CullingMask;
        public RenderingPath RenderingPath;
        [Range(0.1f, 1)] public float TextureScale;
        public bool EnablePointLights;
    }
}
