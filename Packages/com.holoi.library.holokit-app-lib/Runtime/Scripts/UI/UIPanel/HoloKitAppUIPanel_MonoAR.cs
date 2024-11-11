// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIPanel_MonoAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _spectatorButton;

        [SerializeField] private GameObject _starButton;

        [SerializeField] private GameObject _exitButton;

        [SerializeField] private GameObject _starModeHelper;

        [SerializeField] private HoloKitAppUIComponent_MonoAR_RecordButton _recordButton;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // Disable the spectator button on the host if spectator view is not supported in this reality
                if (!HoloKitApp.Instance.CurrentReality.IsSpectatorViewSupported())
                    _spectatorButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                // If the local player is not master, start browsing and try to establish connection.
                if (HoloKitUtils.IsRuntime)
                    HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_WaitingForConnection");
            } 
        }

        public void OnSpectatorButtonPressed()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ShareQRCode");
            }
            else
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_RescanQRCode");
            }
        }

        public void OnStarButtonPressed()
        {
            // Is the device supported by HoloKitX?
            if (!HoloKitOpticsAPI.IsCurrentDeviceSupportedByHoloKit())
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_IncompatiblePhoneModel");
                return;
            }

            // Enter Star Mode
            if (HoloKitApp.Instance.GlobalSettings.InstructionEnabled && HoloKitUtils.IsRuntime)
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_GettingStarted");
            }
            else
            {
                HoloKitApp.Instance.UIPanelManager.PushUIPanel("StarAR", HoloKitAppUICanvasType.StAR);
                HoloKitCameraManager.Instance.OpenStereoWithoutNFC("SomethingForNothing");
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.Shutdown();
        }

        public void OnRecordButtonPressed()
        {
            _recordButton.ToggleRecording();
            if (_recordButton.IsRecording)
            {
                _spectatorButton.SetActive(false);
                _starButton.SetActive(false);
                _exitButton.SetActive(false);
                _starModeHelper.SetActive(false);
            }
            else
            {
                _spectatorButton.SetActive(true);
                _starButton.SetActive(true);
                _exitButton.SetActive(true);
            }
        }

        public void OnHamburgerButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_RealitySettings");
        }
    }
}
