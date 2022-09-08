using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RealityThumbnailContainer : MonoBehaviour
{
    public float currentPostion = 0;
    [HideInInspector]
    public Vector3 positionOffset;
    [HideInInspector]
    public float currentIndex;

    public event Action clickEvent;

    Transform _container;
    //[Header("Rendering")]
    //[SerializeField] Material _wall;

    private void Awake()
    {
        _container = transform.Find("Container");
    }

    private void Update()
    {
        _container.localPosition = new Vector3(-1 * currentPostion, 0,0) + positionOffset;
        SetSelectPrefab();
        GetTouchOnPrefabs();
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