using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [DisallowMultipleComponent]
    public class BuoyancyData : MonoBehaviour
    {
        protected BuoyancyData()
        {
        }

        private bool IsInitialize = false;
        public List<IBuoyancyHandle> Handles { get; private set; }

        public virtual void Collect()
        {
            if (IsInitialize == false)
            {
                Handles = new List<IBuoyancyHandle>();
            }

            Handles.Clear();
            Handles.AddRange(GetComponentsInChildren<IBuoyancyHandle>());
        }

#if UNITY_EDITOR

        public static bool editorOnlyIsDisplayHandles { get; set; } = false;

        private void OnDrawGizmos()
        {
            if (!editorOnlyIsDisplayHandles)
                return;

            foreach (var item in GetComponentsInChildren<IStorageInfoDrawer>())
            {
                item.CustomDrawGizmos();
            }
        }
#endif

    }
}
