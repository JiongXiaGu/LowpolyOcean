using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanSimpleRenderer))]
    public class OceanSimpleRenderer : OceanRendererBase
    {
        [EnumMask] [SerializeField] private PreparedContent preparedContent = PreparedContent.RefractionTexture;
        public PreparedContent PreparedContent
        {
            get { return preparedContent; }
            set { preparedContent = value; }
        }

        public override PreparedContent GetRenderContents(OceanCameraTask oceanCamera)
        {
            return preparedContent;
        }

        private bool isRending;
        private Camera requestCamera;
        private OceanCameraTask requestOceanCamera;

        private void OnWillRenderObject()
        {
            if (isRending)
                return;

            var camera = Camera.current;
            if (camera == null)
                return;

            OceanCameraTask oceanCamera;
            if (RenderHelper.TryGetOceanCamera(camera, out oceanCamera))
            {
                isRending = true;
                requestCamera = camera;
                requestOceanCamera = oceanCamera;
                oceanCamera.AddWillRenderOcean(this);
            }

        }

        private void OnRenderObject()
        {
            if (Camera.current == requestCamera)
            {
                isRending = false;
                requestCamera = null;
                requestOceanCamera = null;
            }
        }
    }
}
