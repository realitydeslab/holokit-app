using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class TheScannerMeshController : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if (TheScannerCenter.Instance != null)
            {
                _material.SetVector("_Center_Position", TheScannerCenter.Instance.transform.position);
                _material.SetFloat("_Angle_Offset", TheScannerCenter.Instance.transform.rotation.eulerAngles.y);
            }
        }
    }
}
