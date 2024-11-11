// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    public class MofaFightingPanelTime : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text _gameTime;

        private bool _isUpdating;

        private float _timeCounter = 80;

        private void Start()
        {
            _gameTime.text = Mathf.Ceil(((MofaBaseRealityManager) HoloKitApp.Instance.RealityManager).RoundDuration) + "s";
        }

        private void OnEnable()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Fighting)
            {
                _isUpdating = true;
                _timeCounter = ((MofaBaseRealityManager) HoloKitApp.Instance.RealityManager).RoundDuration;
            }
            else if (mofaPhase == MofaPhase.RoundOver)
            {
                _isUpdating = false;
                _gameTime.text = 0f + "s";
            }
        }

        private void Update()
        {
            if (!_isUpdating) { return; }

            if (_timeCounter > 0)
            {
                _timeCounter -= Time.deltaTime;
            }
            else
            {
                _timeCounter = 0;
            }
            _gameTime.text = Mathf.Ceil(_timeCounter) + "s";
        }
    }
}