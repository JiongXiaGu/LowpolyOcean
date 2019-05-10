using JiongXiaGu.ShaderTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    public sealed class OceanShaderDrawer : ShaderGUI
    {
        private static readonly SignalPersistentSave expandSave = new SignalPersistentSave(nameof(OceanShaderDrawer) + "Expand");
        private IDrawer[] drawers;
        private KeywordDrawer keywordDrawer;
        private List<string> keywords;
        private static OceanMode alwaysEnableMode = OceanMode.Tessellation;

        private void Init(MaterialProperty[] properties)
        {
            //if (drawers != null)
            //    return;

            keywordDrawer = new KeywordDrawer(properties);
            drawers = new IDrawer[]
            {
                new DefaultDrawer(properties, OceanShaderOptions.TessellationAccessor, nameof(OceanShaderOptions.Tessellation)),
                new WaveDrawer(properties),
                new DefaultDrawer(properties, OceanShaderOptions.LightingAccessor, nameof(OceanShaderOptions.Lighting)),
                new DefaultDrawer(properties, OceanShaderOptions.RefractionAccessor, nameof(OceanShaderOptions.Refraction)),
                new DefaultDrawer(properties, OceanShaderOptions.FoamAccessor, nameof(OceanShaderOptions.Foam)),
                new DefaultDrawer(properties, OceanShaderOptions.CookieAccessor, nameof(OceanShaderOptions.Cookie)),
                new DefaultDrawer(properties, OceanShaderOptions.ReflectionAccessor, nameof(OceanShaderOptions.Reflection)),
                new DefaultDrawer(properties, OceanShaderOptions.BackLightingAccessor, nameof(OceanShaderOptions.BackLighting)),
                new DefaultDrawer(properties, OceanShaderOptions.BackRefractionAccessor, nameof(OceanShaderOptions.BackRefraction)),
                new DefaultDrawer(properties, OceanShaderOptions.PointLightingAccessor, nameof(OceanShaderOptions.PointLighting)),
            };

            keywords = new List<string>();
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Init(properties);
            expandSave.LoadEditor();

            Material material = (Material)materialEditor.target;
            EditorGUI.BeginChangeCheck();
            bool isKeywordChanged = false;

            int mask = 0;
            isKeywordChanged |= keywordDrawer.Draw(materialEditor, keywords, ref mask);
            var oceanMask = (OceanMode)mask | alwaysEnableMode;

            foreach (var drawer in drawers)
            {
                isKeywordChanged |= drawer.Draw(materialEditor, keywords, oceanMask);
            }

            if (isKeywordChanged)
            {
                material.shaderKeywords = keywords.ToArray();
            }
            keywords.Clear();

            materialEditor.RenderQueueField();

            if (EditorGUI.EndChangeCheck())
            {
                expandSave.SaveEditor();
                EditorUtility.SetDirty(materialEditor.target as Material);
            }
        }

        public interface IDrawer
        {
            bool Draw(MaterialEditor materialEditor, List<string> keywords, OceanMode mask);
        }

        public class KeywordDrawer
        {
            public static readonly int expandedID = expandSave.CreateItem(nameof(KeywordDrawer));
            private ShaderFieldDrawer autoDrawer;

            public KeywordDrawer(MaterialProperty[] properties)
            {
                autoDrawer = new ShaderFieldDrawer(OceanShaderOptions.ModeAccessor, properties);
            }

            public bool Draw(MaterialEditor materialEditor, List<string> keywords, ref int mask)
            {
                if (autoDrawer.Drawers.Count == 0)
                {
                    return false;
                }

                bool isExpanded = expandSave[expandedID] = EditorGUILayout.Foldout(expandSave[expandedID], nameof(OceanShaderOptions.Mode));
                if (isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        autoDrawer.Draw(materialEditor, keywords, ~0);
                        mask = autoDrawer.Mask;
                        return autoDrawer.IsKeywordChanged;
                    }
                }
                else
                {
                    autoDrawer.Extract(materialEditor, keywords, ~0);
                    mask = autoDrawer.Mask;
                    return false;
                }
            }
        }

        public class DefaultDrawer : IDrawer
        {
            private int expandedID;
            private ShaderAccessor accessor;
            private ShaderFieldDrawer autoDrawer;
            private string displayName;

            public DefaultDrawer(MaterialProperty[] properties, ShaderAccessor accessor, string name)
            {
                autoDrawer = new ShaderFieldDrawer(accessor, properties);
                expandedID = expandSave.CreateItem(name);
                displayName = name;
                this.accessor = accessor;
            }

            public bool Draw(MaterialEditor materialEditor, List<string> keywords, OceanMode mask)
            {
                if (autoDrawer.Drawers.Count == 0)
                    return false;
                if ((accessor.Mask & (int)mask) == 0)
                    return false;

                bool isExpanded = expandSave[expandedID] = EditorGUILayout.Foldout(expandSave[expandedID], displayName);
                if (isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        autoDrawer.Draw(materialEditor, keywords, (int)mask);
                        return autoDrawer.IsKeywordChanged;
                    }
                }
                else
                {
                    autoDrawer.Extract(materialEditor, keywords, (int)mask);
                    return false;
                }
            }
        }

        public class WaveDrawer : IDrawer
        {
            public static readonly int IsExpanded = expandSave.CreateItem(nameof(WaveDrawer));
            public static readonly int[] IsExpandedWave = new int[]
            {
                expandSave.CreateItem(GetExpandName("IsExpandedWave0")),
                expandSave.CreateItem(GetExpandName("IsExpandedWave1")),
                expandSave.CreateItem(GetExpandName("IsExpandedWave2")),
                expandSave.CreateItem(GetExpandName("IsExpandedWave3")),
            };

            private MaterialProperty Texture;
            private MaterialProperty[] Rect;
            private MaterialProperty Radian;
            private MaterialProperty UniformRadian;
            private MaterialProperty HeightPow;
            private MaterialProperty HeightScale;
            private MaterialProperty SpeedZ;

            public WaveDrawer(MaterialProperty[] properties)
            {
                Texture = FindProperty(WaveOptions.TextureShaderFieldName, properties);
                Rect = new MaterialProperty[4];
                Rect[0] = FindProperty(WaveOptions.Rect0ShaderFieldName, properties);
                Rect[1] = FindProperty(WaveOptions.Rect1ShaderFieldName, properties);
                Rect[2] = FindProperty(WaveOptions.Rect2ShaderFieldName, properties);
                Rect[3] = FindProperty(WaveOptions.Rect3ShaderFieldName, properties);
                Radian = FindProperty(WaveOptions.RadianShaderFieldName, properties);
                UniformRadian = FindProperty(WaveOptions.UniformRadianFieldName, properties);
                HeightPow = FindProperty(WaveOptions.HeightPowShaderFieldName, properties);
                HeightScale = FindProperty(WaveOptions.HeightScaleShaderFieldName, properties);
                SpeedZ = FindProperty(WaveOptions.SpeedZShaderFieldName, properties);
            }

            private static string GetExpandName(string name)
            {
                return nameof(WaveDrawer) + "." + name;
            }

            private void SliderVector(string name, MaterialProperty property, int index, float leftValue, float rightValue)
            {
                var value = property.vectorValue;
                var newValue = EditorGUILayout.Slider(name, value[index], leftValue, rightValue);
                if (value[index] != newValue)
                {
                    value[index] = newValue;
                    property.vectorValue = value;
                }
            }

            private void SliderRadian(string name, MaterialProperty property, float leftValue, float rightValue)
            {
                var radian = property.floatValue;
                var angle = radian * Mathf.Rad2Deg;
                var newAngle = EditorGUILayout.Slider(name, angle, leftValue, rightValue);
                if (angle != newAngle)
                {
                    radian = newAngle * Mathf.Deg2Rad;
                    property.floatValue = radian;
                }
            }

            private void SliderRadian(string name, MaterialProperty property, int index, float leftValue, float rightValue)
            {
                var value = property.vectorValue;
                var radian = value[index];
                var angle = radian * Mathf.Rad2Deg;
                var newAngle = EditorGUILayout.Slider(name, angle, leftValue, rightValue);
                if (angle != newAngle)
                {
                    radian = newAngle * Mathf.Deg2Rad;
                    value[index] = radian;
                    property.vectorValue = value;
                }
            }

            private void DrawWaveOptions(MaterialEditor materialEditor, string name, int index)
            {
                bool isExpanded = expandSave[IsExpandedWave[index]] = EditorGUILayout.Foldout(expandSave[IsExpandedWave[index]], name);
                if (isExpanded)
                {
                    materialEditor.ShaderProperty(Rect[index], WaveOptionsDrawData.RectDisplayName);
                    SliderRadian(WaveOptionsDrawData.AngleDisplayName, Radian, index, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);
                    SliderVector(WaveOptionsDrawData.HeightPowDisplayName, HeightPow, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);
                    SliderVector(WaveOptionsDrawData.HeightScaleDisplayName, HeightScale, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);
                    SliderVector(WaveOptionsDrawData.SpeedZDisplayName, SpeedZ, index, WaveOptionsDrawData.SpeedMinValue, WaveOptionsDrawData.SpeedMaxValue);
                }
            }

            private void DrawWaveHorizontalOffset(MaterialEditor materialEditor, string name, int index)
            {
                bool isExpanded = expandSave[IsExpandedWave[index]] = EditorGUILayout.Foldout(expandSave[IsExpandedWave[index]], name);

                if (isExpanded)
                {
                    materialEditor.ShaderProperty(Rect[index], WaveOptionsDrawData.RectDisplayName);
                    SliderRadian(WaveOptionsDrawData.AngleDisplayName, Radian, index, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);
                    SliderVector(WaveOptionsDrawData.HorizontalOffsetXDisplayName, HeightPow, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);
                    SliderVector(WaveOptionsDrawData.HorizontalOffsetYDisplayName, HeightScale, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);
                    SliderVector(WaveOptionsDrawData.SpeedZDisplayName, SpeedZ, index, WaveOptionsDrawData.SpeedMinValue, WaveOptionsDrawData.SpeedMaxValue);
                }
            }

            public bool Draw(MaterialEditor materialEditor, List<string> keywords, OceanMode mask)
            {
                bool isExpanded = expandSave[IsExpanded] = EditorGUILayout.Foldout(expandSave[IsExpanded], nameof(OceanShaderOptions.Wave));
                if (isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        materialEditor.ShaderProperty(Texture, Texture.displayName);
                        SliderRadian(WaveOptionsDrawData.UniformRadianDisplayName, UniformRadian, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);

                        if ((mask & OceanMode.Wave1) != 0)
                        {
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave1Names[0], 2);
                            DrawWaveHorizontalOffset(materialEditor, WaveOptionsDrawData.Wave1Names[1], 3);
                        }
                        else if ((mask & OceanMode.Wave2) != 0)
                        {
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave2Names[0], 0);
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave2Names[1], 1);
                        }
                        else if ((mask & OceanMode.Wave3) != 0)
                        {
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave4Names[0], 0);
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave4Names[1], 1);
                            DrawWaveOptions(materialEditor, WaveOptionsDrawData.Wave4Names[2], 2);
                            DrawWaveHorizontalOffset(materialEditor, WaveOptionsDrawData.Wave4Names[3], 3);
                        }
                    }
                }
                return false;
            }
        }
    }
}
