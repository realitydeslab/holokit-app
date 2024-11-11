// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib.WatchConnectivity;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using Holoi.Library.HoloKitAppLib.UI;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.Library.Base;
using HoloKit;

namespace RealityDesignLab.MOFA.TheDucks
{
    /// <summary>
    /// This part of the SpellManager class is responsible for the communication
    /// between iPhone and Apple Watch.
    /// </summary>
    public partial class SpellManager
    {
        /// <summary>
        /// The reference of the basic spell the player chose.
        /// </summary>
        private Spell _basicSpell;

        // Only client players should call this method
        private void Init_WatchConnectivity()
        {
            HoloKitAppWatchConnectivityAPI.OnSessionReachabilityChanged += OnWatchReachabilityChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnWatchTriggered;
            // Pressing the trigger button is identical as swinging the Apple Watch
            HoloKitAppUIEventManager.OnStarUITriggered += OnWatchTriggered;

            MofaWatchConnectivityAPI.Initialize();
            string magicSchoolTokenId = HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId;
            int magicSchoolIndex = int.Parse(magicSchoolTokenId);
            SetupBasicSpell(magicSchoolIndex);
            // Since there is no round at all in MOFA: The Ducks, we start the round
            // immediately after the initialization
            SendRoundStartMessageToWatch(magicSchoolIndex);
        }

        // Only client players should call this method
        private void Deinit_WatchConnectivity()
        {
            HoloKitAppWatchConnectivityAPI.OnSessionReachabilityChanged -= OnWatchReachabilityChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered -= OnWatchTriggered;
            HoloKitAppUIEventManager.OnStarUITriggered -= OnWatchTriggered;
        }

        // The player can only cast the basic spell in this reality.
        // The player cannot cast the secondary spell in the magic school.
        private void SetupBasicSpell(int magicSchoolIndex)
        {
            foreach (var spell in _spellList.List)
            {
                if (spell.SpellType == SpellType.Basic && int.Parse(spell.MagicSchool.TokenId) == magicSchoolIndex)
                {
                    _basicSpell = spell;
                    return;
                }       
            }
        }

        private void SendRoundStartMessageToWatch(int magicSchoolIndex)
        {
            // We need to pass the magic school index to make the Apple Watch
            // display the correct MOFA spell image
            MofaWatchConnectivityAPI.OnRoundStarted(magicSchoolIndex);
        }

        /// <summary>
        /// We want to know if the watch becomes unreachable. When the watch becomes
        /// reachable again, the iPhone will send the message to it to let it know the
        /// current state of the game. This makes the watch connectivity more robust.
        /// </summary>
        /// <param name="isReachable"></param>
        private void OnWatchReachabilityChanged(bool isReachable)
        {
            // If the watch connectivity session is interrupted and resumed, we
            // send the round start message again to sync the watch status
            if (isReachable)
            {
                string magicSchoolTokenId = HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId;
                int magicSchoolIndex = int.Parse(magicSchoolTokenId);
                SendRoundStartMessageToWatch(magicSchoolIndex);
            }
        }

        private void OnWatchTriggered()
        {
            // Get the center eye pose (pose = position + rotation)
            Transform centerEyePose = HoloKitCameraManager.Instance.CenterEyePose;
            SpawnSpellServerRpc(_basicSpell.Id, centerEyePose.position, centerEyePose.rotation);
        }
    }
}
