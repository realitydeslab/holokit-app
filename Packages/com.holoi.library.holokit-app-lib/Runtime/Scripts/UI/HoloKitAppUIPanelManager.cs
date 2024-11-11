// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public enum HoloKitAppUICanvasType
    {
        Portrait = 0,
        Landscape = 1,
        StAR = 2
    }

    public class HoloKitAppUIPanelManager : MonoBehaviour
    {
        public List<HoloKitAppUIPanel> UIPanelList;

        [SerializeField] private Canvas _portraitCanvas;

        [SerializeField] private Canvas _landscapeCanvas;

        [SerializeField] private Canvas _starCanvas;

        private readonly Stack<HoloKitAppUIPanel> _uiPanelStack = new();

        public void PushUIPanel(string uiPanelName, HoloKitAppUICanvasType canvasType = HoloKitAppUICanvasType.Portrait)
        {
            foreach (var uiPanel in UIPanelList)
            {
                if (uiPanel.UIPanelName.Equals(uiPanelName))
                {
                    // This is the UIPanel we are looking for
                    PushUIPanel(uiPanel, canvasType);
                    return;
                }
            }
            Debug.LogError($"[HoloKitAppUIPanelManager] Cannot find UIPanel with name {uiPanelName}");
        }

        public void PushUIPanel(HoloKitAppUIPanel uiPanel, HoloKitAppUICanvasType canvasType = HoloKitAppUICanvasType.Portrait)
        {
            var uiPanelInstance = Instantiate(uiPanel);
            switch (canvasType)
            {
                case HoloKitAppUICanvasType.Portrait:
                    uiPanelInstance.transform.SetParent(_portraitCanvas.transform);
                    break;
                case HoloKitAppUICanvasType.Landscape:
                    uiPanelInstance.transform.SetParent(_landscapeCanvas.transform);
                    break;
                case HoloKitAppUICanvasType.StAR:
                    uiPanelInstance.transform.SetParent(_starCanvas.transform);
                    break;
            }
            uiPanelInstance.transform.localPosition = Vector3.zero;
            uiPanelInstance.transform.localRotation = Quaternion.identity;
            uiPanelInstance.transform.localScale = Vector3.one;
            // Ensure the left, right, top and bottom values of the RectTransform are zero
            var rectTransform = uiPanelInstance.GetComponent<RectTransform>();
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

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
            if (_uiPanelStack.Peek().UIPanelName.Equals("LandingPage"))
            {
                return;
            }

            while (!_uiPanelStack.Peek().UIPanelName.Equals("MonoAR") && !_uiPanelStack.Peek().UIPanelName.Equals("MonoAR_NoLiDAR"))
            {
                PopUIPanel();
            }
            // Pop MonoAR panel or MonoAR_NoLiDAR panel
            PopUIPanel();

            if (HoloKitApp.Instance.GlobalSettings.AppConfig.GalleryViewEnabled)
            {
                // Pop Reality preferences page panel
                PopUIPanel();
            }
        }
    }
}
