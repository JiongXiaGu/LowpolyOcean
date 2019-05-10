using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// mark drawer base class, inherit when you need to provide mark to the <see cref="OceanCameraTask"/>
    /// </summary>
    public abstract class OceanMarkDrawer : MonoBehaviour, IObserver<CameraTaskEvent>
    {
        protected OceanMarkDrawer()
        {
        }

        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private Material material;

        protected Mesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        protected Material Material
        {
            get { return material; }
            set { material = value; }
        }

        protected virtual Color GetDrawColor()
        {
            return Color.red;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (mesh == null || !enabled)
                return;

            Gizmos.color = GetDrawColor();
            Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.lossyScale);
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
            Debug.LogError(error, this);
        }

        public virtual void OnNext(CameraTaskEvent value)
        {
            if (mesh == null || material == null)
            {
                Debug.LogWarning(string.Format("Undefined {0}, {1}", nameof(material), nameof(mesh)), this);
                return;
            }

            Matrix4x4 matrix = transform.ToMatrix();
            Graphics.DrawMesh(mesh, matrix, material, value.Layer, value.RenderCamera, 0, null, false, false, false);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Copy material and mesh to the corresponding component for preview
        /// </summary>
        [ContextMenu(nameof(CopyFromComponents))]
        private void CopyFromComponents()
        {
            bool isChanged = false;

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Undo.RecordObject(gameObject, nameof(CopyFromComponents));
                isChanged = true;
                mesh = meshFilter.sharedMesh;
            }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
            {
                if(!isChanged)
                    Undo.RecordObject(gameObject, nameof(CopyFromComponents));

                material = meshRenderer.sharedMaterial;
            }
        }

        /// <summary>
        /// Copy material and mesh to the corresponding component for preview
        /// </summary>
        [ContextMenu(nameof(UpdateOrCreateComponents))]
        private void UpdateOrCreateComponents()
        {
            int undoGroup = Undo.GetCurrentGroup();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = Undo.AddComponent<MeshFilter>(gameObject);
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                Undo.RecordObject(meshFilter, "Change sharedMesh");
                meshFilter.sharedMesh = mesh;
            }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = Undo.AddComponent<MeshRenderer>(gameObject);
                meshRenderer.sharedMaterial = material;
            }
            else
            {
                Undo.RecordObject(meshRenderer, "Change sharedMaterial");
                meshRenderer.sharedMaterial = material;
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        [ContextMenu(nameof(RemoveComponents))]
        private void RemoveComponents()
        {
            int undoGroup = Undo.GetCurrentGroup();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Undo.DestroyObjectImmediate(meshFilter);
            }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Undo.DestroyObjectImmediate(meshRenderer);
            }

            Undo.CollapseUndoOperations(undoGroup);
        }
#endif

    }
}
