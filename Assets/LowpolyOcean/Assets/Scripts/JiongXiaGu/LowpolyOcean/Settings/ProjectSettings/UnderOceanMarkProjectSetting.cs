using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    [Serializable]
    public class UnderOceanMarkProjectSetting
    {
        public static readonly ShaderAccessor ShaderAccessor = new ShaderAccessor(typeof(UnderOceanMarkProjectSetting));

        public static UnderOceanMarkProjectSetting Default => new UnderOceanMarkProjectSetting()
        {
            TextureScale = 1,
            UnderOceanFornMark = 0.5f,
            UnderOceanBackMark = 0,
            OceanFornMark = 0.8f,
            OceanBackMark = 0.2f,
        };

        [Range(0.1f, 1)] public float TextureScale;
        [ShaderField("_LPUnderOceanFornMark")] [Range(0, 1)] public float UnderOceanFornMark;
        [ShaderField("_LPUnderOceanBackMark")] [Range(0, 1)] public float UnderOceanBackMark;
        [ShaderField("_LPOceanFornMark")] [Range(0, 1)] public float OceanFornMark;
        [ShaderField("_LPOceanBackMark")] [Range(0, 1)] public float OceanBackMark;
    }
}
