using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

[ExecuteInEditMode]
public class CollectionDisplayContainer : MonoBehaviour
{
    public MetaObjectCollection metaObjectCollection;

    [SerializeField] GameObject _itemContainer;

    [SerializeField] Image _background;
    [SerializeField] TMPro.TMP_Text _title;
    [SerializeField] TMPro.TMP_Text _id;
    [SerializeField] Transform _content;


    private void Awake()
    {

        Debug.Log("update content!");
        // clear all gameobject
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

        // init
        _title.text = metaObjectCollection.displayName;
        _id.text = metaObjectCollection.metaObjects[0].tokenId;

        // create new by data
        for (int i = 0; i < metaObjectCollection.metaObjects.Count; i++)
        {
            var item = Instantiate(_itemContainer);
            item.transform.parent = _content;

            _id.text = "#" + metaObjectCollection.metaObjects[i].tokenId;
            item.transform.Find("Image").GetComponent<Image>().sprite = metaObjectCollection.metaObjects[i].image;
            item.transform.Find("EnterDetailButton").GetComponent<ObjectPackageObjectButtonDescription>().metaObject = metaObjectCollection.metaObjects[i];
            //item.transform.Find("EnterDetailButton").GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    // here we do onclick event of this button
            //    Debug.Log("EnterDetailButton is clicked.");
            //});
        }
    }

    void Update()
    {
        
    }
}
