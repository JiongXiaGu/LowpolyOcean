using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Prepare textures at runtime, add with <see cref="Camera"/>.
    /// Camera rendering ocean need to turn off 'MSAA'.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanCameraTask))]
    public sealed class OceanCameraTask : MonoBehaviour
    {
        private OceanCameraTask()
        {
        }

        public static string TransparentKeyword => OceanShaderOptions.TransparentKeyword;
        public static int RefractionTextureShaderID => OceanShaderOptions.RefractionTextureShaderID;
        public static int RefractionDepthTextureShaderID => OceanShaderOptions.RefractionDepthTextureShaderID;
        public static int ReflectionTextureShaderID => OceanShaderOptions.ReflectionTextureShaderID;
        public static int ClipTextureShaderID => OceanShaderOptions.ClipTextureShaderID;
        public static int ClipDepthTextureShaderID => OceanShaderOptions.ClipDepthTextureShaderID;
        public static int UnderOceanMarkTextureShaderID => OceanShaderOptions.UnderOceanMarkTextureShaderID;
        public static int UnderOceanMarkDepthTextureShaderID => OceanShaderOptions.UnderOceanMarkDepthTextureShaderID;

        private static readonly Lazy<Texture2D> BlackTexture = new Lazy<Texture2D>(delegate ()
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new Color[4] { Color.black, Color.black, Color.black, Color.black});
            tex.Apply();
            return tex;
        });

        [SerializeField] private bool enableUnderOceanEffect = false;
        public Camera ThisCamera { get; private set; }
        public Skybox Skybox { get; set; }
        private DepthEffectHandle depthEffectHandle;
        private ReflectionHandle reflectionHandle;
        private ClipHandle clipHandle;
        private UnderOceanMarkHandle underOceanMarkHandle;

        /// <summary>
        /// Add one per frame
        /// </summary>
        public int Version { get; private set; }
        public ICameraTaskController TaskController { get; set; }
        public ICameraTaskData Data => TaskController;
        public static OceanCameraTask Current { get; private set; }
        private List<OceanRendererBase> willRenderOceans;
        public IReadOnlyList<OceanRendererBase> WillRenderOceans => willRenderOceans;
        private ProjectSettings projectSetting => ProjectSettings.Current;

        public bool EnableUnderOceanEffect
        {
            get { return enableUnderOceanEffect; }
            set { enableUnderOceanEffect = value; }
        }

        private void Awake()
        {
            ThisCamera = GetComponent<Camera>();
            Skybox = GetComponent<Skybox>();
            depthEffectHandle = new DepthEffectHandle();
            reflectionHandle = new ReflectionHandle();
            clipHandle = new ClipHandle();
            underOceanMarkHandle = new UnderOceanMarkHandle();
            willRenderOceans = new List<OceanRendererBase>();
        }

#if UNITY_EDITOR
        private bool isInit;
#endif
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!isInit)
            {
                Awake();
                isInit = true;
            }
#endif
            depthEffectHandle.Initialize(ThisCamera);
            reflectionHandle.Initialize(ThisCamera);
            clipHandle.Initialize(ThisCamera);
            underOceanMarkHandle.Initialize(ThisCamera);
            TaskController = new CameraTaskController(this);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (isInit)
                isInit = false;
            else
                return;
