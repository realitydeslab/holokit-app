using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanel : MonoBehaviour
    {
        public GameObject Scores;

        public GameObject Time;

        public GameObject Reticle;

        public GameObject Status;

        public GameObject RedScreen;

        private void Start()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            HoloKitAppUIEventManager.OnStartedRecording += OnStartedRecording;
            HoloKitAppUIEventManager.OnStoppedRecording += OnStoppedRecording;

            Scores.SetActive(false);
            Time.SetActive(false);
            Reticle.SetActive(false);
            Status.SetActive(false);
            RedScreen.SetActive(false);
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            HoloKitAppUIEventManager.OnStartedRecording -= OnStartedRecording;
            HoloKitAppUIEventManager.OnStoppedRecording -= OnStoppedRecording;
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    OnCountdown();
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    break;
                case MofaPhase.RoundResult:
                    break;
                case MofaPhase.RoundData:
                    break;
            }
        }

        private void OnCountdown()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator) // Spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
            }
            else // Not spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
                Reticle.SetActive(true);
                Status.SetActive(true);
                RedScreen.SetActive(true);
            }
        }

        private void OnStartedRecording()
        {
            Scores.SetActive(false);
            Time.SetActive(false);
            Reticle.SetActive(false);
            Status.SetActive(false);
            RedScreen.SetActive(false);
        }

        private void OnStoppedRecording()
        {
            OnCountdown();
        }
    }
}