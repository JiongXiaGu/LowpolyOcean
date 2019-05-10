using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Mark under ocean area
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(UnderOceanMarkDrawer))]
    public class UnderOceanMarkDrawer : MonoBehaviour, IObserver<CameraTaskEvent>
    {
        protected UnderOceanMarkDrawer()
        {
        }

        [SerializeField] private Transform target;
        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }

        [SerializeField] private float minOffset = -50f;
        public float MinOffset
        {
            get { return minOffset; }
            set { minOffset = value; }
        }

        [SerializeField] private float maxOffset = 10f;
        public float MaxOffset
        {
            get { return maxOffset; }
            set { maxOffset = value; }
        }

        [SerializeField] private float waveHeight = 1f;
        public float WaveHeight
        {
            get { return waveHeight; }
            set { waveHeight = value; }
        }


        [SerializeField] private Material markMaterial = null;
        public Material MarkMaterial
        {
            get { return markMaterial; }
            set { markMaterial = value; }
        }

        private MeshFilter meshFilter;
        private IDisposable unsubscriber;

        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        private void UpdateMarkPosition(Transform camera)
        {
            Vector3 cameraPosition = camera.transform.position;
            float oceanheight = waveHeight;

            var height = oceanheight - cameraPosition.y;
            height = Mathf.Clamp(height, minOffset, maxOffset);

            Vector3 pos = transform.localPosition;
            pos.y = height;

            if (target != null)
            {
                var targetPos = target.transform.position;
                pos.x = targetPos.x;
                pos.z = targetPos.z;
            }

            transform.localPosition = pos;
        }

        private void OnWillRenderObject()
        {
            var camera = Camera.current;
            if (camera == null)
                return;

            OceanCameraTask oceanCamera = camera.GetComponent<OceanCameraTask>();
            if (oceanCamera != null)
            {
                if (markMaterial == null)
                {
                    Debug.LogError(new ArgumentNullException(nameof(markMaterial)), this);
                    return;
                }

                UpdateMarkPosition(camera.transform);
                oceanCamera.AddUnderOceanMarkDrawer(this);
            }
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
            Matrix4x4 matrix = transform.ToMatrix();
            Graphics.DrawMesh(meshFilter.sharedMesh, matrix, markMaterial, value.Layer, value.RenderCamera, 0, null, false, false, false);
        }
    }
}
