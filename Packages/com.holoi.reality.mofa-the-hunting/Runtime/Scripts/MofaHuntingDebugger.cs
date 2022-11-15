using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MOFATheHunting
{
    public class MofaHuntingDebugger : MonoBehaviour
    {
        public void OnAxisChanged(Vector2 value)
        {
            Debug.Log($"OnAxisChanged: {value}");
        }
    }
}
