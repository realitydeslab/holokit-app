// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFA.TheHunting
{
    public class MofaHuntingPopupManager : MofaPopupManager
    {
        protected override void OnRoundResult()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                SpawnDefeatPopup();
            }
            else if (HoloKitApp.Instance.IsPlayer)
            {
                SpawnVictoryPopup();
            }
        }

        protected override void OnSummaryBoard()
        {
            
        }
    }
}
