using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.UnityTools
{

    [CustomPropertyDrawer(typeof(TextureGenerator.Parameter))]
    public class TextureGeneratorParameterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(TextureGenerator.Parameter.name)));

                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(TextureGenerator.Parameter.format)));

                var valuePoroperty = GetValueProperty(property);
                if (valuePoroperty != null)
                {
                    position.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(position, valuePoroperty, true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight * 2;
            var valuePoroperty = GetValueProperty(property);
            if (valuePoroperty != null)
            {
                height += EditorGUI.GetPropertyHeight(valuePoroperty);
            }
            return height;
        }

        /// <summary>
        /// 获取到参数变量;
        /// </summary>
        private SerializedProperty GetValueProperty(SerializedProperty property)
        {
            TextureGenerator.ParameterFormat format = (TextureGenerator.ParameterFormat)property.FindPropertyRelative(nameof(TextureGenerator.Parameter.format)).intValue;
            switch (format)
            {
                case TextureGenerator.ParameterFormat.Int:
                    return property.FindPropertyRelative(nameof(TextureGenerator.Parameter.intValue));

                case TextureGenerator.ParameterFormat.Float:
                    return property.FindPropertyRelative(nameof(TextureGenerator.Parameter.floatValue));

                case TextureGenerator.ParameterFormat.Color:
                    return property.FindPropertyRelative(nameof(TextureGenerator.Parameter.color));

                case TextureGenerator.ParameterFormat.ColorValue:
                    return property.FindPropertyRelative(nameof(TextureGenerator.Parameter.colorValue));

                case TextureGenerator.ParameterFormat.Texture:
                    return property.FindPropertyRelative(nameof(TextureGenerator.Parameter.texture));

                default:
                    return null;
            }
        }
    }
}
