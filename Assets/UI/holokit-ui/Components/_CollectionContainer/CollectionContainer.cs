using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    [ExecuteInEditMode]
    public class CollectionContainer : MonoBehaviour
    {
        public enum Theme
        {
            Dark = 0,
            Bright = 1
        }

        public enum Type
        {
            objectContainer = 0,
            avatarContainer = 1
        }

        [Header("Meta Data")]
        public MetaObjectCollection metaObjectCollection;
        public MetaAvatarCollection metaAvatarCollection;
        [Header("Container Type")]
        public Type type;
        [Header("Prefabs")]
        [SerializeField] GameObject _portraitScorllView;
        [Header("UI Elements")]
        [SerializeField] FlexibleUIText _title;
        [SerializeField] FlexibleUIText _id;
        [SerializeField] Image _background;
        [SerializeField] Image _divider;
        [SerializeField] Transform _scrollviewContainer;
        [Header("Theme")]
        public Theme theme;

        private void Awake()
        {
            ClearPreviousGenerativeContent();
            SetUITheme();
            CreateScrollViewBasedOnType();
        }

        void ClearPreviousGenerativeContent()
        {
            Debug.Log("update content!");
            // clear all gameobject
            var tempList = new List<Transform>();
            for (int i = 0; i < _scrollviewContainer.childCount; i++)
            {
                tempList.Add(_scrollviewContainer.GetChild(i));
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

        void SetUITheme()
        {
            switch (theme)
            {
                case Theme.Dark:
                    _background.color = Color.black;
                    _divider.color = Color.white;
                    _title.color = FlexibleUIText.HolokitColor.White;
                    _id.color = FlexibleUIText.HolokitColor.White;
                    break;
                case Theme.Bright:
                    _background.color = Color.white;
                    _divider.color = Color.black;
                    _title.color = FlexibleUIText.HolokitColor.Black;
                    _id.color = FlexibleUIText.HolokitColor.Black;
                    break;
            }
        }

        void CreateScrollViewBasedOnType()
        {
            switch (type)
            {
                case Type.objectContainer:
                    CreatePotraitScorllView(metaObjectCollection);
                    break;
                case Type.avatarContainer:
                    CreatePotraitScorllView(metaAvatarCollection);
                    break;
            }
        }

        void CreatePotraitScorllView(MetaObjectCollection moc)
        {
            ClearPreviousGenerativeContent();

            _title.text.text = moc.displayName;
            _id.text.text = moc.metaObjects[0].tokenId;

            // create new by data
            for (int i = 0; i < 1; i++)
            {
                _portraitScorllView.GetComponent<PortraitScrollView>().type =type;
                _portraitScorllView.GetComponent<PortraitScrollView>().theme = theme;
                _portraitScorllView.GetComponent<PortraitScrollView>().metaObjectCollection = moc;
                var portraitContainer = Instantiate(_portraitScorllView, _scrollviewContainer);
                portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one; // lock for a bug scale number around 250

                _id.text.text = "#" + metaObjectCollection.metaObjects[i].tokenId;
            }
        }
        void CreatePotraitScorllView(MetaAvatarCollection mac)
        {
            _title.text.text = mac.displayName;
            _id.text.text = mac.metaAvatars[0].tokenId;

            // create new by data
            for (int i = 0; i < 1; i++)
            {
                _portraitScorllView.GetComponent<PortraitScrollView>().type = type;
                _portraitScorllView.GetComponent<PortraitScrollView>().theme = theme;
                _portraitScorllView.GetComponent<PortraitScrollView>().metaAvatarCollection = mac;
                var portraitContainer = Instantiate(_portraitScorllView, _scrollviewContainer);
                portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one;

                _id.text.text = "#" + mac.metaAvatars[i].tokenId;
            }
        }

        void Update()
        {

        }
    }
}