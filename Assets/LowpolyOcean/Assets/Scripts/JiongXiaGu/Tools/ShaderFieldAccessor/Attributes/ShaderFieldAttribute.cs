namespace JiongXiaGu.ShaderTools
{

    public class ShaderFieldAttribute : ShaderFieldBaseAttribute
    {
        public string Name { get; set; }
        public int Mask { get; set; }

        public ShaderFieldAttribute(string name, int mask)
        {
            Mask = mask;
            Name = name;
        }

        public ShaderFieldAttribute(string name) : this(name, ~0)
        {
            Name = name;
        }

        public ShaderFieldAttribute(string name, object enumValue) : this(name, (int)enumValue)
        {
        }
    }

}
