using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{


    public class BuoyancyBoat : BuoyancyObject
    {
        protected BuoyancyBoat()
        {
        }

        [SerializeField] private float lateralDrag = 1f;
        [SerializeField] private PropellerList propellers = null;
        public List<BuoyancyPropeller> Propellers => propellers;
        [SerializeField] private Rudder rudder = new Rudder();

        protected override void UpdateRigidbodyState(int influenceDragCount, int activatedDrag)
        {
            base.UpdateRigidbodyState(influenceDragCount, activatedDrag);

            _rigidbody.drag += (1 - Vector3.Dot(_rigidbody.velocity.normalized, transform.forward)) * lateralDrag;
        }

        public override void ManualUpdate(IOceanData data)
        {
            base.ManualUpdate(data);

            ControlInfo controlInfo = new ControlInfo();
            controlInfo.ForwardAxis = Input.GetAxis("Vertical");
            controlInfo.TurnAxis = Input.GetAxis("Horizontal");

            foreach (var propeller in propellers)
            {
                PosAndForce force;
                if (propeller.TryGetForce(data, this, controlInfo, out force))
                {
                    AddForce(force);
                }
            }

            var horizontalForce = transform.right * rudder.horizontalPower * -controlInfo.TurnAxis;
            _rigidbody.AddForceAtPosition(horizontalForce, transform.TransformPoint(rudder.horizontalOffset));

            var verticalForce = transform.up * rudder.verticalPower * -controlInfo.ForwardAxis;
            _rigidbody.AddForceAtPosition(verticalForce, transform.TransformPoint(rudder.verticalOffset));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(rudder.horizontalOffset), 1f);
        }

        [Serializable]
        private class PropellerList : CustomReorderableList<BuoyancyPropeller>
        {
        }

        [Serializable]
        public struct Rudder
        {
            [SerializeField] public Vector3 horizontalOffset;
            [SerializeField] public float horizontalPower;

            [SerializeField] public Vector3 verticalOffset;
            [SerializeField] public float verticalPower;

        }
    }
}
