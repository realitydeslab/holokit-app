// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase.UI;
using HoloKit;

namespace Holoi.Reality.MOFA.TheHunting.UI
{
    public class MofaHuntingUIPanel : MofaUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private HoloKitAppUIPanel _dragonControllerUIPanel;

        protected override void Start()
        {
            HoloKitCameraManager.Instance.ForceScreenOrientation = !HoloKitApp.Instance.IsHost;

            if (HoloKitApp.Instance.IsHost)
                HoloKitApp.Instance.UIPanelManager.PushUIPanel(_dragonControllerUIPanel, HoloKitAppUICanvasType.Landscape);
            else if (HoloKitApp.Instance.IsSpectator)
                TriggerButton.gameObject.SetActive(false);
        }
    }
}
