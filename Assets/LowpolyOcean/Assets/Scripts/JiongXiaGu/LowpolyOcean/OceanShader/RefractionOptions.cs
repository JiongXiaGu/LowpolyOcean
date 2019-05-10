using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(OceanMode.RefractionSimple | OceanMode.RefractionFull)]
    public class RefractionOptions
    {
        public const string ClarityShaderFieldName = "_OceanRefractionClarity";
        public const string DeepColorShaderFieldName = "_OceanRefractionDeepColor";
        public const string OffsetShaderFieldName = "_OceanRefractionOffset";

        public static RefractionOptions Default => new RefractionOptions()
        {
            Clarity = 0.2f,
            DeepColor = new Color(0f, 0.19f, 0.32f, 0.41f),
            Offset = 0.8f,
        };

        [ShaderField(ClarityShaderFieldName, OceanMode.RefractionSimple | OceanMode.RefractionFull)] [Range(0, 3)] public float Clarity;
        [ShaderField(DeepColorShaderFieldName, OceanMode.RefractionSimple | OceanMode.RefractionFull)] public Color DeepColor;
        [ShaderField(OffsetShaderFieldName, OceanMode.RefractionFull)] [Range(-3, 3)] public float Offset;
    }
}
