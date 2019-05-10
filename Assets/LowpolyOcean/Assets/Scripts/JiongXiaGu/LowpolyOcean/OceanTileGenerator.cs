using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace JiongXiaGu.LowpolyOcean
{

#if UNITY_EDITOR

    /// <summary>
    /// Help tile oceans
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanTileGenerator))]
    public sealed class OceanTileGenerator : MonoBehaviour
    {
        private OceanTileGenerator()
        {
        }

        public int ring = 3;
        public MeshFilter tileObject;

        private static Transform GetOrCreateChild(Transform parent, string name)
        {
            var child = parent.Find(name);
            if (child == null)
            {
                var ins = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(ins, "Create Empty");
                child = ins.transform;
                child.SetParent(parent, false);
            }
            return child;
        }

        private static Transform CreateTile(Transform parent, GameObject tileObject, Vector3 localPos, string name)
        {
            var child = parent.Find(name);
            if (child != null)
            {
                Undo.DestroyObjectImmediate(child);
            }

            var ins = Instantiate(tileObject);
            Undo.RegisterCreatedObjectUndo(ins, "Create Tile");
            child = ins.transform;
            child.SetParent(parent, false);
            child.localPosition = localPos;
            child.name = name;
            return child;
        }

        private static readonly Path[] paths = new Path[4]
        {
            new Path(new Vector3(-1, 0, 1), new Vector3(1, 0, 0)),
            new Path(new Vector3(1, 0, 1), new Vector3(0, 0, -1)),
            new Path(new Vector3(1, 0, -1), new Vector3(-1, 0, 0)),
            new Path(new Vector3(-1, 0, -1), new Vector3(0, 0, 1)),
        };

        private static void GenerateRingAt(Transform parent, GameObject tileObject, Vector3 size, int ring)
        {
            int ring2 = ring + ring - 1;
            int id = 0;
            foreach (var path in paths)
            {
                Vector3 start = path.Start.Multiply(size) * ring;
                CreateTile(parent, tileObject, start, id.ToString());
                id++;

                Vector3 direction = path.Direction.Multiply(size);
                for (int i = 0; i < ring2; i++)
                {
                    start += direction;
                    CreateTile(parent, tileObject, start, id.ToString());
                    id++;
                }
            }
        }

        private static void Generate(Transform parent, GameObject tileObject, Vector3 size, int ring)
        {
            int undoGroup = Undo.GetCurrentGroup();

            CreateTile(parent, tileObject, Vector3.zero, "0");

            for (int i = 1; i <= ring; i++)
            {
                var ringParent = GetOrCreateChild(parent, i.ToString());
                GenerateRingAt(ringParent, tileObject, size, i);
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        [ContextMenu(nameof(Generate))]
        public void Generate()
        {
            if (tileObject == null)
                throw new ArgumentNullException(nameof(tileObject));
            if(tileObject.sharedMesh == null)
                throw new ArgumentNullException(nameof(tileObject.sharedMesh));

            ring = Mathf.Abs(ring);
            Transform parent = GetOrCreateChild(transform, "Meshs");
            Vector3 meshSize = tileObject.sharedMesh.bounds.size;
            Generate(parent, tileObject.gameObject, meshSize, ring);
        }

        private struct Path
        {
            public Vector3 Start;
            public Vector3 Direction;

            public Path(Vector3 start, Vector3 direction)
            {
                Start = start;
                Direction = direction;
            }
        }
    }

#endif
}
