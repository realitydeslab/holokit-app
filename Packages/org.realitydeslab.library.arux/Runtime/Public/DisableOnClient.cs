using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using RealityDesignLab.Library.HoloKitApp;


public class DisableOnClient : MonoBehaviour
{
    [SerializeField] bool _vfx;
    [SerializeField] bool _gameObject;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!HoloKitApp.Instance.IsHost)
        {
            if (_gameObject)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
            }
            if (_vfx)
            {
                if (GetComponent<VisualEffect>().enabled) GetComponent<VisualEffect>().enabled = false;
            }
        }
        else
        {

        }
    }
}
