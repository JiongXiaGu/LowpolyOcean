using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{

    [DisallowMultipleComponent]
    public class FlyCamera : MonoBehaviour
    {
        private FlyCamera()
        {
        }

        private const string InputMouseX = "Mouse X";
        private const string InputMouseY = "Mouse Y";
        private const string InputVertical = "Vertical";
        private const string InputHorizontal = "Horizontal";
        private const KeyCode InputUp = KeyCode.Q;
        private const KeyCode InputDown = KeyCode.E;
        public const KeyCode InputCursorFreeze = KeyCode.V;

        public bool IsFreezeCursor;
        public float RotateAmount = 120;
        public float HeightAmount = 10;
        public float MoveAmount = 10;

        private void Update()
        {
            if (Input.GetKeyDown(InputCursorFreeze))
            {
                IsFreezeCursor = !IsFreezeCursor;
                if (IsFreezeCursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (IsFreezeCursor)
            {
                var rotation = transform.localEulerAngles;
                rotation.x -= RotateAmount * Input.GetAxis(InputMouseY) * Time.deltaTime;
                rotation.y += RotateAmount * Input.GetAxis(InputMouseX) * Time.deltaTime;
                transform.localEulerAngles = rotation;

                Vector3 position = transform.position;
                position += transform.forward * MoveAmount * Input.GetAxis(InputVertical) * Time.deltaTime;
                position += transform.right * MoveAmount * Input.GetAxis(InputHorizontal) * Time.deltaTime;
                if (Input.GetKey(InputUp))
                {
                    position.y += HeightAmount * Time.deltaTime;
                }
                else if (Input.GetKey(InputDown))
                {
                    position.y -= HeightAmount * Time.deltaTime;
                }
                transform.position = position;
            }
        }
    }
}
