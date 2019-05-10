using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{


    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }



        private void LateUpdate()
        {
            var pos = target.position;
            transform.position = pos;

        }
    }
}
