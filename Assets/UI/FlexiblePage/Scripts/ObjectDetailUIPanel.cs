using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Holoi.AssetFoundation;

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

        if (metaAvatar)
        {
            var metaItem = metaAvatar;

            _image.sprite = metaItem.image;
            _collectionName.text = metaItem.collection.displayName;
            _ID.text = "#" + metaItem.tokenId;
            _objectName.text = metaItem.name;
            _author.text = metaItem.collection.author;
            _description.text = metaItem.collection.description;
        }
        else
        {
            var metaItem = metaObject;

            _image.sprite = metaItem.image;
            _collectionName.text = metaItem.collection.displayName;
            _ID.text = "#" + metaItem.tokenId;
            _objectName.text = metaItem.name;
            _author.text = metaItem.collection.author;
            _description.text = metaItem.collection.description;
        }
        
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            OnObjectDetailUIAweak();
        }
    }
}
