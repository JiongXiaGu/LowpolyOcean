using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{


    public class FoamAreaSystem
    {
        private static Lazy<FoamAreaSystem> CurrentLazy { get; } = new Lazy<FoamAreaSystem>();
        public static FoamAreaSystem Current => CurrentLazy.Value;

        private readonly List<IFoamArea> observers = new List<IFoamArea>(4);
        public IReadOnlyList<IFoamArea> Observers => observers;

        public IDisposable Subscribe(IFoamArea observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            observers.Add(observer);
            return new ListUnsubscriber<IFoamArea>(observers, observer);
        }

        public void ResetShader()
        {
            Shader.SetGlobalTexture(FoamOptions.AreaTexture0Name, null);
            Shader.SetGlobalVector(FoamOptions.AreaRect0Name, Vector4.zero);
            Shader.SetGlobalVector(FoamOptions.AreaPosition0Name, Vector4.zero);
        }

        public void SetValueToShader(IFoamArea area)
        {
            Shader.SetGlobalTexture(FoamOptions.AreaTexture0Name, area.Texture);
            Shader.SetGlobalVector(FoamOptions.AreaRect0Name, area.Rect);
            Shader.SetGlobalVector(FoamOptions.AreaPosition0Name, VectorHelper.Create(area.Position, area.Intensity));
        }

        public void SetValueToShader(OceanCameraTask oceanCamera, PreparedContent preparedContent)
        {
            if ((preparedContent & PreparedContent.FoamArea1) != 0)
            {
                if (observers.Count == 0)
                {
                    ResetShader();
                    return;
                }
                else if (observers.Count == 1)
                {
                    SetValueToShader(observers[0]);
                }
                else
                {
                    Vector3 cameraPos = oceanCamera.transform.position;

                    IFoamArea min = observers[0];
                    float minDis = Vector3.Distance(cameraPos, min.Position);

                    for (int i = 1; i < observers.Count; i++)
                    {
                        var current = observers[i];
                        var currentDis = Vector3.Distance(cameraPos, current.Position);

                        if (currentDis < minDis)
                        {
                            min = current;
                            minDis = currentDis;
                        }
                    }

                    SetValueToShader(min);
                }
            }
        }
    }
}
