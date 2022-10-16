using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;


public class DisableOnClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!HoloKitApp.Instance.IsHost)
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
        else
        {

        }
    }
}
