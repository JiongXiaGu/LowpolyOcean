using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [DisallowMultipleComponent]
    public class BuoyancyPropeller : MonoBehaviour
    {

        [SerializeField] private string componentName;
        public string ComponentName
        {
            get { return componentName; }
            set { componentName = value; }
        }

        [SerializeField] private float effectiveRadius = 1f;
        public float EffectiveRadius
        {
            get { return effectiveRadius; }
            set { effectiveRadius = value; }
        }

        [SerializeField] private float power = 1000f;
        public float Power
        {
            get { return power; }
            set { power = value; }
        }

        [SerializeField] private float reversePower = -300f;
        public float ReversePower
        {
            get { return reversePower; }
            set { reversePower = value; }
        }

        [SerializeField] private float leftTurn = -60;
        [SerializeField] private float rightTurn = 60;


        private float LerpAxis(float left, float right, float axis)
        {
            return Mathf.Lerp(0, left, -axis) * Mathf.Clamp01(-axis) + Mathf.Lerp(0, right, axis) * Mathf.Clamp01(axis);
        }

        public bool TryGetForce(IOceanData data, BuoyancyBoat sender, ControlInfo controlInfo, out PosAndForce value)
        {
            float radius = effectiveRadius * UnityHelper.GetRadiusScaleFactor(transform.lossyScale);
            var distanceToSurface = data.DistanceToSurface(transform.position);

            if (distanceToSurface > radius)
            {
                value = default;
                return false;
            }
            else
            {
                var angle = transform.localEulerAngles;
                angle.y = LerpAxis(leftTurn, rightTurn, controlInfo.TurnAxis);
                transform.localEulerAngles = angle;

                float scale = (radius - distanceToSurface) / (radius * 2);
                scale = Mathf.Clamp01(scale);
                value = new PosAndForce(transform.position, transform.forward * scale);
                value.Force *= LerpAxis(reversePower, power, controlInfo.ForwardAxis);
                return true;
            }
        }

        public void CustomDrawGizmos()
        {
            float effectiveRadius = this.effectiveRadius * UnityHelper.GetRadiusScaleFactor(transform.lossyScale);

            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, effectiveRadius);

                const float scale = 1.5f;
                var pos0 = transform.position + transform.rotation * Vector3.left * effectiveRadius;
                var pos1 = pos0 + transform.rotation * Vector3.back * effectiveRadius * scale;
                Gizmos.DrawLine(pos0, pos1);

                var pos2 = transform.position + transform.rotation * Vector3.up * effectiveRadius;
                var pos3 = pos2 + transform.rotation * Vector3.back * effectiveRadius * scale;
                Gizmos.DrawLine(pos2, pos3);

                Gizmos.DrawLine(pos1, pos3);

                var pos4 = transform.position + transform.rotation * Vector3.right * effectiveRadius;
                var pos5 = pos4 + transform.rotation * Vector3.back * effectiveRadius * scale;
                Gizmos.DrawLine(pos4, pos5);

                Gizmos.DrawLine(pos3, pos5);

                var pos6 = transform.position + transform.rotation * Vector3.down * effectiveRadius;
                var pos7 = pos6 + transform.rotation * Vector3.back * effectiveRadius * scale;
                Gizmos.DrawLine(pos6, pos7);

                Gizmos.DrawLine(pos5, pos7);
                Gizmos.DrawLine(pos7, pos1);
            }

            Gizmos.color = Color.green;
            var pos = transform.position + transform.rotation * Vector3.back * effectiveRadius * 1.7f;
            Gizmos.DrawLine(transform.position, pos);
        }

        private void OnDrawGizmosSelected()
        {
            CustomDrawGizmos();
        }
    }
}
