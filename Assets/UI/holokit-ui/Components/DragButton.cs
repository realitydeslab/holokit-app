using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class DragButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // 
    [Header("Target ScrollRect")]
    public ScrollRect scorllRect;
    public HorizontalScrollSnap horizontalScrollSnap;


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        //transform.GetComponent<Image>().raycastTarget = false;
        //if (scorllRect != null)
        //{
        //    scorllRect.OnBeginDrag(eventData);
        //}
        if (horizontalScrollSnap != null)
        {
            horizontalScrollSnap.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        //if (scorllRect != null)
        //{
        //    scorllRect.OnDrag(eventData);
        //}
        if (horizontalScrollSnap != null)
        {
            horizontalScrollSnap.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //transform.GetComponent<Image>().raycastTarget = true;
        //if (scorllRect != null)
        //{
        //    scorllRect.OnEndDrag(eventData);
        //}

        Debug.Log("OnEndDrag");

        if (horizontalScrollSnap != null)
        {
            horizontalScrollSnap.OnEndDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        Debug.Log("OnPointerUp");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }
}