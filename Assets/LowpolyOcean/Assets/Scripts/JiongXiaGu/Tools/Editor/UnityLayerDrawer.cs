using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{

    [CustomPropertyDrawer(typeof(UnityLayer))]
    public sealed class UnityLayerDrawer : PropertyDrawer
    {
        private SerializedProperty layer;

        private void Init(SerializedProperty property)
        {
            if (layer == null)
            {
                layer = property.FindPropertyRelative(nameof(UnityLayer.Layer));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);
            layer.intValue = EditorGUI.LayerField(position, label, layer.intValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
