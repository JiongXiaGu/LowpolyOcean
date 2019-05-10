using JiongXiaGu.ShaderTools;
using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [Serializable]
    [ShaderFieldGroup(ModeAll)]
    public class FoamOptions
    {
        public const string ColorName = "_OceanFoamColor";
        public const string IntensityName = "_OceanFoamIntensity";
        public const string RangeName = "_OceanFoamRange";
        public const string TextureName = "_OceanFoamTexture";
        public const string SpeedName = "_OceanFoamSpeed";
        public const string RectName = "_OceanFoamRect";
        public static readonly int ShpereData0Name = Shader.PropertyToID("_OceanFoamShpereData0");
        public static readonly int ShpereData1Name = Shader.PropertyToID("_OceanFoamShpereData1");
        public static readonly int AreaTexture0Name = Shader.PropertyToID("_OceanFoamAreaTexture0");
        public static readonly int AreaRect0Name = Shader.PropertyToID("_OceanFoamAreaRect0");
        public static readonly int AreaPosition0Name = Shader.PropertyToID("_OceanFoamAreaPosition0");

        public static FoamOptions Default => new FoamOptions()
        {
            Color = new Color(1, 1, 1, 0.3f),
            Range = 0.5f,
            Texture = null,
            Speed = new Vector4(0.01f, 0.03f, -0.02f, 0.01f),
            Rect = new Vector4(0, 0, 10, 10),
        };

        public const int ModeAll = (int)FoamMode.SimpleEyeDepth
            | (int)FoamMode.EyeDepth
            | (int)FoamShpereMode.Shpere8
            | (int)FoamShpereMode.Shpere16
            | (int)FoamAreaMode.Area1;

        public const int TextureAll = (int)FoamMode.EyeDepth
            | (int)FoamShpereMode.Shpere8
            | (int)FoamShpereMode.Shpere16
            | (int)FoamAreaMode.Area1;

        /// <summary>
        /// foam color
        /// </summary>
        [ShaderField(ColorName, ModeAll)] public Color Color;

        /// <summary>
        /// foam transparency
        /// </summary>
        [ShaderField(IntensityName, ModeAll)] [Range(0, 50)] public float Intensity;

        /// <summary>
        /// foam range, will not affect foam shpere.
        /// </summary>
        [ShaderField(RangeName, ModeAll)] public float Range;

        /// <summary>
        /// Foam style texture
        /// </summary>
        [ShaderField(TextureName, TextureAll)] public Texture Texture;

        /// <summary>
        /// xy: speed 1, zw: speed 2
        /// </summary>
        [ShaderField(SpeedName, TextureAll)] public Vector4 Speed;

        /// <summary>
        /// xy: tile center point ratio (0f~1f), zw:tile size
        /// </summary>
        [ShaderField(RectName, TextureAll)] public Vector4 Rect;
    }
}
