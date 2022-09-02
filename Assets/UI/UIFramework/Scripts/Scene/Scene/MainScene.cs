using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIFramwork
{
    public class MainScene : SceneState
    {
        readonly string _sceneName = "Main";
        PanelManager _panelManager;
        public override void OnEnter()
        {
            _panelManager = new PanelManager();
            if (SceneManager.GetActiveScene().name != _sceneName)
            {
                SceneManager.LoadScene(_sceneName);
                SceneManager.sceneLoaded += SceneLoaded;
            }
            else
            {
                _panelManager.Push(new StartPanel());

            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode load)
        {
            _panelManager.Push(new MainPanel());
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
