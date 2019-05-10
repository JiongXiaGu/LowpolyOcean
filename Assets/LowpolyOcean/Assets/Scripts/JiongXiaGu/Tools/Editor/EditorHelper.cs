using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{



    public static class EditorHelper
    {
        public const string ScriptName = "m_Script";

        private static readonly Lazy<GUIStyle> activeButtonStyle = new Lazy<GUIStyle>(delegate ()
         {
             return new GUIStyle("Button")
             {
                 normal = new GUIStyle("Button").active,
             };
         });
        public static GUIStyle ActiveButtonStyle => activeButtonStyle.Value;

        private static readonly Lazy<GUIStyle> normalButtonStyle = new Lazy<GUIStyle>(delegate ()
        {
            return new GUIStyle("Button");
        });
        public static GUIStyle NormalButtonStyle => normalButtonStyle.Value;

        public static GUIStyle ToggleStyleButton(bool isActive)
        {
            return isActive ? activeButtonStyle.Value : normalButtonStyle.Value;
        }

        public static void AddOneLine(ref Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;
        }
    }
}
