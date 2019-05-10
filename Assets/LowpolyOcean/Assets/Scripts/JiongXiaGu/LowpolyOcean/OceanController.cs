using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// use to initialize ocean setting, only one instance is allow in scene
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu(EditorModeHelper.AddComponentMenuNameRoot + nameof(OceanController))]
    public class OceanController : UnitySingleton<OceanController>
    {
        protected OceanController()
        {
        }

        [SerializeField]
        private ProjectSettings projectSettings = null;
        public ProjectSettings ProjectSettings => projectSettings;

        private void Awake()
        {
            projectSettings?.SetToCurrentSettings();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnValidate()
        {
            projectSettings?.SetToCurrentSettings();
        }
    }
}
