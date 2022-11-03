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

        public void PushUIPanel(HoloKitAppUIPanel uiPanel)
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

            // Deactivate all previous UIPanels
            if (uiPanel.OverlayPreviousPanel)
            {
                foreach (var previousPanel in _uiPanelStack)
                {
                    previousPanel.gameObject.SetActive(false);
                }
            }
            _uiPanelStack.Push(uiPanelInstance);
        }

        public void PopUIPanel()
        {
            // Firstly, we pop and destroy the last UI panel
            if (_uiPanelStack.TryPop(out var lastUIPanel))
            {
                Destroy(lastUIPanel.gameObject);

                var tempStack = new Stack<HoloKitAppUIPanel>();
                if (_uiPanelStack.TryPop(out var uiPanel1))
                {
                    uiPanel1.gameObject.SetActive(true);
                    tempStack.Push(uiPanel1);

                    while (!uiPanel1.OverlayPreviousPanel && _uiPanelStack.TryPop(out var uiPanel2))
                    {
                        uiPanel1 = uiPanel2;
                        uiPanel1.gameObject.SetActive(true);
                        tempStack.Push(uiPanel1);
                    }

                    while (tempStack.TryPop(out var uiPanel))
                    {
                        _uiPanelStack.Push(uiPanel);
                    }
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

            while (!_uiPanelStack.Peek().UIPanelName.Equals("MonoAR") && !_uiPanelStack.Peek().UIPanelName.Equals("MonoAR_NoLiDAR"))
            {
                PopUIPanel();
            }
            // Pop MonoAR panel or MonoAR_NoLiDAR panel
            PopUIPanel();

            if (!HoloKitApp.Instance.Test)
            {
                // Pop Reality preferences page panel
                PopUIPanel();
            }
        }
    }
}
