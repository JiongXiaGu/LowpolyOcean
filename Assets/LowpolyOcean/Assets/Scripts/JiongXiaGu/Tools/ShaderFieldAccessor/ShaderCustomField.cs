using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{


    public class ShaderCustomField : ShaderField, IShaderStandardFiled, IShaderFieldGroup
    {
        public override ShaderFieldFormat Format => base.Format | ShaderFieldFormat.Custom;

        public ShaderCustomField(string name, int mask, IReflectiveField fieldAccessor, IShaderField customField) : base(name, mask, fieldAccessor, customField)
        {
        }
    }
}
