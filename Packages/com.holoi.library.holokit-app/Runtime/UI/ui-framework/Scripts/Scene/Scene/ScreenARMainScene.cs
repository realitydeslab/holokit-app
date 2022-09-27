using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARMainScene : Scene
    {
        public string _sceneName = "ScreenARMain";
        public enum State
        {
            screenAR = 0,
            spectator = 1
        }
        public State openState = State.screenAR;

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
            switch (openState)
            {
                case State.screenAR:
                    _panelManager.Push(new ScreenARModePanel());
                    break;
                case State.spectator:
                    _panelManager.Push(new ScreenARModePanel());
                    _panelManager.Push(new ScreenARWaitPlayerPanel());
                    break;
            }
        }
    }
}
