using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [ExecuteInEditMode]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanObstructMarkDrawer))]
    public class OceanObstructMarkDrawer : OceanMarkDrawer
    {
        private OceanObstructMarkDrawer()
        {
        }

        private IDisposable unsubscriber;

        private void OnEnable()
        {
            unsubscriber = OceanCameraTask.SubscribeUnderOceanMarkDraw(this);
        }

        private void OnDisable()
        {
            unsubscriber.Dispose();
            unsubscriber = null;
        }
    }
}
