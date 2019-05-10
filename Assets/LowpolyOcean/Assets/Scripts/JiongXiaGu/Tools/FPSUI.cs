using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JiongXiaGu
{

    [DisallowMultipleComponent]
    public sealed class FPSUI : MonoBehaviour
    {
        private FPSUI()
        {
        }

        [SerializeField] private Text textObject = null;
        private float deltaTime;
        public float FPS { get; private set; } = 999;

        private void Start()
        {
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (true)
            {
                textObject.text = Mathf.Ceil(FPS).ToString();
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        private void Update()
        {
            if (textObject != null)
            {
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
                FPS = 1f / deltaTime;
            }
        }
    }
}
