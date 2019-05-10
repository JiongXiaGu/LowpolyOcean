using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JiongXiaGu
{

    [CustomPropertyDrawer(typeof(CustomReorderableList), true)]
    public sealed class CustomReorderableListDrawer : PropertyDrawer
    {
        private bool isDrawable = true;
        private ReorderableList reorderableList;
        private string headerName;

        private void Expand(bool isExpanded)
        {
            var array = reorderableList.serializedProperty;
            for (int i = 0; i < array.arraySize; i++)
            {
                var sp = array.GetArrayElementAtIndex(i);
                sp.isExpanded = isExpanded;
            }
        }

        private void DrawHeader(Rect rect)
        {
            rect.width -= rect.height * 3;
            GUI.Label(rect, headerName);

            rect.x += rect.width;
            rect.width = rect.height;
            if (GUI.Button(rect, "+"))
            {
                Expand(true);
            }

            rect.x += rect.height;
            if (GUI.Button(rect, "-"))
            {
                Expand(false);
            }

            rect.x += rect.height;
            if (GUI.Button(rect, "c") && EditorUtility.DisplayDialog("Warning!", "Are you sure you want to clear list?", "Yes", "No"))
            {
                reorderableList.serializedProperty.ClearArray();
            }
        }

        private void DrawOptionData(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            RectOffset rectOffset = new RectOffset(0, 0, -2, -2);
            rect = rectOffset.Add(rect);
            rect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUI.PropertyField(rect, element, true);
            }
        }

        private float ElementHeightCallback(int index)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            float height = 0;

            height += EditorGUI.GetPropertyHeight(element, true);
            return height + 4;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }

        private bool TryInit(SerializedProperty property)
        {
            if (reorderableList != null)
            {
                return true;
            }
            else if (isDrawable)
            {
                SerializedProperty elements = property.FindPropertyRelative(CustomReorderableList.ListFieldName);
                if (elements == null)
                {
                    Debug.LogWarning("not found " + CustomReorderableList.ListFieldName);
                    return isDrawable = false;
                }

                var reorderableList = new ReorderableList(property.serializedObject, elements);
                headerName = property.displayName;
                reorderableList.drawHeaderCallback = DrawHeader;
                reorderableList.drawElementCallback = DrawOptionData;
                reorderableList.elementHeightCallback = ElementHeightCallback;
                reorderableList.onRemoveCallback = OnRemoveElement;
                this.reorderableList = reorderableList;

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (TryInit(property))
            {
                reorderableList.DoList(position);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (TryInit(property))
            {
                return reorderableList.GetHeight();
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }
    }
}
