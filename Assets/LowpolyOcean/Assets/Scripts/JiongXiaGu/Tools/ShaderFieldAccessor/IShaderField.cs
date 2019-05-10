using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    public interface IShaderField
    {
        ShaderFieldFormat Format { get; }
        object GetGlobalValues(ShaderField info);
        void SetGlobalValues(ShaderField info, object value);
        object GetValue(ShaderField info, Material material);
        void SetMaterial(ShaderField info, Material material, object value);
        object Lerp(object v0, object v1, float t);
    }

}