#endif
            ReleaseTempValues();
            depthEffectHandle.Destroy(ThisCamera);
            reflectionHandle.Destory(ThisCamera);
            clipHandle.Destroy(ThisCamera);
            underOceanMarkHandle.Destroy(ThisCamera);
            ThisCamera.depthTextureMode = DepthTextureMode.None;

            TaskController.Dispose();
            TaskController = null;
        }

        public void AddWillRenderOcean(OceanRendererBase sender)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            willRenderOceans.Add(sender);
        }
        
        private void ReleaseTempValues()
        {
            depthEffectHandle.ReleaseTempValues();
            reflectionHandle.ReleaseTempValues();
            clipHandle.ReleaseTempValues();
            underOceanMarkHandle.ReleaseTempValues();

            willRenderOceans.Clear();
        }

        private void OnPreCull()
        {
            TaskController.OnPreOceanCull(this);
        }

        private void OnPreRender()
        {
            try
            {
                Current = this;
                var depthTextureMode = DepthTextureMode.None;

                if (TaskController == null)
                {
                    Debug.LogError(new ArgumentNullException(nameof(TaskController)), this);
                    return;
                }

                TaskController.OnPreOceanRender(this);
                var renderContents = TaskController.PreparedContents;

                depthTextureMode |= underOceanMarkHandle.Render(ThisCamera, this, renderContents);
                depthTextureMode |= clipHandle.Render(ThisCamera, this, renderContents);
                depthTextureMode |= depthEffectHandle.Render(ThisCamera, this, renderContents);
                depthTextureMode |= reflectionHandle.Render(ThisCamera, this, Data);

                ThisCamera.depthTextureMode = depthTextureMode;
            }
            catch
            {
                ReleaseTempValues();
                Current = null;
                throw;
            }
        }

        private void OnPostRender()
        {
            try
            {
                ReleaseTempValues();
                TaskController.OnPostOceanRender(this);
            }
            finally
            {
                Current = null;
                Version++;
            }
        }

        private static void TryReleaseTemporary(ref RenderTexture renderTexture)
        {
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = null;
            }
        }

        private static readonly List<IObserver<CameraTaskEvent>> underOceanMarkDrawers = new List<IObserver<CameraTaskEvent>>();
        private static readonly List<IObserver<CameraTaskEvent>> underOceanMarkObservers = new List<IObserver<CameraTaskEvent>>();
        private static readonly List<IObserver<CameraTaskEvent>> clipMarkDrawers = new List<IObserver<CameraTaskEvent>>();
        private static readonly List<IObserver<CameraTaskEvent>> clipMarkObservers = new List<IObserver<CameraTaskEvent>>();

        /// <summary>
        /// notify observer at next rendering and remove observer
        /// </summary>
        public void AddUnderOceanMarkDrawer(IObserver<CameraTaskEvent> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            underOceanMarkDrawers.Add(observer);
        }

        /// <summary>
        /// notify observer every rendering until the observer is removed
        /// </summary>
        public static IDisposable SubscribeUnderOceanMarkDraw(IObserver<CameraTaskEvent> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            underOceanMarkObservers.Add(observer);
            return new Unsubscriber<IObserver<CameraTaskEvent>>(underOceanMarkObservers, observer);
        }

        /// <summary>
        /// notify observer at next rendering and remove observer
        /// </summary>
        public static void AddClipMarkDrawer(IObserver<CameraTaskEvent> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            clipMarkObservers.Add(observer);
        }

        /// <summary>
        /// notify observer every rendering until the observer is removed
        /// </summary>
        public static IDisposable SubscribeClipMarkDraw(IObserver<CameraTaskEvent> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            clipMarkObservers.Add(observer);
            return new Unsubscriber<IObserver<CameraTaskEvent>>(clipMarkObservers, observer);
        }

        private static void NotifyObservers(List<IObserver<CameraTaskEvent>> observers, CameraTaskEvent eventValue)
        {
            foreach (var observer in observers)
            {
                try
                {
                    observer.OnNext(eventValue);
                }
                catch(Exception ex)
                {
                    observer.OnError(ex);
                }
            }
        }

        private class Unsubscriber<T> : IDisposable
        {
            private List<T> list;
            private T value;

            public Unsubscriber(List<T> list, T value)
            {
                this.list = list;
                this.value = value;
            }

            ~Unsubscriber()
            {
                if (list != null)
                {
                    list.Remove(value);
                    list = null;
                    value = default(T);
                }
            }

            public void Dispose()
            {
                if (list != null)
                {
                    list.Remove(value);
                    list = null;
                    value = default(T);
                    GC.SuppressFinalize(this);
                }
            }
        }

        #region Debug Tools

