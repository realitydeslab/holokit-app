// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using HoloKit;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase.Test.UI
{
    public class Test_UIManager_UI : MonoBehaviour
    {
        [SerializeField] private GameObject _monoCanvas;

        [SerializeField] private GameObject _starCanvas;

        [SerializeField] private GameObject _starUIPanelPrefab;

        [SerializeField] private GameObject _wall;

        [SerializeField] private GameObject _armor;

        private GameObject _starUIPanel;

        private void Start()
        {
            OnHoloKitRenderModeChanged(HoloKitRenderMode.Mono);
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void Update()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Mono)
            {
                _monoCanvas.SetActive(true);
                _starCanvas.SetActive(false);

                if (_starUIPanel != null)
                    Destroy(_starUIPanel);
            }
            else if (renderMode == HoloKitRenderMode.Stereo)
            {
                _monoCanvas.SetActive(false);
                _starCanvas.SetActive(true);

                _starUIPanel = Instantiate(_starUIPanelPrefab, _starCanvas.transform);
                _starUIPanel.transform.localPosition = Vector3.zero;
                _starUIPanel.transform.localRotation = Quaternion.identity;
                _starUIPanel.transform.localScale = Vector3.one;

                var masks = FindObjectsOfType<Mask>();
                foreach (var mask in masks)
                {
                    mask.enabled = true;
                }
            }
        }

        public void OnStarButtonPressed()
        {
            HoloKitCameraManager.Instance.OpenStereoWithoutNFC("SomethingForNothing");
        }

        public void OnTriggerButtonPressed()
        {
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
        }

        public void OnBoostButtonPressed()
        {
            HoloKitAppUIEventManager.OnStarUIBoosted?.Invoke();
        }

        public void OnWallToggleChanged(bool toggle)
        {
            _wall.SetActive(toggle);
        }

        public void OnArmorToggleChanged(bool toggle)
        {
            _armor.SetActive(toggle);
        }
    }
}
