using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ObjectPackageUIPanel : ObjectPackageUI
{
    int _collectionCount;

    Transform _content;
    Transform _prefix;

    [SerializeField] GameObject _collectionScorllView;
    [SerializeField] GameObject _itemContainer;

    public enum MetaType
    {
        Object,
        Avatar
    }

    public MetaType metaType = MetaType.Object;

    private void Awake()
    {
        OnObjectPackageUIAweak();
    }

    protected override void OnObjectPackageUIAweak()
    {
        base.OnObjectPackageUIAweak();

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

        switch (metaType)
        {
            case MetaType.Object:

                _collectionCount = metaObjectCollection.Count;

                if (_content.childCount != _collectionCount)
                {
                    // create new by data
                    for (int i = 0; i < _collectionCount; i++)
                    {
                        var collection = Instantiate(_collectionScorllView);
                        collection.transform.parent = _content;
                        _prefix = collection.transform.Find("Prefix");

                        for (int j = 0; j < metaObjectCollection[i].MetaObject.Count; j++)
                        {
                            var item = Instantiate(_itemContainer);
                            var itemContent = collection.transform.Find("Scroll View/Viewport/Content");
                            item.transform.parent = itemContent;

                            _prefix.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaObjectCollection[i].MetaObject[j].name == null ? "not found name" : metaObjectCollection[i].displayName;
                            _prefix.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaObjectCollection[i].MetaObject[j].tokenId;
                            item.transform.Find("Image").GetComponent<Image>().sprite = metaObjectCollection[i].MetaObject[j].image;
                            item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection[i].MetaObject[j];
                            //item.transform.Find("EnterDetailButton").GetComponent<Button>().onClick.AddListener(() =>
                            //{
                            //    // here we do onclick event of this button
                            //    Debug.Log("EnterDetailButton is clicked.");
                            //});
                        }
                    }
                }
                break;
            case MetaType.Avatar:

                _collectionCount = metaAvatarCollection.Count;

                if (_content.childCount != _collectionCount)
                {
                    // create new by data
                    for (int i = 0; i < _collectionCount; i++)
                    {
                        var collection = Instantiate(_collectionScorllView);
                        collection.transform.parent = _content;
                        _prefix = collection.transform.Find("Prefix");

                        for (int j = 0; j < metaAvatarCollection[i].MetaAvatar.Count; j++)
                        {
                            var item = Instantiate(_itemContainer);
                            var itemContent = collection.transform.Find("Scroll View/Viewport/Content");
                            item.transform.parent = itemContent;

                            _prefix.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaAvatarCollection[i].displayName;
                            _prefix.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaAvatarCollection[i].MetaAvatar[j].tokenId;
                            item.transform.Find("Image").GetComponent<Image>().sprite = metaAvatarCollection[i].MetaAvatar[j].image;
                            item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaAvatar = metaAvatarCollection[i].MetaAvatar[j];
                            //item.transform.Find("EnterDetailButton").GetComponent<Button>().onClick.AddListener(() =>
                            //{
                            //    // here we do onclick event of this button
                            //    Debug.Log("EnterDetailButton is clicked.");
                            //});
                        }
                    }
                }
                break;
        }

    }

    private void Update()
    {
        if (Application.isEditor)
        {

        }
    }
}
