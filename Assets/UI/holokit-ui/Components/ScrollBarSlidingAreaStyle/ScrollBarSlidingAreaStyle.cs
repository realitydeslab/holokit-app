using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollBarSlidingAreaStyle : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Transform _slidingArea;

    [Header("Styles")]
    [SerializeField] GameObject _slidingAreaElement;
    [SerializeField] Sprite _activeSprite;
    [SerializeField] Sprite _inactiveSprite;
    [SerializeField] Color _color = Color.black;
    [SerializeField] float _spacing;
    [SerializeField] float _offsetY;


    public int count = 0;
    float _scrollValue;

    public enum State
    {
        init,
        update,
    }

    State state = State.init;

    List<GameObject> _dots = new List<GameObject>();


    public void Init(int num)
    {
        count = num;
        ClearLastContent(transform);

        Debug.Log("slidingbar count: " + count);

        float firstDotPosOffsetX = 0;
        float paddingCount = 0;

        if (count % 2 == 1)
        {
            paddingCount = (count - 1) / 2;
        }
        else
        {
            paddingCount = (count - 2) / 2 + 0.5f;
        }

        firstDotPosOffsetX = -1 * paddingCount * _spacing;

        for (int i = 0; i < count; i++)
        {
            Debug.Log("create sliding bar, parent: " + transform.parent.name);
            var go = Instantiate(_slidingAreaElement, transform);
            go.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(firstDotPosOffsetX + (_spacing * i), _offsetY, 0);

            go.GetComponent<Image>().color = _color;
            _dots.Add(go);
        }

        SetDotState(_scrollValue);
        state = State.update;
    }

    void Update()
    {
        if(state == State.update)
        {
            _scrollValue = GetComponent<Scrollbar>().value;
            Debug.Log(_scrollValue);
            _scrollValue = Mathf.Clamp01(_scrollValue);
            var currentIndex = Mathf.RoundToInt(_scrollValue * (count - 1)); // start from index 0!
            //Debug.Log(currentIndex);

            SetDotState(currentIndex);
        }
    }

    void ClearLastContent(Transform content)
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
        for (int i = 0; i < count; i++)
        {
            if (value == i)
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
