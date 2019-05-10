using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(OceanMode.ReflectionSimpleOffset)]
    public class ReflectionOptions
    {
        public const string IntensityShaderFieldName = "_OceanReflectionIntensity";
        public const string OffsetShaderFieldName = "_OceanReflectionOffset";
        public const string HeightOffsetFieldName = "_OceanReflectionHeight";

        public static ReflectionOptions Default => new ReflectionOptions()
        {
            Intensity = 0.4f,
            Offset = 1f,
            HeightOffset = 0,
        };

        [ShaderField(IntensityShaderFieldName)] [Range(0, 1)] public float Intensity;
        [ShaderField(OffsetShaderFieldName)] public float Offset;
        [ShaderField(HeightOffsetFieldName)] public float HeightOffset;
    }
}
