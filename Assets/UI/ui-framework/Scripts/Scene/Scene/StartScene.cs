using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.HoloKit.App.UI
{
    public class StartScene : SceneState
    {
        readonly string _sceneName = "Start";
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
            _panelManager.Push(new StartPanel());
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
