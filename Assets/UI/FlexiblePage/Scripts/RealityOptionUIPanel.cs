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
    [SerializeField] GameObject _collectionScrollContainer;
    [SerializeField] GameObject _collectionContainer;
    [SerializeField] GameObject _buttonContainer;

    int _count;
    Transform _content;

    private void Awake()
    {
        
        _content = transform.Find("Scroll View/Viewport/Content");

        DeletePreviousElement();


        if (metaObjectCollections.Count > 0)
        {
            _count = metaObjectCollections.Count;
            var _scrollViewObject = Instantiate(_collectionScrollContainer, _content);
            _scrollViewObject.transform.Find("YourObjectButton/Text").GetComponent<TMPro.TMP_Text>().text = "Your Object";

            for (int i = 0; i < _count; i++)
            {

                // create new by data
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.objectContainer;
                _collectionContainer.GetComponent<CollectionContainer>().metaObjectCollection = metaObjectCollections[i];
                var collectionContainer = Instantiate(_collectionContainer);
                collectionContainer.transform.parent = _scrollViewObject.transform.Find("Content");
                collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
        }
        else
        {

        }
        if (metaAvatarCollections.Count > 0)
        {
            _count = metaAvatarCollections.Count;

            var _scrollViewAvatar = Instantiate(_collectionScrollContainer, _content);
            _scrollViewAvatar.transform.Find("YourObjectButton/Text").GetComponent<TMPro.TMP_Text>().text = "Your Avatar";


            for (int i = 0; i < _count; i++)
            {
                // create new by data
                _collectionContainer.GetComponent<CollectionContainer>().type = CollectionContainer.Type.avatarContainer;
                _collectionContainer.GetComponent<CollectionContainer>().metaAvatarCollection = metaAvatarCollections[i];
                var collectionContainer = Instantiate(_collectionContainer);
                collectionContainer.transform.parent = _scrollViewAvatar.transform.Find("Content");
                collectionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }

        }
        else
        {
        }

        var buttons = Instantiate(_buttonContainer, _content);

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
}
