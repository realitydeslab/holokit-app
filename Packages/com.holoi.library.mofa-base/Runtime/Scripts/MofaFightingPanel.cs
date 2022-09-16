using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanel : MonoBehaviour
    {
        public GameObject Scores;

        public GameObject Reticle;

        public GameObject Status;

        private void Awake()
        {
            transform.SetParent(HoloKitCamera.Instance.CenterEyePose);
        }
    }
}