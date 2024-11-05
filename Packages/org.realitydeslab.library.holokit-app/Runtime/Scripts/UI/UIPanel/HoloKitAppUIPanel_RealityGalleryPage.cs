// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_RealityGalleryPage : HoloKitAppUIPanel
    {
        public override string UIPanelName => "RealityGalleryPage";

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
        /// Only taps inside this area are effective.
        /// </summary>
        [SerializeField] private RectTransform _inputArea;

        /// <summary>
        /// Keeps a reference of this to destroy later.
        /// </summary>
        private GameObject _roomListRoot;

        /// <summary>
        /// The index of the currently selected room.
        /// </summary>
        private int _currentRoomIndex = 0;

        private bool _touchable = false;

        private Vector2 _touchBeganPosition;

        /// <summary>
        /// The distance between two rooms.
        /// </summary>
        private const float RoomSpacingDist = 13.5f;

        /// <summary>
        /// The duration for camera moving from one room to another.
        /// </summary>
        private const float CameraMovingDuration = 0.7f;

        /// <summary>
        /// The UI will only respond to scroll when finger movement magnitude is larger than this value.
        /// </summary>
        private const float FingerMovementScrollThreshold = 160f;

        /// <summary>
        /// The UI will only respond to click when finger movement magnitude is smaller than this value.
        /// </summary>
        private const float FingerMovementClickThreshold = 20f;

        private const float CamaraOrthographicSize = 18f;

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
            // TODO: This might not be good. I just want to save the file more frequently.
            HoloKitApp.Instance.GlobalSettings.Save();

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
            Camera.main.orthographicSize = CamaraOrthographicSize;
            Camera.main.transform.SetPositionAndRotation(currentRoomPosition + RoomCenterToCameraOffsetPosition, Quaternion.Euler(RoomCenterToCameraOffsteEulerRotation));
            OnTargetRoomArrived();
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.5f, () =>
            {
                _touchable = true;
            }));
        }

        private void OnDisable()
        {
            _touchable = false;
            Destroy(_roomListRoot);
            RoomList.Clear();
        }

        private void Update()
        {
            if (_touchable && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Began && touch.phase != TouchPhase.Ended)
                {
                    return;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    _touchBeganPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Vector3 fingerMovementVector = touch.position - _touchBeganPosition;
                    if (fingerMovementVector.magnitude > FingerMovementScrollThreshold)
                    {
                        if (fingerMovementVector.x > 0f && fingerMovementVector.y > 0f)
                        {
                            ScrollToLastRoom();
                        }
                        else if (fingerMovementVector.x < 0f && fingerMovementVector.y < 0f)
                        {
                            ScrollToNextRoom();
                        }
                        else if (Mathf.Abs(fingerMovementVector.x) > Mathf.Abs(fingerMovementVector.y))
                        {
                            if (fingerMovementVector.x > 0f)
                            {
                                ScrollToLastRoom();
                            }
                            else if (fingerMovementVector.x < 0f)
                            {
                                ScrollToNextRoom();
                            }
                        }
                        else if (Mathf.Abs(fingerMovementVector.x) < Mathf.Abs(fingerMovementVector.y))
                        {
                            if (fingerMovementVector.y > 0f)
                            {
                                ScrollToLastRoom();
                            }
                            else if (fingerMovementVector.y < 0f)
                            {
                                ScrollToNextRoom();
                            }
                        }
                    }
                    else if (fingerMovementVector.magnitude < FingerMovementClickThreshold)
                    {
                        if (!IsInsideInputArea(_touchBeganPosition))
                        {
                            return;
                        }
                        // Cancel all LeanTween operations
                        LeanTween.cancelAll();
                        // Enter the current room
                        HoloKitApp.Instance.CurrentReality = HoloKitApp.Instance.GlobalSettings.RealityList.List[_currentRoomIndex];
                        HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityDetailPage");
                    }
                }
            }
        }

        private bool IsInsideInputArea(Vector2 position)
        {
            return position.y > 0.2f * Screen.height && position.y < 0.8f * Screen.height;
        }

        private void OnTargetRoomArrived()
        {
            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "en":
                    _realityIndexText.text = "Reality #" + HoloKitAppUtils.IntToStringF3(_currentRoomIndex + 1);
                    _realityNameText.text = HoloKitApp.Instance.GlobalSettings.RealityList.List[_currentRoomIndex].DisplayName;
                    break;
                case "zh-Hans":
                    _realityIndexText.text = "混合现实 #" + HoloKitAppUtils.IntToStringF3(_currentRoomIndex + 1);
                    _realityNameText.text = HoloKitApp.Instance.GlobalSettings.RealityList.List[_currentRoomIndex].DisplayName_Chinese;
                    break;
            }
        }

        private void ScrollToNextRoom()
        {
            if (_currentRoomIndex < RoomList.Count - 1)
            {
                _currentRoomIndex++;
                var cameraTargetPosition = RoomCenterToCameraOffsetPosition +
                    new Vector3(_currentRoomIndex * RoomSpacingDist, 0f, 0f);
                LeanTween.cancel(Camera.main.gameObject);
                LeanTween.move(Camera.main.gameObject, cameraTargetPosition, CameraMovingDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(OnTargetRoomArrived);
            }
        }

        private void ScrollToLastRoom()
        {
            if (_currentRoomIndex > 0)
            {
                _currentRoomIndex--;
                var cameraTargetPosition = RoomCenterToCameraOffsetPosition +
                    new Vector3(_currentRoomIndex * RoomSpacingDist, 0f, 0f);
                LeanTween.cancel(Camera.main.gameObject);
                LeanTween.move(Camera.main.gameObject, cameraTargetPosition, CameraMovingDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(OnTargetRoomArrived);
            }
        }

        public void OnMenuButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MenuPage");
        }
    }
}
