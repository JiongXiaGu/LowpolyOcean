using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Some settings that should be determined in editor mode;
    /// use <see cref="SetCurrentProjectSettings(ProjectSettings)"/> if you need to modify the settings at runtime, otherwise the default settings will always be used
    /// </summary>
    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(ProjectSettings))]
    public sealed class ProjectSettings : ScriptableObject
    {
        private ProjectSettings()
        {
        }

        public const int DefaultOceanMarkLayer = 24;
        public const int DefaultOceanLayer = 4;
        public const int DefaultOceanCullingMask = 1 << DefaultOceanLayer;

        public static bool IsInitialized { get; private set; }
        public static ProjectSettings Current { get; private set; }

        public OceanRenderQueue RenderQueue = OceanRenderQueue.Opaque;
        public UnityLayer OceanLayer = DefaultOceanLayer;
        public int OceanCullingMask => 1 << OceanLayer;
        public UnityLayer MarkLayer = DefaultOceanMarkLayer;
        public int MarkCullingMask => 1 << MarkLayer;

        public DepthEffecttProjectSettings DepthEffect = DepthEffecttProjectSettings.Default;
        public ReflectionProjectSetting Reflection = ReflectionProjectSetting.Default;
        public OceanClipProjectSetting OceanClip = OceanClipProjectSetting.Default;
        public UnderOceanMarkProjectSetting UnderOceanMark = UnderOceanMarkProjectSetting.Default;
        public FoamProjectSettings Foam = FoamProjectSettings.Default;

        internal static ProjectSettings GetDefaultProjectSettings()
        {
            var setting = CreateInstance<ProjectSettings>();
            return setting;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void Initialize()
        {
            if (!IsInitialized)
            {
                var settings = GetDefaultProjectSettings();
                SetValueToShader(settings);
                Current = settings;
            }
        }

        private static void SetValueToShader(ProjectSettings settings)
        {
            UnderOceanMarkProjectSetting.ShaderAccessor.SetGlobalValues(settings.UnderOceanMark);
        }

        public static void SetCurrentProjectSettings(ProjectSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            SetValueToShader(settings);
            Current = settings;
            IsInitialized = true;
        }

        private void OnValidate()
        {
            if (Current == this)
            {
                SetValueToShader(this);
            }
        }

        [ContextMenu(nameof(SetToCurrentSettings))]
        public void SetToCurrentSettings()
        {
            SetCurrentProjectSettings(this);
        }
    }
}
