using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIPanel : MonoBehaviour
{
    [SerializeField] Image _banner;
    [SerializeField] AnimationCurve _curve;

    float _time;
    float _duration = 2;

    private void Start()
    {
        _time = Time.time;
    }
    private void Update()
    {
        var sampler = (Time.time - _time) / _duration;
        var value = _curve.Evaluate(sampler);
        _banner.materialForRendering.SetVector("_Offset", new Vector2(value, 0));
    }
}
