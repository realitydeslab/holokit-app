using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class ARShadowedPlane : MonoBehaviour
{
    [SerializeField] bool _isSoftShadowed = true;

    Material _shadowedMat;
    [Header("Soft Shadow")]
    [SerializeField] float _fadeTightness = 80;
    [SerializeField] float _blurRadius = 0.1f;
    [SerializeField] float _strength = 0.4f;
    Color _screenARShadowColor = new Color(8 / 255f, 8 / 255f, 8 / 255f, 1);
    Color _stARShadowColor = new Color(70 / 255f, 0, 140 / 255f, 1);

    [Header("Hard Shadow")]
    Color _screenARShadowHardColor = new Color(8 / 255f, 8 / 255f, 8 / 255f, 1);
    Color _stARShadowHardColor = new Color(70 / 255f, 0, 140 / 255f, 1);

    private void OnEnable()
    {
        HoloKitCamera.OnHoloKitRenderModeChanged += OnRenderModeChanged;
    }

    private void OnDisable()
    {
        HoloKitCamera.OnHoloKitRenderModeChanged -= OnRenderModeChanged;
    }

    void Start()
    {
        _shadowedMat = GetComponent<MeshRenderer>().material;
        if (_isSoftShadowed)
        {
            _shadowedMat.SetColor("_Color", _screenARShadowColor);
            _shadowedMat.SetFloat("_FadeTightness", _fadeTightness);
            _shadowedMat.SetFloat("_BlurRadius", _blurRadius);
            _shadowedMat.SetFloat("_MultiplyStrength", _strength);
        }
        else
        {
            _shadowedMat.SetColor("_Color", _screenARShadowHardColor);
        }

    }

    void OnRenderModeChanged(HoloKitRenderMode mode)
    {
        Debug.Log("RenderModeChanged:" + mode);

        if (mode == HoloKitRenderMode.Mono)
        {
            if (_isSoftShadowed)
            {
                _shadowedMat.SetColor("_Color", _screenARShadowColor);
            }
            else
            {
                _shadowedMat.SetColor("_Color", _screenARShadowHardColor);

            }

        }
        else
        {
            if (_isSoftShadowed)
            {
                _shadowedMat.SetColor("_Color", _stARShadowColor);
            }
            else
            {
                _shadowedMat.SetColor("_Color", _stARShadowHardColor);

            }
        }
    }
}
