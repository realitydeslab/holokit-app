using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class CollectionContainer : MonoBehaviour
    {
        public enum Type
        {
            objectContainer,
            avatarContainer
        }

        public Type type;
        public MetaObjectCollection metaObjectCollection;
        public MetaAvatarCollection metaAvatarCollection;
        [Header("Prefabs")]
        [SerializeField] GameObject _portraitScorllView;
        [Header("UI Elements")]
        [SerializeField] Image _background;
        [SerializeField] TMPro.TMP_Text _title;
        [SerializeField] TMPro.TMP_Text _id;
        [SerializeField] Transform _scrollviewContainer;


        private void Awake()
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
            _title.text = moc.displayName;
            _id.text = moc.metaObjects[0].tokenId;

            // create new by data
            for (int i = 0; i < 1; i++)
            {
                _portraitScorllView.GetComponent<NFTPortraitContainer>().type = Type.objectContainer;
                _portraitScorllView.GetComponent<NFTPortraitContainer>().metaObjectCollection = moc;
                var portraitContainer = Instantiate(_portraitScorllView, _scrollviewContainer);
                portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one; // lock for a auto-huge scale number around 250

                _id.text = "#" + metaObjectCollection.metaObjects[i].tokenId;
            }
        }
        void CreatePotraitScorllView(MetaAvatarCollection mac)
        {
            _title.text = mac.displayName;
            _id.text = mac.metaAvatars[0].tokenId;

            // create new by data
            for (int i = 0; i < 1; i++)
            {
                _portraitScorllView.GetComponent<NFTPortraitContainer>().type = Type.avatarContainer;
                _portraitScorllView.GetComponent<NFTPortraitContainer>().metaAvatarCollection = mac;
                var portraitContainer = Instantiate(_portraitScorllView, _scrollviewContainer);
                portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one;

                _id.text = "#" + mac.metaAvatars[i].tokenId;
            }
        }

        void Update()
        {

        }
    }
}