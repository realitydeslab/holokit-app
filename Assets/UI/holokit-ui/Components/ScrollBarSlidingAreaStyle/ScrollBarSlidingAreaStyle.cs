using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollBarSlidingAreaStyle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Transform _scrollContent;
    [SerializeField] Transform _slidingArea;

    [Header("Styles")]
    [SerializeField] GameObject _slidingAreaElement;
    [SerializeField] Sprite _activeSprite;
    [SerializeField] Sprite _inactiveSprite;
    [SerializeField] Color _color = Color.black;
    [SerializeField] float _spacing;
    [SerializeField] float _offsetY;


    int _count;
    float _scrollValue;


    List<GameObject> _dots = new List<GameObject>();

    private void Awake()
    {
        DeletePreviousContent(transform);

        _count = _scrollContent.childCount;

        float firstDotPosOffsetX = 0;
        float paddingCount = 0;

        if (_count % 2 == 1)
        {
            paddingCount = (_count - 1) / 2;
        }
        else
        {
            paddingCount = (_count - 2) / 2 + 0.5f;
        }

        firstDotPosOffsetX = -1 * paddingCount * _spacing;

        for (int i = 0; i < _count; i++)
        {
            var go = Instantiate(_slidingAreaElement, transform);
            go.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(firstDotPosOffsetX + (_spacing * i), _offsetY, 0);

            go.GetComponent<Image>().color = _color;
            _dots.Add(go);
        }
        SetDotState(_scrollValue);
    }

    void Update()
    {
        _scrollValue = GetComponent<Scrollbar>().value;
        _scrollValue = Mathf.Clamp01(_scrollValue);
        SetDotState(_scrollValue);
    }

    void DeletePreviousContent(Transform content)
    {
        var tempList = new List<Transform>();
        for (int i = 0; i < content.childCount; i++)
        {
            if(content.GetChild(i).name == "Sliding Area")
            {

            }
            else
            {
                tempList.Add(content.GetChild(i));
            }
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
    void SetDotState(float value)
    {
        //var valueFixer = Mathf.RoundToInt(value*(_count-1));

        for (int i = 0; i < _count; i++)
        {
            if (_scrollValue == i)
            {
                _dots[i].GetComponent<Image>().sprite = _activeSprite;
            }
            else
            {
                _dots[i].GetComponent<Image>().sprite = _inactiveSprite;
            }
        }
    }
}
