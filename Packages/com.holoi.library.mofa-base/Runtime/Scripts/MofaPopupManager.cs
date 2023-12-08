// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using Unity.Netcode;
using HoloKit;

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

        [SerializeField] private GameObject _deathCircleEnemyPrefab;

        [SerializeField] private GameObject _summaryBoardPrefab;

        private GameObject _currentPopup;

        private const float PopupStarScale = 0.68f;

        protected virtual void Start()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;
            LifeShield.OnBeingDestroyed += OnLifeShieldBeingDestroyed;
        }

        protected virtual void OnDestroy()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
            LifeShield.OnBeingDestroyed -= OnLifeShieldBeingDestroyed;
        }

        protected void SpawnPopup(GameObject popupPrefab)
        {
            if (popupPrefab == null)
                return;
            if (_currentPopup != null)
                Destroy(_currentPopup);

            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(_fightingPanelRoot);
            _currentPopup.transform.localPosition = Vector3.zero;
            _currentPopup.transform.localRotation = Quaternion.identity;
            float scale = HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono ? 1f : PopupStarScale;
            _currentPopup.transform.localScale = new Vector3(scale, scale, scale);
        }

        protected IEnumerator SpawnPopupAndDestroy(GameObject popupPrefab, float destroyDelay)
        {
            if (popupPrefab == null)
                yield return null;
            if (_currentPopup != null)
                Destroy(_currentPopup);

            _currentPopup = Instantiate(popupPrefab);
            _currentPopup.transform.SetParent(_fightingPanelRoot);
            _currentPopup.transform.localPosition = Vector3.zero;
            _currentPopup.transform.localRotation = Quaternion.identity;
            float scale = HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono ? 1f : PopupStarScale;
            _currentPopup.transform.localScale = new Vector3(scale, scale, scale);

            yield return new WaitForSeconds(destroyDelay);
            if (_currentPopup != null)
                Destroy(_currentPopup);
        }

        protected virtual void OnMofaPhaseChanged(MofaPhase mofaPhase)
        {
            switch (mofaPhase)
            {
                case MofaPhase.Waiting:
                    OnSummaryBoard();
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
                    OnRoundResult();
                    break;
            }
        }

        protected virtual void OnRoundResult()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
                return;

            var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var localMofaPlayer = mofaBaseRealityManager.LocalMofaPlayer;
            // Get the personal round result for the local player
            var personalRoundResult = mofaBaseRealityManager.GetPersonalRoundResult(localMofaPlayer);
            switch (personalRoundResult)
            {
                case MofaPersonalRoundResult.Victory:
                    SpawnVictoryPopup();
                    break;
                case MofaPersonalRoundResult.Defeat:
                    SpawnDefeatPopup();
                    break;
                case MofaPersonalRoundResult.Draw:
                    SpawnDrawPopup();
                    break;
            }
        }

        protected void SpawnVictoryPopup()
        {
            StartCoroutine(SpawnPopupAndDestroy(_victoryPrefab, 3f));
        }

        protected void SpawnDefeatPopup()
        {
            StartCoroutine(SpawnPopupAndDestroy(_defeatPrefab, 3f));
        }

        protected void SpawnDrawPopup()
        {
            StartCoroutine(SpawnPopupAndDestroy(_drawPrefab, 3f));
        }

        protected virtual void OnSummaryBoard()
        {
            SpawnPopup(_summaryBoardPrefab);
        }

        private void OnLifeShieldBeingDestroyed(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                StartCoroutine(SpawnPopupAndDestroy(_deathPrefab, 3f));
            }
            else
            {
                var deathCircleEnemyInstance = Instantiate(_deathCircleEnemyPrefab);

                var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                Vector3 position = mofaBaseRealityManager.MofaPlayerDict[ownerClientId].LifeShield.transform.position;
                deathCircleEnemyInstance.transform.position = position;
            }
        }
    }
}