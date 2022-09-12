using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;
namespace Holoi.HoloKit.App.UI
{
    [ExecuteInEditMode]
    public class ObjectPackageUIPanel : MonoBehaviour
    {
        public CollectionContainer.Type type;
        public MetaObjectCollectionList metaObjectCollectionList;
        public MetaAvatarCollectionList metaAvatarCollectionList;

        int _count;

        [SerializeField] Transform _content;

        [SerializeField] GameObject _collectionContainer;
        [SerializeField] GameObject _footer;

        private void Awake()
        {
            DeletePreviousElement();

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

        void DeletePreviousElement()
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
                _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = mocl.list[i];
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
                _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = macl.list[i];
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