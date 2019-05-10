using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    public interface IStorageInfoDrawer
    {
        void CustomDrawGizmos();
    }

    public class PrefabStorageInfo : MonoBehaviour
    {

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
