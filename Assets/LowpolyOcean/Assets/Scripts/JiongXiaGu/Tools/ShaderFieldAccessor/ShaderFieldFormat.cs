using System;

namespace JiongXiaGu.ShaderTools
{
    [Flags]
    public enum ShaderFieldFormat
    {
        None = 0,
        Color = 1,
        ColorArray = 1 << 1,
        Vector2 = 1 << 2,
        Vector3 = 1 << 3,
        Vector4 = 1 << 4,
        Vector4Array = 1 << 5,
        Float = 1 << 6,
        FloatArray = 1 << 7,
        Texture = 1 << 8,
        TextureOffset = 1 << 9,
        TextureScale = 1 << 10,
        Matrix = 1 << 11,
        MatrixArray = 1 << 12,
        Keyword = 1 << 13,
        Custom = 1 << 14,
        Int = 1 << 15,
        Enum = 1 << 16,
        EnumKeyword = 1 << 17,
        EnumFlagKeyword = 1 << 18,
        Mark = 1 << 19,
        All = ~0,
    }

}
