using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JiongXiaGu.LowpolyOcean
{

    public static class EditorModeHelper
    {
        public const string AssetMenuNameRoot = nameof(LowpolyOcean) + "/";
        public const string AddComponentMenuNameRoot = nameof(LowpolyOcean) + "/";

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void Check()
        {
            if (SystemInfo.graphicsShaderLevel < 30)
            {
                Debug.LogWarning("LowpolyOcean does not support the platform of SystemInfo.graphicsShaderLevel < 30");
            }
        }
    }
}
