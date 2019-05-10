using System;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{

    [DisallowMultipleComponent]
    public sealed class OceanFollowTarget : MonoBehaviour
    {
        private OceanFollowTarget()
        {
        }

        [SerializeField] private Transform target = null;
        [SerializeField] private float interval = 12f;

        private float Move(float value)
        {
            var result = (int)(value / interval) * interval;
            return result;
        }

        private void Start()
        {
            if (target == null)
            {
                enabled = false;
                Debug.LogError(new ArgumentNullException(nameof(target)), this);
            }
        }

        private void LateUpdate()
        {
            var pos = target.position;
            pos.x = Move(pos.x);
            pos.y = transform.position.y;
            pos.z = Move(pos.z);
            transform.position = pos;
        }
    }
}
