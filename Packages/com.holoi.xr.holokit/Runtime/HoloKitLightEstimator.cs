using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloKit
{
    public class HoloKitLightEstimator : MonoBehaviour
    {
        [SerializeField] private ARCameraManager _arCameraManager;

        public float? AverageBrightness { get; private set; }

        public float? AverageColorTemperature { get; private set; }

        public float? AverageIntensityInLumens { get; private set; }

        private void OnEnable()
        {
            if (_arCameraManager != null)
            {
                _arCameraManager.requestedLightEstimation = LightEstimation.AmbientIntensity;
                _arCameraManager.frameReceived += OnFrameReceived;
            }
        }

        private void OnDisable()
        {
            if (_arCameraManager != null)
            {
                _arCameraManager.frameReceived -= OnFrameReceived;
            }
        }

        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                AverageBrightness = args.lightEstimation.averageBrightness.Value;
            }
            else
            {
                AverageBrightness = null;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                AverageColorTemperature = args.lightEstimation.averageColorTemperature.Value;
            }
            else
            {
                AverageColorTemperature = null;
            }

            if (args.lightEstimation.averageIntensityInLumens.HasValue)
            {
                AverageIntensityInLumens = args.lightEstimation.averageIntensityInLumens.Value;
            }
            else
            {
                AverageIntensityInLumens = null;
            }
        }
    }
}