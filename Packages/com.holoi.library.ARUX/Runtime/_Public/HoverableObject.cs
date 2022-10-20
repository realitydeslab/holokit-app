using UnityEngine;
using UnityEngine.Events;

namespace Holoi.Library.ARUX
{
    public class HoverableObject : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent OnLoadedEvents;

        [Header("Interacton Properties")]
        [SerializeField] Vector3 _offset = new Vector3(0, 0, 0);

        [SerializeField] float _triggerDistance = .2f;

        [SerializeField] float _loadTime = 1f;

        [SerializeField] bool _autoReset = false;

        bool _isTriggered = false;

        float _loadSpeed;

        HandObject _ho;

        float _process = 0;

        public float Process { get { return _process; } }

        private void Start()
        {
            _ho = HandObject.Instance;
            _loadSpeed = 1 / _loadTime;
        }
        private void OnEnable()
        {
            Debug.Log($"{gameObject.name} : Onenable");
            _isTriggered = false;
            _process = 0;
        }
        private void OnDisable()
        {

        }

        public void OnReset()
        {
            _isTriggered = false;
            _process = 0;
        }

        void Update()
        {
            if (_ho.IsValid)
            {
                if (Vector3.Distance(_ho.transform.position, transform.position + _offset) < _triggerDistance)
                {
                    _process += Time.deltaTime * _loadSpeed;
                    if (_process > 1)
                    {
                        _process = 1;
                        if (!_isTriggered)
                        {
                            _isTriggered = true;
                            OnLoadedEvents?.Invoke();
                            if(_autoReset)
                            {
                                OnReset();
                            }
                        }
                    }
                }
                else
                {
                    _process -= Time.deltaTime * _loadSpeed * 0.5f;
                    if (_process < 0) _process = 0;
                }
            }
        }
    }
}