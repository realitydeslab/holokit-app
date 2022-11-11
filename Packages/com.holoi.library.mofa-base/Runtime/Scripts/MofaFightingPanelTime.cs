using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelTime : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text _gameTime;

        private bool _isUpdating;

        private float _timeCounter = 80;

        private void Start()
        {
            _gameTime.text = Mathf.Ceil(((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).RoundTime) + "s";
        }

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
                _isUpdating = true;
                _timeCounter = ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).RoundTime;
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