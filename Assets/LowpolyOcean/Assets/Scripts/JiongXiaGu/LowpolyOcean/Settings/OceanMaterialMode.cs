using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(OceanMaterialMode))]
    public class OceanMaterialMode : ScriptableObject
    {

        [SerializeField] private ModeOptions mode = ModeOptions.Default;
        public ModeOptions Mode
        {
            get { return mode; }
            set { mode = value; }
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
