using System;
using UnityEngine;

namespace Holoi.HoloKit.App.UI
{
    public class RealityThumbnailContainer : MonoBehaviour
    {
        Vector3 _translate = new Vector3(-0.03f, -0.77f, 7.89f);
        Vector3 _rotate = new Vector3(-26.6f, -60.824f, 39.185f);

        public float currentPostion = 0;

        public Vector3 _offset;

        [HideInInspector] public float currentIndex;

        public event Action clickOnThumbnailsEvent;

        [Header("UI Elements")]
        [SerializeField] Transform _container;
        [SerializeField] Transform _lightGroup;
        public Transform _homePageDeco;
        [SerializeReference] Transform _arrowPath;

        //[Header("Rendering")]

        private void Start()
        {
            transform.position = _translate;
            transform.rotation = Quaternion.Euler(_rotate);
        }

        private void Update()
        {
            _container.localPosition = new Vector3(-1 * currentPostion, 0, 0);
            _container.position += _offset;
            ;

            _lightGroup.localPosition = _offset;

            _arrowPath.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_Offset", new Vector2(currentPostion * 4, 0));

            SetSelectedThumbnail();
            //GetTouchOnPrefabs();
        }

        public void SetSelectedThumbnail()
        {
            //Debug.Log("SetSelectPrefab");
            for (int i = 0; i < _container.childCount; i++)
            {
                if (i == currentIndex)
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
                clickOnThumbnailsEvent?.Invoke();
            }
        }
    }
}