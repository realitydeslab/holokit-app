using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class LockTargetPlayerIndicator : MonoBehaviour
    {
        public Vector3 Offset = new(0f, 0.2f, 0f);

        public MofaPlayer MofaPlayer;

        private void LateUpdate()
        {
            // The pose visualizer should always look at the local camera.
            transform.LookAt(HoloKit.HoloKitCamera.Instance.CenterEyePose);
            if (MofaPlayer != null)
            {
                transform.position = MofaPlayer.transform.position + Offset;
            }
        }
    }
}
