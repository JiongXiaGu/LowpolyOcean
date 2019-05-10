namespace JiongXiaGu.ShaderTools
{

    public class ShaderFieldKeywordAttribute : ShaderFieldBaseAttribute
    {
        public int Mask { get; set; }
        public string Keyword { get; set; }
        public string ShortName { get; set; }

        public ShaderFieldKeywordAttribute(string keyword, int mask)
        {
            Keyword = keyword;
            Mask = mask;
        }

        public ShaderFieldKeywordAttribute(string keyword) : this(keyword, ~0)
        {
        }

        public ShaderFieldKeywordAttribute(string keyword, object mask) : this(keyword, (int)mask)
        {
            ShortName = mask.ToString();
        }
    }

}
