using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class HandObject : MonoBehaviour
{
    private static HandObject _instance;

    public static HandObject Instance { get { return _instance;  } }

    bool _isHandValid = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        _isHandValid = HoloKitHandTracker.Instance.Valid;
    }
}
