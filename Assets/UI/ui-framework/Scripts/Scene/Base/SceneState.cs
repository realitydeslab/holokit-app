using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Holoi.HoloKit.App.UI
{
    public abstract class SceneState
    {
        public abstract void OnEnter();
        public abstract void OnExit();

    }

}