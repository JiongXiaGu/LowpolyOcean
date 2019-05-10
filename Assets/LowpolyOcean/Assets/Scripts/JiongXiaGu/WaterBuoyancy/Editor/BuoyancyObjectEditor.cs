//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEditor;

//namespace JiongXiaGu.OceanBuoyancy
//{

//    [CustomEditor(typeof(BuoyancyObject))]
//    public sealed class BuoyancyObjectEditor : Editor
//    {
//        private BuoyancyObject Target => target as BuoyancyObject;
//        private Rigidbody rigidbody;

//        private void OnEnable()
//        {
//            rigidbody = Target.GetComponent<Rigidbody>();
//        }

//        private void OnDisable()
//        {
//            rigidbody.hideFlags = HideFlags.None;
//        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            if (Application.isPlaying)
//            {
//                rigidbody.hideFlags = HideFlags.NotEditable;
//            }
//        }
//    }
//}
