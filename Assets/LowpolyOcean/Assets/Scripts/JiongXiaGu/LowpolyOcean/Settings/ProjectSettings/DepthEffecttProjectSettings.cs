using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    [Serializable]
    public class DepthEffecttProjectSettings
    {
        public static DepthEffecttProjectSettings Default => new DepthEffecttProjectSettings()
        {
            RenderMode = DepthEffectRenderMode.Camera,
            CullingMask = ~(ProjectSettings.DefaultOceanCullingMask),
            CameraRenderingPath = RenderingPath.UsePlayerSettings,
            TextureScale = 1,
            EnablePointLights = true,
        };

        public DepthEffectRenderMode RenderMode;
        public LayerMask CullingMask;
        public RenderingPath CameraRenderingPath;
        [Range(0.1f, 1)] public float TextureScale;
        public bool EnablePointLights;
    }
}
