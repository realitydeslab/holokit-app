using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityListPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityListPage";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private TMP_Text _realityIndexText;

        [SerializeField] private TMP_Text _realityNameText;

        /// <summary>
        /// The root object for all subsequently spawned rooms.
        /// </summary>
        [SerializeField] private GameObject _roomListRootPrefab;

        /// <summary>
        /// This prefab is used when there is no thumbnail prefab provided by Reality.
        /// </summary>
        [SerializeField] private GameObject _defaultRoomPrefab;

        /// <summary>
        /// The spot light for each room.
        /// </summary>
        [SerializeField] private GameObject _roomLightPrefab;

        /// <summary>
        /// Only taps inside this area are effective.
        /// </summary>
        [SerializeField] private RectTransform _inputArea;

        /// <summary>
        /// Keeps a reference of this to destroy later.
        /// </summary>
        private GameObject _roomListRoot;

        /// <summary>
        /// Keeps a reference of this to destroy later.
        /// </summary>
        private GameObject _roomLight;

        /// <summary>
        /// The index of the currently selected room.
        /// </summary>
        private int _currentRoomIndex = 0;

        private Vector2 _touchBeganPosition;

        private Vector3 _cameraTargetPosition;

        /// <summary>
        /// The distance between two rooms.
        /// </summary>
        private const float RoomSpacingDist = 13.5f;

        private const float CameraMovementSpeed = 32f;

        /// <summary>
        /// The UI will only respond to scroll when finger movement magnitude is larger than this value.
        /// </summary>
        private const float FingerMovementScrollThreshold = 160f;

        /// <summary>
        /// The UI will only respond to click when finger movement magnitude is smaller than this value.
        /// </summary>
        private const float FingerMovementClickThreshold = 20f;

        /// <summary>
        /// The local position of camera relative to room center.
        /// </summary>
        private readonly Vector3 RoomCenterToCameraOffsetPosition = new(-10f, 18f, -8.8f);

        /// <summary>
        /// The local rotation in Euler of camera relative to room center.
        /// </summary>
        private readonly Vector3 RoomCenterToCameraOffsteEulerRotation = new(48f, 48f, 0f);

        // The list of spawned rooms.
        private readonly List<GameObject> RoomList = new();

        // We dynamically spawn and destroy reality rooms to save resources.
        private void OnEnable()
        {
            _roomListRoot = Instantiate(_roomListRootPrefab);
            int realityIndex = -1;
            foreach (var reality in HoloKitApp.Instance.GlobalSettings.RealityList.List)
            {
                realityIndex++;
                GameObject roomInstance;
                if (reality.ThumbnailPrefab != null)
                {
                    roomInstance = Instantiate(reality.ThumbnailPrefab);
                }
                else
                {
                    roomInstance = Instantiate(_defaultRoomPrefab);
                }
                RoomList.Add(roomInstance);
                roomInstance.transform.SetParent(_roomListRoot.transform);
                roomInstance.transform.localPosition = new Vector3(realityIndex * RoomSpacingDist, 0f, 0f);
                roomInstance.transform.localRotation = Quaternion.identity;
                roomInstance.transform.localScale = Vector3.one;
            }
            Vector3 currentRoomPosition = new(_currentRoomIndex * RoomSpacingDist, 0f, 0f);
            Camera.main.transform.SetPositionAndRotation(currentRoomPosition + RoomCenterToCameraOffsetPosition, Quaternion.Euler(RoomCenterToCameraOffsteEulerRotation));
            _cameraTargetPosition = Camera.main.transform.position;
            OnTargetRoomArrived();
        }

        private void OnDisable()
        {
            Destroy(_roomListRoot);
        }

        private void Update()
        {
            if (Vector3.Distance(Camera.main.transform.position, _cameraTargetPosition) > CameraMovementSpeed * Time.deltaTime)
            {
                Camera.main.transform.position += CameraMovementSpeed * Time.deltaTime * (_cameraTargetPosition - Camera.main.transform.position).normalized;
                return;
            }
            else
            {
                Camera.main.transform.position = _cameraTargetPosition;
                OnTargetRoomArrived();
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Began && touch.phase != TouchPhase.Ended)
                {
                    return;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    _touchBeganPosition = touch.position;

                    // TODO: Light the current room

                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Vector3 fingerMovementVector = touch.position - _touchBeganPosition;
                    if (fingerMovementVector.magnitude > FingerMovementScrollThreshold)
                    {
                        if (fingerMovementVector.x > 0f && fingerMovementVector.y > 0f)
                        {
                            // Back to last room
                            ScrollToLastRoom();
                        }
                        else if (fingerMovementVector.x < 0f && fingerMovementVector.y < 0f)
                        {
                            // Go to next room
                            ScrollToNextRoom();
                        }
                    }
                    else if (fingerMovementVector.magnitude < FingerMovementClickThreshold)
                    {
                        if (!IsInsideInputArea(_touchBeganPosition))
                        {
                            return;
                        }
                        // Enter the current room
                        HoloKitApp.Instance.CurrentReality = HoloKitApp.Instance.GlobalSettings.RealityList.List[_currentRoomIndex];
                        HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityDetailPage");
                    }
                }
            }
        }

        private bool IsInsideInputArea(Vector3 position)
        {
            if (position.x > (_inputArea.position.x - _inputArea.sizeDelta.x / 2f)
                && position.x < (_inputArea.position.x + _inputArea.sizeDelta.x / 2f)
                &&
                position.y > (_inputArea.position.y - _inputArea.sizeDelta.y / 2f)
                && position.y < (_inputArea.position.y + _inputArea.sizeDelta.y / 2f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnTargetRoomArrived()
        {
            _realityIndexText.text = "Reality #" + HoloKitAppUtils.IntToStringF3(_currentRoomIndex + 1);
            _realityNameText.text = HoloKitApp.Instance.GlobalSettings.RealityList.List[_currentRoomIndex].DisplayName;
        }

        private void ScrollToNextRoom()
        {
            if (_currentRoomIndex < RoomList.Count - 1)
            {
                _currentRoomIndex++;
                _cameraTargetPosition = Camera.main.transform.position + new Vector3(RoomSpacingDist, 0f, 0f);
            }
        }

        private void ScrollToLastRoom()
        {
            if (_currentRoomIndex > 0)
            {
                _currentRoomIndex--;
                _cameraTargetPosition = Camera.main.transform.position - new Vector3(RoomSpacingDist, 0f, 0f);
            }
        }

        public void OnMenuButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MenuPage");
        }
    }
}
