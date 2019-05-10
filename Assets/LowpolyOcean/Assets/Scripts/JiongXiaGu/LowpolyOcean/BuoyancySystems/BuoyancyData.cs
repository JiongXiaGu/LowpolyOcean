using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiongXiaGu.WaterBuoyancy;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{


    [CreateAssetMenu(menuName = nameof(LowpolyOcean) + "/" + nameof(BuoyancyData))]
    public class BuoyancyData : WaterBuoyancy.OceanData
    {
        [SerializeField] private OceanData oceanData = null;
        public OceanData OceanData
        {
            get { return oceanData; }
            set { oceanData = value; }
        }

        [SerializeField] private float height = 0;
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        private float GetWaveOffset(Vector3 pos)
        {
            if (oceanData == null)
                return 0;

            return oceanData.GetHeightAt(pos);
        }

        public override float DistanceToSurface(Vector3 postion)
        {
            float height = this.height;
            height += GetWaveOffset(postion);
            return postion.y - height;
        }
    }
}
