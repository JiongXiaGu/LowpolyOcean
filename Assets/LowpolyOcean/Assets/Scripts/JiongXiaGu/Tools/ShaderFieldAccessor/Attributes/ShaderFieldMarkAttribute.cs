namespace JiongXiaGu.ShaderTools
{
    /// <summary>
    /// Assigned to the shader by external method
    /// </summary>
    public class ShaderFieldMarkAttribute : ShaderFieldBaseAttribute
    {
        public int Mask { get; set; }

        public ShaderFieldMarkAttribute(int mask)
        {
            Mask = mask;
        }

        public ShaderFieldMarkAttribute(object mask) : this((int)mask)
        {
        }

        public ShaderFieldMarkAttribute() : this(~0)
        {
        }
    }

}
