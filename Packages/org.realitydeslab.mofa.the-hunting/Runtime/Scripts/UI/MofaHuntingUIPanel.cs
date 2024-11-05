// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using RealityDesignLab.Library.HoloKitApp;
using RealityDesignLab.Library.HoloKitApp.UI;
using org.realitydeslab.MOFABase.UI;
using HoloKit;

namespace RealityDesignLab.MOFA.TheHunting.UI
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
