using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanelTime : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text _gameTime;

        float _timeCounter = 80;

        private void OnEnable()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Fighting)
            {
                // SIZHENGTODO: 计时开始，80s一局
                _timeCounter = 80;
            }
        }

        private void Update()
        {
            if (_timeCounter > 0)
            {
                _timeCounter -= Time.deltaTime;
            }
            else
            {
                _timeCounter = 0;
            }
            _gameTime.text = "" + Mathf.Ceil(_timeCounter);
        }
    }
}