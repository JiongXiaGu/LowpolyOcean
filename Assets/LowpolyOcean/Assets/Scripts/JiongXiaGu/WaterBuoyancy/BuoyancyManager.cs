using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [DisallowMultipleComponent]
    public sealed class BuoyancyManager : UnitySingleton<BuoyancyManager>
    {

        [SerializeField] private bool isAutoUpdate = true;
        [SerializeField] private OceanData buoyancyData = null;
        private static readonly HashSet<BuoyancyObject> objects = new HashSet<BuoyancyObject>();

        public static bool Subscribe(BuoyancyObject sender)
        {
            Initialize();
            return objects.Add(sender);
        }

        public static bool Unsubscribe(BuoyancyObject sender)
        {
            return objects.Remove(sender);
        }

        private bool destroyDontClear = false;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                destroyDontClear = true;
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (!destroyDontClear)
            {
                RemoveInstance(this);
            }
        }

        public void ManualUpdate()
        {
            if (buoyancyData != null)
            {
                foreach (var obj in objects)
                {
                    if (obj.IsRequestUpdate)
                    {
                        obj.ManualUpdate(buoyancyData);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (isAutoUpdate)
            {
                ManualUpdate();
            }
        }
    }
}
