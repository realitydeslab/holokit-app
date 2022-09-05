using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RealityOptionUIPanel : RealityOptionUI
{
    int _collectionCount;

    Transform _content;


    [SerializeField] GameObject _collectionScorllView;
    [SerializeField] GameObject _itemContainer;

    private void Awake()
    {
        OnUIAweak();
    }

    protected override void OnUIAweak()
    {
        base.OnUIAweak();

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

                    item.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = metaObjectCollection[i].displayName;
                    item.transform.Find("ID").GetComponent<TMPro.TMP_Text>().text = "#" + metaObjectCollection[i].metaObjects[j].tokenId;
                    item.transform.Find("Image").GetComponent<Image>().sprite = metaObjectCollection[i].metaObjects[j].image;
                    //item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection[i].MetaObject[j];
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
