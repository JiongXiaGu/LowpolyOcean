using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(OceanMaterialData))]
    public class OceanMaterialData : ScriptableObject
    {
        [SerializeField] private OceanMaterialMode modeObject;
        public OceanMaterialMode ModeObject
        {
            get { return modeObject; }
            set { modeObject = value; }
        }

        [SerializeField] private MaterialOptions data;
        public MaterialOptions Data
        {
            get { return data; }
            set { data = value; }
        }

#if UNITY_EDITOR
        internal int Version { get; private set; }

        private void OnValidate()
        {
            Version++;
        }
#endif
    }
}
