// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealitySlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _realityId;

        [SerializeField] private TMP_Text _realityName;

        [SerializeField] private Button[] _entranceButtons;

        public void Init(Reality reality, int index)
        {
            _realityId.text = "Reality #" + HoloKitAppUtils.IntToStringF3(index);
            _realityName.text = reality.DisplayName;

            for (int i = 0; i < reality.RealityEntranceOptions.Count; i++)
            {
                var entraceOption = reality.RealityEntranceOptions[i];
                var button = _entranceButtons[i];

                button.GetComponentInChildren<TMP_Text>().text = entraceOption.Text;
                button.onClick.AddListener(() =>
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.EnterRealityAs(entraceOption.IsHost,
                        (HoloKitAppPlayerType)(int)entraceOption.PlayerType,
                        entraceOption.PlayerTypeSubindex);
                });
            }
        }
    }
}
