using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(UnderOceanData))]
    public class UnderOceanData : UnderOceanDataBase
    {
        [SerializeField] private UnderOceanMaterialData materialData;
        public UnderOceanMaterialData MaterialData
        {
            get { return materialData; }
            set { materialData = value; }
        }

        public override PreparedContent OnPreOceanRender(OceanCameraTask oceanCamera, OceanVolume ocean)
        {
            Matrix4x4 worldToLightMatrix;
            if (materialData.ModeObject.Mode.Cookie > 0 && oceanCamera.Data.SunLight != null)
            {
                var sunLightTransform = oceanCamera.Data.SunLight.transform;
                worldToLightMatrix = Matrix4x4.TRS(sunLightTransform.position, sunLightTransform.rotation, materialData.Data.Cookie.Scale).inverse;
            }
            else
            {
                worldToLightMatrix = Matrix4x4.identity;
            }
            Shader.SetGlobalMatrix(UnderCookieOptions.WorldToCookieMatrixShaderID, worldToLightMatrix);


            UnderOceanModeOptions.UpdateKeywords(materialData.ModeObject.Mode);
            if (!UnderMaterialOptions.UpdateShaderFields(materialData.Data))
            {
                if (materialData.Data.Cookie.Texture != null)
                {
                    Shader.SetGlobalTexture(UnderCookieOptions.TextureShaderFieldID, materialData.Data.Cookie.Texture.GetCurrentTexture());
                }
            }

            if (ProjectSettings.Current.RenderQueue == OceanRenderQueue.Transparent)
            {
                UnderOceanModeOptions.DisableUnderOceanFogEffect();
            }

            Shader.SetGlobalVector(UnderMaterialOptions.UnderOceanPositionShaderID, new Vector4(0, ocean.Height, 0, 0));

            return PreparedContent.UnderOceanMarkTexture;
        }

        public override void OnPostOceanRender(OceanCameraTask oceanCamera)
        {
            return;
        }

        private void OnValidate()
        {
            materialData.Data.Dirty();
            materialData.ModeObject?.Mode.Dirty();
        }
    }
}