#if UNITY_EDITOR

        [Header("Debug")]
        [SerializeField] private OutputColorMode outputColor = OutputColorMode.None;
        [SerializeField] private OuputTextureMode ouputTexture = OuputTextureMode.None;

        private enum OutputColorMode
        {
            None, R, G, B, A,
        }

        private enum OuputTextureMode
        {
            None,
            RefractionTexture,
            RefractionDepthTexture,
            ReflectionTexture,
            ClipTexture,
            ClipDepthTexture,
            UnderOceanMaskTexture,
            UnderOceanMarkDepthTexture,
        }

        private void CustomBlitColor(Texture source, RenderTexture destination)
        {
            const string rShaderName = "Hidden/LowpolyOcean/Tools/OutputColorR";
            const string bShaderName = "Hidden/LowpolyOcean/Tools/OutputColorB";
            const string gShaderName = "Hidden/LowpolyOcean/Tools/OutputColorG";
            const string aShaderName = "Hidden/LowpolyOcean/Tools/OutputColorA";

            if (outputColor == OutputColorMode.None)
            {
                Graphics.Blit(source, destination);
            }
            else if (outputColor == OutputColorMode.R)
            {
                Material material = new Material(Shader.Find(rShaderName));
                Graphics.Blit(source, destination, material);
            }
            else if (outputColor == OutputColorMode.G)
            {
                Material material = new Material(Shader.Find(gShaderName));
                Graphics.Blit(source, destination, material);
            }
            else if (outputColor == OutputColorMode.B)
            {
                Material material = new Material(Shader.Find(bShaderName));
                Graphics.Blit(source, destination, material);
            }
            else if (outputColor == OutputColorMode.A)
            {
                Material material = new Material(Shader.Find(aShaderName));
                Graphics.Blit(source, destination, material);
            }
        }

        private void CustomBlit(Texture source, RenderTexture destination)
        {
            switch (ouputTexture)
            {
                case OuputTextureMode.None:
                    Graphics.Blit(source, destination);
                    break;

                case OuputTextureMode.RefractionTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(RefractionTextureShaderID), destination);
                    break;

                case OuputTextureMode.RefractionDepthTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(RefractionDepthTextureShaderID), destination);
                    break;

                case OuputTextureMode.ReflectionTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(ReflectionTextureShaderID), destination);
                    break;

                case OuputTextureMode.ClipTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(ClipTextureShaderID), destination);
                    break;

                case OuputTextureMode.ClipDepthTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(ClipDepthTextureShaderID), destination);
                    break;

                case OuputTextureMode.UnderOceanMaskTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(UnderOceanMarkTextureShaderID), destination);
                    break;

                case OuputTextureMode.UnderOceanMarkDepthTexture:
                    CustomBlitColor(Shader.GetGlobalTexture(UnderOceanMarkDepthTextureShaderID), destination);
                    break;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            try
            {
                CustomBlit(source, destination);
            }
            catch
            {
                Graphics.Blit(source, destination);
                throw;
            }
        }
