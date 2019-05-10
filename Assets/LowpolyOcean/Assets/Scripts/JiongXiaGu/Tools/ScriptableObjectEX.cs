using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JiongXiaGu
{

    /// <summary>
    /// Extension methods
    /// </summary>
    public class ScriptableObjectEX : ScriptableObject
    {
        protected ScriptableObjectEX()
        {
        }

        public static string CopyValue { get; private set; }
        public static UnityEngine.Object CopyFrom { get; private set; }

        public static void Copy(UnityEngine.Object sender)
        {
            CopyValue = JsonUtility.ToJson(sender);
            CopyFrom = sender;
        }

        [ContextMenu(nameof(Copy))]
        private void Copy()
        {
            Copy(this);
        }

        private bool TypeCheck(UnityEngine.Object sender)
        {
            return sender.GetType().IsAssignableFrom(CopyFrom.GetType());
        }

        protected void Paste(UnityEngine.Object target)
        {
            JsonUtility.FromJsonOverwrite(CopyValue, target);
        }

        protected void Paste(UnityEngine.Object target, Action beforePaste, Action afterPaste)
        {
#if UNITY_EDITOR
            if (!TypeCheck(this))
            {
                if (!EditorUtility.DisplayDialog("Warning!", "Different types, are you sure to paste?", "Yes", "No"))
                    return;
            }
            Undo.RecordObject(this, "Paste");
#endif
            beforePaste?.Invoke();
            Paste(this);
            afterPaste?.Invoke();
        }

        [ContextMenu(nameof(Paste))]
        private void Paste()
        {
            Paste(this, null, null);
        }
    }
}
