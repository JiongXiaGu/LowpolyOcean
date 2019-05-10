using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [CustomEditor(typeof(BuoyancyData))]
    public class BuoyancyDataEditor : Editor
    {

        public bool IsDisplayHandles
        {
            get { return BuoyancyData.editorOnlyIsDisplayHandles; }
            set { BuoyancyData.editorOnlyIsDisplayHandles = value; }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if( GUILayout.Button("DisplayAllHandles", EditorHelper.ToggleStyleButton(IsDisplayHandles)))
            {
                IsDisplayHandles = !IsDisplayHandles;
            }
        }
    }
}
