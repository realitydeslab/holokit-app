using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RealityOptionUIPanel : RealityOptionUI
{
    int _collectionCount;
    Transform _content;
    Transform _prefix;

    //[Header("Components")]
    //[SerializeField] GameObject _scrollViewObject;
    //[SerializeField] GameObject _scrollViewAvatar;



    [Header("Prefabs")]
    [SerializeField] GameObject _scrollViewObject;
    [SerializeField] GameObject _scrollViewAvatar;
    [SerializeField] GameObject _collectionContainer;
    [SerializeField] GameObject _itemContainer;
    [SerializeField] GameObject _buttonContainer;

    private void Awake()
    {
        OnUIAweak();
    }

    protected override void OnUIAweak()
    {
        base.OnUIAweak();

        _collectionCount = metaObjectCollections.Count;
        _content = transform.Find("Scroll View/Viewport/Content");
        
        Debug.Log("update content!");
        // clear all gameobject
        var tempList = new List<Transform>();
        for (int i = 0; i < _content.childCount; i++)
        {
            tempList.Add(_content.GetChild(i));
        }
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }



        if (metaObjectCollections.Count > 0)
        {
            Debug.Log("create object sv");
            //_scrollViewObject.SetActive(true);
            var _scrollViewObject = Instantiate(this._scrollViewObject, _content);

            for (int i = 0; i < _collectionCount; i++)
            {

                // create new by data
                var collection = Instantiate(_collectionContainer);
                collection.transform.parent = _scrollViewObject.transform.Find("Viewport/Content");
                _prefix = collection.transform.Find("Prefix");

                for (int j = 0; j < metaObjectCollections[i].metaObjects.Count; j++)
                {
                    var item = Instantiate(_itemContainer);
                    var itemContent = collection.transform.Find("Scroll View/Viewport/Content");

                    item.transform.parent = itemContent;

                    _prefix.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaObjectCollections[i].displayName;
                    _prefix.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaObjectCollections[i].metaObjects[j].tokenId;
                    item.transform.Find("Image").GetComponent<Image>().sprite = metaObjectCollections[i].metaObjects[j].image;
                    //item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection[i].MetaObject[j];
                }
            }
        }
        else
        {
            //_scrollViewObject.SetActive(false);

        }
        if (metaAvatarCollections.Count > 0)
        {
            Debug.Log("create ava sv");
            //_scrollViewAvatar.SetActive(true);
            var _scrollViewAvatar = Instantiate(this._scrollViewAvatar, _content);

            for (int i = 0; i < _collectionCount; i++)
            {

                // create new by data
                var collection = Instantiate(_collectionContainer);
                collection.transform.parent = _scrollViewAvatar.transform.Find("Viewport/Content");
                _prefix = collection.transform.Find("Prefix");

                for (int j = 0; j < metaAvatarCollections[i].metaAvatars.Count; j++)
                {
                    var item = Instantiate(_itemContainer);
                    var itemContent = collection.transform.Find("Scroll View/Viewport/Content");

                    item.transform.parent = itemContent;

                    _prefix.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaAvatarCollections[i].displayName;
                    _prefix.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaAvatarCollections[i].metaAvatars[j].tokenId;
                    item.transform.Find("Image").GetComponent<Image>().sprite = metaAvatarCollections[i].metaAvatars[j].image;
                    //item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection[i].MetaObject[j];
                }
            }

        }
        else
        {
            //_scrollViewAvatar.SetActive(false);

        }

        var buttons = Instantiate(_buttonContainer, _content);

    }

    private void Update()
    {
        if (Application.isEditor)
        {

        }
    }
}
