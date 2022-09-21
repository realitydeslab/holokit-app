using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Holoi.Library.Permissions;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class PermissionPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/PermissionPanel";
        public PermissionPanel() : base(new UIType(_path)) { }

        private Button _cameraButton;

        private Button _microphoneButton;

        private Button _photoButton;

        private Button _locationButton;

        private Button _networkButton;

        private Button _notificationButton;

        private Button _doneButton;

        public override void OnOpen()
        {
            PermissionsAPI.OnRequestCameraPermissionCompleted += OnCameraPermissionStatusUpdated;
            PermissionsAPI.OnRequestMicrophonePermissionCompleted += OnMicrophonePermissionStatusUpdated;
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted += OnPhotoLibraryAddPermissionStatusUpdate;
            PermissionsAPI.OnLocationPermissionStatusChanged += OnLocationPermissionStatusUpdated;

            _cameraButton = UITool.GetOrAddComponentInChildren<Button>("CameraButton");
            _microphoneButton = UITool.GetOrAddComponentInChildren<Button>("MicrophoneButton");
            _photoButton = UITool.GetOrAddComponentInChildren<Button>("PhotoButton");
            _locationButton = UITool.GetOrAddComponentInChildren<Button>("LocationButton");
            _networkButton = UITool.GetOrAddComponentInChildren<Button>("NetworkButton");
            _notificationButton = UITool.GetOrAddComponentInChildren<Button>("NotificationButton");
            _doneButton = UITool.GetOrAddComponentInChildren<Button>("DoneButton");

            UpdateAllPermissionButtons();
        }

        public override void OnClose()
        {
            base.OnClose();

            PermissionsAPI.OnRequestCameraPermissionCompleted -= OnCameraPermissionStatusUpdated;
            PermissionsAPI.OnRequestMicrophonePermissionCompleted -= OnMicrophonePermissionStatusUpdated;
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted -= OnPhotoLibraryAddPermissionStatusUpdate;
            PermissionsAPI.OnLocationPermissionStatusChanged -= OnLocationPermissionStatusUpdated;
        }

        public void UpdateAllPermissionButtons()
        {
            HoloKitAppPermissionStatus cameraPermissionStatus = HoloKitAppPermissionsManager.GetCameraPermissionStatus();
            HoloKitAppPermissionStatus microphonePermissionStatus = HoloKitAppPermissionsManager.GetMicrophonePermissionStatus();
            HoloKitAppPermissionStatus photoLibraryAddPermissionStatus = HoloKitAppPermissionsManager.GetPhotoLibraryAddPermissionStatus();
            HoloKitAppPermissionStatus locationPermissionStatus = HoloKitAppPermissionsManager.GetLocationPermissionStatus();
            //Debug.Log($"[CameraPermission] {cameraPermissionStatus}");
            //Debug.Log($"[Microphone] {microphonePermissionStatus}");
            //Debug.Log($"[Photo] {photoLibraryAddPermissionStatus}");
            //Debug.Log($"[Location] {locationPermissionStatus}");

            UpdateAllPermissionButtonsVisual(cameraPermissionStatus, microphonePermissionStatus, photoLibraryAddPermissionStatus, locationPermissionStatus);
            UpdateAllPermissionButtonsFunction(cameraPermissionStatus, microphonePermissionStatus, photoLibraryAddPermissionStatus, locationPermissionStatus);
        }

        private void UpdateAllPermissionButtonsVisual(HoloKitAppPermissionStatus cameraPermissionStatus,
                                           HoloKitAppPermissionStatus microphonePermissionStatus,
                                           HoloKitAppPermissionStatus photoLibraryAddPermissionStatus,
                                           HoloKitAppPermissionStatus locationPermissionStatus)
        {
            if (cameraPermissionStatus == HoloKitAppPermissionStatus.Granted)
            {
                _cameraButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Checked);
            }
            else
            {
                _cameraButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Uncheck);
            }

            if (microphonePermissionStatus == HoloKitAppPermissionStatus.Granted)
            {
                _microphoneButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Checked);
            }
            else
            {
                _microphoneButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Uncheck);
            }

            if (photoLibraryAddPermissionStatus == HoloKitAppPermissionStatus.Granted)
            {
                _photoButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Checked);
            }
            else
            {
                _photoButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Uncheck);
            }

            if (locationPermissionStatus == HoloKitAppPermissionStatus.Granted)
            {
                _locationButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Checked);
            }
            else
            {
                _locationButton.GetComponent<FlexibleUIPermissionButton>().SetUIAppearance(FlexibleUIPermissionButton.State.Uncheck);
            }

            if (cameraPermissionStatus == HoloKitAppPermissionStatus.Granted &&
                microphonePermissionStatus == HoloKitAppPermissionStatus.Granted &&
                photoLibraryAddPermissionStatus == HoloKitAppPermissionStatus.Granted &&
                locationPermissionStatus == HoloKitAppPermissionStatus.Granted)
            {
                _doneButton.GetComponent<FlexibleUIButton>().SetUIAppearance(FlexibleUIButton.State.Active);
            }
            else
            {
                _doneButton.GetComponent<FlexibleUIButton>().SetUIAppearance(FlexibleUIButton.State.Inactive);
            }
        }

        private void UpdateAllPermissionButtonsFunction(HoloKitAppPermissionStatus cameraPermissionStatus,
                                                        HoloKitAppPermissionStatus microphonePermissionStatus,
                                                        HoloKitAppPermissionStatus photoLibraryAddPermissionStatus,
                                                        HoloKitAppPermissionStatus locationPermissionStatus)
        {
            _cameraButton.onClick.AddListener(() =>
            {
                switch (cameraPermissionStatus)
                {
                    case HoloKitAppPermissionStatus.NotDetermined:
                        PermissionsAPI.RequestCameraPermission();
                        break;
                    case HoloKitAppPermissionStatus.Denied:
                        PermissionsAPI.OpenAppSettings();
                        break;
                    case HoloKitAppPermissionStatus.Granted:
                        break;
                }
            });

            _microphoneButton.onClick.AddListener(() =>
            {
                switch (microphonePermissionStatus)
                {
                    case HoloKitAppPermissionStatus.NotDetermined:
                        PermissionsAPI.RequestMicrophonePermission();
                        break;
                    case HoloKitAppPermissionStatus.Denied:
                        PermissionsAPI.OpenAppSettings();
                        break;
                    case HoloKitAppPermissionStatus.Granted:
                        break;
                }
            });

            _photoButton.onClick.AddListener(() =>
            {
                switch (photoLibraryAddPermissionStatus)
                {
                    case HoloKitAppPermissionStatus.NotDetermined:
                        PermissionsAPI.RequestPhotoLibraryAddPermission();
                        break;
                    case HoloKitAppPermissionStatus.Denied:
                        PermissionsAPI.OpenAppSettings();
                        break;
                    case HoloKitAppPermissionStatus.Granted:
                        break;
                }
            });

            _locationButton.onClick.AddListener(() =>
            {
                switch (locationPermissionStatus)
                {
                    case HoloKitAppPermissionStatus.NotDetermined:
                        PermissionsAPI.RequestLocationWhenInUsePermission();
                        break;
                    case HoloKitAppPermissionStatus.Denied:
                        PermissionsAPI.OpenAppSettings();
                        break;
                    case HoloKitAppPermissionStatus.Granted:
                        break;
                }
            });

            //_networkButton.onClick.AddListener(() =>
            //{
            //    if (_networkButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.GoOnState.Checked)
            //    {
            //        _networkButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            //    }
            //    else
            //    {
            //        _networkButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            //    }
            //});

            //_notificationButton.onClick.AddListener(() =>
            //{
            //    if (_notificationButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.GoOnState.Checked)
            //    {
            //        _notificationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            //    }
            //    else
            //    {
            //        _notificationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            //    }
            //});

            _doneButton.onClick.AddListener(() =>
            {
                if (_doneButton.GetComponent<FlexibleUIButton>().state == FlexibleUIButton.State.Active)
                {
                    var panel = new SignInPanel();
                    PanelManager.Push(panel);
                }
            });
        }

        private void OnCameraPermissionStatusUpdated(bool granted)
        {
            UpdateAllPermissionButtons();
        }

        private void OnMicrophonePermissionStatusUpdated(bool granted)
        {
            UpdateAllPermissionButtons();
        }

        private void OnPhotoLibraryAddPermissionStatusUpdate(PhotoLibraryPermissionStatus status)
        {
            UpdateAllPermissionButtons();
        }

        private void OnLocationPermissionStatusUpdated(LocationPermissionStatus status)
        {
            UpdateAllPermissionButtons();
        }
    }
}