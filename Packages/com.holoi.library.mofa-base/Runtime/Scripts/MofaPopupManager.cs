using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Mofa.Base
{
    public class MofaPopupManager : MonoBehaviour
    {
        [SerializeField] private Vector3 _centerEyeOffset;

        [SerializeField] private GameObject _countdownPrefab;

        [SerializeField] private GameObject _roundOverPrefab;

        [SerializeField] private GameObject _victoryPrefab;

        [SerializeField] private GameObject _defeatPrefab;

        [SerializeField] private GameObject _deathPrefab;

        private GameObject _currentPopup;

        private void Awake()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        private IEnumerator SpawnPopup(GameObject popupPrefab, float destroyDelay)
        {
            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(HoloKitCamera.Instance.CenterEyePose);
            _currentPopup.transform.localPosition = _centerEyeOffset;
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
                    break;
                case MofaPhase.RoundResult:

                    break;
                case MofaPhase.RoundData:
                    break;
            }
        }
    }
}