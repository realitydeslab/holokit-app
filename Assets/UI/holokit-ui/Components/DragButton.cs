using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class DragButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
//IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Target ScrollRect")]
    public ScrollRect scorllRect;
    //public HorizontalScrollSnap horizontalScrollSnap;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (scorllRect != null)
        {
            scorllRect.OnBeginDrag(eventData);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");

        if (scorllRect != null)
        {
            scorllRect.OnDrag(eventData);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if (scorllRect != null)
        {
            scorllRect.OnEndDrag(eventData);
        }
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{

    //    Debug.Log("OnPointerUp");
    //}
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerDown");
    //}
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerClick");
    //}
}