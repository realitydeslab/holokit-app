using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class HomeUIPanel : MonoBehaviour
    {
        public RealityList realityCollection;
        public RealityThumbnailContainer realityThumbnailContainer;

        [Header("Prefabs")]
        [SerializeField] GameObject _prefabTitle;

        [Header("UI Elements")]
        [SerializeField] Transform _verticleScrollContainer;
        [SerializeField] Transform _titleScrollContent;
        [SerializeField] Transform _horizentalScrollContainer;
        [SerializeField] Transform _handle;

        Canvas _canvas;

        //List<GameObject> _thumbnailList = new List<GameObject>();

        int _realityCount;

        int _activeIndex = 0;

        public int CurrentIndex
        {
            get { return _activeIndex; }
        }

        float _scrollValue;

        float _offset = 4f;
        public Vector3 _eular;

        private void OnEnable()
        {
            _horizentalScrollContainer.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.AddListener(realityThumbnailContainer.TriggerArrowEnterAnimation);
            _horizentalScrollContainer.GetComponent<HorizontalScrollSnap>().OnPreviousScreenEvent.AddListener(realityThumbnailContainer.TriggerArrowEnterAnimation);
        }

        private void OnDisable()
        {
            _horizentalScrollContainer.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.RemoveListener(realityThumbnailContainer.TriggerArrowEnterAnimation);
            _horizentalScrollContainer.GetComponent<HorizontalScrollSnap>().OnPreviousScreenEvent.RemoveListener(realityThumbnailContainer.TriggerArrowEnterAnimation);
        }

        private void Awake()
        {
            _realityCount = realityCollection.realities.Count;
            _canvas = FindObjectOfType<Canvas>();
            realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();

            DeletePreviousElement(_titleScrollContent);
            DeletePreviousElement(realityThumbnailContainer.transform.Find("Container").transform);

            InitialCoverContent();

            _handle.GetComponent<ScrollBarSlidingAreaStyle>().Init(_realityCount);
        }

        private void Start()
        {

        }

        private void Update()
        {
            
            if (PanelManager.Instance.GetActivePanel().UIType.Name == "StartPanel")
            {
                UpdateScrollValue();

                HomePanelUIlayout();
            }
            else if(PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
            {
                DetailsPanelUILayout();
            }
            else
            {
                OthersPanelUILayout();
            }
        }

        void InitialCoverContent()
        {
            realityThumbnailContainer._thumbnailList.Clear();
            for (int i = 0; i < _realityCount; i++)
            {
                // create thumbnailPrefabs
                var realityThumbnailGO = Instantiate(realityCollection.realities[i].thumbnailPrefab, realityThumbnailContainer.transform.Find("Container").transform);
                realityThumbnailGO.transform.localPosition = new Vector3(i * _offset, 0, 0);
                realityThumbnailGO.transform.localScale = Vector3.one * 0.28f;
                realityThumbnailGO.tag = "Reality Thumbnail";
                realityThumbnailContainer._thumbnailList.Add(realityThumbnailGO);

                // create titles
                _prefabTitle.transform.Find("Index").GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + realityCollection.realities[i].realityId;
                _prefabTitle.transform.Find("Title").GetChild(0).GetComponent<TMPro.TMP_Text>().text = realityCollection.realities[i].displayName;
                var titleGO = Instantiate(_prefabTitle, _titleScrollContent);
            }
        }

        void UpdateScrollValue()
        {
            _scrollValue = _horizentalScrollContainer.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value;
            _scrollValue = Mathf.Clamp01(_scrollValue);
            _activeIndex = Mathf.RoundToInt(_scrollValue * (_realityCount - 1));

            realityThumbnailContainer.activeIndex = _activeIndex;
            PanelManager.Instance._realityIndex = _activeIndex;

            // set value to thumbnails container
            var thumbnailContainerPosition = _scrollValue * (_realityCount - 1) * _offset;
            realityThumbnailContainer.currentPostion = thumbnailContainerPosition;
            // set valut to name container, to slide the name with thumbnails
            _verticleScrollContainer.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - _scrollValue;
        }

        void DeletePreviousElement(Transform content)
        {
            var tempList = new List<Transform>();
            for (int i = 0; i < content.childCount; i++)
            {
                tempList.Add(content.GetChild(i));
            }
            foreach (var child in tempList)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void OthersPanelUILayout()
        {
            realityThumbnailContainer.gameObject.SetActive(false);

            transform.Find("HamburgerButton").gameObject.SetActive(false);
        }

        public void DetailsPanelUILayout()
        {
            realityThumbnailContainer.gameObject.SetActive(true);
            realityThumbnailContainer._homePageDeco.gameObject.SetActive(false);

            transform.Find("HamburgerButton").gameObject.SetActive(false);

            for (int i = 0; i < _realityCount; i++)
            {
                if (i == _activeIndex)
                {
                    realityThumbnailContainer._thumbnailList[i].SetActive(true);
                }
                else
                {
                    realityThumbnailContainer._thumbnailList[i].SetActive(false);
                }
            }

            realityThumbnailContainer.offset = new Vector3(0, 1.7f, 0);
        }

        public void HomePanelUIlayout()
        {
            realityThumbnailContainer.gameObject.SetActive(true);
            realityThumbnailContainer._homePageDeco.gameObject.SetActive(true);

            transform.Find("HamburgerButton").gameObject.SetActive(true);

            for (int i = 0; i < _realityCount; i++)
            {
                realityThumbnailContainer._thumbnailList[i].SetActive(true);
            }

            realityThumbnailContainer.offset = new Vector3(0, 0f, 0);
            realityThumbnailContainer.scrollOffset = new Vector3(0, 0f, 0);
        }

        public void RecoverHomePage()
        {
            _activeIndex = PanelManager.Instance._realityIndex;
            UpdateScrollValue();
        }
    }
}