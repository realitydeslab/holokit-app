using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

[ExecuteInEditMode]
public class ObjectPackageUIPanel : MonoBehaviour
{

    public MetaObjectCollectionList metaObjectCollectionList;

    int _listCount;

    [SerializeField] Transform _content;

    [SerializeField] GameObject _collectionContainer;
    [SerializeField] GameObject _footer;

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

        _listCount = metaObjectCollectionList.list.Count;


        // create new by data
        for (int i = 0; i < _listCount; i++)
        {
            _collectionContainer.GetComponent<CollectionDisplayContainer>().metaObjectCollection = metaObjectCollectionList.list[i];
            var collection = Instantiate(_collectionContainer);
            collection.transform.parent = _content;
        }

        var footer = Instantiate(_footer);
        footer.transform.parent = _content;
    }
}
