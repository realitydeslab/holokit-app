using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class HomeUIPanel : MonoBehaviour
{
    public RealityList realityCollection;
    Canvas _canvas;
    RealityThumbnailContainer _realityThumbnailContainer;

    [Header("Prefabs")]
    [SerializeField] GameObject _prefabTitle;

    [Header("UI Elements")]
    [SerializeField] Transform _verticleScrollContainer;
    [SerializeField] Transform _titleScrollContent;
    [SerializeField] Transform _horizentalScrollContainer;


    List<GameObject> _realityThumbnailList = new List<GameObject>();

    int _realityCount;

    int _currentIndex = 0;

    public int CurrentIndex
    {
        get{ return _currentIndex; }
    }

    float _scrollValue;

    float _offset = 4f;

    private void Awake()
    {
        _realityCount = realityCollection.realities.Count;
        _canvas = FindObjectOfType<Canvas>();
        _realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();

        DeletePreviousElement(_titleScrollContent);
        DeletePreviousElement(_realityThumbnailContainer.transform);

        InitialCoverContent();
    }


    private void Update()
    {
        UpdateScrollValue();
    }

    void InitialCoverContent()
    {
        for (int i = 0; i < _realityCount; i++)
        {
            // create thumbnailPrefabs
            var realityThumbnailGO = Instantiate(realityCollection.realities[i].thumbnailPrefab, _realityThumbnailContainer.transform);
            realityThumbnailGO.transform.position = new Vector3(i* _offset, 0, 0);
            _realityThumbnailList.Add(realityThumbnailGO);

            // create titles
            _prefabTitle.transform.Find("Index").GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + realityCollection.realities[i].realityId;
            _prefabTitle.transform.Find("Title").GetChild(0).GetComponent<TMPro.TMP_Text>().text = realityCollection.realities[i].displayName;
            var titleGO = Instantiate(_prefabTitle, _titleScrollContent);
        }
    }

    void UpdateScrollValue()
    {
        _scrollValue = _horizentalScrollContainer.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value;
        _scrollValue = Mathf.Clamp01(_scrollValue);
        Debug.Log(_scrollValue);
        // set value to thumbnails
        var positionOffset = _scrollValue * (_realityCount-1) * _offset;
        _realityThumbnailContainer.positionOffset = positionOffset;
        // set valut to name container
        _verticleScrollContainer.GetComponent<ScrollRect>().verticalNormalizedPosition = _scrollValue;
    }

    void DeletePreviousElement(Transform content)
    {
        var tempList = new List<Transform>();
        for (int i = 0; i < content.childCount; i++)
        {
            tempList.Add(content.GetChild(i));
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
    }

    //public void SwitchToRealityDetailPageLayout()
    //{
    //    transform.Find("HamburgerButton").gameObject.SetActive(false);
    //    transform.Find("PlayButton").gameObject.SetActive(false);
    //    _indexAndNameContainer.gameObject.SetActive(false);

    //    for (int i = 0; i < _realityCount; i++)
    //    {
    //        if (i == _currentIndex)
    //        {
    //            _realityThumbnailList[i].SetActive(true);
    //        }
    //        else
    //        {
    //            _realityThumbnailList[i].SetActive(false);
    //        }
    //    }

    //    _realityThumbnailContainer.transform.position = new Vector3(0, 2, 0);
    //}

    //public void SwitchToHomePageLayout()
    //{
    //    transform.Find("HamburgerButton").gameObject.SetActive(true);
    //    transform.Find("PlayButton").gameObject.SetActive(true);
    //    _indexAndNameContainer.gameObject.SetActive(true);


    //    for (int i = 0; i < _realityCount; i++)
    //    {
    //            _realityThumbnailList[i].SetActive(true);
    //    }

    //    _realityThumbnailContainer.transform.position = new Vector3(0, 0, 0);
    //}

}


