using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaPopupManager : MonoBehaviour
    {
        [SerializeField] private GameObject _countdownPrefab;

        [SerializeField] private GameObject _roundOverPrefab;

        [SerializeField] private GameObject _victoryPrefab;

        [SerializeField] private GameObject _defeatPrefab;

        [SerializeField] private GameObject _drawPrefab;

        [SerializeField] private GameObject _deathPrefab;

        [SerializeField] private GameObject _summaryBoardPrefab;

        private GameObject _currentPopup;

        private MofaFightingPanel _mofaFightingPanel;

        private void Awake()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;

            _mofaFightingPanel = HoloKitCamera.Instance.CenterEyePose.GetComponentInChildren<MofaFightingPanel>();
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        private IEnumerator SpawnPopup(GameObject popupPrefab, float destroyDelay)
        {
            if (_currentPopup != null)
            {
                Destroy(_currentPopup);
            }
            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(_mofaFightingPanel.transform);
            _currentPopup.transform.localPosition = Vector3.zero;
            _currentPopup.transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(destroyDelay);
            if (_currentPopup != null)
            {
                Destroy(_currentPopup);
            }
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    StartCoroutine(SpawnPopup(_countdownPrefab, 4f));
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    StartCoroutine(SpawnPopup(_roundOverPrefab, 4f));
                    break;
                case MofaPhase.RoundResult:
                    OnRoundResult();
                    break;
                case MofaPhase.RoundData:
                    OnRoundData();
                    break;
            }
        }

        private void OnRoundResult()
        {
            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var localPlayer = mofaRealityManager.GetLocalPlayer();
            if (localPlayer == null)
            {
                return;
            }
            var roundResult = mofaRealityManager.GetRoundResult();
            switch (roundResult)
            {
                case MofaRoundResult.BlueTeamWins:
                    if (localPlayer.Team.Value == MofaTeam.Blue)
                    {
                        StartCoroutine(SpawnPopup(_victoryPrefab, 3f));
                    }
                    else
                    {
                        StartCoroutine(SpawnPopup(_defeatPrefab, 3f));
                    }
                    break;
                case MofaRoundResult.RedTeamWins:
                    if (localPlayer.Team.Value == MofaTeam.Blue)
                    {
                        StartCoroutine(SpawnPopup(_defeatPrefab, 3f));
                    }
                    else
                    {
                        StartCoroutine(SpawnPopup(_victoryPrefab, 3f));
                    }
                    break;
                case MofaRoundResult.Draw:
                    StartCoroutine(SpawnPopup(_drawPrefab, 3f));
                    break;
            }
        }

        private void OnRoundData()
        {
            StartCoroutine(SpawnPopup(_summaryBoardPrefab, 30f));
        }
    }
}