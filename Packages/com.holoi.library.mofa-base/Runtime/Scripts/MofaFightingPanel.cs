using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanel : MonoBehaviour
    {
        public GameObject Scores;

        public GameObject Time;

        public GameObject Reticle;

        public GameObject Status;

        public GameObject RedScreen;

        private void Awake()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        private void Start()
        {
            Scores.SetActive(false);
            Time.SetActive(false);
            Reticle.SetActive(false);
            Status.SetActive(false);
            RedScreen.SetActive(false);
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
            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            if (mofaRealityManager.IsLocalPlayerSpectator()) // Spectator
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
    }
}