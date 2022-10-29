using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityDetailPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityDetailPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RenderTexture _roomViewRenderTexture;

        [SerializeField] private GameObject _roomListRootPrefab;

        [SerializeField] private GameObject _defaultRoomPrefab;

        [SerializeField] private RectTransform _touchArea;

        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private TMP_Text _realityIndexText;

        [SerializeField] private TMP_Text _realityNameText;

        [SerializeField] private TMP_Text _realityAuthorText;

        [SerializeField] private TMP_Text _realityDescriptionText;

        private GameObject _roomListRoot;

        private GameObject _room;

        private bool _isTouching;

        private Vector2 _lastTouchPosition;

        private float _accumulatedRotationX;

        private float _accumulatedRotationY;

        private const float RoomRotationSpeed = 0.3f;

        private const float CamaraOrthographicSize = 10f;

        private readonly Vector3 RoomCenterOffset = new(0f, 3f, 0f);

        /// <summary>
        /// The local position of camera relative to room center.
        /// </summary>
        private readonly Vector3 RoomCenterToCameraOffsetPosition = new(-10f, 12f, -8.8f);

        private readonly Vector3 RotationAxisY = new(1f, 0f, -1f);

        private void OnEnable()
        {
            _roomListRoot = Instantiate(_roomListRootPrefab);
            if (HoloKitApp.Instance.CurrentReality.ThumbnailPrefab == null)
            {
                _room = Instantiate(_defaultRoomPrefab);
            }
            else
            {
                _room = Instantiate(HoloKitApp.Instance.CurrentReality.ThumbnailPrefab);
            }
            _room.transform.SetParent(_roomListRoot.transform);
            _room.transform.localPosition = Vector3.zero;
            _room.transform.localRotation = Quaternion.identity;
            _room.transform.localScale = Vector3.one;
            Camera.main.orthographicSize = CamaraOrthographicSize;
            Camera.main.transform.position = RoomCenterToCameraOffsetPosition;
            // Camera always looks at the room
            Camera.main.transform.LookAt(RoomCenterOffset);
            Camera.main.targetTexture = _roomViewRenderTexture;

            _accumulatedRotationX = 0f;
            _accumulatedRotationY = 0f;
        }

        private void OnDisable()
        {
            Destroy(_roomListRoot);
            if (Camera.main != null)
            {
                Camera.main.targetTexture = null;
            }
        }

        private void Start()
        {
            _realityIndexText.text = "Reality #" +
                HoloKitAppUtils.IntToStringF3(HoloKitApp.Instance.GlobalSettings.GetRealityIndex(HoloKitApp.Instance.CurrentReality) + 1);
            _realityNameText.text = HoloKitApp.Instance.CurrentReality.DisplayName;
            _realityAuthorText.text = HoloKitApp.Instance.CurrentReality.Author;
            _realityDescriptionText.text = HoloKitApp.Instance.CurrentReality.Description;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (IsInsideInputArea(touch.position))
                    {
                        _scrollRect.vertical = false;
                        _isTouching = true;
                        _lastTouchPosition = touch.position;
                    }
                    return;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _scrollRect.vertical = true;
                    _isTouching = false;
                    return;
                }

                if (_isTouching && IsInsideInputArea(touch.position))
                {
                    Vector3 fingerMovement = touch.position - _lastTouchPosition;
                    float rotationX = fingerMovement.x * RoomRotationSpeed * Time.deltaTime;
                    _accumulatedRotationX += rotationX;
                    if (_accumulatedRotationX > -30f && _accumulatedRotationX < 30f)
                    {
                        Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, rotationX);
                        //_room.transform.Rotate(RotationAxisX, rotationX);
                    }
                    else
                    {
                        if (_accumulatedRotationX < 0f)
                        {
                            _accumulatedRotationX = -30f;
                        }
                        else
                        {
                            _accumulatedRotationX = 30f;
                        }
                    }

                    float rotationY = -fingerMovement.y * RoomRotationSpeed * Time.deltaTime;
                    _accumulatedRotationY += rotationY;
                    if (_accumulatedRotationY > -30f && _accumulatedRotationY < 30f)
                    {
                        Camera.main.transform.RotateAround(Vector3.zero, RotationAxisY, rotationY);
                    }
                    else
                    {
                        if (_accumulatedRotationY < 0f)
                        {
                            _accumulatedRotationY = -30f;
                        }
                        else
                        {
                            _accumulatedRotationY = 30f;
                        }
                    }
                }
            }
        }

        private bool IsInsideInputArea(Vector3 position)
        {
            if (position.x > (_touchArea.position.x - _touchArea.sizeDelta.x / 2f)
                && position.x < (_touchArea.position.x + _touchArea.sizeDelta.x / 2f)
                &&
                position.y > (_touchArea.position.y - _touchArea.sizeDelta.y / 2f)
                && position.y < (_touchArea.position.y + _touchArea.sizeDelta.y / 2f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
