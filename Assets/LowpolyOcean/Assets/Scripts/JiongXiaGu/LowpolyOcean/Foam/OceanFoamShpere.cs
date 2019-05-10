using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Display foam in position
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanFoamShpere))]
    public sealed class OceanFoamShpere : MonoBehaviour, IFoamShpere
    {
        private OceanFoamShpere()
        {
        }

        public static bool EditorOnlyIsDisplyAll { get; set; }

        [SerializeField] private float radius = 5;
        [SerializeField] [Range(0, 10)] private float intensity = 1;
        private IDisposable unsubscriber;
        public Vector3 Position => transform.position;
        float IFoamShpere.Radius => radius * GetRadiusScaleFactor(transform.lossyScale);

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        private void OnEnable()
        {
            unsubscriber = FoamShpereSystem.Current.Subscribe(this);
        }

        private void OnDisable()
        {
            unsubscriber.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius * GetRadiusScaleFactor(transform.lossyScale));
        }

        private void OnDrawGizmos()
        {
            if (EditorOnlyIsDisplyAll)
            {
                OnDrawGizmosSelected();
            }
        }

        public static float GetRadiusScaleFactor(Vector3 lossyScale)
        {
            float num = 0f;
            num = Mathf.Max(num, Mathf.Abs(lossyScale.x));
            num = Mathf.Max(num, Mathf.Abs(lossyScale.y));
            num = Mathf.Max(num, Mathf.Abs(lossyScale.z));
            return num;
        }
    }
}
