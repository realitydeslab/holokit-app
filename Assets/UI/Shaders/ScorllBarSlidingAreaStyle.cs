using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScorllBarSlidingAreaStyle : MonoBehaviour
{
    [SerializeField] Transform _content;
    [SerializeField] int _count;
    [SerializeField] float _value;


    [SerializeField] GameObject _slidingAreaElementStyleDot;
    [SerializeField] float _spacing;


    [SerializeField] Sprite _activeSprite;
    [SerializeField] Sprite _inactiveSprite;

    List<GameObject> _dots = new List<GameObject>();

    private void Awake()
    {
        //_count = _content.childCount;

        Debug.Log("update content!");
        // clear all gameobject
        var tempList = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            tempList.Add(transform.GetChild(i));
        }
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
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
            var go = Instantiate(_slidingAreaElementStyleDot);
            go.transform.parent = transform;
            go.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(firstDotPosOffsetX + (_spacing * i), 0, 0);

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
