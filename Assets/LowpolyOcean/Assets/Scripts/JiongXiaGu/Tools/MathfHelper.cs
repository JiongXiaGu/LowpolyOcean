using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{


    public static class MathfHelper
    {
        public const float AngleToRadina = (Mathf.PI / 180f);
        public const float RadinaToAngle = (180f / Mathf.PI);

        private static float IntersectDeterminant(float v1, float v2, float v3, float v4)
        {
            return (v1 * v3 - v2 * v4);
        }

        public static bool IsIntersect(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1)
        {
            float delta = IntersectDeterminant(a1.x - a0.x, b0.x - b1.x, a1.y - a0.y, b0.y - b1.y);
            if (Mathf.Approximately(delta, 0))
            {
                return false;
            }
            float namenda = IntersectDeterminant(b0.x - a0.x, b0.x - b1.x, b0.y - a0.y, b0.y - b1.y) / delta;
            if (namenda > 1 || namenda < 0)
            {
                return false;
            }
            float miu = IntersectDeterminant(a1.x - a0.x, b0.x - a0.x, a1.y - a0.y, b0.y - a0.y) / delta;
            if (miu > 1 || miu < 0)
            {
                return false;
            }
            return true;
        }

        public static float Cross(this Vector2 v0, Vector2 v1)
        {
            return v0.x * v1.y - v0.y * v1.x;
        }

        public static bool IsPointInTriangle(Vector2 t0, Vector2 t1, Vector2 t2, Vector2 point)
        {
            Vector2 p0 = t0 - point;
            Vector2 p1 = t1 - point;
            Vector2 p2 = t2 - point;
            float c0 = p0.Cross(p1);
            float c1 = p1.Cross(p2);
            float c2 = p2.Cross(p0);
            return c0* c1 >= 0 && c0* c2 >= 0;
        }
}
}
