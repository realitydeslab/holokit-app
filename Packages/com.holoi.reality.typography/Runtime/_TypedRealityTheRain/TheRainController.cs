using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class TheRainController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _rainVfx;

        private void Update()
        {
            Vector3 forward = -HoloKitCamera.Instance.CenterEyePose.forward;
            _rainVfx.SetVector3("worldForward", forward);
        }
    }
}
