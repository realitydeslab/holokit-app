using UnityEngine;
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
            if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
            {
                GetPlayer().Ready.Value = true;
                Debug.Log("Local player is ready");
            }
        }
    }
}
