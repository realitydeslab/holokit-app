using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ObjectDetailUIPanel : ObjectDetailUI
{
    [SerializeField] Image _image;
    [SerializeField] TMPro.TMP_Text _ID;
    [SerializeField] TMPro.TMP_Text _collectionName;
    [SerializeField] TMPro.TMP_Text _objectName;
    [SerializeField] TMPro.TMP_Text _author;
    [SerializeField] TMPro.TMP_Text _description;
    [SerializeField] TMPro.TMP_Text _siteAddress;

    private void Awake()
    {
        OnObjectDetailUIAweak();
    }

    protected override void OnObjectDetailUIAweak()
    {
        base.OnObjectDetailUIAweak();

        _image.sprite = metaObject.image;
        _collectionName.text = metaObject.collection.displayName;
        _ID.text = "#" + metaObject.tokenId;
        _objectName.text = metaObject.name;
        _author.text = metaObject.collection.author;
        _description.text = metaObject.collection.description;
        //_siteAddress.text = metaObject.collection.;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            OnObjectDetailUIAweak();
        }
    }
}
