using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public class RippleFilterByDistance : IRippleFilter
    {
        private const int FinalRippleCount = RippleOptions.RippleCount;
        private readonly ContrastInfo[] temp;

        public RippleFilterByDistance()
        {
            temp = new ContrastInfo[FinalRippleCount];
        }

        private void ResetTempArray()
        {
            for (int i = 0; i < temp.Length; i++)
            {
                var value = temp[i];
                value.Info = null;
                temp[i] = value;
            }
        }

        private Vector2[] rippleWorldPos = new Vector2[4];

        private bool TryGetPriority(Camera camera, CameraTaskRippleData data, IRippleData ripple, out float priority)
        {
            Vector3 cameraPosition = camera.transform.position;
            priority = 0;

            VertexHelper.GetRippleWorldPos(new Vector2(ripple.Position.x, ripple.Position.z), ripple.Rect, ripple.Radian, ref rippleWorldPos);

            bool isRange = false;
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = rippleWorldPos[i];
                Vector3 worldPos = new Vector3(pos.x, data.OceanHeight, pos.y);
                Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
                if (camera.pixelRect.Contains(screenPos))
                {
                    if (screenPos.z > 0)
                    {
                        isRange |= true;
                        priority += screenPos.z;
                        continue;
                    }
                    else
                    {
                        priority -= screenPos.z * 3;
                    }
                }
                priority += screenPos.z * 3;
            }

            return isRange;
        }

        public void Filter(OceanCameraTask oceanCamera, CameraTaskRippleData rippleData, IReadOnlyList<IRippleData> source, IRippleData[] result)
        {
            if (source.Count <= FinalRippleCount)
            {
                int i = 0;
                foreach (var ripple in source)
                {
                    if (ripple.OnEnter(oceanCamera))
                    {
                        ripple.OnSelecte(oceanCamera);
                        result[i] = ripple;
                        i++;
                    }
                }
            }
            else
            {
                Vector3 cameraPosition = oceanCamera.transform.position;
                foreach (var ripple in source)
                {
                    if (ripple.OnEnter(oceanCamera))
                    {
                        float priority;
                        if (TryGetPriority(oceanCamera.ThisCamera, rippleData, ripple, out priority))
                        {
                            int i;
                            for (i = 0; i < temp.Length; i++)
                            {
                                var current = temp[i];
                                if (current.Info == null)
                                {
                                    current.Priority = priority;
                                    current.Info = ripple;
                                    temp[i] = current;
                                    break;
                                }
                                else if (current.Priority > priority)
                                {
                                    for (int j = temp.Length - 1; j > i; j--)
                                    {
                                        temp[j] = temp[j - 1];
                                    }

                                    current.Priority = priority;
                                    current.Info = ripple;
                                    temp[i] = current;
                                    break;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < result.Length; i++)
                {
                    var rippleInfo = temp[i].Info;
                    rippleInfo?.OnSelecte(oceanCamera);
                    result[i] = rippleInfo;
                }
            }
            ResetTempArray();
        }

        private struct ContrastInfo
        {
            public float Priority;
            public IRippleData Info;
        }
    }

}
