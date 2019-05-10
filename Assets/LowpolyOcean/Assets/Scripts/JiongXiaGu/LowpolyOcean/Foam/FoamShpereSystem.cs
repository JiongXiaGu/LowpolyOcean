using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public class FoamShpereSystem
    {
        public const int DefaultSphereMaxCount = 100;
        public const int DefaultMaxFoamCount = 16;
        public const int DefaultMaxFoamCountQuarter = DefaultMaxFoamCount / 4;

        private static Lazy<FoamShpereSystem> CurrentLazy { get; } = new Lazy<FoamShpereSystem>();
        public static FoamShpereSystem Current => CurrentLazy.Value;

        private readonly List<IFoamShpere> foamObservers = new List<IFoamShpere>(DefaultSphereMaxCount);
        public IReadOnlyList<IFoamShpere> FoamObservers => foamObservers;

        public BoundingSphere[] BoundingSpheres { get; } = new BoundingSphere[DefaultSphereMaxCount];
        private readonly IFoamShpere[] effectiveShperes = new IFoamShpere[DefaultSphereMaxCount];
        private readonly int[] cullingGroupResult = new int[DefaultSphereMaxCount];

        private readonly SortedArray<ComparableItem> sortedArray = new SortedArray<ComparableItem>(DefaultMaxFoamCount);

        private readonly Vector4[] data0 = new Vector4[DefaultMaxFoamCount];
        private readonly Vector4[] data1 = new Vector4[DefaultMaxFoamCountQuarter];

        public IDisposable Subscribe(IFoamShpere observer)
        {
            if (foamObservers.Count >= DefaultSphereMaxCount)
                throw new InvalidOperationException(string.Format("Unable to subscribe because it exceeds the defined size ({0})", DefaultSphereMaxCount));

            foamObservers.Add(observer);
            return new ListUnsubscriber<IFoamShpere>(foamObservers, observer);
        }

        public void OnPreOceanCull(FoamCameraData data)
        {
            data.CullingGroup.enabled = ProjectSettings.Current.Foam.EnabledFoamShpereCulling;

            int boundingSphereCount = 0;

            for (int index = 0; index < foamObservers.Count; index++)
            {
                var observer = foamObservers[index];
                if (Physics.CheckSphere(observer.Position, observer.Radius, ProjectSettings.Current.OceanCullingMask, QueryTriggerInteraction.UseGlobal))
                {
                    BoundingSpheres[boundingSphereCount] = new BoundingSphere(observer.Position, observer.Radius);
                    effectiveShperes[boundingSphereCount] = observer;
                    boundingSphereCount++;
                }
            }

            data.CullingGroup.SetBoundingSphereCount(boundingSphereCount);
        }

        private void ClearArray<T>(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default(T);
            }
        }

        public void SetValueToShader(OceanCameraTask oceanCamera, FoamCameraData data, PreparedContent preparedContent)
        {
            if (ProjectSettings.Current.Foam.EnabledFoamShpereCulling)
            {
                if ((preparedContent & PreparedContent.Foam16) != 0)
                {
                   SetValueToShader(oceanCamera, data, 16, 4);
                }
                else if ((preparedContent & PreparedContent.Foam8) != 0)
                {
                    SetValueToShader(oceanCamera, data, 8, 2);
                }
            }
        }

        public void SetValueToShader(OceanCameraTask oceanCamera, FoamCameraData data, int count, int countQuarter)
        {
            int visibleCount = data.CullingGroup.QueryIndices(true, cullingGroupResult, 0);

            if (visibleCount == 0)
            {
                ClearArray(data0);
                ClearArray(data1);
            }
            else if (visibleCount <= count)
            {
                int index;
                for (index = 0; index < visibleCount; index++)
                {
                    int observerIndex = cullingGroupResult[index];
                    IFoamShpere observer = effectiveShperes[observerIndex];
                    data0[index] = VectorHelper.Create(observer.Position, observer.Radius);
                }
                for (; index < count; index++)
                {
                    data0[index] = default(Vector4);
                }

                for (index = 0; index < countQuarter; index++)
                {
                    Vector4 value = Vector4.zero;
                    for (int j = 0, newIndex = index * 4; newIndex < visibleCount && j < 4; j++, newIndex++)
                    {
                        int observerIndex = cullingGroupResult[newIndex];
                        IFoamShpere observer = effectiveShperes[observerIndex];
                        value[j] = observer.Intensity;
                    }
                    data1[index] = value;
                }
            }
            else
            {
                int index;
                for (index = 0; index < visibleCount; index++)
                {
                    int observerIndex = cullingGroupResult[index];
                    IFoamShpere observer = effectiveShperes[observerIndex];
                    sortedArray.Add(new ComparableItem(observer, oceanCamera.transform.position));
                }

                for (index = 0; index < Mathf.Min(visibleCount, count); index++)
                {
                    IFoamShpere observer = sortedArray[index].Foam;
                    data0[index] = VectorHelper.Create(observer.Position, observer.Radius);
                }
                for (; index < count; index++)
                {
                    data0[index] = default(Vector4);
                }

                for (index = 0; index < countQuarter; index++)
                {
                    Vector4 value = Vector4.zero;
                    for (int j = 0, newIndex = index * 4; newIndex < visibleCount && j < 4; j++, newIndex++)
                    {
                        IFoamShpere observer = sortedArray[newIndex].Foam;
                        value[j] = observer.Intensity;
                    }
                    data1[index] = value;
                }

                sortedArray.Clear();
            }

            Shader.SetGlobalVectorArray(FoamOptions.ShpereData0Name, data0);
            Shader.SetGlobalVectorArray(FoamOptions.ShpereData1Name, data1);
        }

        public struct ComparableItem : IComparable<ComparableItem>
        {
            public IFoamShpere Foam { get; set; }
            public float Distance { get; set; }

            public ComparableItem(IFoamShpere foam, Vector3 pos)
            {
                Foam = foam;
                Distance = Vector3.Distance(foam.Position, pos);
            }

            public int CompareTo(ComparableItem other)
            {
                if (Foam == null)
                {
                    return 1;
                }
                else if (other.Foam == null)
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
                return Foam == null ? "Null" : Distance.ToString();
            }
        }
    }
}
