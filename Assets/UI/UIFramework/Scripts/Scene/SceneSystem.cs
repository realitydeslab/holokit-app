using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramwork
{
    public class SceneSystem
    {
        SceneState _sceneState;

        public void SetScene(SceneState state)
        {
            _sceneState?.OnEnter();
            _sceneState = state;
            _sceneState?.OnEnter();
        }
    }
}

