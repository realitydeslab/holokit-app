using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class MofaPopupManager : MonoBehaviour
    {
        [SerializeField] private Transform _fightingPanelRoot;

        [SerializeField] private GameObject _countdownPrefab;

        [SerializeField] private GameObject _roundOverPrefab;

        [SerializeField] private GameObject _victoryPrefab;

        [SerializeField] private GameObject _defeatPrefab;

        [SerializeField] private GameObject _drawPrefab;

        [SerializeField] private GameObject _deathPrefab;

        [SerializeField] private GameObject _summaryBoardPrefab;

        private GameObject _currentPopup;

        protected virtual void Start()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedRoundResult += OnReceivedRoundResult;
            LifeShield.OnBeingDestroyed += OnLifeShieldBeingDestroyed;
            MofaPlayer.OnHealthDataUpdated += UpdateSummaryBoard;
        }

        protected virtual void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedRoundResult -= OnReceivedRoundResult;
            LifeShield.OnBeingDestroyed -= OnLifeShieldBeingDestroyed;
            MofaPlayer.OnHealthDataUpdated -= UpdateSummaryBoard;
        }

        protected void SpawnPopup(GameObject popupPrefab)
        {
            if (popupPrefab == null)
            {
                return;
            }
            if (_currentPopup != null)
            {
                Destroy(_currentPopup);
            }
            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(_fightingPanelRoot);
            _currentPopup.transform.localPosition = Vector3.zero;
            _currentPopup.transform.localRotation = Quaternion.identity;
            _currentPopup.transform.localScale = Vector3.one;
        }

        protected IEnumerator SpawnPopupAndDestroy(GameObject popupPrefab, float destroyDelay)
        {
            if (popupPrefab == null)
            {
                yield return null;
            }
            if (_currentPopup != null)
            {
                Destroy(_currentPopup);
            }
            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(_fightingPanelRoot);
            _currentPopup.transform.localPosition = Vector3.zero;
            _currentPopup.transform.localRotation = Quaternion.identity;
            _currentPopup.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(destroyDelay);
            if (_currentPopup != null)
            {
                Destroy(_currentPopup);
            }
        }

        protected virtual void OnPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    StartCoroutine(SpawnPopupAndDestroy(_countdownPrefab, 4f));
                    break;
                case MofaPhase.Fighting:
                    break;
                case MofaPhase.RoundOver:
                    StartCoroutine(SpawnPopupAndDestroy(_roundOverPrefab, 4f));
                    break;
                case MofaPhase.RoundResult:
                    break;
                case MofaPhase.RoundData:
                    UpdateSummaryBoard();
                    break;
            }
        }

        private void OnReceivedRoundResult(MofaRoundResult roundResult)
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
            {
                // TODO: Play blue team wins or red team wins on spectator
                return;
            }

            if (roundResult == MofaRoundResult.Draw)
            {
                StartCoroutine(SpawnPopupAndDestroy(_drawPrefab, 3f));
                return;
            }

            MofaTeam team = ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).GetPlayer().Team.Value;
            if (team == MofaTeam.Blue)
            {
                if (roundResult == MofaRoundResult.BlueTeamWins)
                {
                    StartCoroutine(SpawnPopupAndDestroy(_victoryPrefab, 3f));
                }
                else
                {
                    StartCoroutine(SpawnPopupAndDestroy(_defeatPrefab, 3f));
                }
            }
            else if (team == MofaTeam.Red)
            {
                if (roundResult == MofaRoundResult.RedTeamWins)
                {
                    StartCoroutine(SpawnPopupAndDestroy(_victoryPrefab, 3f));
                }
                else
                {
                    StartCoroutine(SpawnPopupAndDestroy(_defeatPrefab, 3f));
                }
            }
        }

        protected SummaryBoard SpawnSummaryBoard()
        {
            SpawnPopup(_summaryBoardPrefab);
            return _currentPopup.GetComponent<SummaryBoard>();
        }

        protected virtual void UpdateSummaryBoard()
        {
            var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            if (mofaBaseRealityManager.CurrentPhase != MofaPhase.RoundData) return;
        }

        private void OnLifeShieldBeingDestroyed(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                StartCoroutine(SpawnPopupAndDestroy(_deathPrefab, 3f));
            }
        }
    }
}