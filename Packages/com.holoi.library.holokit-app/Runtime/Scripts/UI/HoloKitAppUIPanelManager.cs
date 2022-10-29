using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanelManager : MonoBehaviour
    {
        public List<HoloKitAppUIPanel> UIPanelList;

        [SerializeField] private Canvas _portraitCanvas;

        [SerializeField] private Canvas _starCanvas;

        private readonly Stack<HoloKitAppUIPanel> _uiPanelStack = new();

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
            Debug.LogError($"[HoloKitAppUIPanelManager] Cannot find UIPanel with name {uiPanelName}");
        }

        private void PushUIPanel(HoloKitAppUIPanel uiPanel)
        {
            var uiPanelInstance = Instantiate(uiPanel);
            if (uiPanel.UIPanelName.Equals("StarAR"))
            {
                uiPanelInstance.transform.SetParent(_starCanvas.transform);
            }
            else
            {
                uiPanelInstance.transform.SetParent(_portraitCanvas.transform);
            }
            uiPanelInstance.transform.localPosition = Vector3.zero;
            uiPanelInstance.transform.localRotation = Quaternion.identity;
            uiPanelInstance.transform.localScale = Vector3.one;

            // Inactivate the previous UIPanel
            if (uiPanel.OverlayPreviousPanel && _uiPanelStack.TryPeek(out var previousUIPanel)) {
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

        public string PeekUIPanel()
        {
            if (_uiPanelStack.TryPeek(out var lastUIPanel))
            {
                return lastUIPanel.UIPanelName;
            }
            return null;
        }

        public void OnStartSceneLoaded()
        {
            // If this is the first time entering the App?
            if (HoloKitApp.Instance.Test)
            {
                if (_uiPanelStack.Peek().UIPanelName.Equals("TestRealityList"))
                {
                    return;
                }
            }
            else
            {
                if (_uiPanelStack.Peek().UIPanelName.Equals("LandingPage"))
                {
                    return;
                }
            }

            while (!_uiPanelStack.Peek().UIPanelName.Equals("MonoAR"))
            {
                PopUIPanel();
            }
            // Pop MonoAR panel
            PopUIPanel();

            if (!HoloKitApp.Instance.Test)
            {
                // Pop Reality preferences page panel
                PopUIPanel();
            }
        }
    }
}
