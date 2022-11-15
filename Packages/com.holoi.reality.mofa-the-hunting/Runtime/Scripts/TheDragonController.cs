using UnityEngine;
using Unity.Netcode;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonController : NetworkBehaviour
    {
        [SerializeField] private MAnimal _animal;

        [SerializeField] private Aim _aim;

        private void Awake()
        {
            _animal.m_MainCamera = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Start()
        {
            _aim.MainCamera = HoloKitCamera.Instance.CenterEyePose;
        }

        //public override void OnNetworkSpawn()
        //{
        //    base.OnNetworkSpawn();
        //    _aim.MainCamera = HoloKitCamera.Instance.CenterEyePose;
        //}
    }
}
