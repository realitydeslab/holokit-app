using UnityEngine;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// This class is a reactor for adjustment in reality settings.
    /// Adjustment means translating, rotating and scaling.
    /// This class is a singleton.
    /// </summary>
    public class ARObjectAdjuster : MonoBehaviour
    {
        public static ARObjectAdjuster Instance { get { return _instance; } }

        private static ARObjectAdjuster _instance;

        [SerializeField] private bool _translation = true;

        [SerializeField] private bool _rotation = true;

        [SerializeField] private bool _scale = true;

        /// <summary>
        /// The AR object you want to adjust.
        /// </summary>
        [SerializeField] private Transform _arObject;

        public bool Translation => _translation;

        public bool Rotation => _rotation;

        public bool Scale => _scale;

        private const float TranslationSpeed = 0.002f;

        private const float RotationSpeed = 1.6f;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            if (_translation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnPositionChanged += OnPositionChanged;
            }
            if (_rotation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnRotationChanged += OnRotationChanged;
            }
            if (_scale)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnScaleChanged += OnScaleChanged;
            }
        }

        private void OnDestroy()
        {
            if (_translation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnPositionChanged -= OnPositionChanged;
            }
            if (_rotation)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnRotationChanged -= OnRotationChanged;
            }
            if (_scale)
            {
                HoloKitAppUIRealitySettingTab_Adjust.OnScaleChanged -= OnScaleChanged;
            }
        }

        public void SetARObject(Transform arObject)
        {
            _arObject = arObject;
        }

        private void OnPositionChanged(Vector2 offset)
        {
            if (_arObject == null) { return; }
            Vector3 forward = HoloKitCamera.Instance.CenterEyePose.forward;
            Vector3 horizontalForward = new(forward.x, 0f, forward.z);
            Vector3 right = HoloKitCamera.Instance.CenterEyePose.right;
            Vector3 horizontalRight = new(right.x, 0f, right.z);
            _arObject.position += TranslationSpeed * (offset.x * horizontalRight + offset.y * horizontalForward);
        }

        private void OnRotationChanged(float angle)
        {
            if (_arObject == null) { return; }
            _arObject.Rotate(0f, -RotationSpeed * angle, 0f);
        }

        private void OnScaleChanged(float factor)
        {
            if (_arObject == null) { return; }
            _arObject.localScale *= factor;
        }
    }
}
