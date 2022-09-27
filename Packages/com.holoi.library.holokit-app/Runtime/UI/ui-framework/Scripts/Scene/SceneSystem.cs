using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class SceneSystem
    {
        Scene _scene;

        public void SetScene(Scene scene)
        {
            _scene?.OnExit();
            _scene = scene;
            _scene?.OnEnter();
        }
    }
}

