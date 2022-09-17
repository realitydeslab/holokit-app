using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.Library.HoloKitApp.UI
{
    public class StARMainScene : Scene
    {
        public string _sceneName = "StARMain";
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
                _panelManager.Push(new StARModePanel());

            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            _panelManager.Pop();
        }

        private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode load)
        {
            _panelManager.Push(new StARModePanel());
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
