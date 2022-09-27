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
        public event Action OnThumbnailClickedEvent;

        [Header("Transfrom")]
        Vector3 _offset;
        public Vector3 scrollOffset;
        public Vector2 rotateValue;

        [Header("UI Elements")]
        [SerializeField] Transform _container;
        [SerializeField] Transform _lightGroup;
        public Transform _homePageDeco;
        [SerializeReference] Transform _arrowPath;
        [SerializeReference] Transform _arrowEnter;

        [Header("Prefabs")]
        public GameObject realitySampleBox;

        [Header("Touch Detection")]
        Vector2 startPos = new Vector2();
        Vector2 direction = new Vector2();
        Vector2 endPos = new Vector2();

        Vector3 _defaultTranslate = new Vector3(-0.03f, -0.77f, 7.89f);
        Vector3 _defaultRotate = new Vector3(-26.6f, -60.824f, 39.185f);

        private void Awake()
        {
            _offset = Vector3.zero;
            scrollOffset = Vector3.zero;
            rotateValue = Vector2.zero;
        }
        private void Start()
        {
            transform.position = _defaultTranslate;
            transform.rotation = Quaternion.Euler(_defaultRotate);
        }

        private void Update()
        {
            _container.localPosition = new Vector3(-1 * currentPostion, 0, 0);
            _container.position += (_offset + scrollOffset);

            _lightGroup.localPosition = Vector3.zero;
            _lightGroup.position += scrollOffset;

            _arrowPath.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_Offset", new Vector2(currentPostion * 1, 0));


            if (PanelManager.Instance.GetActivePanel().UIType.Name == "StartPanel")
            {
                _offset = Vector3.zero;
                rotateValue = new Vector2(0.422f, 0.5f);

                //SetSelectedThumbnail();
                GetTouchClickOnPrefabs();
                UpdateThumbnailRotation(rotateValue);
            }
            else if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
            {
                _offset = new Vector3(0, .47f, 0);
                //rotateValue = 0.5f;
                UpdateThumbnailRotation(rotateValue);
            }
            else
            {

            }
        }

        public void TriggerArrowEnterAnimation()
        {
            _arrowEnter.GetComponent<Animator>().SetTrigger("out");
        }

        // scrollValue(-.5f ~ 1.5f)
        void UpdateThumbnailRotation(Vector2 value)
        {
            // box should have y rotate value form -38 ~ 52;
            var clampX = Mathf.Clamp01(value.x);
            var clampY = Mathf.Clamp01(value.y);
            var valueFixerX = Remap(clampX,0,1,-38f, 52f);
            var valueFixerY = Remap(clampY, 0,1,-10f, 10f);
            _thumbnailList[activeIndex].transform.localRotation = Quaternion.Euler(new Vector3(0, valueFixerX, 0));
        }

        float Remap(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin) / inMax * (outMax - outMin) + outMin;
        }

        public void SetSelectedThumbnail()
        {
            for (int i = 0; i < _container.childCount; i++)
            {
                if (i == activeIndex)
                {
                    var parent = _container.GetChild(i).gameObject;
                    parent.layer = 3;

                    var transformList = parent.GetComponentsInChildren<Transform>();
                    foreach (var go in transformList)
                    {
                        go.gameObject.layer = 3;
                    }
                }
                else
                {
                    var parent = _container.GetChild(i).gameObject;
                    parent.layer = 0;

                    var transformList = parent.GetComponentsInChildren<Transform>();
                    foreach (var go in transformList)
                    {
                        go.gameObject.layer = 0;
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
                        break;

                    //Determine if the touch is a moving touch
                    case TouchPhase.Moved:
                        // Determine direction by comparing the current touch position with the initial one
                        direction = touch.position - startPos;
                        break;

                    case TouchPhase.Ended:
                        // Report that the touch has ended when it ends
                        endPos = touch.position;

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
                OnThumbnailClickedEvent?.Invoke();
            }
        }
    }
}