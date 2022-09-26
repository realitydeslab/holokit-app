using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;
using Holoi.Library.HoloKitApp;
using UnityEngine.UI.Extensions;


namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityOptionUIPanel : MonoBehaviour
    {
        [Header("Meta Datas")]
         List<MetaObjectCollection> _realityCompatibleMetaObjectCollections;
         List<MetaAvatarCollection> _realityCompatibleMetaAvatarCollections;

        [Header("Prefabs")]
        [SerializeField] GameObject _collectionContainer;
        [SerializeField] GameObject _buttonContainer;
        [Header("UI Elements")]
        [SerializeField] Transform _scrollViewObjectCollection;
        [SerializeField] Scrollbar _scrollBarObjectCollection;
        [SerializeField] Transform _scrollViewAvatarCollection;
        [SerializeField] Scrollbar _scrollBarAvatarCollection;
        Transform _contentObj;
        Transform _contentAva;

        [Header("UI Elements")]
        [SerializeField] HoloKitAppLocalPlayerPreferences holoKitAppLocalPlayerPreferences;

        int _objectCount;
        int _avatarCount;
        int _objectDefaultIndex;
        int _avatarDefaultIndex;

        private void Awake()
        {

            _contentObj = _scrollViewObjectCollection.Find("Viewport/Content");
            _contentAva = _scrollViewAvatarCollection.Find("Viewport/Content");

            ClearPreviousElements(_contentObj);
            ClearPreviousElements(_contentAva);

            _scrollViewObjectCollection.GetComponent<CollectionScrollViewUI>().CollectionContainerList.Clear();
            _scrollViewAvatarCollection.GetComponent<CollectionScrollViewUI>().CollectionContainerList.Clear();

            if (Application.isPlaying)
            {
                
            }
            else
            {
                SetUIInfo();
            }

            // todo: sizheng
            _realityCompatibleMetaObjectCollections = holoKitAppLocalPlayerPreferences.GetCompatibleMetaObjectCollectionList(HoloKitApp.Instance.CurrentReality);
            _realityCompatibleMetaAvatarCollections = holoKitAppLocalPlayerPreferences.GetCompatibleMetaAvatarCollectionList(HoloKitApp.Instance.CurrentReality);
        }

        private void Start()
        {
            SetUIInfo();
            SetUIButtons();
        }

        private void Update()
        {

        }

        public void SetUIInfo()
        {
            //Debug.Log("Set Option Page UI Info");
            if (_realityCompatibleMetaObjectCollections.Count > 0)
            {
                _objectCount = _realityCompatibleMetaObjectCollections.Count;

                _scrollViewObjectCollection.gameObject.SetActive(true);
                _scrollViewObjectCollection.GetComponent<CollectionScrollViewUI>().count = _objectCount;

                for (int i = 0; i < _objectCount; i++)
                {
                    // create new by data
                    _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.objectContainer;
                    _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = _realityCompatibleMetaObjectCollections[i];

                    _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.GetComponent<DragButton>().horizontalScrollSnap =
                        _scrollViewObjectCollection.GetComponent<HorizontalScrollSnap>(); ;
                    _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.GetComponent<DragButton>().scorllRect =
                        _scrollViewObjectCollection.GetComponent<ScrollRect>();

                    var collectionContainer = Instantiate(_collectionContainer, _scrollViewObjectCollection.transform.Find("Viewport/Content"));
                    _collectionContainer.GetComponent<RectTransform>().localScale = Vector3.one;
                    collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    _scrollViewObjectCollection.GetComponent<CollectionScrollViewUI>().CollectionContainerList.Add(collectionContainer);
                }
            }
            else
            {
                _scrollViewObjectCollection.gameObject.SetActive(false);
            }
            if (_realityCompatibleMetaAvatarCollections.Count > 0)
            {
                _avatarCount = _realityCompatibleMetaAvatarCollections.Count;
                _scrollViewAvatarCollection.gameObject.SetActive(true);
                _scrollViewAvatarCollection.GetComponent<CollectionScrollViewUI>().count = _avatarCount;

                for (int i = 0; i < _avatarCount; i++)
                {
                    // create new by data
                    _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.avatarContainer;
                    _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = _realityCompatibleMetaAvatarCollections[i];

                    _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.GetComponent<DragButton>().horizontalScrollSnap =
                        _scrollViewAvatarCollection.GetComponent<HorizontalScrollSnap>(); ;
                    _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.GetComponent<DragButton>().scorllRect =
                        _scrollViewAvatarCollection.GetComponent<ScrollRect>();

                    var collectionContainer = Instantiate(_collectionContainer, _scrollViewAvatarCollection.transform.Find("Viewport/Content"));
                    _collectionContainer.GetComponent<RectTransform>().localScale = Vector3.one;
                    collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    _scrollViewAvatarCollection.GetComponent<CollectionScrollViewUI>().CollectionContainerList.Add(collectionContainer);
                }

            }
            else
            {
                _scrollViewAvatarCollection.gameObject.SetActive(false);
            }

            ScrollBarSetInitValue();
        }

        public void SetUIButtons()
        {

            var sceneName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().SceneName;

            var scene = new ScreenARMainScene();

            scene._sceneName = sceneName;

            // Create Buttons base on what scene this reality have:

            // var buttonContainer = Instantiate(_buttonContainer, _content);

            // create interval spacing container:

            // create button by reality support:

            // Create Footer:
        }

        void ScrollBarSetInitValue()
        {
            var _reality = HoloKitApp.Instance.CurrentReality;

            var metaObject = holoKitAppLocalPlayerPreferences.GetRealityPreferencedObject(_reality);
            if (metaObject != null)
            {
                var selectedObject = metaObject;
                _objectDefaultIndex = GetIndex(selectedObject);
                _scrollBarObjectCollection.value = ScrollBarIndexToValue(_objectDefaultIndex, _objectCount);
                //Debug.Log($"_scrollBarObjectCollection.value = {_scrollBarObjectCollection.value}");

            }
            else
            {
                Debug.Log("not found object");

                _scrollBarObjectCollection.value = 0;
            }

            var metaAvatar = holoKitAppLocalPlayerPreferences.GetRealityPreferencedAvatar(_reality);
            if (metaAvatar != null)
            {
                var selectedAvatar = metaAvatar;
                _avatarDefaultIndex = GetIndex(selectedAvatar);
                _scrollBarAvatarCollection.value = ScrollBarIndexToValue(_avatarDefaultIndex, _avatarCount);
                //Debug.Log($"_scrollBarAvatarCollection.value = {_scrollBarAvatarCollection.value}");

            }
            else
            {
                Debug.Log("not found avatar");
                _scrollBarAvatarCollection.value = 0;
            }
        }

        float ScrollBarIndexToValue(int index, int count)
        {
            float value;
            if (count > 1)
            {
                value = index / (count - 1f);
            }
            else
            {
                value = 0;
            }
            //Debug.Log($"index to value: {value}");
            return value;
        }

        int GetIndex(MetaObject metaItem)
        {
            var collection = metaItem.collection;

            for (int i = 0; i < collection.metaObjects.Count; i++)
            {
                if (metaItem == collection.metaObjects[i])
                {
                    //Debug.Log($"selected object is the {i}");
                    return i;
                }
            }

            return 0;
        }

        int GetIndex(MetaAvatar metaItem)
        {
            var collection = metaItem.collection;

            for (int i = 0; i < collection.metaAvatars.Count; i++)
            {
                if (metaItem == collection.metaAvatars[i])
                {
                    //Debug.Log($"selected avatar is the {i}");

                    return i;
                }
            }

            return 0;
        }

        void ClearPreviousElements(Transform content)
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
    }
}