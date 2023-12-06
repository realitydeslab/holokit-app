using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class TornadoVFXController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        [SerializeField] AngularVelocityCalculator _AVC;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            _vfx.SetFloat("Angular Velocity", _AVC.AngularVelocityY/90f);
        }
    }
}
