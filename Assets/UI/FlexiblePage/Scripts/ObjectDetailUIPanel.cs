using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

[ExecuteInEditMode]
public class ObjectDetailUIPanel : MonoBehaviour
{
    public MetaObject metaObject;
    [SerializeField] Image _image;
    [SerializeField] TMPro.TMP_Text _ID;
    [SerializeField] TMPro.TMP_Text _collectionName;
    [SerializeField] TMPro.TMP_Text _objectName;
    [SerializeField] TMPro.TMP_Text _author;
    [SerializeField] TMPro.TMP_Text _description;
    [SerializeField] TMPro.TMP_Text _siteAddress;

    private void Awake()
    {
        _image.sprite = metaObject.image;
        _collectionName.text = metaObject.collection.displayName;
        _ID.text = "#" + metaObject.tokenId;
        _objectName.text = metaObject.name;
        _author.text = metaObject.collection.author;
        _description.text = metaObject.collection.description;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
        }
    }
}
