using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HomeUIPanel : HomeUI
{
    Canvas _canvas;

    Transform _indexAndNameContainer;

    RealityThumbnailContainer _realityThumbnailContainer;

    List<GameObject> _indexAndNameList = new List<GameObject>();

    List<GameObject> _realityThumbnailList = new List<GameObject>();

    string IndexAndNamePath = "Prefabs/UI/Components/HomePage-Reality-IndexAndName";

    int _realityTotal;

    int _activeIndex = 0;

    public int ActiveIndex
    {
        get{ return _activeIndex; }
    }

    float _realityBoxDistance = 4f;

    private void Awake()
    {
        OnHomeUIAweak();
    }

    protected override void OnHomeUIAweak()
    {
        base.OnHomeUIAweak();

        _realityTotal = realityCollection.realities.Count;
        _canvas = FindObjectOfType<Canvas>();
        _realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
        _indexAndNameContainer = transform.Find("IndexAndNameContainer");
        InstantiateRealities();
    }


    void InstantiateRealities()
    {
        for (int i = 0; i < _realityTotal; i++)
        {
            var realityThumbnailGO = Instantiate(realityCollection.realities[i].thumbnailPrefab, _realityThumbnailContainer.transform);
            realityThumbnailGO.transform.position = new Vector3(i* _realityBoxDistance, 0, 0);
            _realityThumbnailList.Add(realityThumbnailGO);

            var indexAndNameGO = Instantiate(Resources.Load<GameObject>(IndexAndNamePath), _indexAndNameContainer);
            _indexAndNameList.Add(indexAndNameGO);

            _indexAndNameList[i].transform.Find("Index").GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + realityCollection.realities[i].realityId;
            _indexAndNameList[i].transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = realityCollection.realities[i].displayName;
            if (i == 0)
            {
                _indexAndNameList[i].SetActive(true);
            }
            else
            {
                _indexAndNameList[i].SetActive(false);
            }
        }
    }

    public void SwitchToRealityDetailPageLayout()
    {
        transform.Find("HamburgerButton").gameObject.SetActive(false);
        transform.Find("PlayButton").gameObject.SetActive(false);
        _indexAndNameContainer.gameObject.SetActive(false);

        for (int i = 0; i < _realityTotal; i++)
        {
            if (i == _activeIndex)
            {
                _realityThumbnailList[i].SetActive(true);
            }
            else
            {
                _realityThumbnailList[i].SetActive(false);
            }
        }

        _realityThumbnailContainer.transform.position = new Vector3(0, 2, 0);
    }

    public void SwitchToHomePageLayout()
    {
        transform.Find("HamburgerButton").gameObject.SetActive(true);
        transform.Find("PlayButton").gameObject.SetActive(true);
        _indexAndNameContainer.gameObject.SetActive(true);


        for (int i = 0; i < _realityTotal; i++)
        {
                _realityThumbnailList[i].SetActive(true);
        }

        _realityThumbnailContainer.transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                // to next reality
                _activeIndex++;
                if (_activeIndex > _realityTotal-1) _activeIndex = _realityTotal-1;

                _indexAndNameList[_activeIndex-1].SetActive(false);
                _indexAndNameList[_activeIndex].SetActive(true);

                _realityThumbnailContainer.transform.position = new Vector3(-1* _activeIndex * _realityBoxDistance,0,0);
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                // to last reality
                _activeIndex--;
                if (_activeIndex < 0) _activeIndex = 0;
                _indexAndNameList[_activeIndex + 1].SetActive(false);
                _indexAndNameList[_activeIndex].SetActive(true);
                _realityThumbnailContainer.transform.position = new Vector3(-1 * _activeIndex * _realityBoxDistance, 0, 0);

            }
        }
    }
}


