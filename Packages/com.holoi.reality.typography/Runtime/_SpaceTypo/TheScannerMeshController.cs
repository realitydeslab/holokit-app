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
    }
}
