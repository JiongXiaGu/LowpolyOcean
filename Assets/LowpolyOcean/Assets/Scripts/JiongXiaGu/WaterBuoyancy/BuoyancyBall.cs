using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    public sealed class BuoyancyBall : MonoBehaviour, IBuoyancyHandle
    {
        private BuoyancyBall()
        {
        }

        [SerializeField] private string componentName = null;
        public string ComponentName
        {
            get { return componentName; }
            set { componentName = value; }
        }

        [SerializeField] private Vector3 offset = Vector3.zero;
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        [SerializeField] private float radius = 1f;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float ActualRadius => radius * UnityHelper.GetRadiusScaleFactor(transform.lossyScale);

        [SerializeField] private float powerScale = 1f;
        public float PowerScale
        {
            get { return powerScale; }
            set { powerScale = value; }
        }

        [SerializeField] private bool isInfluenceDrag = true;
        public bool IsInfluenceDrag
        {
            get { return isInfluenceDrag; }
            set { isInfluenceDrag = value; }
        }

        public bool TryGetForce(IOceanData data, BuoyancyObject sender, out PosAndForce value)
        {
            value = new PosAndForce();
            var pos = value.Position = transform.TransformPoint(offset);
            var distanceToSurface = data.DistanceToSurface(pos);
            float radius = ActualRadius;
            if (distanceToSurface > radius)
            {
                return false;
            }
            else
            {
                float scale = (radius - distanceToSurface) / (radius * 2);
                scale = Mathf.Clamp01(scale);
                value.Force = data.Density * -Physics.gravity * scale * powerScale * sender.Power;
                return true;
            }
        }

        public void CustomDrawGizmos()
        {
            float radius = ActualRadius;

            Gizmos.color = Color.blue;
            var pos = transform.TransformPoint(offset);
            Gizmos.DrawWireSphere(pos, radius);

            Gizmos.color = Color.green;
            var pos2 = pos + -Physics.gravity.normalized * radius + -Physics.gravity.normalized * radius / 3;
            Gizmos.DrawLine(pos, pos2);
        }

        public void OnDrawGizmosSelected()
        {
            CustomDrawGizmos();
        }
    }
}
