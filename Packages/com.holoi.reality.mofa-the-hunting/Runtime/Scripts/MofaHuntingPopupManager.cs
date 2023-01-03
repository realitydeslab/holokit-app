using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
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
