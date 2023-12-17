// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine.XR.ARFoundation;
using System.Linq;

namespace UnityEngine.XR.HoloKit
{

    public class LiteHandTrackingManager : MonoBehaviour
    {

        private float MaxHandDepth = 1.0f;

        public ComputeShader _computeShader;

        private Camera _arCamera;
        private AROcclusionManager _occlusionManager;
        private ARCameraManager _arCameraManager;

        [SerializeField] private bool _handTrackingEnabled = true;

        private bool _isHandValid = false;

        [SerializeField] bool _writeToObjectPosition = false;
        [SerializeField] GameObject _handObject;
        private Vector3 _currentHandPosition;

        private int _initializeHorizonalHistogramKernel;
        private int _initializeVerticalHistogramKernel;
        private int _calculateHistogramKernel;

        private ComputeBuffer _verticalHistogramBuffer;
        private ComputeBuffer _horizonalHistogramBuffer;
        private uint[] _verticalHistogramData;
        private uint[] _horizonalHistogramData;

        private int _width = 0;
        private int _height = 0;

        public Vector3 CurrentHandPosition
        {
            get { return _currentHandPosition; }
        }

        private void Start()
        {
            _arCameraManager = GameObject.FindFirstObjectByType<ARCameraManager>();
            _arCamera = _arCameraManager.GetComponent<Camera>();
            _occlusionManager = GameObject.FindFirstObjectByType<AROcclusionManager>();

            Debug.Assert(_computeShader != null);
            Debug.Assert(_occlusionManager != null);
            Debug.Assert(_arCamera != null);

            _horizonalHistogramBuffer = null;
            _verticalHistogramBuffer = null;
            _width = 0;
            _height = 0;

            _initializeHorizonalHistogramKernel = _computeShader.FindKernel("InitializeHorizonalHistogram");
            _initializeVerticalHistogramKernel = _computeShader.FindKernel("InitializeVerticalHistogram");
            _calculateHistogramKernel = _computeShader.FindKernel("CalculateHistogram");

            _computeShader.SetFloat("MaxDepth", MaxHandDepth);
        }

        struct HistStruct
        {
            public uint sum;
        }

        private void Update()
        {
            if (_initializeHorizonalHistogramKernel >= 0 &&
                _initializeVerticalHistogramKernel >= 0 &&
                _calculateHistogramKernel >= 0 &&
                _occlusionManager.humanDepthTexture != null && _occlusionManager.humanStencilTexture != null)
            {
                if (_width == 0 || _height == 0)
                {
                    _width = _occlusionManager.humanDepthTexture.width;
                    _height = _occlusionManager.humanDepthTexture.height;
                    // Debug.Log($"Update Holokit Hand Tracking with humanDepthTexture.width={_width} humanDepthTexture.height={_height}");
                    // Debug.Log($"Update Holokit Hand Tracking with _occlusionManager.humanStencilTexture={_occlusionManager.humanStencilTexture.width} _occlusionManager.humanStencilTexture={_occlusionManager.humanStencilTexture.height}");

                    _horizonalHistogramBuffer = new ComputeBuffer(_width, sizeof(uint));
                    _verticalHistogramBuffer = new ComputeBuffer(_height, sizeof(uint));
                    _horizonalHistogramData = new uint[_width];
                    _verticalHistogramData = new uint[_height];
                    _computeShader.SetBuffer(_initializeHorizonalHistogramKernel, "HorizonalHistogram", _horizonalHistogramBuffer);
                    _computeShader.SetBuffer(_initializeVerticalHistogramKernel, "VerticalHistogram", _verticalHistogramBuffer);
                    _computeShader.SetBuffer(_calculateHistogramKernel, "HorizonalHistogram", _horizonalHistogramBuffer);
                    _computeShader.SetBuffer(_calculateHistogramKernel, "VerticalHistogram", _verticalHistogramBuffer);
                }

                _computeShader.Dispatch(_initializeHorizonalHistogramKernel, Mathf.CeilToInt(_width / 64f), 1, 1);
                _computeShader.Dispatch(_initializeVerticalHistogramKernel, Mathf.CeilToInt(_height / 64f), 1, 1);

                _computeShader.SetTexture(_calculateHistogramKernel, "DepthTexture", _occlusionManager.humanDepthTexture);
                _computeShader.SetTexture(_calculateHistogramKernel, "StencilTexture", _occlusionManager.humanStencilTexture);
                _computeShader.Dispatch(_calculateHistogramKernel, Mathf.CeilToInt(_width / 8f), Mathf.CeilToInt(_height / 8f), 1);

                _horizonalHistogramBuffer.GetData(_horizonalHistogramData);
                _verticalHistogramBuffer.GetData(_verticalHistogramData);

                float xCoordinate = _horizonalHistogramData.Select((x, i) => new { weightedX = (float)x * (_width - i) / _width, i }).Aggregate((a, a1) => a.weightedX > a1.weightedX ? a : a1).i; //Friendly for Right Hand
                float yCoordinate = _height - _verticalHistogramData.Select((x, i) => new { weightedX = (float)x * (_height - i) / _height, i }).Aggregate((a, a1) => a.weightedX > a1.weightedX ? a : a1).i;
                float xCoordinateInCamera = xCoordinate / _width * _arCamera.pixelWidth;
                float yCoordinateInCamera = yCoordinate / _height * _arCamera.pixelHeight;
                float zCoordinateInCamera = 0.4f;

                float moreThanThresholdX = _horizonalHistogramData.Where(x => x > 10).Count();
                float moreThanThresholdY = _verticalHistogramData.Where(x => x > 10).Count();

                _isHandValid = moreThanThresholdX / _width > 0.05f && moreThanThresholdY / _height > 0.05f;
                //Debug.Log($"Update xCoordinate={xCoordinate} yCoordinate={yCoordinate} moreThanThresholdX={moreThanThresholdX} moreThanThresholdY={moreThanThresholdY}");

                if (_isHandValid)
                {
                    _currentHandPosition = _arCamera.ScreenToWorldPoint(new Vector3(xCoordinateInCamera, yCoordinateInCamera, zCoordinateInCamera));
                    if (_writeToObjectPosition)
                    {
                        _handObject.SetActive(true);
                        _handObject.transform.position = _currentHandPosition;
                        //Debug.Log("Hand is valid with a value of:" + _handObject.transform.position);
                    }
                }
                else
                {
                    _currentHandPosition = Camera.main.transform.position;
                    if (_writeToObjectPosition)
                    {
                        _handObject.SetActive(true);
                        _handObject.transform.position = _currentHandPosition;
                    }
                }
            }
            else
            {
                //_HandCenter.SetActive(false);
                _isHandValid = false;
                _currentHandPosition = Camera.main.transform.position;
                if (_writeToObjectPosition)
                {
                    _handObject.SetActive(true);
                    _handObject.transform.position = _currentHandPosition;
                }
            }
        }

        void OnDestroy()
        {
            if (null != _horizonalHistogramBuffer)
            {
                _horizonalHistogramBuffer.Release();
                _horizonalHistogramBuffer = null;
            }

            if (null != _verticalHistogramBuffer)
            {
                _verticalHistogramBuffer.Release();
                _verticalHistogramBuffer = null;
            }
        }
    }
}
