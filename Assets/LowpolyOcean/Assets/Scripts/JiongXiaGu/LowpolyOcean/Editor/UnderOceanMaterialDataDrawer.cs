using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace JiongXiaGu.LowpolyOcean
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnderOceanMaterialData))]
    public class UnderOceanMaterialDataDrawer : Editor
    {
        private UnderOceanMaterialDataDrawer()
        {
        }

        private SerializedProperty script;
        private SerializedProperty modeObject;
        private SerializedObject modeSerializedObject;
        private SerializedProperty modeOptions;
        private SerializedProperty dataOptions;
        private SerializedPropertyDrawer modeDrawer;
        private SerializedPropertyDrawer dataDrawer;

        private void CreateModeOptions()
        {
            if (modeObject.objectReferenceValue != null)
            {
                modeSerializedObject = new SerializedObject(modeObject.objectReferenceValue);
                modeOptions = modeSerializedObject.FindProperty("mode");
                modeDrawer = new SerializedPropertyDrawer(UnderOceanModeOptions.Accessor, modeOptions);
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
            dataDrawer = new SerializedPropertyDrawer(UnderMaterialOptions.Accessor, dataOptions);
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
                dataDrawer.OnGUI(mask);
            }
            else
            {
                dataDrawer.OnGUI(~0);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
