using System;

namespace JiongXiaGu.ShaderTools
{
    /// <summary>
    /// Format : static IShaderField MethodName();
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ShaderCustomFieldMethodAttribute : Attribute
    {
    }
}
