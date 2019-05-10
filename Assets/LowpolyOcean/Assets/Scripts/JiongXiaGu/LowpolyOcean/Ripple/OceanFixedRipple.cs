using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// fixed ripple animation, provided to a buoy or similar object
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class OceanFixedRipple : MonoBehaviour, IRippleData
    {
        private OceanFixedRipple()
        {
        }

        [SerializeField] private ScriptableFrameAnimation frameAnimation = null;
        [SerializeField] private Vector4 rippleRect = new Vector4(0.5f, 0.5f, 5f, 5f);
        [SerializeField] [Range(0, 360)] private float angle;
        [SerializeField] private Vector4 heightScale = new Vector4(1, 1, 1, 1);
        private IDisposable unsubscriber;

        public Vector3 Position => transform.position;
        public Texture Texture { get; private set; }

        public Vector4 Rect
        {
            get { return rippleRect; }
            set { rippleRect = value; }
        }

        public float Radian
        {
            get { return angle * MathfHelper.AngleToRadina; }
            set { angle = value * MathfHelper.RadinaToAngle; }
        }

        public Vector4 HeightScale
        {
            get { return heightScale; }
            set { heightScale = value; }
        }

        private void OnEnable()
        {
            unsubscriber = RippleSystem.Current.Subscribe(this);
        }

        private void OnDisable()
        {
            if (unsubscriber != null)
            {
                unsubscriber.Dispose();
                unsubscriber = null;
            }
        }

        private static Vector2[] worldPos = new Vector2[4];

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            const float height = 3f;
            VertexHelper.GetRippleWorldPos(new Vector2(Position.x, Position.z), Rect, Radian, ref worldPos);

            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(new Vector3(worldPos[i].x, height, worldPos[i].y), new Vector3(worldPos[i].x, -height, worldPos[i].y));

                int next = i + 1;
                if (next == 4)
                    next = 0;

                Gizmos.DrawLine(new Vector3(worldPos[i].x, height, worldPos[i].y), new Vector3(worldPos[next].x, height, worldPos[next].y));
                Gizmos.DrawLine(new Vector3(worldPos[i].x, -height, worldPos[i].y), new Vector3(worldPos[next].x, -height, worldPos[next].y));
            }
        }

        bool IRippleData.OnEnter(OceanCameraTask camera)
        {
            return frameAnimation != null;
        }

        void IRippleData.OnSelecte(OceanCameraTask camera)
        {
            Texture = frameAnimation.GetCurrentTexture();
        }
    }
}
