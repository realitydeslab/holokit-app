using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class MofaPopupManager : MonoBehaviour
    {
        [SerializeField] private Transform _fightingPanelTransform;

        [SerializeField] private GameObject _countdownPrefab;

        [SerializeField] private GameObject _roundOverPrefab;

        [SerializeField] private GameObject _victoryPrefab;

        [SerializeField] private GameObject _defeatPrefab;

        [SerializeField] private GameObject _drawPrefab;

        [SerializeField] private GameObject _deathPrefab;

        [SerializeField] private GameObject _summaryBoardPrefab;

        private GameObject _currentPopup;

        private void Start()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedRoundResult += OnReceivedRoundResult;
            LifeShield.OnDead += OnLifeShieldDestroyed;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedRoundResult -= OnReceivedRoundResult;
            LifeShield.OnDead -= OnLifeShieldDestroyed;
        }

        private IEnumerator SpawnPopup(GameObject popupPrefab, float destroyDelay)
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
            _currentPopup.transform.SetParent(_fightingPanelTransform);
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
                    break;
                case MofaPhase.RoundData:
                    OnRoundData();
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
                StartCoroutine(SpawnPopup(_drawPrefab, 3f));
                return;
            }

            MofaTeam team = ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).GetPlayer().Team.Value;
            if (team == MofaTeam.Blue)
            {
                if (roundResult == MofaRoundResult.BlueTeamWins)
                {
                    StartCoroutine(SpawnPopup(_victoryPrefab, 3f));
                }
                else
                {
                    StartCoroutine(SpawnPopup(_defeatPrefab, 3f));
                }
            }
            else if (team == MofaTeam.Red)
            {
                if (roundResult == MofaRoundResult.RedTeamWins)
                {
                    StartCoroutine(SpawnPopup(_victoryPrefab, 3f));
                }
                else
                {
                    StartCoroutine(SpawnPopup(_defeatPrefab, 3f));
                }
            }
        }

        private void OnRoundData()
        {
            //StartCoroutine(SpawnPopup(_summaryBoardPrefab, 30f));
            // TODO: Display detailed data
        }

        private void OnLifeShieldDestroyed(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                StartCoroutine(SpawnPopup(_deathPrefab, 3f));
            }
        }
    }
}