using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TitleContainer : MonoBehaviour
{
    Transform _title;
    Transform _index;
    Transform _arrow;

    private void Awake()
    {
        _title = transform.Find("Title");
        _index = transform.Find("Index");
        _arrow = transform.Find("Title/Arrow");

        var text = _title.GetChild(0).GetComponent<TMPro.TMP_Text>().text;
        var width = _title.GetChild(0).GetComponent<TMPro.TMP_Text>().preferredWidth;
        //Debug.Log(text + ":" + width);

        _arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1* width - 70 - 54, -137);
    }
}