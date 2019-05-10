using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(UnderOceanMaterialMode))]
    public class UnderOceanMaterialMode : ScriptableObject
    {

        [SerializeField] private UnderOceanModeOptions mode = UnderOceanModeOptions.Default;
        public UnderOceanModeOptions Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private void OnValidate()
        {
            mode.Dirty();
        }
    }
}
