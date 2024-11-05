using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RealityDesignLab.Typed.TheDuck
{
    public class DragableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Image thisImage;
        public Vector3 startPosition;
        public Vector3 Direction;
        public float Length;

        public UnityEvent OnDragBeginEvent;
        public UnityEvent OnDragEvent;
        public UnityEvent OnDragEndEvent;

        void Start()
        {
            thisImage = GetComponent<Image>();
            startPosition = transform.position;


            var manager = FindObjectOfType<TypedTheDuckRealityManager>();

            OnDragBeginEvent.AddListener(manager.OnDragBegin);
            OnDragEvent.AddListener(manager.OnDrag);
            OnDragEndEvent.AddListener(manager.OnDragEnd);

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            thisImage.raycastTarget = false;

            OnDragBeginEvent?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");

            transform.position = eventData.position;

            Direction = (transform.position - startPosition).normalized;

            Length = (transform.position - startPosition).magnitude;

            OnDragEvent?.Invoke();

            //Debug.Log("Length: " + Length);
            //Debug.Log("Direction: " + Direction);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");

            transform.position = startPosition;
            thisImage.raycastTarget = true;

            OnDragEndEvent?.Invoke();
        }
    }
}
