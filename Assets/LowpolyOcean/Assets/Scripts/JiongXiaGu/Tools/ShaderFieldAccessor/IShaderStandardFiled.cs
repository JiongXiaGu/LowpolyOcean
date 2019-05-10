namespace JiongXiaGu.ShaderTools
{
    public interface IShaderStandardFiled
    {
        int Mask { get; }
        string ShaderFieldName { get; }
        int ShderFieldID { get; }
        IReflectiveField ReflectiveField { get; }
        ShaderFieldFormat Format { get; }
    }
}
