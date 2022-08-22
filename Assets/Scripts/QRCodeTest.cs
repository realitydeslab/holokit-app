using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCodeTest : MonoBehaviour
{
    public void OnQRScanFinished(string data)
    {
        Debug.Log($"OnQRScanFinished: {data}");
    }
}
