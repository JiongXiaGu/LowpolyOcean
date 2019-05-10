using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(OceanData))]
    public class OceanData : OceanDataBase
    {
        [SerializeField] [Range(0, 1)] private float level = 0f;
        public float Level => level;

        [SerializeField] private OceanLevelDataList levelDefinition = null;
        public List<OceanLevelData> LevelDefinition => levelDefinition;

        [SerializeField] private LODQualityList lodQualitys = null;
        public List<LODQuality> LODQualitys => lodQualitys;

        public MaterialOptions Result { get; } = new MaterialOptions();

        public LODQuality GetActualLOD(int lodLevel)
        {
            lodLevel = Mathf.Clamp(lodLevel, 0, lodQualitys.Count);
            return lodQualitys[lodLevel];
        }

        public override Material GetMaterial(int lodLevel)
        {
            return GetActualLOD(lodLevel).Material;
        }

        public override Material GetMarkMaterial(int lodLevel)
        {
            return GetActualLOD(lodLevel).MarkMaterial;
        }

        public override PreparedContent GetRenderContents(int lodLevel)
        {
#if UNITY_EDITOR
            EditorUpdateMaterial();
#endif
            return GetActualLOD(lodLevel).ModeObject.Mode.GetRenderContents();
        }

        public void SetLevel(float level)
        {
            this.level = level;
            if (levelDefinition.Count == 0)
            {
                throw new ArgumentException("Undefined level info");
            }
            else if (levelDefinition.Count == 1)
            {
                var source = levelDefinition[0].DataObject.Data;
                MaterialOptions.Accessor.Copy(source, Result);
            }
            else if (levelDefinition.Count == 2)
            {
                var left = levelDefinition[0];
                var right = levelDefinition[1];
                Result.Lerp(left.DataObject.Data, right.DataObject.Data, level);
            }
            else
            {
                var left = levelDefinition[0];
                var rightIndex = 1;

                int index = 1;
                while (index < levelDefinition.Count)
                {
                    var current = levelDefinition[index];
                    if (current.Threshol < level)
                    {
                        left = current;
                        rightIndex++;
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                var right = levelDefinition[rightIndex];
                Result.Lerp(left.DataObject.Data, right.DataObject.Data, level);
            }
        }

        public void RegenerateResult()
        {
            SetLevel(level);
        }

        [ContextMenu(nameof(UpdateMaterial))]
        public void UpdateMaterial()
        {
            RegenerateResult();

            foreach (var lodQuality in lodQualitys)
            {
                lodQuality.Material.renderQueue = (int)ProjectSettings.Current.RenderQueue;

                ModeOptions.Accessor.Copy(lodQuality.ModeObject.Mode, lodQuality.Material);
                int mask = ModeOptions.Accessor.GetEnabledKeywords(lodQuality.ModeObject.Mode);
                mask |= (int)OceanMode.Tessellation;
                MaterialOptions.Accessor.CopyWithoutKeywords(Result, lodQuality.Material, group => ShaderAccessor.FilterByMask(group, mask));

                if (lodQuality.MarkMaterial != null)
                {
                    mask = (int)(WaveOptions.WaveAll | OceanMode.Tessellation);
                    ModeOptions.Accessor.Copy(lodQuality.ModeObject.Mode, lodQuality.MarkMaterial, group => ShaderAccessor.FilterByMask(group, mask));
                    MaterialOptions.Accessor.CopyWithoutKeywords(Result, lodQuality.MarkMaterial, group => ShaderAccessor.FilterByMask(group, mask));
                }
            }
        }

#if UNITY_EDITOR

        [Header("Editor only")]
        [SerializeField] private bool isAutoUpdateMaterial = true;
        internal bool IsAutoUpdateMaterial
        {
            get { return isAutoUpdateMaterial; }
            set { isAutoUpdateMaterial = value; }
        }

        internal float CurrentVersion { get; private set; } = -1;

        private void EditorUpdateMaterial()
        {
            if (!isAutoUpdateMaterial)
                return;

            float version = 0;
            foreach (var level in levelDefinition)
            {
                if (level.DataObject != null)
                    version += level.DataObject.Version;
            }

            foreach (var lod in lodQualitys)
            {
                if (lod.ModeObject != null)
                {
                    version += lod.ModeObject.Version;
                }
            }

            if (version != CurrentVersion)
            {
                UpdateMaterial();
                CurrentVersion = version;
            }
        }

        private void OnValidate()
        {
            UpdateMaterial();
        }

#endif

        public override float GetHeightAt(Vector3 worldPos)
        {
            return VertexHelper.GetVertexHeight(Result, lodQualitys[0].ModeObject.Mode, worldPos);
        }

        private void OnEnable()
        {
            UpdateMaterial();
        }

        [Serializable]
        public class OceanLevelData
        {
            [SerializeField] private string name = null;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [SerializeField] [Range(0, 1)] private float threshol = 0f;
            public float Threshol
            {
                get { return threshol; }
                set { threshol = value; }
            }

            [SerializeField] private OceanMaterialData dataObject;
            public OceanMaterialData DataObject
            {
                get { return dataObject; }
                set { dataObject = value; }
            }
        }

        [Serializable]
        private class OceanLevelDataList : CustomReorderableList<OceanLevelData>
        {
        }

        [Serializable]
        public class LODQuality
        {
            [SerializeField] private OceanMaterialMode modeObject;
            public OceanMaterialMode ModeObject
            {
                get { return modeObject; }
                set { modeObject = value; }
            }

            [SerializeField] private Material material;
            public Material Material
            {
                get { return material; }
                set { material = value; }
            }

            [SerializeField] private Material markMaterial = null;
            public Material MarkMaterial
            {
                get { return markMaterial; }
                set { markMaterial = value; }
            }

        }

        [Serializable]
        private class LODQualityList : CustomReorderableList<LODQuality>
        {
        }
    }
}
