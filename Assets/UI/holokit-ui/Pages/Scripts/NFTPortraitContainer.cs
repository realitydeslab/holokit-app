using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    [ExecuteInEditMode]
    public class NFTPortraitContainer : MonoBehaviour
    {
        public CollectionContainer.Type type;
        public MetaObjectCollection metaObjectCollection;
        public MetaAvatarCollection metaAvatarCollection;
        [SerializeField] GameObject _objectPortraitContainer;
        [SerializeField] Transform _content;
        [SerializeField] float _portraitSize = 460;



        [Header("debug")]
        int _count = 0;

        void Awake()
        {
            //transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_portraitSize, _portraitSize+72);
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


            switch (type)
            {
                case CollectionContainer.Type.objectContainer:
                    CreatePortrait(metaObjectCollection);
                    break;
                case CollectionContainer.Type.avatarContainer:
                    CreatePortrait(metaAvatarCollection);
                    break;
            }

            var ss = transform.Find("Scrollbar Horizontal").GetComponent<ScrollBarSlidingAreaStyle>();
            ss.Init(_count);

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

        }
    }
}