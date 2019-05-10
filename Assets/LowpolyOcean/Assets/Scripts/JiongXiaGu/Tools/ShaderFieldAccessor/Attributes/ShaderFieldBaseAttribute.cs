using System;

namespace JiongXiaGu.ShaderTools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ShaderFieldBaseAttribute : Attribute
    {
    }

}
