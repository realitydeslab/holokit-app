using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollBarSlidingAreaStyle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Transform _content;

    [Header("Styles")]
    [SerializeField] GameObject _slidingAreaElement;
    [SerializeField] Sprite _activeSprite;
    [SerializeField] Sprite _inactiveSprite;
    [SerializeField] Color _color = Color.black;
    [SerializeField] float _spacing;
    [SerializeField] float _offsetY;


    int _count;
    float _value;


    List<GameObject> _dots = new List<GameObject>();

    private void Awake()
    {
        _count = _content.childCount;

        Debug.Log("update content!");
        // clear all gameobject
        var tempList = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            tempList.Add(transform.GetChild(i));
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
            var go = Instantiate(_slidingAreaElement);
            go.transform.parent = transform;
            go.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(firstDotPosOffsetX + (_spacing * i), _offsetY, 0);

            go.GetComponent<Image>().color = _color;
            _dots.Add(go);
        }
        SetDotState(_value);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            _value = GetComponent<Scrollbar>().value;
            SetDotState(_value);
        }
    }

    void SetDotState(float value)
    {
        var valueFixer = Mathf.CeilToInt(value*(_count-1));

        for (int i = 0; i < _count; i++)
        {
            if (valueFixer == i)
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
