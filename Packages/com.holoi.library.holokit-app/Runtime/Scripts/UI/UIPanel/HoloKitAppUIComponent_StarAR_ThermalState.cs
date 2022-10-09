using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_ThermalState : MonoBehaviour
    {
        [SerializeField] private TMP_Text _thermalStateText;

        private readonly Color32 _normalColor = Color.blue;

        private readonly Color32 _fairColor = Color.green;

        private readonly Color32 _seriousColor = Color.yellow;

        private readonly Color32 _criticalColor = Color.red;

        private void OnEnable()
        {
            if (HoloKitHelper.IsRuntime)
            {
                OnThermalStateChanged(HoloKitARSessionControllerAPI.GetThermalState());
                HoloKitARSessionControllerAPI.OnThermalStateChanged += OnThermalStateChanged;
            }
        }

        private void OnDisable()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitARSessionControllerAPI.OnThermalStateChanged -= OnThermalStateChanged;
            }
        }

        private void OnThermalStateChanged(ThermalState thermalState)
        {
            switch (thermalState)
            {
                case ThermalState.ThermalStateNominal:
                    _thermalStateText.text = "Normal";
                    _thermalStateText.color = _normalColor;
                    break;
                case ThermalState.ThermalStateFair:
                    _thermalStateText.text = "Fair";
                    _thermalStateText.color = _fairColor;
                    break;
                case ThermalState.ThermalStateSerious:
                    _thermalStateText.text = "Serious";
                    _thermalStateText.color = _seriousColor;
                    break;
                case ThermalState.ThermalStateCritical:
                    _thermalStateText.text = "Critical";
                    _thermalStateText.color = _criticalColor;
                    break;
            }
        }
    }
}
