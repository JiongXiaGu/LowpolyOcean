namespace JiongXiaGu.ShaderTools
{

    /// <summary>
    /// Define a keyword collection, the order is: [null][0][Keyword0][Mask0][Keyword1][Mask1]....
    /// Enum for the flag type order is [Keyword0][Mask0][Keyword1][Mask1]....
    /// </summary>
    public sealed class ShaderFieldEnumKeywordAttribute : ShaderFieldBaseAttribute
    {
        public int Mask { get; set; }
        public object[] Keywords { get; set; }

        public ShaderFieldEnumKeywordAttribute(params object[] keywords)
        {
            Keywords = keywords;
            for (int i = 0; i < keywords.Length; i += 2)
            {
                Mask |= (int)keywords[i + 1];
            }
        }
    }

}
