using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARMainScene : Scene
    {
        readonly string _sceneName = "ScreenARMain";
        PanelManager _panelManager;
        public override void OnEnter()
        {
            _panelManager = PanelManager.Instance;
            if (SceneManager.GetActiveScene().name != _sceneName)
            {
                SceneManager.LoadScene(_sceneName);
                SceneManager.sceneLoaded += SceneLoaded;
            }
            else
            {
                _panelManager.Push(new ScreenARModePanel());
            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            _panelManager.Pop();
        }

        private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode load)
        {
            _panelManager.Push(new ScreenARModePanel());
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
