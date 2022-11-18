using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOrientationTest : MonoBehaviour
{
    private void Update()
    {
        Debug.Log($"Input.deviceOrientation: {Input.deviceOrientation}");
    }
}
