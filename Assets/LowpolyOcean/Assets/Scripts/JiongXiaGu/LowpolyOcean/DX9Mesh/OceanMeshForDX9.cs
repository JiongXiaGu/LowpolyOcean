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
    /// Mesh solutions for multiple platforms, Because DX11 supports tessellation, DX10 supports geometry shaders, DX9 does not support, so their mesh are different.
    /// </summary>
    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(OceanMeshForDX9))]
    public class OceanMeshForDX9 : ScriptableObjectEX
    {
        protected OceanMeshForDX9()
        {
        }

        [SerializeField] private Mesh mesh = null;
        private Mesh meshForDX9;

        public static void CalculateDX9Mesh(Mesh mesh, List<Vector3> uv0, List<Vector3> uv1)
        {
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            if (vertices.Length != triangles.Length)
            {
                var newVertices = new Vector3[triangles.Length];
                for (int i0 = 0; i0 < triangles.Length; i0 += 3)
                {
                    int i1 = i0 + 1;
                    int i2 = i0 + 2;

                    int t0 = triangles[i0];
                    int t1 = triangles[i1];
                    int t2 = triangles[i2];

                    var p0 = vertices[t0];
                    var p1 = vertices[t1];
                    var p2 = vertices[t2];

                    newVertices[i0] = p0;
                    newVertices[i1] = p1;
                    newVertices[i2] = p2;

                    triangles[i0] = i0;
                    triangles[i1] = i1;
                    triangles[i2] = i2;

                    uv0.Add(p1);
                    uv0.Add(p2);
                    uv0.Add(p0);

                    uv1.Add(p2);
                    uv1.Add(p0);
                    uv1.Add(p1);
                }

                mesh.vertices = newVertices;
            }
            else
            {
                for (int i0 = 0; i0 < triangles.Length; i0 += 3)
                {
                    int i1 = i0 + 1;
                    int i2 = i0 + 2;
                    int t0 = triangles[i0];
                    int t1 = triangles[i1];
                    int t2 = triangles[i2];
                    var p0 = vertices[t0];
                    var p1 = vertices[t1];
                    var p2 = vertices[t2];

                    triangles[i0] = i0;
                    triangles[i1] = i1;
                    triangles[i2] = i2;

                    uv0.Add(p1);
                    uv0.Add(p2);
                    uv0.Add(p0);

                    uv1.Add(p2);
                    uv1.Add(p0);
                    uv1.Add(p1);
                }
            }

            mesh.triangles = triangles;
            mesh.SetUVs(0, uv0);
            mesh.SetUVs(1, uv1);
        }

        private static Mesh CreateDX9Mesh(Mesh mesh)
        {
            var newMesh = Instantiate(mesh);
            newMesh.name = mesh.name + "for dx9";
            List<Vector3> uv0 = new List<Vector3>(newMesh.triangles.Length);
            List<Vector3> uv1 = new List<Vector3>(newMesh.triangles.Length);
            CalculateDX9Mesh(newMesh, uv0, uv1);
            return newMesh;
        }

        [ContextMenu(nameof(CreateMesh))]
        public virtual Mesh CreateMesh()
        {
            if (meshForDX9 == null)
            {
                meshForDX9 = CreateDX9Mesh(mesh);
            }

            return meshForDX9;
        }

        [ContextMenu(nameof(RecreateMesh))]
        public virtual Mesh RecreateMesh()
        {
            if (meshForDX9 != null)
            {
                DestroyImmediate(meshForDX9);
                meshForDX9 = null;
            }

            if (mesh != null)
            {
                meshForDX9 = CreateDX9Mesh(mesh);
            }

            return meshForDX9;
        }

        protected virtual void OnValidate()
        {
            if (meshForDX9 != null)
            {
                RecreateMesh();
            }
        }
    }
}
