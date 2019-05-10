using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{


    public static class RenderHelper
    {

        public static bool TryGetOceanCamera(Camera camera, out OceanCameraTask oceanCamera)
        {
            oceanCamera = camera.GetComponent<OceanCameraTask>();
            if (oceanCamera == null)
            {
                oceanCamera = camera.gameObject.AddComponent<OceanCameraTask>();
            }
            return true;

            //oceanCamera = camera.GetComponent<OceanCameraTask>();
            //if (oceanCamera != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    if ((camera.hideFlags & HideFlags.HideAndDontSave) == 0)
            //    {
            //        oceanCamera = camera.gameObject.AddComponent<OceanCameraTask>();
            //        return true;
            //    }

            //    return false;
            //}
        }


        private const HideFlags defaultHideFlags = HideFlags.HideAndDontSave;

        public static string GetTempCameraName(string targetCamera, string renderCamera)
        {
            return targetCamera + " : " + renderCamera;
        }

        public static Camera CreateCamera(string name)
        {
            GameObject cameraObject = new GameObject(name, typeof(Camera));
            cameraObject.hideFlags = defaultHideFlags;
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.enabled = false;
            return camera;
        }

        public static Camera CreateCamera(string name, out Skybox skybox)
        {
            Camera camera = CreateCamera(name);
            camera.clearFlags = CameraClearFlags.Skybox;
            skybox = camera.gameObject.AddComponent<Skybox>();
            return camera;
        }

        public static void CopySkyBox(OceanCameraTask source, Skybox skybox)
        {
            if (source.ThisCamera.clearFlags == CameraClearFlags.Skybox)
            {
                Skybox sourceSkyBox = source.Skybox;
                if (sourceSkyBox != null && sourceSkyBox.material != null)
                {
                    skybox.enabled = true;
                    skybox.material = sourceSkyBox.material;
                    return;
                }
            }
            skybox.enabled = false;
        }

        public static void CopyCameraOptions(Camera source, Camera destination)
        {
            destination.farClipPlane = source.farClipPlane;
            destination.nearClipPlane = source.nearClipPlane;
            destination.orthographic = source.orthographic;
            destination.fieldOfView = source.fieldOfView;
            destination.aspect = source.aspect;
            destination.orthographicSize = source.orthographicSize;
        }

        public static void CopyCameraOptionsAll(Camera source, Camera destination)
        {
            destination.farClipPlane = source.farClipPlane;
            destination.nearClipPlane = source.nearClipPlane;
            destination.orthographic = source.orthographic;
            destination.fieldOfView = source.fieldOfView;
            destination.aspect = source.aspect;
            destination.orthographicSize = source.orthographicSize;

            destination.allowMSAA = source.allowMSAA;
            destination.allowHDR = source.allowHDR;
            destination.useOcclusionCulling = source.useOcclusionCulling;
        }

        public static void SetLayerWithoutWater(Camera camera, LayerMask layers)
        {
            camera.cullingMask = ~ProjectSettings.Current.OceanCullingMask & layers.value;
        }

        public static Vector2Int GetTextureSize(Camera camera, float textureScale)
        {
            Vector2Int size = Vector2Int.zero;

            size.x = (int)(camera.pixelWidth * textureScale);
            size.y = (int)(camera.pixelHeight * textureScale);

            return size;
        }

        public static Matrix4x4 ToMatrix(this Transform transform)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = transform.lossyScale;
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix;
        }

        public struct PointLightsScope : IDisposable
        {
            public int OldPixelLightCount { get; private set; }

            public PointLightsScope(bool isEnable)
            {
                OldPixelLightCount = QualitySettings.pixelLightCount;
                if (!isEnable)
                {
                    QualitySettings.pixelLightCount = 0;
                }
            }

            public void Dispose()
            {
                QualitySettings.pixelLightCount = OldPixelLightCount;
            }
        }

        public class InvertCullingScope : IDisposable
        {
            bool oldCulling;

            public InvertCullingScope()
            {
                oldCulling = GL.invertCulling;
                GL.invertCulling = !oldCulling;
            }

            public InvertCullingScope(bool isEnable)
            {
                oldCulling = GL.invertCulling;
                GL.invertCulling = isEnable;
            }

            public void Dispose()
            {
                GL.invertCulling = oldCulling;
            }
        }
    }
}
