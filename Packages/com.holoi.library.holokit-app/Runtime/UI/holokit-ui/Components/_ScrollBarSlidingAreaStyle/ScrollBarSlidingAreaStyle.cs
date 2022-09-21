using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollBarSlidingAreaStyle : MonoBehaviour
{
    [Header("Styles")]
    [SerializeField] GameObject _slidingStylePrefab;
    [SerializeField] Sprite _selected;
    [SerializeField] Sprite _notSelected;
    public Color color = Color.black;
    [SerializeField] float _spacing = 50;
    [SerializeField] float _offsetY = -50;

    [HideInInspector] public int objectCount = 0;

    float _scrollValue;

    public enum State
    {
        init,
        update,
    }

    State state = State.init;

    List<GameObject> _dots = new List<GameObject>();


    public void Init(int count)
    {
        objectCount = count;
        ClearLastContent(transform);

        Debug.Log("slidingbar count: " + objectCount);

        float firstDotPosOffsetX = 0;
        float paddingCount = 0;

        if (objectCount % 2 == 1)
        {
            paddingCount = (objectCount - 1) / 2;
        }
        else
        {
            paddingCount = (objectCount - 2) / 2 + 0.5f;
        }

        firstDotPosOffsetX = -1 * paddingCount * _spacing;

        for (int i = 0; i < objectCount; i++)
        {
            Debug.Log("create sliding bar, parent: " + transform.parent.name);
            var go = Instantiate(_slidingStylePrefab, transform);
            go.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(firstDotPosOffsetX + (_spacing * i), _offsetY, 0);

            go.GetComponent<Image>().color = color;
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
            //Debug.Log(_scrollValue);
            _scrollValue = Mathf.Clamp01(_scrollValue);
            var currentIndex = Mathf.RoundToInt(_scrollValue * (objectCount - 1)); // start from index 0!
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
    void SetDotState(float index)
    {
        for (int i = 0; i < objectCount; i++)
        {
            if (index == i)
            {
                _dots[i].GetComponent<Image>().sprite = _selected;
            }
            else
            {
                _dots[i].GetComponent<Image>().sprite = _notSelected;
            }
        }
    }
}
