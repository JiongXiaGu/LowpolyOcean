using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using JiongXiaGu.ShaderTools;

namespace JiongXiaGu.LowpolyOcean
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(OceanMaterialData))]
    public sealed class OceanMaterialDataEditor : Editor
    {
        private SerializedProperty script;
        private SerializedProperty modeObject;
        private SerializedObject modeSerializedObject;
        private SerializedProperty modeOptions;
        private SerializedProperty dataOptions;
        private SerializedPropertyDrawer modeDrawer;
        private SerializedPropertyDrawer dataDrawer;
        private static OceanMode alwaysEnableMode = OceanMode.Tessellation;

        private void CreateModeOptions()
        {
            if (modeObject.objectReferenceValue != null)
            {
                modeSerializedObject = new SerializedObject(modeObject.objectReferenceValue);
                modeOptions = modeSerializedObject.FindProperty("mode");
                modeDrawer = new SerializedPropertyDrawer(ModeOptions.Accessor, modeOptions);
            }
            else
            {
                modeSerializedObject = null;
                modeOptions = null;
                modeDrawer = null;
            }
        }

        private void OnEnable()
        {
            script = serializedObject.FindProperty(EditorHelper.ScriptName);
            modeObject = serializedObject.FindProperty("modeObject");
            CreateModeOptions();
            dataOptions = serializedObject.FindProperty("data");
            dataDrawer = new SerializedPropertyDrawer(MaterialOptions.Accessor, dataOptions);
            dataDrawer.ChangeDrawer(() => new WaveDrawer(dataOptions.FindPropertyRelative(nameof(MaterialOptions.Wave))), nameof(MaterialOptions.Wave));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.PropertyField(script, true);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(modeObject, true);

                    if (changed.changed)
                    {
                        CreateModeOptions();
                    }
                }
            }

            if (modeOptions != null)
            {
                modeSerializedObject.Update();
                EditorGUILayout.PropertyField(modeOptions, true);
                modeSerializedObject.ApplyModifiedProperties();
            }

            if (modeDrawer != null)
            {
                var mask = modeDrawer.Extract();
                dataDrawer.OnGUI(mask | (int)alwaysEnableMode);
            }
            else
            {
                dataDrawer.OnGUI(~0);
            }

            serializedObject.ApplyModifiedProperties();
        }


        private class WaveDrawer : SerializedPropertyDrawer.Drawer
        {
            private SerializedProperty Texture;
            private SerializedProperty[] Rect;
            private SerializedProperty Radian;
            private SerializedProperty UniformRadian;
            private SerializedProperty HeightPow;
            private SerializedProperty HeightScale;
            private SerializedProperty SpeedZ;

            public WaveDrawer(SerializedProperty property) : base(property)
            {
                Texture = property.FindPropertyRelative(nameof(WaveOptions.Texture));
                Rect = new SerializedProperty[4];
                Rect[0] = property.FindPropertyRelative(nameof(WaveOptions.Rect0));
                Rect[1] = property.FindPropertyRelative(nameof(WaveOptions.Rect1));
                Rect[2] = property.FindPropertyRelative(nameof(WaveOptions.Rect2));
                Rect[3] = property.FindPropertyRelative(nameof(WaveOptions.Rect3));
                Radian = property.FindPropertyRelative(nameof(WaveOptions.Radian));
                UniformRadian = property.FindPropertyRelative(nameof(WaveOptions.UniformRadian));
                HeightPow = property.FindPropertyRelative(nameof(WaveOptions.HeightPow));
                HeightScale = property.FindPropertyRelative(nameof(WaveOptions.HeightScale));
                SpeedZ = property.FindPropertyRelative(nameof(WaveOptions.SpeedZ));
            }

            private void SliderVector4(string name, SerializedProperty serializedProperty, int index, float leftValue, float rightValue)
            {
                var value = serializedProperty.vector4Value;
                var newValue = EditorGUILayout.Slider(name, value[index], leftValue, rightValue);
                if (value[index] != newValue)
                {
                    value[index] = newValue;
                    serializedProperty.vector4Value = value;
                }
            }

            private void SliderRadian(string name, SerializedProperty serializedProperty, float leftValue, float rightValue)
            {
                var radian = serializedProperty.floatValue;
                var angle = radian * MathfHelper.RadinaToAngle;
                var newAngle = EditorGUILayout.Slider(name, angle, leftValue, rightValue);
                if (angle != newAngle)
                {
                    radian = newAngle * MathfHelper.AngleToRadina;
                    serializedProperty.floatValue = radian;
                }
            }

            private void SliderRadian(string name, SerializedProperty serializedProperty, int index, float leftValue, float rightValue)
            {
                var value = serializedProperty.vector4Value;
                var radian = value[index];
                var angle = radian * MathfHelper.RadinaToAngle;
                var newAngle = EditorGUILayout.Slider(name, angle, leftValue, rightValue);
                if (angle != newAngle)
                {
                    radian = newAngle * MathfHelper.AngleToRadina;
                    value[index] = radian;
                    serializedProperty.vector4Value = value;
                }
            }

            private void DrawWaveOptions(string name, int index)
            {
                Rect[index].isExpanded = EditorGUILayout.Foldout(Rect[index].isExpanded, name);
                if (Rect[index].isExpanded)
                {
                    Rect[index].vector4Value = EditorGUILayout.Vector4Field(WaveOptionsDrawData.RectDisplayName, Rect[index].vector4Value);

                    SliderRadian(WaveOptionsDrawData.AngleDisplayName, Radian, index, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);

                    SliderVector4(WaveOptionsDrawData.HeightPowDisplayName, HeightPow, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);

                    SliderVector4(WaveOptionsDrawData.HeightScaleDisplayName, HeightScale, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);

                    SliderVector4(WaveOptionsDrawData.SpeedZDisplayName, SpeedZ, index, WaveOptionsDrawData.SpeedMinValue, WaveOptionsDrawData.SpeedMaxValue);
                }
            }

            private void DrawWaveHorizontalOffset(string name, int index)
            {
                Rect[index].isExpanded = EditorGUILayout.Foldout(Rect[index].isExpanded, name);
                if (Rect[index].isExpanded)
                {
                    Rect[index].vector4Value = EditorGUILayout.Vector4Field(WaveOptionsDrawData.RectDisplayName, Rect[index].vector4Value);

                    SliderRadian(WaveOptionsDrawData.AngleDisplayName, Radian, index, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);

                    SliderVector4(WaveOptionsDrawData.HorizontalOffsetXDisplayName, HeightPow, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);

                    SliderVector4(WaveOptionsDrawData.HorizontalOffsetYDisplayName, HeightScale, index, WaveOptionsDrawData.HeightMinValue, WaveOptionsDrawData.HeightMaxValue);

                    SliderVector4(WaveOptionsDrawData.SpeedZDisplayName, SpeedZ, index, WaveOptionsDrawData.SpeedMinValue, WaveOptionsDrawData.SpeedMaxValue);
                }
            }

            public override void OnGUI(int mask)
            {
                if ((mask & (int)WaveOptions.WaveAll) == 0)
                    return;

                SerializedProperty.isExpanded = EditorGUILayout.Foldout(SerializedProperty.isExpanded, nameof(OceanShaderOptions.Wave));

                if (SerializedProperty.isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(Texture);
                        SliderRadian(WaveOptionsDrawData.UniformRadianDisplayName, UniformRadian, WaveOptionsDrawData.AngleLeftValue, WaveOptionsDrawData.AngleRightValue);

                        if ((mask & (int)OceanMode.Wave1) != 0)
                        {
                            DrawWaveOptions(WaveOptionsDrawData.Wave1Names[0], 2);
                            DrawWaveHorizontalOffset(WaveOptionsDrawData.Wave1Names[1], 3);
                        }
                        else if ((mask & (int)OceanMode.Wave2) != 0)
                        {
                            DrawWaveOptions(WaveOptionsDrawData.Wave2Names[0], 0);
                            DrawWaveOptions(WaveOptionsDrawData.Wave2Names[1], 1);
                        }
                        else if ((mask & (int)OceanMode.Wave3) != 0)
                        {
                            DrawWaveOptions(WaveOptionsDrawData.Wave4Names[0], 0);
                            DrawWaveOptions(WaveOptionsDrawData.Wave4Names[1], 1);
                            DrawWaveOptions(WaveOptionsDrawData.Wave4Names[2], 2);
                            DrawWaveHorizontalOffset(WaveOptionsDrawData.Wave4Names[3], 3);
                        }
                    }
                }
            }
        }

    }
}
