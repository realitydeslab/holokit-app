using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class RealityThumbnailContainer : MonoBehaviour
    {
        public float currentPostion = 0;
        public List<GameObject> _thumbnailList = new List<GameObject>();
        [HideInInspector] public int activeIndex;
        public event Action clickOnThumbnailsEvent;

        [Header("Transfrom")]
        public Vector3 offset;
        public Vector3 scrollOffset;
        public float rotateValue;

        [Header("UI Elements")]
        [SerializeField] Transform _container;
        [SerializeField] Transform _lightGroup;
        public Transform _homePageDeco;
        [SerializeReference] Transform _arrowPath;
        [SerializeReference] Transform _arrowEnter;

        [Header("Touch Detection")]
        Vector2 startPos = new Vector2();
        Vector2 direction = new Vector2();
        Vector2 endPos = new Vector2();
        string message = new string("hello");

        Vector3 _defaultTranslate = new Vector3(-0.03f, -0.77f, 7.89f);
        Vector3 _defaultRotate = new Vector3(-26.6f, -60.824f, 39.185f);

        private void Awake()
        {
            offset = Vector3.zero;
            scrollOffset = Vector3.zero;
        }
        private void Start()
        {
            transform.position = _defaultTranslate;
            transform.rotation = Quaternion.Euler(_defaultRotate);
        }

        private void Update()
        {
            _container.localPosition = new Vector3(-1 * currentPostion, 0, 0);
            _container.position += (offset + scrollOffset);

            _lightGroup.localPosition = Vector3.zero;
            _lightGroup.position += scrollOffset;

            _arrowPath.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_Offset", new Vector2(currentPostion * 1, 0));

            
            if(PanelManager.Instance.GetActivePanel().UIType.Name == "StartPanel")
            {
                SetSelectedThumbnail();
                GetTouchClickOnPrefabs();
            }else if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
            {
                UpdateThumbnailRotation(rotateValue);
            }
        }

        public void TriggerArrowEnterAnimation()
        {
            _arrowEnter.GetComponent<Animator>().SetTrigger("out");
        }

        // scrollValue(-.5f ~ 1.5f)
        void UpdateThumbnailRotation(float scrollValue)
        {
            var valueFixer = scrollValue - 0.5f;
            // the active thumbnail, set rotate on its local axis for a value = eular
            _thumbnailList[activeIndex].transform.localRotation = Quaternion.Euler(new Vector3(0, scrollValue * 25f, 0));
        }

        public void SetSelectedThumbnail()
        {
            for (int i = 0; i < _container.childCount; i++)
            {
                if (i == activeIndex)
                {
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

        public void GetTouchClickOnPrefabs()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Handle finger movements based on TouchPhase
                switch (touch.phase)
                {
                    //When a touch has first been detected, change the message and record the starting position
                    case TouchPhase.Began:
                        // Record initial touch position.
                        startPos = touch.position;
                        message = "Begun ";
                        break;

                    //Determine if the touch is a moving touch
                    case TouchPhase.Moved:
                        // Determine direction by comparing the current touch position with the initial one
                        direction = touch.position - startPos;
                        message = "Moving ";
                        break;

                    case TouchPhase.Ended:
                        // Report that the touch has ended when it ends
                        endPos = touch.position;
                        message = "Ending ";

                        if (Vector2.Distance(endPos, startPos) < 10f)
                        {
                            // should be a click:
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

                        break;
                }
            }
        }

        public void CurrentClickedGameObject(GameObject gameObject)
        {
            if (gameObject.tag == "Reality Thumbnail")
            {
                clickOnThumbnailsEvent?.Invoke();
            }
        }
    }
}