using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanRenderer))]
    public sealed class OceanRenderer : OceanRendererBase, IObserver<CameraTaskEvent>
    {
        private OceanRenderer()
        {
        }

        [SerializeField] private OceanDataBase data;
        public OceanDataBase Data
        {
            get { return data; }
            set { data = value; }
        }

        [SerializeField] [Range(0, 6)] private int lodLevel = 0;
        public int LODLevel
        {
            get { return lodLevel; }
            set { lodLevel = value; }
        }

        public MeshFilter MeshFilter { get; private set; }

        private void Awake()
        {
            MeshFilter = GetComponent<MeshFilter>();
        }

        private bool isRending;
        private Camera requestCamera;
        private OceanCameraTask requestOceanCamera;

        private void OnWillRenderObject()
        {
            if (isRending || data == null)
                return;

            var camera = Camera.current;
            if (camera == null)
                return;

            OceanCameraTask oceanCamera;
            if (RenderHelper.TryGetOceanCamera(camera, out oceanCamera))
            {
                isRending = true;
                requestCamera = camera;
                requestOceanCamera = oceanCamera;
                oceanCamera.AddWillRenderOcean(this);
                oceanCamera.AddUnderOceanMarkDrawer(this);
            }

        }

        private void OnRenderObject()
        {
            if (Camera.current == requestCamera)
            {
                isRending = false;
                requestCamera = null;
                requestOceanCamera = null;
            }
        }

        public override PreparedContent GetRenderContents(OceanCameraTask oceanCamera)
        {
            PreparedContent preparedContents = Data.GetRenderContents(lodLevel);
            return preparedContents;
        }

        void IObserver<CameraTaskEvent>.OnCompleted()
        {
        }

        void IObserver<CameraTaskEvent>.OnError(Exception error)
        {
            Debug.LogError(error, this);
        }

        void IObserver<CameraTaskEvent>.OnNext(CameraTaskEvent value)
        {
            var markMat = data.GetMarkMaterial(lodLevel);
            if (markMat != null)
            {
                Matrix4x4 matrix = transform.ToMatrix();
                Graphics.DrawMesh(MeshFilter.sharedMesh, matrix, markMat, value.Layer, value.RenderCamera, 0, null, false, false, false);
            }
        }
    }
}
