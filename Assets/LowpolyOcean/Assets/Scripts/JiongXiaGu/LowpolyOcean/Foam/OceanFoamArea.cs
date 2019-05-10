using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Show foam by texture
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanFoamArea))]
    public sealed class OceanFoamArea : MonoBehaviour, IFoamArea
    {
        private OceanFoamArea()
        {
        }

        [SerializeField] internal Texture texture;
        [SerializeField] [Range(0, 10)] internal float intensity = 1;
        private IDisposable unsubscriber;

        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        
        public float Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public Vector4 Rect => new Vector4(0.5f, 0.5f, transform.lossyScale.x, transform.lossyScale.z);
        public Vector3 Position => transform.position;

        public Vector4 GetScaledRect()
        {
            return new Vector4(0.5f, 0.5f, transform.lossyScale.x, transform.lossyScale.z);
        }

        private void OnEnable()
        {
            unsubscriber = FoamAreaSystem.Current.Subscribe(this);
        }

        private void OnDisable()
        {
            unsubscriber.Dispose();
            unsubscriber = null;
        }

        private void OnDrawGizmosSelected()
        {
            var rect = GetScaledRect();
            var position = transform.position;

            Vector3 worldPos0 = position;
            Vector3 worldPos1 = position;
            Vector3 worldPos2 = position;
            Vector3 worldPos3 = position;

            worldPos0.x = worldPos1.x = position.x - rect.z * rect.x;
            worldPos0.z = worldPos3.z = position.z - rect.w * rect.y;
            worldPos1.z = worldPos2.z = position.z + rect.w * (1 - rect.y);
            worldPos2.x = worldPos3.x = position.x + rect.z * (1 - rect.x);

            Gizmos.DrawLine(worldPos0, worldPos1);
            Gizmos.DrawLine(worldPos1, worldPos2);
            Gizmos.DrawLine(worldPos2, worldPos3);
            Gizmos.DrawLine(worldPos3, worldPos0);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Editor only;
        /// </summary>
        [SerializeField] public AreaGeneratorOptions areaGeneratorOptions;

        /// <summary>
        /// Editor only;
        /// </summary>
        [Serializable]
        public class AreaGeneratorOptions
        {
            public float seaLevel = 0f;
            public float cameraHeight = 100f;
            public LayerMask CullingMask = ~(1 << ProjectSettings.DefaultOceanLayer);
            public float farClipPlane = 200f;
            public float nearClipPlane = 0.1f;

            /// <summary>
            /// Texture size in pixels
            /// </summary>
            public Vector2Int textureSize = new Vector2Int(256, 256);

            /// <summary>
            /// Edge soften radius in pixels
            /// </summary>
            public float softenRadius = 5f;
        }
#endif

    }
}
