using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class PortraitScrollViewUI : MonoBehaviour
    {
        [Header("Data Type")]
        public CollectionContainer.Type type;

        [Header("Meta Datas")]
        public MetaObjectCollection metaObjectCollection;
        public MetaAvatarCollection metaAvatarCollection;
        public int _activeIndex;
        float _scrollValue;

        [Header("Prefabs")]
        [SerializeField] GameObject _objectPortraitContainer;

        [Header("UI Elements")]
        [SerializeField] Transform _content;
        [SerializeField] ScrollBarSlidingAreaStyle _scrollBar;
        [SerializeField] float _portraitSize = 480;

        [Header("Theme")]
        public CollectionContainer.Theme theme;

        int _count = 0;

        void Awake()
        {

            ClearPreviousGenerativeContent();
            SetUIAppearance();
            CreateScrollViewBasedOnType();

            _scrollBar.Init(_count);
        }

        void ClearPreviousGenerativeContent()
        {
            Debug.Log("update content!");
            // clear all gameobject
            var tempList = new List<Transform>();
            for (int i = 0; i < _content.childCount; i++)
            {
                tempList.Add(_content.GetChild(i));
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

        void SetUIAppearance()
        {
            switch (theme)
            {
                case CollectionContainer.Theme.Dark:
                    _scrollBar.color = Color.white;
                    break;
                case CollectionContainer.Theme.Bright:
                    _scrollBar.color = Color.black;
                    break;
            }
        }

        void CreateScrollViewBasedOnType()
        {
            switch (type)
            {
                case CollectionContainer.Type.objectContainer:
                    CreatePortrait(metaObjectCollection);
                    break;
                case CollectionContainer.Type.avatarContainer:
                    CreatePortrait(metaAvatarCollection);
                    break;
            }
        }

        void CreatePortrait(MetaObjectCollection moc)
        {
            _count = moc.metaObjects.Count;

            for (int i = 0; i < _count; i++)
            {
                var go = Instantiate(_objectPortraitContainer, _content);
                go.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_portraitSize, _portraitSize);
                go.transform.Find("Image").GetComponent<Image>().sprite = moc.metaObjects[i].image;
            }
        }

        void CreatePortrait(MetaAvatarCollection mac)
        {
            _count = mac.metaAvatars.Count;

            for (int i = 0; i < _count; i++)
            {
                var go = Instantiate(_objectPortraitContainer, _content);
                go.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_portraitSize, _portraitSize);
                go.transform.Find("Image").GetComponent<Image>().sprite = mac.metaAvatars[i].image;
            }
        }

        void Update()
        {
            _scrollValue = transform.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value;
            _scrollValue = Mathf.Clamp01(_scrollValue);
            _activeIndex = Mathf.RoundToInt(_scrollValue * (_count - 1));
        }
    }
}