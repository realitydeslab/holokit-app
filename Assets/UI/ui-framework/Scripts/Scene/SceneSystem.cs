using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App.UI
{
    public class SceneSystem
    {
        Scene _scene;

        public void SetScene(Scene scene)
        {
            //Debug.Log("scene exit");
            _scene?.OnExit();
            _scene = scene;
            //Debug.Log("scene enter");
            _scene?.OnEnter();
        }
    }
}

