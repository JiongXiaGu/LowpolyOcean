using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [Serializable]
    public struct ControlInfo
    {
        [SerializeField] private float forwardAxis;
        public float ForwardAxis
        {
            get { return forwardAxis; }
            set { forwardAxis = value; }
        }

        public float Forward => (forwardAxis + 1) / 2;

        [SerializeField] private float turnAxis;
        public float TurnAxis
        {
            get { return turnAxis; }
            set { turnAxis = value; }
        }

        public float Turn => (turnAxis + 1) / 2;

        public ControlInfo(float forwardAxis, float turnAxis)
        {
            this.forwardAxis = forwardAxis;
            this.turnAxis = turnAxis;
        }
    }
}
