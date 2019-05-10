using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{

    [DisallowMultipleComponent]
    public class FollowCamera : MonoBehaviour
    {

        [SerializeField] private Transform target = null;
        [SerializeField] private float speed = 1;
        [SerializeField] private Vector3 relativePosition;


        private void Start()
        {
            relativePosition = target.InverseTransformPoint(transform.position);
        }

        private void LateUpdate()
        {
            var targetPos = target.TransformPoint(relativePosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
            transform.LookAt(target);
        }
    }
}
