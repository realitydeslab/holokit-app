using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class RealityThumbnailContainer : MonoBehaviour
{

     public float currentPostion = 0;

     public Vector3 _offset;

    [HideInInspector]public float currentIndex;

    public event Action clickEvent;

    [Header("UI Elements")]
    [SerializeField] Transform _container;
    [SerializeField] Transform _lightGroup;

    //[Header("Rendering")]

    private void Awake()
    {

    }

    private void Update()
    {
        _container.localPosition = new Vector3(-1 * currentPostion, 0,0) + _offset;
        _lightGroup.localPosition = _offset;

        SetSelectPrefab();
        //DebugTouchItem();
        TouchOnTrashButton();
        //GetTouchOnPrefabs();
    }

    public void SetSelectPrefab()
    {
        //Debug.Log("SetSelectPrefab");
        for (int i = 0; i < _container.childCount; i++)
        {
            if(i == currentIndex)
            {
                //Debug.Log("set index layer" + transform.GetChild(i).name);

                var go = _container.GetChild(i).gameObject;
                go.layer = 3;

                foreach (Transform child in go.transform)
                {
                    child.gameObject.layer = 3;
                }
            }
            else
            {
                var go = _container.GetChild(i).gameObject;
                go.layer = 0;

                foreach (Transform child in go.transform)
                {
                    child.gameObject.layer = 0;
                }
            }
        }
    }

    public void GetTouchOnPrefabs()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("get touch 0");
            //Check for mouse click 
            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out raycastHit, 100f))
                {
                    if (raycastHit.transform != null)
                    {
                        //Our custom method. 
                        CurrentClickedGameObject(raycastHit.transform.gameObject);
                    }
                }
            }
        }
    }

    public void DebugTouchItem()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //Check for mouse click 
            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out raycastHit, 100f))
                {
                    if (raycastHit.transform != null)
                    {
                        //Our custom method. 
                        Debug.Log(raycastHit.transform.gameObject.name);
                    }
                }
            }
        }
    }

    public bool TouchOnTrashButton()//if touch phase ends on this button 
    {
        if (Input.touchCount > 0)
        {//than the function will return true .
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0)
                {
                    Debug.Log(results[0].gameObject.name);
                }
            }
        }
        return false;
    }

    public void CurrentClickedGameObject(GameObject gameObject)
    {
        Debug.Log("hit sth");
        if (gameObject.tag == "Reality Thumbnail")
        {
            Debug.Log("hit target");

            clickEvent?.Invoke();
        }
    }
}