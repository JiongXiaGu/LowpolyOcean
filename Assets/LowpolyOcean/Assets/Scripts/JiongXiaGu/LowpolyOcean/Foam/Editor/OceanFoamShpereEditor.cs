using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace JiongXiaGu.LowpolyOcean
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(OceanFoamShpere))]
    public class OceanFoamShpereEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            OceanFoamShpere.EditorOnlyIsDisplyAll = EditorGUILayout.Toggle("AlwayDisplayInScene", OceanFoamShpere.EditorOnlyIsDisplyAll);
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
    }
}
