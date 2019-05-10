using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LowpolyOcean.LowpolyOcean
{

    [DisallowMultipleComponent]
    public class DemoController : MonoBehaviour
    {

        public Text sceneNameText = null;
        public List<string> sceneNames = null;
        public int currentSceneIndex = 0;

        private void SetSceneName(string name)
        {
            if (sceneNameText != null)
            {
                sceneNameText.text = "Scene : " + name;
            }
        }

        private void PreviousScene()
        {
            currentSceneIndex--;

            if (currentSceneIndex < 0)
            {
                currentSceneIndex = sceneNames.Count - 1;
            }

            string name = sceneNames[currentSceneIndex];
            SceneManager.LoadScene(name);
            SetSceneName(name);
        }

        private void NextScene()
        {
            currentSceneIndex++;

            if (currentSceneIndex >= sceneNames.Count)
            {
                currentSceneIndex = 0;
            }

            string name = sceneNames[currentSceneIndex];
            SceneManager.LoadScene(name);
            SetSceneName(name);
        }

        private static bool sceneHas;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (!sceneHas)
            {
                sceneHas = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetSceneName(sceneNames[currentSceneIndex]);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                PreviousScene();
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                NextScene();
            }
        }

    }
}
