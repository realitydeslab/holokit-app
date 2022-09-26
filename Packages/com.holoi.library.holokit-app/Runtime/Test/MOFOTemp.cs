using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;
using UnityEngine.SceneManagement;

namespace Holoi.Library.HoloKitApp
{
    public class MOFOTemp : MonoBehaviour
    {
        [SerializeField] private RealityList _realityList;

        private void Start()
        {
            
        }

        public void EnterMOFO()
        {
            HoloKitApp.Instance.CurrentReality = _realityList.realities[4];
            HoloKitApp.Instance.EnterRealityAsHost();
            SceneManager.LoadScene("MOFO", LoadSceneMode.Single);
        }

        public void EnterMOFOAsSpectator()
        {
            HoloKitApp.Instance.CurrentReality = _realityList.realities[4];
            HoloKitApp.Instance.JoinRealityAsSpectator();
            SceneManager.LoadScene("MOFO", LoadSceneMode.Single);
        }
    }
}
