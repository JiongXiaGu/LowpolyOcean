using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [Serializable]
    public struct PosAndForce : IEquatable<PosAndForce>
    {
        [SerializeField] private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        [SerializeField] private Vector3 force;
        public Vector3 Force
        {
            get { return force; }
            set { force = value; }
        }

        public PosAndForce(Vector3 position, Vector3 force)
        {
            this.position = position;
            this.force = force;
        }

        public override string ToString()
        {
            return "Position : " + position + ", Force : " + force;
        }

        public override bool Equals(object obj)
        {
            return obj is PosAndForce && Equals((PosAndForce)obj);
        }

        public bool Equals(PosAndForce other)
        {
            return position.Equals(other.position) &&
                   force.Equals(other.force);
        }

        public override int GetHashCode()
        {
            var hashCode = -1046980066;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(force);
            return hashCode;
        }

        public static bool operator ==(PosAndForce force1, PosAndForce force2)
        {
            return force1.Equals(force2);
        }

        public static bool operator !=(PosAndForce force1, PosAndForce force2)
        {
            return !(force1 == force2);
        }
    }
}
