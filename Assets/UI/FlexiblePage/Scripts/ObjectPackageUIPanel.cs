using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ObjectPackageUIPanel : ObjectPackageUI
{
    Canvas _canvas;

    int _collectionCount;

    Transform _content;

    UnityEvent _enterDetialPageEvent;

    [SerializeField] GameObject _collectionScorllView;
    [SerializeField] GameObject _itemContainer;

    private void Awake()
    {
        OnObjectPackageUIAweak();
    }

    protected override void OnObjectPackageUIAweak()
    {
        base.OnObjectPackageUIAweak();

        _collectionCount = CollectionLists.Count;
        Debug.Log("item count:" + _collectionCount);

        _content = transform.Find("Scroll View/Viewport/Content");
        if (_content)
        {

        }
        else
        {
            Debug.Log("not found content");
        }

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

                for (int j = 0; j < CollectionLists[i].MetaObject.Count; j++)
                {
                    var item = Instantiate(_itemContainer);
                    var itemContent = collection.transform.Find("Viewport/Content");
                    if (itemContent)
                    {

                    }
                    else
                    {
                        Debug.Log("not found itemContent");
                    }
                    item.transform.parent = itemContent;

                    item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = CollectionLists[i].MetaObject[j].name == null?"not found name": CollectionLists[i].displayName;
                    item.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + CollectionLists[i].MetaObject[j].tokenId;
                    item.transform.Find("Image").GetComponent<Image>().sprite = CollectionLists[i].MetaObject[j].image;
                    item.transform.Find("EnterDtailButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // here we do onclick event of this button
                        Debug.Log("EnterDtailButton is clicked.");
                        _enterDetialPageEvent?.Invoke();
                    });

                    Debug.Log("item count: " + CollectionLists[i].MetaObject.Count);
                    Debug.Log("Name: " + item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text);
                }
            }
        }
    }

    private void Update()
    {
        if (Application.isEditor)
        {

        }
    }
}
