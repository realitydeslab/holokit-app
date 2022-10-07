using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_HorizontalScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Mask _mask;

        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private Image _arrowUntriggered;

        [SerializeField] private Sprite _arrowRight;

        [SerializeField] private Sprite _arrowRightStroke;

        [SerializeField] private GameObject _triggeredMarker;

        [SerializeField] private float _initialHorizontalScrollValue;

        private bool _selected;

        private const float RecoverSpeed = 0.2f;

        private const float CoolDown = 1f;

        protected virtual void Awake()
        {

        }

        protected virtual void Update()
        {
            if (!_selected)
            {
                if (_scrollRect.horizontalScrollbar.value < _initialHorizontalScrollValue)
                {
                    _scrollRect.horizontalScrollbar.value += RecoverSpeed * Time.deltaTime;
                    if (_scrollRect.horizontalScrollbar.value > _initialHorizontalScrollValue)
                    {
                        _scrollRect.horizontalScrollbar.value = _initialHorizontalScrollValue;
                        OnRecovered();
                    }
                }

            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnSelected();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            OnUnselected();
        }

        protected virtual void OnSelected()
        {
            Handheld.Vibrate();
            _selected = true;
            _mask.enabled = false;
            _arrowUntriggered.sprite = _arrowRightStroke;
        }

        protected virtual void OnUnselected()
        {
            _selected = false;
            if (_scrollRect.horizontalScrollbar.value == _initialHorizontalScrollValue)
            {
                OnRecovered();
            }
        }

        protected virtual void OnRecovered()
        {
            _mask.enabled = true;
            _arrowUntriggered.sprite = _arrowRight;
        }

        public void OnScrollValueChanged(Vector2 value)
        {
            if (value.x > _initialHorizontalScrollValue)
            {
                _scrollRect.horizontalScrollbar.value = _initialHorizontalScrollValue;
            }

            if (value.x == 0f)
            {
                OnTriggerred();
            }
        }

        protected virtual void OnTriggerred()
        {
            _triggeredMarker.SetActive(true);

            StartCoroutine(StartCoolDown());
        }

        protected virtual void OnUntriggered()
        {

        }

        private IEnumerator StartCoolDown()
        {
            yield return new WaitForSeconds(CoolDown);
            OnUntriggered();
        }
    }
}
