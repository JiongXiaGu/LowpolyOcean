using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(UnderOceanMaterialData))]
    public class UnderOceanMaterialData : ScriptableObject
    {
        [SerializeField] private UnderOceanMaterialMode modeObject;
        public UnderOceanMaterialMode ModeObject
        {
            get { return modeObject; }
            set { modeObject = value; }
        }

        [SerializeField] private UnderMaterialOptions data;
        public UnderMaterialOptions Data
        {
            get { return data; }
            set { data = value; }
        }

        private void OnValidate()
        {
            modeObject?.Mode.Dirty();
            data.Dirty();
        }
    }
}