#endif

        #endregion

        private sealed class DepthEffectHandle
        {
            public RenderTexture RefractionTexture;
            public RenderTexture RefractionDepthTexture;
            public bool RefractionTextureCompleted => RefractionTexture != null;
            public bool RefractionDepthTextureCompleted => RefractionDepthTexture != null;
            private Camera renderCamera;
            private Skybox skybox;
            private const CameraEvent colorCameraEvent = CameraEvent.AfterSkybox;
            private CommandBuffer colorCommandBuffer;
            private bool isAddCameraColor;

            private void SetValuesToShader()
            {
                Shader.SetGlobalTexture(RefractionTextureShaderID, RefractionTexture);
                Shader.SetGlobalTexture(RefractionDepthTextureShaderID, RefractionDepthTexture);
            }

            public void ReleaseTempValues()
            {
                TryReleaseTemporary(ref RefractionTexture);
                TryReleaseTemporary(ref RefractionDepthTexture);
            }

            internal void AddCommandBufferColor(Camera camera, OceanCameraTask data)
            {
                if (!isAddCameraColor)
                {
                    if (colorCommandBuffer == null)
                    {
                        colorCommandBuffer = new CommandBuffer();
                        colorCommandBuffer.name = "Refraction ocean color";
                        int screenCopy = Shader.PropertyToID("ScreenCopy");
                        colorCommandBuffer.GetTemporaryRT(screenCopy, -1, -1, 0, FilterMode.Bilinear);
                        colorCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopy);
                        colorCommandBuffer.SetGlobalTexture(RefractionTextureShaderID, screenCopy);
                    }
                    isAddCameraColor = true;
                    camera.AddCommandBuffer(colorCameraEvent, colorCommandBuffer);
                }
            }

            internal void RemoveCommandBufferColor(Camera camera)
            {
                if (isAddCameraColor)
                {
                    camera.RemoveCommandBuffer(colorCameraEvent, colorCommandBuffer);
                    isAddCameraColor = false;
                }
            }

            internal void OceanIsTransparent()
            {
                Shader.EnableKeyword(TransparentKeyword);
            }

            internal void OceanIsOpaque()
            {
                Shader.DisableKeyword(TransparentKeyword);
            }

            public void Initialize(Camera camera)
            {
                string name = RenderHelper.GetTempCameraName(camera.name, "Refraction");
                renderCamera = RenderHelper.CreateCamera(name, out skybox);
                renderCamera.depthTextureMode = DepthTextureMode.Depth;
                renderCamera.allowMSAA = false;
                renderCamera.allowHDR = false;
            }

            public void Destroy(Camera camera)
            {
                DestroyImmediate(renderCamera.gameObject);
                renderCamera = null;

                RemoveCommandBufferColor(camera);
            }

            internal void InternalRenderDepthOnly(Camera camera, OceanCameraTask oceanCamera, DepthEffecttProjectSettings setting, PreparedContent renderContents)
            {
                var textureSize = RenderHelper.GetTextureSize(camera, setting.TextureScale);
                var depthRenderTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 24, RenderTextureFormat.Depth);

                renderCamera.clearFlags = CameraClearFlags.Depth;
                renderCamera.renderingPath = setting.CameraRenderingPath;
                RenderHelper.CopyCameraOptions(camera, renderCamera);
                RenderHelper.SetLayerWithoutWater(renderCamera, setting.CullingMask & camera.cullingMask);
                renderCamera.transform.position = camera.transform.position;
                renderCamera.transform.rotation = camera.transform.rotation;

                using (new RenderHelper.PointLightsScope(false))
                {
                    oceanCamera.TaskController.OnPreRefractionRender(oceanCamera);
                    renderCamera.targetTexture = depthRenderTexture;
                    renderCamera.Render();
                    renderCamera.targetTexture = null;
                    oceanCamera.TaskController.OnPostRefractionRender(oceanCamera);
                }

                RefractionDepthTexture = depthRenderTexture;
            }

            internal void InternalRenderColorAndDepth(Camera camera, OceanCameraTask oceanCamera, DepthEffecttProjectSettings setting, PreparedContent renderContents)
            {
                var textureSize = RenderHelper.GetTextureSize(camera, setting.TextureScale);
                var colorRenderTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y);
                var depthRenderTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 24, RenderTextureFormat.Depth);

                renderCamera.clearFlags = CameraClearFlags.Skybox;
                renderCamera.renderingPath = setting.CameraRenderingPath;
                RenderHelper.CopyCameraOptions(camera, renderCamera);
                RenderHelper.SetLayerWithoutWater(renderCamera, setting.CullingMask & camera.cullingMask);
                RenderHelper.CopySkyBox(oceanCamera, skybox);
                renderCamera.transform.position = camera.transform.position;
                renderCamera.transform.rotation = camera.transform.rotation;

                using (new RenderHelper.PointLightsScope(setting.EnablePointLights))
                {
                    oceanCamera.TaskController.OnPreRefractionRender(oceanCamera);
                    renderCamera.SetTargetBuffers(colorRenderTexture.colorBuffer, depthRenderTexture.depthBuffer);
                    renderCamera.Render();
                    renderCamera.targetTexture = null;
                    oceanCamera.TaskController.OnPostRefractionRender(oceanCamera);
                }

                RefractionTexture = colorRenderTexture;
                RefractionDepthTexture = depthRenderTexture;
            }

            public DepthTextureMode RenderWhenOpaque(Camera camera, OceanCameraTask data, DepthEffecttProjectSettings setting, PreparedContent renderContents)
            {
                RemoveCommandBufferColor(camera);
                OceanIsOpaque();
                InternalRenderColorAndDepth(camera, data, setting, renderContents);
                SetValuesToShader();
                return DepthTextureMode.None;
            }

            public DepthTextureMode RenderDepthOnlyWhenOpaque(Camera camera, OceanCameraTask data, DepthEffecttProjectSettings setting, PreparedContent renderContents)
            {
                RemoveCommandBufferColor(camera);
                OceanIsOpaque();
                InternalRenderDepthOnly(camera, data, setting, renderContents);
                SetValuesToShader();
                return DepthTextureMode.None;
            }

            public DepthTextureMode RenderWhenTransparent(Camera camera, OceanCameraTask data)
            {
                AddCommandBufferColor(camera, data);
                OceanIsTransparent();
                return DepthTextureMode.Depth;
            }

            public DepthTextureMode RenderDepthOnlyWhenTransparent(Camera camera, OceanCameraTask data)
            {
                RemoveCommandBufferColor(camera);
                OceanIsTransparent();
                return DepthTextureMode.Depth;
            }

            public DepthTextureMode Render(Camera camera, OceanCameraTask data, PreparedContent renderContents)
            {
                var quality = data.projectSetting.DepthEffect;

                if ((renderContents & PreparedContent.RefractionTexture) != 0)
                {
                    switch (quality.RenderMode)
                    {
                        case DepthEffectRenderMode.Camera:
                            return RenderWhenOpaque(camera, data, quality, renderContents);

                        case DepthEffectRenderMode.Buffer:
                            return RenderWhenTransparent(camera, data);

                        default:
                            Debug.LogError(new ArgumentOutOfRangeException(nameof(quality.RenderMode), quality.RenderMode, "Unkonw value"), camera);
                            return DepthTextureMode.None;
                    }
                }
                else if ((renderContents & PreparedContent.RefractionDepthTexture) != 0)
                {
                    switch (quality.RenderMode)
                    {
                        case DepthEffectRenderMode.Buffer:
                            return RenderDepthOnlyWhenTransparent(camera, data);

                        case DepthEffectRenderMode.Camera:
                            return RenderDepthOnlyWhenOpaque(camera, data, quality, renderContents);

                        default:
                            Debug.LogError(new ArgumentOutOfRangeException(nameof(quality.RenderMode), quality.RenderMode, "Unkonw value"), camera);
                            return DepthTextureMode.None;
                    }
                }
                return DepthTextureMode.None;
            }
        }

        private sealed class ReflectionHandle
        {
            public RenderTexture ReflectionTexture;

            private Camera renderCamera;
            private Skybox skybox;
            public float clipPlaneOffset = 0.1f;

            private void SetValuesToShader()
            {
                Shader.SetGlobalTexture(ReflectionTextureShaderID, ReflectionTexture);
            }

            public void ReleaseTempValues()
            {
                TryReleaseTemporary(ref ReflectionTexture);
            }

            public void Initialize(Camera camera)
            {
                string name = RenderHelper.GetTempCameraName(camera.name, "Reflection");
                renderCamera = RenderHelper.CreateCamera(name, out skybox);
                renderCamera.allowMSAA = false;
            }

            public void Destory(Camera camera)
            {
                DestroyImmediate(renderCamera.gameObject);
                renderCamera = null;
            }

            // Calculates reflection matrix around the given plane 
            private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
            {
                reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
                reflectionMat.m01 = (-2F * plane[0] * plane[1]);
                reflectionMat.m02 = (-2F * plane[0] * plane[2]);
                reflectionMat.m03 = (-2F * plane[3] * plane[0]);

                reflectionMat.m10 = (-2F * plane[1] * plane[0]);
                reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
                reflectionMat.m12 = (-2F * plane[1] * plane[2]);
                reflectionMat.m13 = (-2F * plane[3] * plane[1]);

                reflectionMat.m20 = (-2F * plane[2] * plane[0]);
                reflectionMat.m21 = (-2F * plane[2] * plane[1]);
                reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
                reflectionMat.m23 = (-2F * plane[3] * plane[2]);

                reflectionMat.m30 = 0F;
                reflectionMat.m31 = 0F;
                reflectionMat.m32 = 0F;
                reflectionMat.m33 = 1F;
            }

            // Given position/normal of the plane, calculates plane in camera space.
            private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
            {
                Vector3 offsetPos = pos + normal * clipPlaneOffset;
                Matrix4x4 m = cam.worldToCameraMatrix;
                Vector3 cpos = m.MultiplyPoint(offsetPos);
                Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
                return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
            }

            internal void InternalRender(Camera camera, OceanCameraTask oceanCamera, ReflectionProjectSetting setting, ICameraTaskData data)
            {
                if (camera.orthographic)
                    return;

                var textureSize = RenderHelper.GetTextureSize(camera, setting.TextureScale);
                var reflectionRenderTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y);

                renderCamera.renderingPath = setting.RenderingPath;
                RenderHelper.CopyCameraOptions(camera, renderCamera);
                RenderHelper.SetLayerWithoutWater(renderCamera, setting.CullingMask);
                RenderHelper.CopySkyBox(oceanCamera, skybox);

                {
                    Vector3 pos = new Vector3(0, data.Height, 0);
                    Vector3 normal = data.Normal;

                    // Reflect camera around reflection plane
                    float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
                    Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                    Matrix4x4 reflection = Matrix4x4.zero;
                    CalculateReflectionMatrix(ref reflection, reflectionPlane);
                    renderCamera.transform.position = reflection.MultiplyPoint(camera.transform.position);
                    renderCamera.worldToCameraMatrix = camera.worldToCameraMatrix * reflection;

                    // Setup oblique projection matrix so that near plane is our reflection
                    // plane. This way we clip everything below/above it for free.
                    Vector4 clipPlane = CameraSpacePlane(renderCamera, pos, normal, 1.0f);
                    renderCamera.projectionMatrix = camera.CalculateObliqueMatrix(clipPlane);

                    //// Set custom culling matrix from the current camera
                    renderCamera.cullingMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;

                    Vector3 euler = camera.transform.eulerAngles;
                    renderCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
                }

                using (new RenderHelper.InvertCullingScope())
                {
                    using (new RenderHelper.PointLightsScope(setting.EnablePointLights))
                    {
                        oceanCamera.TaskController.OnPreReflectionRender(oceanCamera);
                        renderCamera.targetTexture = reflectionRenderTexture;
                        renderCamera.Render();
                        renderCamera.targetTexture = null;
                        oceanCamera.TaskController.OnPostReflectionRender(oceanCamera);
                    }
                }

                ReflectionTexture = reflectionRenderTexture;
                SetValuesToShader();
            }

            public DepthTextureMode Render(Camera camera, OceanCameraTask oceanCamera, ICameraTaskData data)
            {
                var quality = oceanCamera.projectSetting.Reflection;
                if ((data.PreparedContents & PreparedContent.ReflectionTexture) != 0)
                {
                    InternalRender(camera, oceanCamera, quality, data);
                }
                return DepthTextureMode.None;
            }
        }

        private sealed class ClipHandle
        {
            public RenderTexture ClipTexture;
            public RenderTexture ClipDepthTexture;
            public bool IsCompleted => ClipTexture != null;
            private Camera renderCamera;

            public void SetValuesToShder()
            {
                Shader.SetGlobalTexture(ClipTextureShaderID, ClipTexture);
                Shader.SetGlobalTexture(ClipDepthTextureShaderID, ClipDepthTexture);
            }

            public void ReleaseTempValues()
            {
                TryReleaseTemporary(ref ClipTexture);
                TryReleaseTemporary(ref ClipDepthTexture);
            }

            public void Initialize(Camera camera)
            {
                string name = RenderHelper.GetTempCameraName(camera.name, "OceanClip");
                renderCamera = RenderHelper.CreateCamera(name);
                renderCamera.clearFlags = CameraClearFlags.Color;
                renderCamera.backgroundColor = new Color(0, 0, 0, 0);
                renderCamera.allowMSAA = false;
                renderCamera.allowHDR = false;
                renderCamera.depthTextureMode = DepthTextureMode.Depth;
                renderCamera.renderingPath = RenderingPath.Forward;
            }

            public void Destroy(Camera camera)
            {
                DestroyImmediate(renderCamera.gameObject);
                renderCamera = null;
            }

            public DepthTextureMode Render(Camera camera, OceanCameraTask oceanCamera, PreparedContent renderContents)
            {
                try
                {
                    if (((renderContents & PreparedContent.ClipTexture) == 0) || (clipMarkObservers.Count == 0 && clipMarkDrawers.Count == 0))
                    {
                        Shader.SetGlobalTexture(ClipTextureShaderID, BlackTexture.Value);
                        Shader.SetGlobalTexture(ClipDepthTextureShaderID, BlackTexture.Value);
                        return DepthTextureMode.None;
                    }

                    CameraTaskEvent eventValue = new CameraTaskEvent(renderCamera, oceanCamera, oceanCamera.projectSetting.MarkLayer);
                    NotifyObservers(clipMarkDrawers, eventValue);
                    NotifyObservers(clipMarkObservers, eventValue);

                    var textureSize = RenderHelper.GetTextureSize(camera, oceanCamera.projectSetting.OceanClip.TextureScale);
                    ClipTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 0);
                    ClipDepthTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 24, RenderTextureFormat.Depth);

                    RenderHelper.CopyCameraOptions(camera, renderCamera);
                    renderCamera.cullingMask = oceanCamera.projectSetting.MarkCullingMask;

                    renderCamera.transform.position = camera.transform.position;
                    renderCamera.transform.rotation = camera.transform.rotation;

                    renderCamera.SetTargetBuffers(ClipTexture.colorBuffer, ClipDepthTexture.depthBuffer);
                    renderCamera.Render();
                    renderCamera.targetTexture = null;

                    SetValuesToShder();
                    return DepthTextureMode.None;
                }
                finally
                {
                    clipMarkDrawers.Clear();
                }
            }
        }

        private sealed class UnderOceanMarkHandle
        {
            public RenderTexture UnderOceanMarkTexture;
            public RenderTexture UnderOceanMarkDepthTexture;
            private Camera renderCamera;

            public void SetValuesToShader()
            {
                Shader.SetGlobalTexture(UnderOceanMarkTextureShaderID, UnderOceanMarkTexture);
                Shader.SetGlobalTexture(UnderOceanMarkDepthTextureShaderID, UnderOceanMarkDepthTexture);
            }

            public void ReleaseTempValues()
            {
                TryReleaseTemporary(ref UnderOceanMarkTexture);
                TryReleaseTemporary(ref UnderOceanMarkDepthTexture);
            }

            public void Initialize(Camera camera)
            {
                string name = RenderHelper.GetTempCameraName(camera.name, "OceanMark");
                renderCamera = RenderHelper.CreateCamera(name);
                renderCamera.clearFlags = CameraClearFlags.Color;
                renderCamera.backgroundColor = new Color(0, 0, 0, 0);
                renderCamera.allowMSAA = false;
                renderCamera.allowHDR = false;
                renderCamera.depthTextureMode = DepthTextureMode.Depth;
                renderCamera.renderingPath = RenderingPath.Forward;
            }

            public void Destroy(Camera camera)
            {
                DestroyImmediate(renderCamera.gameObject);
                renderCamera = null;
            }

            public DepthTextureMode Render(Camera camera, OceanCameraTask oceanCamera, PreparedContent renderContents)
            {
                try
                {
                    if (((renderContents & PreparedContent.UnderOceanMarkTexture) == 0) || (underOceanMarkDrawers.Count == 0 && underOceanMarkObservers.Count == 0))
                    {
                        Shader.SetGlobalTexture(UnderOceanMarkTextureShaderID, BlackTexture.Value);
                        Shader.SetGlobalTexture(UnderOceanMarkDepthTextureShaderID, BlackTexture.Value);
                        return DepthTextureMode.None;
                    }

                    CameraTaskEvent eventValue = new CameraTaskEvent(renderCamera, oceanCamera, oceanCamera.projectSetting.MarkLayer);
                    NotifyObservers(underOceanMarkDrawers, eventValue);
                    NotifyObservers(underOceanMarkObservers, eventValue);

                    var textureSize = RenderHelper.GetTextureSize(camera, oceanCamera.projectSetting.UnderOceanMark.TextureScale);
                    UnderOceanMarkTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y);
                    UnderOceanMarkDepthTexture = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 24, RenderTextureFormat.Depth);

                    RenderHelper.CopyCameraOptions(camera, renderCamera);
                    renderCamera.cullingMask = oceanCamera.projectSetting.MarkCullingMask;

                    renderCamera.transform.position = camera.transform.position;
                    renderCamera.transform.rotation = camera.transform.rotation;

                    renderCamera.SetTargetBuffers(UnderOceanMarkTexture.colorBuffer, UnderOceanMarkDepthTexture.depthBuffer);
                    renderCamera.Render();
                    renderCamera.targetTexture = null;

                    SetValuesToShader();
                    return DepthTextureMode.None;
                }
                finally
                {
                    underOceanMarkDrawers.Clear();
                }
            }
        }
    }
}
