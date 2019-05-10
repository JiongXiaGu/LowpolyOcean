using UnityEngine;
using System;
using System.Collections.Generic;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    public struct CameraTaskReflectionData : IEquatable<CameraTaskReflectionData>
    {
        [SerializeField] private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        [SerializeField] private Vector3 normal;
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public static CameraTaskReflectionData Default => new CameraTaskReflectionData()
        {
            position = Vector3.zero,
            normal = Vector3.up,
        };

        public CameraTaskReflectionData(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }

        public override string ToString()
        {
            return string.Format("Postion:{0}, Normal:{1}", Position, Normal);
        }

        public override bool Equals(object obj)
        {
            return obj is CameraTaskReflectionData && Equals((CameraTaskReflectionData)obj);
        }

        public bool Equals(CameraTaskReflectionData other)
        {
            return position.Equals(other.position) &&
                   normal.Equals(other.normal);
        }

        public override int GetHashCode()
        {
            var hashCode = 779775468;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(normal);
            return hashCode;
        }

        public static bool operator ==(CameraTaskReflectionData data1, CameraTaskReflectionData data2)
        {
            return data1.Equals(data2);
        }

        public static bool operator !=(CameraTaskReflectionData data1, CameraTaskReflectionData data2)
        {
            return !(data1 == data2);
        }
    }
}
