using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityOptionUIPanel : MonoBehaviour
    {
        [Header("Meta Datas")]
        public List<MetaObjectCollection> realityMetaObjectCollections;
        public List<MetaAvatarCollection> realityMetaAvatarCollections;

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


            //SetUIInfo();
            //SetUIButtons();
        }

        public void SetUIInfo()
        {
            if (realityMetaObjectCollections.Count > 0)
            {
                _objectCount = realityMetaObjectCollections.Count;

                _scrollViewObjectCollection.gameObject.SetActive(true);
                _scrollViewObjectCollection.GetComponent<CollectionScrollViewUI>().count = _objectCount;

                for (int i = 0; i < _objectCount; i++)
                {
                    // create new by data
                    _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.objectContainer;
                    _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = realityMetaObjectCollections[i];
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
            if (realityMetaAvatarCollections.Count > 0)
            {
                _avatarCount = realityMetaAvatarCollections.Count;
                _scrollViewAvatarCollection.gameObject.SetActive(true);
                _scrollViewAvatarCollection.GetComponent<CollectionScrollViewUI>().count = _avatarCount;

                for (int i = 0; i < _avatarCount; i++)
                {
                    // create new by data
                    _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.avatarContainer;
                    _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = realityMetaAvatarCollections[i];
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

            //SetSelectState();
        }

        public void SetUIButtons()
        {
            // Create Buttons base on what scene this reality have:

            // var buttonContainer = Instantiate(_buttonContainer, _content);

            // create interval spacing container:

            // create button by reality support:

            // Create Footer:
        }

        void SetSelectState()
        {
            var _reality = HoloKitApp.Instance.CurrentReality;
            //Debug.Log("CurrentReality" + _reality.name);
            Debug.Log(holoKitAppLocalPlayerPreferences);
            Debug.Log(holoKitAppLocalPlayerPreferences.RealityPreferences[_reality]);

            var selectedObject = holoKitAppLocalPlayerPreferences.RealityPreferences[_reality].MetaObject;
            var selectedAvatar = holoKitAppLocalPlayerPreferences.RealityPreferences[_reality].MetaAvatar;

            _objectDefaultIndex = GetIndex(selectedObject);
            _avatarDefaultIndex = GetIndex(selectedAvatar);

            _scrollBarObjectCollection.value = ScrollBarIndexToValue(_objectDefaultIndex, _objectCount);
            _scrollBarAvatarCollection.value = ScrollBarIndexToValue(_avatarDefaultIndex, _avatarCount);
        }

        float ScrollBarIndexToValue(int index, int count)
        {
            float value = index / (count - 1f);
            return value;
        }

        int GetIndex(MetaObject metaItem)
        {
            var collection = metaItem.collection;

            for (int i = 0; i < collection.metaObjects.Count; i++)
            {
                if (metaItem == collection.metaObjects[i])
                {
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