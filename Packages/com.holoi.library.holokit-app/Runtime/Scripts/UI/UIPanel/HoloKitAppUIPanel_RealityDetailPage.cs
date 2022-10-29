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

        private const float RoomListRootScale = 1.9f;

        private const float RoomRotationSpeed = 0.1f;

        private readonly Vector3 RotationAxisX = new(0f, 1f, 0f);

        private readonly Vector3 RotationAxisY = new(1f, 0f, -1f);

        private void OnEnable()
        {
            _roomListRoot = Instantiate(_roomListRootPrefab);
            _roomListRoot.transform.localScale = new Vector3(RoomListRootScale, RoomListRootScale, RoomListRootScale);
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
            if (Camera.main == null)
            {
                Debug.Log("No main camera found");
            }
            Camera.main.transform.SetPositionAndRotation(HoloKitAppUIPanel_RealityListPage.RoomCenterToCameraOffsetPosition,
                                                         Quaternion.Euler(HoloKitAppUIPanel_RealityListPage.RoomCenterToCameraOffsteEulerRotation));
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

                    float rotationX = -fingerMovement.x * RoomRotationSpeed * Time.deltaTime;
                    _accumulatedRotationX += rotationX;
                    if (_accumulatedRotationX > -30f && _accumulatedRotationX < 30f)
                    {
                        _room.transform.Rotate(RotationAxisX, rotationX);
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

                    float rotationY = fingerMovement.y * RoomRotationSpeed * Time.deltaTime;
                    _accumulatedRotationY += rotationY;
                    if (_accumulatedRotationY > -30f && _accumulatedRotationY < 30f)
                    {
                        _room.transform.Rotate(RotationAxisY, rotationY);
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
