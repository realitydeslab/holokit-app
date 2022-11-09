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

        /// <summary>
        /// The AR object you want to adjust.
        /// </summary>
        [SerializeField] private Transform _arObject;

        private const float TranslationSpeed = 0.2f;

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
            HoloKitAppUIRealitySettingTab_Adjust.OnDragPositionChanged += OnDragPositionChanged;
        }

        private void OnDestroy()
        {
            HoloKitAppUIRealitySettingTab_Adjust.OnDragPositionChanged -= OnDragPositionChanged;
        }

        public void SetARObject(Transform arObject)
        {
            _arObject = arObject;
        }

        private void OnDragPositionChanged(Vector2 offset)
        {
            if (_arObject == null) { return; }
            Vector3 forward = HoloKitCamera.Instance.CenterEyePose.forward;
            Vector3 horizontalForward = new(forward.x, 0f, forward.z);
            Vector3 right = HoloKitCamera.Instance.CenterEyePose.right;
            Vector3 horizontalRight = new(right.x, 0f, right.z);
            _arObject.position += TranslationSpeed * Time.deltaTime * (offset.x * horizontalRight + offset.y * horizontalForward);
        }
    }
}
