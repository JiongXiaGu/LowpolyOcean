using JiongXiaGu.ShaderTools;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [DataContract]
    [Serializable]
    [ShaderFieldGroup(OceanMode.PointLightingSimple)]
    public class PointLightingOptions
    {
        public const string IntensityShaderFieldName = "_OceanPointLightIntensity";
        public const string BackIntensityFieldName = "_OceanBackPointLightIntensity";

        public static PointLightingOptions Default => new PointLightingOptions()
        {
            Intensity = 1,
            BackIntensity = 1,
        };

        [DataMember] [ShaderField(IntensityShaderFieldName)] [Range(0, 1)] public float Intensity;
        [DataMember] [ShaderField(BackIntensityFieldName)] [Range(0, 1)] public float BackIntensity;
    }
}
