using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Mofa.Base;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingTemp : MonoBehaviour
    {
        public AssetFoundation.Reality Reality;

        public void EnterReality()
        {
            HoloKitApp.Instance.CurrentReality = Reality;
            HoloKitApp.Instance.EnterRealityAsHost();
        }

        public void JoinReality()
        {
            HoloKitApp.Instance.CurrentReality = Reality;
            HoloKitApp.Instance.JoinRealityAsSpectator();
        }
    }
}
