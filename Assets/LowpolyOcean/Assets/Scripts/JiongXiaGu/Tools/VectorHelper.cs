using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{

    /// <summary>
    /// Provide vector operations, just like HLSL;
    /// </summary>
    public static class VectorHelper
    {

        public static Vector2 Abs(this Vector2 v0)
        {
            return new Vector2(Mathf.Abs(v0.x), Mathf.Abs(v0.y));
        }

        public static Vector2 Max(this Vector2 v0, Vector2 v1)
        {
            return new Vector2(Mathf.Max(v0.x, v1.x), Mathf.Max(v0.y, v1.y));
        }

        public static Vector2 Max(this Vector2 v0, float v1)
        {
            return new Vector2(Mathf.Max(v0.x, v1), Mathf.Max(v0.y, v1));
        }

        public static Vector2 Sum(this Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x + v1.x, v0.y + v1.y);
        }

        public static Vector2 Subtract(this Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x - v1.x, v0.y - v1.y);
        }

        public static Vector2 Multiply(this Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x * v1.x, v0.y * v1.y);
        }

        public static Vector2 Divide(this Vector2 v0, Vector2 v1)
        {
            return new Vector2(v0.x / v1.x, v0.y / v1.y);
        }


        public static Vector3 Sum(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
        }

        public static Vector3 Subtract(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
        }

        public static Vector3 Multiply(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z);
        }

        public static Vector3 Divide(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(v0.x / v1.x, v0.y / v1.y, v0.z / v1.z);
        }


        public static Vector4 Sum(this Vector4 v0, Vector4 v1)
        {
            return new Vector4(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z, v0.w + v1.w);
        }

        public static Vector4 Subtract(this Vector4 v0, Vector4 v1)
        {
            return new Vector4(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z, v0.w - v1.w);
        }

        public static Vector4 Multiply(this Vector4 v0, Vector4 v1)
        {
            return new Vector4(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z, v0.w * v1.w);
        }

        public static Vector4 Divide(this Vector4 v0, Vector4 v1)
        {
            return new Vector4(v0.x / v1.x, v0.y / v1.y, v0.z / v1.z, v0.w / v1.w);
        }


        public static Vector4 Create(Vector3 xyz, float w)
        {
            return new Vector4(xyz.x, xyz.y, xyz.z, w);
        }

        public static Vector4 Create(Vector2 xy, float z, float w)
        {
            return new Vector4(xy.x, xy.y, z, w);
        }

        public static Vector4 Create(Vector2 xy, Vector2 zw)
        {
            return new Vector4(xy.x, xy.y, zw.x, zw.y);
        }

        public static Vector4 Create(Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }
    }
}
