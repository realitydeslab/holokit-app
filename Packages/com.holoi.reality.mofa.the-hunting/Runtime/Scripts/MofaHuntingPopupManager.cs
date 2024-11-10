// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFA.TheHunting
{
    public class MofaHuntingPopupManager : MofaPopupManager
    {
        protected override void OnRoundResult()
        {
            if (HoloKitApp.Instance.IsSpectator)
                return;

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
