using UnityEngine;
using Unity.Netcode;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonController : NetworkBehaviour
    {
        [SerializeField] private MAnimal _animal;

        [SerializeField] private Aim _aim;

        private void Awake()
        {
            _animal.m_MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
        }

        private void Start()
        {
            _aim.MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
        }
    }
}
