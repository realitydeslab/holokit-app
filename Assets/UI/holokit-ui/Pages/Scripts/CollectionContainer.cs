using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

[ExecuteInEditMode]
public class CollectionContainer : MonoBehaviour
{
    public enum Type
    {
        objectContainer,
        avatarContainer
    }

    public Type type;
    public MetaObjectCollection metaObjectCollection;
    public MetaAvatarCollection metaAvatarCollection;
    [Header("Prefabs")]
    [SerializeField] GameObject _portraitContainerHorizentalScroll;
    [Header("UI Elements")]
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

        switch (type)
        {
            case Type.objectContainer:
                CreateCollectionContainer(metaObjectCollection);
                break;
            case Type.avatarContainer:
                CreateCollectionContainer(metaAvatarCollection);
                break;
        }


    }

    void CreateCollectionContainer(MetaObjectCollection moc)
    {
        _title.text = moc.displayName;
        _id.text = moc.metaObjects[0].tokenId;

        // create new by data
        for (int i = 0; i < moc.metaObjects.Count; i++)
        {
            _portraitContainerHorizentalScroll.GetComponent<NFTPortraitContainer>().type = Type.objectContainer;
            _portraitContainerHorizentalScroll.GetComponent<NFTPortraitContainer>().metaObjectCollection = moc;
            var portraitContainer = Instantiate(_portraitContainerHorizentalScroll, _content);
            portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one; // lock for a auto-huge scale number around 250

            _id.text = "#" + metaObjectCollection.metaObjects[i].tokenId;
        }
    }
    void CreateCollectionContainer(MetaAvatarCollection mac)
    {
        _title.text = mac.displayName;
        _id.text = mac.metaAvatars[0].tokenId;

        // create new by data
        for (int i = 0; i < mac.metaAvatars.Count; i++)
        {
            _portraitContainerHorizentalScroll.GetComponent<NFTPortraitContainer>().type = Type.avatarContainer;
            _portraitContainerHorizentalScroll.GetComponent<NFTPortraitContainer>().metaAvatarCollection = mac;
            var portraitContainer = Instantiate(_portraitContainerHorizentalScroll, _content);
            portraitContainer.GetComponent<RectTransform>().localScale = Vector3.one;

            _id.text = "#" + mac.metaAvatars[i].tokenId;
        }
    }

    void Update()
    {
        
    }
}
