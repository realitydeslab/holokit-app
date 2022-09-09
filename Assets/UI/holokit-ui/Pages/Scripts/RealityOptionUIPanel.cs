using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

[ExecuteInEditMode]
public class RealityOptionUIPanel : MonoBehaviour
{

    public List<MetaObjectCollection> metaObjectCollections;
    public List<MetaAvatarCollection> metaAvatarCollections;

    [Header("Prefabs")]
    [SerializeField] Transform _scrollViewCollectionObject;
    [SerializeField] Transform _scrollViewCollectionAvatar;
    [SerializeField] GameObject _collectionContainer;
    [SerializeField] GameObject _buttonContainer;

    int _count;
    Transform _contentObj;
    Transform _contentAva;

    private void Awake()
    {

        _contentObj = _scrollViewCollectionObject.Find("Viewport/Content");
        _contentAva = _scrollViewCollectionAvatar.Find("Viewport/Content");

        ClearLastElements(_contentObj);
        ClearLastElements(_contentAva);
        //ClearLastElements(transform.Find("Scroll View/Viewport/Content"));
        // delete:


        if (metaObjectCollections.Count > 0)
        {
            _scrollViewCollectionObject.gameObject.SetActive(true);
            _count = metaObjectCollections.Count;
            //var _scrollViewObject = Instantiate(_collectionScrollContainer, _content);
            //_scrollViewObject.transform.Find("YourObjectButton/Text").GetComponent<TMPro.TMP_Text>().text = "Your Object";

            for (int i = 0; i < _count; i++)
            {

                // create new by data
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.objectContainer;
                _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = metaObjectCollections[i];
                var collectionContainer = Instantiate(_collectionContainer, _scrollViewCollectionObject.transform.Find("Viewport/Content"));
                _collectionContainer.GetComponent<RectTransform>().localScale = Vector3.one;
                collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
        }
        else
        {
            _scrollViewCollectionObject.gameObject.SetActive(false);

        }
        if (metaAvatarCollections.Count > 0)
        {
            _scrollViewCollectionAvatar.gameObject.SetActive(true);

            _count = metaAvatarCollections.Count;

            //var _scrollViewAvatar = Instantiate(_collectionScrollContainer, _content);
            //_scrollViewAvatar.transform.Find("YourObjectButton/Text").GetComponent<TMPro.TMP_Text>().text = "Your Avatar";


            for (int i = 0; i < _count; i++)
            {
                // create new by data
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.avatarContainer;
                _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = metaAvatarCollections[i];
                var collectionContainer = Instantiate(_collectionContainer, _scrollViewCollectionAvatar.transform.Find("Viewport/Content"));
                _collectionContainer.GetComponent<RectTransform>().localScale = Vector3.one;
                collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }

        }
        else
        {
            _scrollViewCollectionAvatar.gameObject.SetActive(false);

        }

        //var buttons = Instantiate(_buttonContainer, _content);

        // create interval spacing container:

        // create button by reality support:

        // create footer:

    }

    void ClearLastElements(Transform content)
    {
        var tempList = new List<Transform>();

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i);
            if(child.name == "Object Scroll Snap Object" || child.name == "Object Scroll Snap Avatar")
            {
            }
            else
            {
                tempList.Add(content.GetChild(i));
            }
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
