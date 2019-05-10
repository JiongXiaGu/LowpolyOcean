using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(OceanMode.Tessellation)]
    public class TessellationOptions
    {
        public const string TessellationShaderFieldName = "_OceanTessellation";
        public const string TessMinDistanceShaderFieldName = "_OceanTessMinDistance";
        public const string TessMaxDistanceShaderFieldName = "_OceanTessMaxDistance";

        public static TessellationOptions Default => new TessellationOptions()
        {
            Tessellation = 4,
            TessMinDistance = 100,
            TessMaxDistance = 200,
        };

        [ShaderField(TessellationShaderFieldName)] [Range(1, 32)] public float Tessellation;
        [ShaderField(TessMinDistanceShaderFieldName)] public float TessMinDistance;
        [ShaderField(TessMaxDistanceShaderFieldName)] public float TessMaxDistance;
    }
}
