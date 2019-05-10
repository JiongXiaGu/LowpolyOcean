using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanVolume))]
    public class OceanVolume : MonoBehaviour
    {
        protected OceanVolume()
        {
        }

        private static readonly List<OceanVolume> activated = new List<OceanVolume>();
        private static IReadOnlyCollection<OceanVolume> Activated => activated;

        [Header("Ocean")]
        [SerializeField] private float height = 0;
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        [SerializeField] private Vector3 normal = Vector3.up;
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        [Header("Under ocean")]
        [SerializeField] private bool enableUnderOcean = false;
        public bool EnableUnderOcean
        {
            get { return enableUnderOcean; }
            set { enableUnderOcean = value; }
        }

        [SerializeField] private UnderOceanDataBase underOceanData = null;
        public UnderOceanDataBase UnderOceanData
        {
            get { return underOceanData; }
            set { underOceanData = value; }
        }

        private void OnEnable()
        {
            activated.Add(this);
        }

        private void OnDisable()
        {
            activated.Remove(this);
        }

        public static bool TryGet(OceanCameraTask oceanCamera, out OceanVolume result)
        {
            if (activated.Count != 0)
            {
                result = activated[0];
                return true;
            }

            result = default;
            return false;
        }
    }
}
