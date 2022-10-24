using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheDuel
{
    public class MofaDuelRealityManager : MofaBaseRealityManager
    {
        protected override void Start()
        {
            base.Start();

            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
        }

        private void OnTriggered()
        {
            GetPlayer().Ready.Value = true;
        }
    }
}
