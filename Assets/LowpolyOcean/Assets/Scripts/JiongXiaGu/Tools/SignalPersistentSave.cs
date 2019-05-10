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

    public class SignalPersistentSave
    {
        public string KeyName { get; }
        public List<string> Names { get; }
        public int Value { get; private set; }

        public SignalPersistentSave(string keyName)
        {
            KeyName = keyName;
            Names = new List<string>(32);
        }

        public bool this[int index]
        {
            get { return (Value & 1 << index) != 0; }
            set
            {
                if (value)
                    Value |= 1 << index;
                else
                    Value &= ~(1 << index);
            }
        }

        public int FindIndex(string name)
        {
            return Names.FindIndex(item => item == name);
        }

        public int CreateItem(string name)
        {
            if (Names.Count == 32)
                throw new InvalidOperationException();

            int index = FindIndex(name);
            if (index < 0)
            {
                Names.Add(name);
                return Names.Count - 1;
            }
            return index;
        }

        public void Save()
        {
            PlayerPrefs.SetInt(KeyName, Value);
        }

        public void Load()
        {
            Value = PlayerPrefs.GetInt(KeyName);
        }

#if UNITY_EDITOR
        public void SaveEditor()
        {
            EditorPrefs.SetInt(KeyName, Value);
        }

        public void LoadEditor()
        {
            Value = EditorPrefs.GetInt(KeyName);
        }
#endif
    }

}
