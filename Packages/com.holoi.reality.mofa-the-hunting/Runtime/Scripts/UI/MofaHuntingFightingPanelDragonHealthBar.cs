using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingFightingPanelDragonHealthBar : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            _material = GetComponent<Image>().material;
        }

        private void Update()
        {
            var mofaHuntingRealityManager = HoloKitApp.Instance.RealityManager as MofaHuntingRealityManager;
            var dragonController = mofaHuntingRealityManager.DragonController;
            if (dragonController != null)
            {
                _material.SetFloat("_Health", dragonController.CurrentHealthPercent);
            }
        }
    }
}
