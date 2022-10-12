using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public class AxisController : MonoBehaviour
    {
        private void Update()
        {
            Debug.Log($"[Axis] position: {transform.position}, rotation: {transform.rotation}");
        }
    }
}
