using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// filter show when rendering ripples, and to set the shader
    /// </summary>
    public class RippleSystem
    {
        public static RippleSystem Current { get; } = new RippleSystem();

        private List<IRippleData> observers;
        public IRippleData[] FinalRipples { get; private set; }
        public RippleOptions FinalRippleOptions { get; private set; }
        public int FinalRippleCount { get; private set; }
        public IRippleFilter RippleFilter { get; set; }

        public RippleSystem() : this(new RippleFilterByDistance())
        {
        }

        public RippleSystem(IRippleFilter rippleFilter)
        {
            observers = new List<IRippleData>();
            FinalRipples = new IRippleData[RippleOptions.RippleCount];
            FinalRippleOptions = new RippleOptions();
            RippleFilter = rippleFilter;
        }

        public IDisposable Subscribe(IRippleData observer)
        {
            observers.Add(observer);
            return new ListUnsubscriber<IRippleData>(observers, observer);
        }

        private void SetValue(IReadOnlyList<IRippleData> source, RippleOptions dest)
        {
            if (source[0] == null)
            {
                dest.Clear();
                FinalRippleCount = 0;
                return;
            }

            FinalRippleCount = 0;
            for (int i = 0; i < source.Count; i++)
            {
                IRippleData rippleInfo = source[i];
                if (rippleInfo == null)
                {
                    dest.Clear(i);
                }
                else
                {
                    FinalRippleCount++;
                    dest.SetTexture(rippleInfo.Texture, i);
                    dest.Rect[i] = rippleInfo.Rect;
                    dest.Position[i] = new Vector4(rippleInfo.Position.x, rippleInfo.Position.y, rippleInfo.Position.z, rippleInfo.Radian);
                    dest.HeightScale[i] = rippleInfo.HeightScale;
                }
            }
        }

        private void ClearFinalRipples()
        {
            for(int i = 0; i < FinalRipples.Length; i++)
            {
                FinalRipples[i] = null;
            }
        }

        public void SetValueToShader(OceanCameraTask oceanCamera, CameraTaskRippleData rippleData)
        {
            ClearFinalRipples();
            RippleFilter.Filter(oceanCamera, rippleData, observers, FinalRipples);
            SetValue(FinalRipples, FinalRippleOptions);
            RippleOptions.Accessor.SetGlobalValues(FinalRippleOptions);
        }
    }
}
