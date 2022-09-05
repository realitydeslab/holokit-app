using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ObjectPackageUIPanel : ObjectPackageUI
{
    int _collectionCount;

    Transform _content;

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

        switch (metaType)
        {
            case MetaType.Object:

                _collectionCount = metaObjectCollection.Count;

                _content = transform.Find("Scroll View/Viewport/Content");

                if (_content.childCount != _collectionCount)
                {
                    Debug.Log("update content!");
                    // clear all gameobject
                    foreach (Transform child in _content.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    // create new by data
                    for (int i = 0; i < _collectionCount; i++)
                    {
                        var collection = Instantiate(_collectionScorllView);
                        collection.transform.parent = _content;

                        for (int j = 0; j < metaObjectCollection[i].metaObjects.Count; j++)
                        {
                            var item = Instantiate(_itemContainer);
                            var itemContent = collection.transform.Find("Viewport/Content");
                            item.transform.parent = itemContent;

                            item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaObjectCollection[i].metaObjects[j].name == null ? "not found name" : metaObjectCollection[i].displayName;
                            item.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaObjectCollection[i].metaObjects[j].tokenId;
                            item.transform.Find("Image").GetComponent<Image>().sprite = metaObjectCollection[i].metaObjects[j].image;
                            item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection[i].metaObjects[j];
                            item.transform.Find("EnterDetailButton").GetComponent<Button>().onClick.AddListener(() =>
                            {
                                // here we do onclick event of this button
                                Debug.Log("EnterDetailButton is clicked.");
                            });

                            Debug.Log("item count: " + metaObjectCollection[i].metaObjects.Count);
                            Debug.Log("Name: " + item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text);
                        }
                    }
                }
                break;
            case MetaType.Avatar:

                _collectionCount = metaAvatarCollection.Count;

                _content = transform.Find("Scroll View/Viewport/Content");

                if (_content.childCount != _collectionCount)
                {
                    Debug.Log("update content!");
                    // clear all gameobject
                    foreach (Transform child in _content.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    // create new by data
                    for (int i = 0; i < _collectionCount; i++)
                    {
                        var collection = Instantiate(_collectionScorllView);
                        collection.transform.parent = _content;

                        for (int j = 0; j < metaAvatarCollection[i].metaAvatars.Count; j++)
                        {
                            var item = Instantiate(_itemContainer);
                            var itemContent = collection.transform.Find("Viewport/Content");
                            item.transform.parent = itemContent;

                            item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaAvatarCollection[i].displayName;
                            item.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaAvatarCollection[i].metaAvatars[j].tokenId;
                            item.transform.Find("Image").GetComponent<Image>().sprite = metaAvatarCollection[i].metaAvatars[j].image;
                            item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaAvatar = metaAvatarCollection[i].metaAvatars[j];
                            //item.transform.Find("EnterDetailButton").GetComponent<Button>().onClick.AddListener(() =>
                            //{
                            //    // here we do onclick event of this button
                            //    Debug.Log("EnterDetailButton is clicked.");
                            //});

                            Debug.Log("item count: " + metaAvatarCollection[i].metaAvatars.Count);
                            Debug.Log("Name: " + item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text);
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
