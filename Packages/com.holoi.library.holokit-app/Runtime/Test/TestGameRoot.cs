using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class TestGameRoot : MonoBehaviour
    {
        public static TestGameRoot Instance { get; private set; }
        public SceneSystem SceneSystem { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            SceneSystem = new SceneSystem();
            DontDestroyOnLoad(this.gameObject);
        }
        private void Start()
        {
            SceneSystem.SetScene(new TestScene());
        }
    }
}
