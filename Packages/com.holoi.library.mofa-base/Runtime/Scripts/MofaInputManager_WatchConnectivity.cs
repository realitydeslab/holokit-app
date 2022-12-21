using UnityEngine;
using Holoi.Library.HoloKitApp.WatchConnectivity;
using Holoi.Library.HoloKitApp.WatchConnectivity.MOFA;

namespace Holoi.Library.MOFABase
{
    /// <summary>
    /// This is the part of the MofaInputManager class controlling MOFA WatchConnectivity.
    /// </summary>
    public partial class MofaInputManager : MonoBehaviour
    {
        public MofaWatchState CurrentWatchState;

        private void InitializeWatchConnectivity()
        {
            MofaWatchConnectivityAPI.OnReceivedStartRoundMessage += OnReceivedRoundMessage;
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnWatchTriggered;
            MofaWatchConnectivityAPI.OnReceivedHealthDataMessage += OnReceivedHealthDataMessage;
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;

            HoloKitAppWatchConnectivityAPI.UpdateWatchPanel(HoloKitWatchPanel.MOFA);
            MofaWatchConnectivityAPI.Initialize();
            MofaWatchConnectivityAPI.UpdateMagicSchool(int.Parse(HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId));
        }

        private void DeinitializeWatchConnectivity()
        {
            MofaWatchConnectivityAPI.OnReceivedStartRoundMessage -= OnReceivedRoundMessage;
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered -= OnWatchTriggered;
            MofaWatchConnectivityAPI.OnReceivedHealthDataMessage -= OnReceivedHealthDataMessage;
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
        }

        private void OnReceivedRoundMessage()
        {
            if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Waiting)
                _mofaBaseRealityManager.TryGetReady();
            else if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Fighting)
                MofaWatchConnectivityAPI.OnRoundStarted();
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            CurrentWatchState = watchState;
        }

        private void OnWatchTriggered()
        {
            TryCastSpell();
        }

        private void OnReceivedHealthDataMessage(float distance, float energy)
        {
            var localMofaPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
            localMofaPlayer.UpdateHealthDataServerRpc(distance, energy);
        }

        private void OnMofaPhaseChanged(MofaPhase newPhase)
        {
            if (newPhase == MofaPhase.Countdown)
            {
                MofaWatchConnectivityAPI.OnRoundStarted();
                return;
            }

            //if (newPhase == MofaPhase.Fighting)
            //{
            //    MofaWatchConnectivityAPI.QueryWatchState();
            //    return;
            //}

            if (newPhase == MofaPhase.RoundResult)
            {
                var localPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
                var localPlayerStats = _mofaBaseRealityManager.GetPlayerStats(localPlayer);
                MofaWatchConnectivityAPI.OnRoundEnded((int)localPlayerStats.PersonalRoundResult,
                                                           localPlayerStats.Kill,
                                                           localPlayerStats.HitRate);
                return;
            }
        }
    }
}
