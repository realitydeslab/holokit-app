using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UIFramwork
{
    public abstract class SceneState
    {
        public abstract void OnEnter();
        public abstract void OnExit();

    }

}