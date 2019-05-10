using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public interface IShpere
    {
        Vector3 Position { get; }
        float Radius { get; }
    }

    public class ShpereFilter<T>
        where T : IShpere
    {
        public int ShpereCount { get; }
        public int ResultCount { get; }
        public BoundingSphere[] BoundingSpheres;
        private readonly int[] cullingGroupResult;
        private readonly T[] effectiveShperes;
        private readonly SortedArray<ComparableItem> sortedArray;
        private readonly T[] result;

        public ShpereFilter(int shpereCount, int resultCount)
        {
            ShpereCount = shpereCount;
            ResultCount = resultCount;
            BoundingSpheres = new BoundingSphere[shpereCount];
            cullingGroupResult = new int[shpereCount];
            effectiveShperes = new T[shpereCount];
            sortedArray = new SortedArray<ComparableItem>(resultCount);
            result = new T[resultCount];
        }

        public void OnPreCullAndCheck(CullingGroup cullingGroup, IList<T> shperes, int cullingMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            int boundingSphereCount = 0;

            for (int index = 0; index < shperes.Count; index++)
            {
                var shpere = shperes[index];
                if (Physics.CheckSphere(shpere.Position, shpere.Radius, cullingMask, queryTriggerInteraction))
                {
                    BoundingSpheres[boundingSphereCount] = new BoundingSphere(shpere.Position, shpere.Radius);
                    effectiveShperes[boundingSphereCount] = shpere;
                    boundingSphereCount++;
                }
            }

            cullingGroup.SetBoundingSphereCount(boundingSphereCount);
        }

        public int Query(Vector3 target, CullingGroup cullingGroup, int resultCount, out IReadOnlyList<T> result)
        {
            result = this.result;
            int visibleCount = cullingGroup.QueryIndices(true, cullingGroupResult, 0);

            if (visibleCount == 0)
            {
                return 0;
            }
            else if (visibleCount <= resultCount)
            {
                int index;
                for (index = 0; index < visibleCount; index++)
                {
                    int shpereIndex = cullingGroupResult[index];
                    T shpere = effectiveShperes[shpereIndex];
                    this.result[index] = shpere;
                }
                for (; index < resultCount; index++)
                {
                    this.result[index] = default(T);
                }

                return visibleCount;
            }
            else
            {
                int index;
                for (index = 0; index < visibleCount; index++)
                {
                    int shpereIndex = cullingGroupResult[index];
                    T shpere = effectiveShperes[shpereIndex];
                    sortedArray.Add(new ComparableItem(shpere, target));
                }

                for (index = 0; index < resultCount; index++)
                {
                    T shpere = sortedArray[index].Value;
                    this.result[index] = shpere;
                }
                for (; index < resultCount; index++)
                {
                    this.result[index] = default(T);
                }

                return resultCount;
            }
        }

        public struct ComparableItem : IComparable<ComparableItem>
        {
            public T Value { get; set; }
            public float Distance { get; set; }

            public ComparableItem(T item, Vector3 pos)
            {
                Value = item;
                Distance = Vector3.Distance(item.Position, pos);
            }

            public int CompareTo(ComparableItem other)
            {
                if (Value == null)
                {
                    return 1;
                }
                else if (other.Value == null)
                {
                    return -1;
                }
                else
                {
                    return Math.Sign(Distance - other.Distance);
                }
            }

            public override string ToString()
            {
                return Value == null ? "Null" : Distance.ToString();
            }
        }
    }
}
