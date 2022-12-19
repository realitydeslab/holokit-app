using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase
{
    public partial class MofaInputManager : MonoBehaviour
    {
        private void InitializeStarUI()
        {
            HoloKitAppUIEventManager.OnStarUITriggered += OnStarUITriggered;
            HoloKitAppUIEventManager.OnStarUIBoosted += OnStarUIBoosted;
        }

        private void DeinitializeStarUI()
        {
            HoloKitAppUIEventManager.OnStarUITriggered -= OnStarUITriggered;
            HoloKitAppUIEventManager.OnStarUIBoosted -= OnStarUIBoosted;
        }

        private void OnStarUITriggered()
        {
            if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Fighting)
                TryCastBasicSpell();
            else if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Waiting)
                _mofaBaseRealityManager.TryGetReady();
        }

        private void OnStarUIBoosted()
        {
            TryCastSecondarySpell();
        }
    }
}
