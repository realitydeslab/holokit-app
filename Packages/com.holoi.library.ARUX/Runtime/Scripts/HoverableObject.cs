using System;
using UnityEngine;
using UnityEngine.Events;

namespace Holoi.Library.ARUX
{
    public class HoverableObject : MonoBehaviour
    {
        [SerializeField] private float _radius;

        [SerializeField] private Vector3 _offset;

        [SerializeField] private float _loadTime;

        public Vector3 CenterPosition
        {
            get
            {
                return transform.position + transform.TransformVector(_offset);
            }
        }

        public float Radius => _radius;

        public bool IsLoading => _isLoading;

        public float CurrentLoadPercentage => _currentLoad / _loadTime;

        private float _currentLoad;

        private bool _isLoading;

        private float _lastTriggerTime;

        private const float TriggerCoolDownTime = 5f;

        public UnityEvent OnStartedLoading;

        public UnityEvent OnStoppedLoading;

        public UnityEvent OnTriggered;

        public static event Action<HoverableObject> OnHoverableObjectEnabled;

        public static event Action<HoverableObject> OnHoverableObjectDisabled;

        private void OnEnable()
        {
            Reset();
            OnHoverableObjectEnabled?.Invoke(this);
        }

        private void OnDisable()
        {
            OnHoverableObjectDisabled?.Invoke(this);
        }

        private void Reset()
        {
            _currentLoad = 0f;
            _isLoading = false;
        }

        public void OnLoaded(float time)
        {
            if (_currentLoad == _loadTime)
            {
                return;
            }
            _currentLoad += time;
            if (!_isLoading)
            {
                _isLoading = true;
                OnStartedLoading?.Invoke();
            }
            if (_currentLoad > _loadTime)
            {
                _currentLoad = _loadTime;
                if (Time.time - _lastTriggerTime > TriggerCoolDownTime)
                {
                    _lastTriggerTime = Time.time;
                    OnTriggered?.Invoke();
                }
            }
        }

        public void OnUnloaded(float time)
        {
            if (_currentLoad == 0)
            {
                return;
            }
            _currentLoad -= time;
            if (_isLoading)
            {
                _isLoading = false;
                OnStoppedLoading?.Invoke();
            }
            if (_currentLoad < 0f)
            {
                _currentLoad = 0f;
            }
        }
    }
}