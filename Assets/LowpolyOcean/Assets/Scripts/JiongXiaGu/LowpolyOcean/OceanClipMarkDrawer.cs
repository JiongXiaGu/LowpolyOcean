using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// subscribe ocean clip event, and mark clip range
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanClipMarkDrawer))]
    public sealed class OceanClipMarkDrawer : OceanMarkDrawer
    {
        private OceanClipMarkDrawer()
        {
        }

        private IDisposable unsubscriber;

        private void OnEnable()
        {
            unsubscriber = OceanCameraTask.SubscribeClipMarkDraw(this);
        }

        private void OnDisable()
        {
            unsubscriber.Dispose();
            unsubscriber = null;
        }
    }
}
