using System;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{

    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public sealed class EnumMaskDrawer : PropertyDrawer
    {
        private Array valueMap;
        private string[] enumNames;

        private static readonly string errorText = string.Format("[{0}] only work at 'Enum' type", nameof(EnumMaskAttribute));

        private void Init(Array array)
        {
            enumNames = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                enumNames[i] = array.GetValue(i).ToString();
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = fieldInfo.FieldType;

            if (typeof(Enum).IsAssignableFrom(type))
            {
                if (valueMap == null)
                {
                    valueMap = Enum.GetValues(type);
                    Init(valueMap);
                }

                int mask = 0;
                for (int i = 0; i < enumNames.Length; i++)
                {
                    int currentMask = 1 << i;
                    var target = (int)valueMap.GetValue(i);
                    if ((property.intValue & target) == target)
                    {
                        mask |= currentMask;
                    }
                }

                mask = EditorGUI.MaskField(position, label, mask, enumNames);
                if (mask == ~0)
                {
                    property.intValue = mask;
                    return;
                }

                int result = 0;
                for (int i = 0; i < enumNames.Length; i++)
                {
                    int currentMask = 1 << i;
                    var target = (int)valueMap.GetValue(i);
                    if ((mask & currentMask) != 0)
                    {
                        result |= target;
                    }
                    else
                    {
                        result &= ~target;
                    }
                }

                property.intValue = result;
            }
            else
            {
                EditorGUI.LabelField(position, property.displayName, errorText);
            }
        }
    }
}
