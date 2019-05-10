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
    public class OceanMeshForDX9Updater : MonoBehaviour
    {
        protected OceanMeshForDX9Updater()
        {
        }

        private MeshFilter meshFilter;
        [SerializeField] private OceanMeshForDX9 meshforDX9 = null;

        private void UpdateOceanMesh()
        {
            Mesh target = meshforDX9.CreateMesh();
            if (target != null)
            {
                meshFilter.sharedMesh = target;
            }
        }

        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null)
            {
                if (meshforDX9 != null)
                {
                    Mesh target = meshforDX9.CreateMesh();
                    if (target != null)
                    {
                        meshFilter.sharedMesh = target;
                        return;
                    }
                }

                Debug.LogWarning("Undefined ocean mesh, may not be rendered", this);
            }
        }

        private void OnWillRenderObject()
        {
            if (meshforDX9 != null)
            {
                Mesh target = meshforDX9.CreateMesh();
                if (target != null)
                {
                    meshFilter.sharedMesh = target;
                }
            }
        }
    }
}
