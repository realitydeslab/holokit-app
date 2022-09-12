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
    [SerializeField] GameObject _nameContainer;

    [Header("UI Elements")]
    [SerializeField] Transform _nameScrollContainer;
    [SerializeField] Transform _horizentalScrollContainer;
    [SerializeField] Transform _verticalScrollContainer;

    List<GameObject> _realityThumbnailList = new List<GameObject>();

    int _realityCount;

    int _currentIndex = 0;

    public int CurrentIndex
    {
        get{ return _currentIndex; }
    }

    float _thumbnailSpacing = 4f;

    float _scrollValue;

    private void Awake()
    {
        _realityCount = realityCollection.Realities.Count;
        _canvas = FindObjectOfType<Canvas>();
        _realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
        InitialCoverContent();
    }


    void InitialCoverContent()
    {
        for (int i = 0; i < _realityCount; i++)
        {
            var realityThumbnailGO = Instantiate(realityCollection.Realities[i].ThumbnailPrefab, _realityThumbnailContainer.transform);
            realityThumbnailGO.transform.position = new Vector3(i* _thumbnailSpacing, 0, 0);
            _realityThumbnailList.Add(realityThumbnailGO);

            var indexAndNameGO = Instantiate(_nameContainer, _nameScrollContainer);

            indexAndNameGO.transform.Find("Index").GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + realityCollection.Realities[i].RealityId;
            indexAndNameGO.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = realityCollection.Realities[i].DisplayName;
        }
    }

    void UpdateScrollValue()
    {
        _scrollValue = _horizentalScrollContainer.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value;

        // set value to thumbnails
        _realityThumbnailContainer._scrollValue = _scrollValue;
        // set valut to name container
        _verticalScrollContainer.GetComponent<ScrollRect>().verticalNormalizedPosition = _scrollValue;

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

    private void Update()
    {
        if (Application.isEditor)
        {
        }
    }
}


