using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;
namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class ObjectPackageUIPanel : MonoBehaviour
    {
        public CollectionContainer.Type type;
        public MetaObjectCollectionList metaObjectCollectionList;
        public MetaAvatarCollectionList metaAvatarCollectionList;

        int _count;

        [SerializeField] ScrollRect _scrollView;
        [SerializeField] Transform _content;

        [SerializeField] GameObject _collectionContainer;
        [SerializeField] GameObject _footer;

        private void Awake()
        {
            ClearPreviousGenerativeContent();

            switch (type)
            {
                case CollectionContainer.Type.objectContainer:
                    CreateCollectionContainer(metaObjectCollectionList);
                    break;
                case CollectionContainer.Type.avatarContainer:
                    CreateCollectionContainer(metaAvatarCollectionList);
                    break;
            }


            CreateFooter();
        }

        void ClearPreviousGenerativeContent()
        {
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

        void CreateCollectionContainer(MetaObjectCollectionList mocl)
        {
            _count = mocl.list.Count;

            // create new by data
            for (int i = 0; i < _count; i++)
            {
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.objectContainer;
                /*
                 * wait add theme tag to collection
                 * if(metaObjectCollection.themt == dark)
                 * _collectionContainer.GetComponent<CollectionContainer>().theme = Darl;
                */
                _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = mocl.list[i];
                _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.
                    GetComponent<DragButton>().scorllRect = _scrollView;
                var collection = Instantiate(_collectionContainer, _content);
                collection.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }

        void CreateCollectionContainer(MetaAvatarCollectionList macl)
        {
            _count = macl.list.Count;

            // create new by data
            for (int i = 0; i < _count; i++)
            {
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.avatarContainer;
                /*
                 * wait add theme tag to collection
                 * if(metaObjectCollection.themt == dark)
                 * _collectionContainer.GetComponent<CollectionContainer>().theme = Darl;
                */
                _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = macl.list[i];
                _collectionContainer.GetComponent<CollectionContainer>().emptyDragButton.
                    GetComponent<DragButton>().scorllRect = _scrollView;
                var collection = Instantiate(_collectionContainer, _content);
                collection.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }

        void CreateFooter()
        {
            var footer = Instantiate(_footer);
            footer.transform.parent = _content;
        }
    }
}