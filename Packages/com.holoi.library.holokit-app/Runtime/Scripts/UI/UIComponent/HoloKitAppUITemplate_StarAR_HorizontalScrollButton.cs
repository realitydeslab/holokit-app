using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_StarAR_HorizontalScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public abstract bool SwipeRight { get; }

        [SerializeField] private Mask _mask;

        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private Image _arrowUntriggered;

        [SerializeField] private Sprite _arrowRight;

        [SerializeField] private Sprite _arrowRightStroke;

        [SerializeField] private GameObject _triggeredMarker;

        [SerializeField] private GameObject _cover;

        [SerializeField] private float _initialHorizontalScrollValue;

        protected bool Selected;

        private const float RecoverSpeed = 0.5f;

        protected virtual void Awake()
        {
            _scrollRect.horizontalScrollbar.value = _initialHorizontalScrollValue;
            OnRecovered();
        }

        protected virtual void Update()
        {
            if (!Selected)
            {
                if (SwipeRight)
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
                else
                {
                    if (_scrollRect.horizontalScrollbar.value > _initialHorizontalScrollValue)
                    {
                        _scrollRect.horizontalScrollbar.value -= RecoverSpeed * Time.deltaTime;
                        if (_scrollRect.horizontalScrollbar.value < _initialHorizontalScrollValue)
                        {
                            _scrollRect.horizontalScrollbar.value = _initialHorizontalScrollValue;
                            OnRecovered();
                        }
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
            Selected = true;
            _mask.enabled = false;
            if (_arrowUntriggered != null)
            {
                _arrowUntriggered.sprite = _arrowRightStroke;
            }
        }

        protected virtual void OnUnselected()
        {
            Selected = false;
            if (_scrollRect.horizontalScrollbar.value == _initialHorizontalScrollValue)
            {
                OnRecovered();
            }
        }

        protected virtual void OnRecovered()
        {
            _mask.enabled = true;
            if (_arrowUntriggered != null)
            {
                _arrowUntriggered.sprite = _arrowRight;
            }
            _cover.SetActive(false);
            _triggeredMarker.SetActive(false);
        }

        public void OnScrollValueChanged(Vector2 value)
        {
            if (SwipeRight)
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
            else
            {
                if (value.x < _initialHorizontalScrollValue)
                {
                    _scrollRect.horizontalScrollbar.value = _initialHorizontalScrollValue;
                }

                if (value.x == 1f)
                {
                    OnTriggerred();
                }
            }
        }

        protected virtual void OnTriggerred()
        {
            _triggeredMarker.SetActive(true);
            _cover.SetActive(true);
        }
    }
}
