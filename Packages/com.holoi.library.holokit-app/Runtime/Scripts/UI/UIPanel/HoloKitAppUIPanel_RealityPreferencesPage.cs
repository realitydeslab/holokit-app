// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityPreferencesPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityPreferencesPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private Button[] _entranceButtons;

        [SerializeField] private GameObject _spacer;

        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;

            for (int i = 0; i < _entranceButtons.Length; i++)
            {
                if (currentReality.RealityEntranceOptions.Count <= i)
                {
                    _entranceButtons[i].gameObject.SetActive(false);
                    continue;
                }

                var entranceOption = currentReality.RealityEntranceOptions[i];
                var entranceButton = _entranceButtons[i];
                entranceButton.GetComponentInChildren<TMP_Text>().text = entranceOption.Text;
                entranceButton.onClick.AddListener(() =>
                {
                    HoloKitApp.Instance.EnterRealityAs(entranceOption.IsHost,
                        (HoloKitAppPlayerType)entranceOption.PlayerType,
                        entranceOption.PlayerTypeSubindex);
                });
            }

            if (HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList().Count == 0
                && HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaObjectCollectionList().Count == 0)
            {
                _spacer.SetActive(true);
            }
            else
            {
                _spacer.SetActive(false);
            }
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
