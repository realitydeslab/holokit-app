using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.Library.HoloKitApp.UI
{
    public class StartScene : Scene
    {
        readonly string _sceneName = "Start";
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
                _panelManager.Push(new StartPanel());
            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode load)
        {
            //Debug.Log($"{_panelManager._panelStack.Count} panel before recover");
            _panelManager.RecoverPanel();
            //Debug.Log($"{_panelManager._panelStack.Count} panel after recover");
            Debug.Log($"{_sceneName} scene is loaded.");
        }
    }
}
