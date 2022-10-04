using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanelManager : MonoBehaviour
    {
        public static HoloKitAppUIPanelManager Instance { get { return _instance; } }

        private static HoloKitAppUIPanelManager _instance;

        public List<HoloKitAppUIPanel> UIPanelList;

        public string InitialScene;

        private readonly Stack<HoloKitAppUIPanel> _uiPanelStack = new();

        private Canvas _canvas;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            PushUIPanel(InitialScene);
        }

        public void PushUIPanel(string uiPanelName)
        {
            foreach (var uiPanel in UIPanelList)
            {
                if (uiPanel.UIPanelName.Equals(uiPanelName))
                {
                    // This is the UIPanel we are looking for
                    PushUIPanel(uiPanel);
                    return;
                }
            }
        }

        private void PushUIPanel(HoloKitAppUIPanel uiPanel)
        {
            var uiPanelInstance = Instantiate(uiPanel);
            uiPanelInstance.transform.SetParent(_canvas.transform);
            uiPanelInstance.transform.localPosition = Vector3.zero;
            uiPanelInstance.transform.localRotation = Quaternion.identity;

            // Inactivate the previous UIPanels
            if (_uiPanelStack.TryPeek(out var previousUIPanel)) {
                previousUIPanel.gameObject.SetActive(false);
            }
            _uiPanelStack.Push(uiPanelInstance);
        }

        public void PopUIPanel()
        {
            if (_uiPanelStack.TryPop(out var lastUIPanel))
            {
                Destroy(lastUIPanel.gameObject);
                if (_uiPanelStack.TryPeek(out var secondLastUIPanel))
                {
                    secondLastUIPanel.gameObject.SetActive(true);
                }
            }
        }

        public void OnARSceneUnloaded()
        {
            if (_uiPanelStack.Peek().UIPanelName.Equals("StarAR"))
            {
                PopUIPanel();
            }
            PopUIPanel();
        }
    }
}
