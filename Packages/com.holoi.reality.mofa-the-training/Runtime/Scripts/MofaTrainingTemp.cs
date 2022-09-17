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
            //HoloKitApp.Instance.EnterRealityAsHost(Reality);
        }

        public void JoinReality()
        {
            //HoloKitApp.Instance.JoinRealityAsSpectator(Reality);
        }
    }
}
