using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.HoloKit.App.UI
{
    public class StARMainScene : Scene
    {
        readonly string _sceneName = "StARMain";
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
                _panelManager.Push(new StARModePanel());

            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            _panelManager.PopAll();
            Debug.Log($"exit scene, pop all panel with {_panelManager._panelStack.Count} left");
        }

        private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode load)
        {
            _panelManager.Push(new StARModePanel());
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
