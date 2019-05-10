using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{


    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextureGenerator))]
    public class TextureGeneratorDrawer : Editor
    {
        private SerializedProperty UseShderType;
        private SerializedProperty Material;
        private SerializedProperty Shader;
        private SerializedProperty UsePass;
        private SerializedProperty ComputeShader;
        private SerializedProperty Parameters;
        private SerializedProperty Target;
        private SerializedProperty TextureSize;
        private SerializedProperty TextureFormat;

        private void OnEnable()
        {
            UseShderType = serializedObject.FindProperty(nameof(TextureGenerator.UseShderType));
            Material = serializedObject.FindProperty(nameof(TextureGenerator.Material));
            Shader = serializedObject.FindProperty(nameof(TextureGenerator.Shader));
            UsePass = serializedObject.FindProperty(nameof(TextureGenerator.UsePass));
            ComputeShader = serializedObject.FindProperty(nameof(TextureGenerator.ComputeShader));
            Parameters = serializedObject.FindProperty(nameof(TextureGenerator.Parameters));
            Target = serializedObject.FindProperty(nameof(TextureGenerator.Target));
            TextureSize = serializedObject.FindProperty(nameof(TextureGenerator.TextureSize));
            TextureFormat = serializedObject.FindProperty(nameof(TextureGenerator.TextureFormat));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var target = this.target as TextureGenerator;

            EditorGUILayout.PropertyField(UseShderType);

            switch (UseShderType.intValue)
            {
                case (int)TextureGenerator.GenerateForm.Material:
                    EditorGUILayout.PropertyField(Material);
                    EditorGUILayout.PropertyField(UsePass);
                    break;

                case (int)TextureGenerator.GenerateForm.Shader:
                    EditorGUILayout.PropertyField(Shader);
                    EditorGUILayout.PropertyField(UsePass);
                    break;

                case (int)TextureGenerator.GenerateForm.ComputeShader:
                    EditorGUILayout.PropertyField(ComputeShader);
                    break;
            }

            EditorGUILayout.PropertyField(Parameters);

            EditorGUILayout.PropertyField(Target);
            if (Target.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(TextureSize);
                EditorGUILayout.PropertyField(TextureFormat);
            }

            if (GUILayout.Button(nameof(target.Write)))
            {
                target.Write();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
