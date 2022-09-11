using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloKit
{
    public struct HoloKitCameraData
    {
        public Rect LeftViewportRect;
        public Rect RightViewportRect;
        public float NearClipPlane;
        public float FarClipPlane;
        public Matrix4x4 LeftProjectionMatrix;
        public Matrix4x4 RightProjectionMatrix;
        public Vector3 CameraToCenterEyeOffset;
        public Vector3 CameraToScreenCenterOffset;
        public Vector3 CenterEyeToLeftEyeOffset;
        public Vector3 CenterEyeToRightEyeOffset;
        // The horizontal distance from the screen center in pixels
        public float AlignmentMarkerOffset;
    }

    public enum HoloKitRenderMode
    {
        Mono = 0,
        Stereo = 1
    }

    public enum VideoEnhancementMode
    {
        None = 0,      // Default HD
        HighRes = 1,   // 4K
        HighResWithHDR // 4K with HDR
    }

    public class HoloKitCamera : MonoBehaviour
    {
        public static HoloKitCamera Instance { get { return _instance; } }

        private static HoloKitCamera _instance;

        [SerializeField] private Transform _centerEyePose;

        [SerializeField] private Camera _monoCamera;

        [SerializeField] private Camera _leftEyeCamera;

        [SerializeField] private Camera _rightEyeCamera;

        [SerializeField] private Camera _blackCamera;

        [SerializeField]
        [Range(0.054f, 0.074f)]
        private float _ipd = 0.064f;

        [SerializeField] private float _farClipPlane = 50f;

        [SerializeField] private VideoEnhancementMode _videoEnhancementMode = VideoEnhancementMode.None;

        public Transform CenterEyePose => _centerEyePose;

        public HoloKitRenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                if (_renderMode != value)
                {
                    if (value == HoloKitRenderMode.Stereo)
                    {
                        HoloKitNFCSessionControllerAPI.StartNFCSession(HoloKitType.HoloKitX, _ipd, _farClipPlane);
                    }
                    else
                    {
                        _renderMode = HoloKitRenderMode.Mono;
                        OnRenderModeChanged();
                        OnHoloKitRenderModeChanged?.Invoke(HoloKitRenderMode.Mono);
                    }
                }
            }
        }

        public VideoEnhancementMode VideoEnhancementMode
        {
            get => _videoEnhancementMode;
            set
            {
                _videoEnhancementMode = value;
            }
        }

        public float AlignmentMarkerOffset => _alignmentMarkerOffset;

        private HoloKitRenderMode _renderMode = HoloKitRenderMode.Mono;

        private float _alignmentMarkerOffset;

        private ARCameraBackground _arCameraBackground;

        public static event Action<HoloKitRenderMode> OnHoloKitRenderModeChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            _arCameraBackground = GetComponent<ARCameraBackground>();
            RenderMode = HoloKitRenderMode.Mono;
            OnRenderModeChanged();
            HoloKitARSessionControllerAPI.SetVideoEnhancementMode(_videoEnhancementMode);
            HoloKitNFCSessionControllerAPI.OnNFCSessionCompleted += OnNFCSessionCompleted;
        }

        private void OnDestroy()
        {
            HoloKitNFCSessionControllerAPI.OnNFCSessionCompleted -= OnNFCSessionCompleted;
        }

        public void SetupHoloKitCameraData(HoloKitCameraData holokitCameraData)
        {
            _centerEyePose.localPosition = holokitCameraData.CameraToCenterEyeOffset;
            _leftEyeCamera.transform.localPosition = holokitCameraData.CenterEyeToLeftEyeOffset;
            _rightEyeCamera.transform.localPosition = holokitCameraData.CenterEyeToRightEyeOffset;

            // Setup left eye camera
            _leftEyeCamera.nearClipPlane = holokitCameraData.NearClipPlane;
            _leftEyeCamera.farClipPlane = holokitCameraData.FarClipPlane;
            _leftEyeCamera.rect = holokitCameraData.LeftViewportRect;
            _leftEyeCamera.projectionMatrix = holokitCameraData.LeftProjectionMatrix;
            // Setup right eye camera
            _rightEyeCamera.nearClipPlane = holokitCameraData.NearClipPlane;
            _rightEyeCamera.farClipPlane = holokitCameraData.FarClipPlane;
            _rightEyeCamera.rect = holokitCameraData.RightViewportRect;
            _rightEyeCamera.projectionMatrix = holokitCameraData.RightProjectionMatrix;

            _alignmentMarkerOffset = holokitCameraData.AlignmentMarkerOffset;
        }

        private void OnRenderModeChanged()
        {
            if (_renderMode == HoloKitRenderMode.Stereo)
            {
                _monoCamera.enabled = false;
                _arCameraBackground.enabled = false;
                _leftEyeCamera.gameObject.SetActive(true);
                _rightEyeCamera.gameObject.SetActive(true);
                _blackCamera.gameObject.SetActive(true);
            }
            else
            {
                _monoCamera.enabled = true;
                _arCameraBackground.enabled = true;
                _leftEyeCamera.gameObject.SetActive(false);
                _rightEyeCamera.gameObject.SetActive(false);
                _blackCamera.gameObject.SetActive(false);
                _centerEyePose.localPosition = Vector3.zero;
            }
        }

        private void OnNFCSessionCompleted(bool success)
        {
            if (success)
            {
                _renderMode = HoloKitRenderMode.Stereo;
                OnRenderModeChanged();
                OnHoloKitRenderModeChanged?.Invoke(HoloKitRenderMode.Stereo);
            }
        }

        public void OpenStereoWithoutNFC(string password)
        {
            HoloKitNFCSessionControllerAPI.SkipNFCSessionWithPassword(password, HoloKitType.HoloKitX, _ipd, _farClipPlane);
        }
    }
}
