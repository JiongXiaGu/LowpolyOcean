using System.Collections.Generic;
using UnityEditor;

namespace JiongXiaGu.LowpolyOcean
{

    public sealed class WaveOptionsDrawData
    {
        public static IReadOnlyList<string> Wave1Names { get; } = new string[]
        {
            "Wave 0(rg)", "Vertex horizontal offset(ba)"
        };
        public static IReadOnlyList<string> Wave2Names { get; } = new string[]
        {
            "Wave 0(rg)", "Wave 1(ba)"
        };
        public static IReadOnlyList<string> Wave4Names { get; } = new string[]
        {
            "Wave 0(r)", "Wave 1(g)", "Wave 2(b)", "Vertex horizontal offset(a)"
        };

        public const string UniformRadianDisplayName = "Angle";
        public const string RectDisplayName = "Rect";
        public const string AngleDisplayName = "Angle";
        public const string HeightPowDisplayName = "Height pow";
        public const string HeightScaleDisplayName = "Height scale";
        public const string SpeedZDisplayName = "SpeedZ";
        public const string HorizontalOffsetXDisplayName = "Offset x";
        public const string HorizontalOffsetYDisplayName = "Offset z";

        public const float HeightMinValue = -10;
        public const float HeightMaxValue = 10;
        public const float SpeedMinValue = -3;
        public const float SpeedMaxValue = 3;
        public const float AngleLeftValue = 0;
        public const float AngleRightValue = 360;
    }
}
