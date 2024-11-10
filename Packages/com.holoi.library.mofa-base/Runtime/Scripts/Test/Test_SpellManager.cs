// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_SpellManager : MonoBehaviour
    {
        public SpellList SpellList;

        [SerializeField] private NetworkManager _networkManager;

        private Spell _currentSpell;

        private void Start()
        {
            // Initialize HoloKit SDK
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitNFCSessionControllerAPI.RegisterNFCSessionControllerDelegates();
                HoloKitARSessionControllerAPI.RegisterARSessionControllerDelegates();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                HoloKitARSessionControllerAPI.SetSessionShouldAttemptRelocalization(false);
            }

            InitNetworkManager();
            _networkManager.StartHost();

            HoloKitAppUIEventManager.OnStarUITriggered += CastSpell;
            HoloKitAppUIEventManager.OnStarUIBoosted += CastSpellOpposite;
        }

        private void OnDestroy()
        {
            HoloKitAppUIEventManager.OnStarUITriggered -= CastSpell;
            HoloKitAppUIEventManager.OnStarUIBoosted -= CastSpellOpposite;
        }

        private void InitNetworkManager()
        {
            foreach (var spell in SpellList.List)
            {
                _networkManager.AddNetworkPrefab(spell.gameObject);
            }
        }

        public void OnSpellSelected(int spellId)
        {
            _currentSpell = SpellList.GetSpell(spellId);
        }

        public void CastSpell()
        {
            Vector3 centerEyePosition = HoloKitCameraManager.Instance.CenterEyePose.position;
            Quaternion centerEyeRotation = GetCameraGravitationalRotation();

            Vector3 spellPosition = centerEyePosition + centerEyeRotation * _currentSpell.SpawnOffset;
            Quaternion spellRotation = centerEyeRotation;
            if (_currentSpell.PerpendicularToGround)
                spellRotation = MofaUtils.GetHorizontalRotation(spellRotation);

            var spellInstance = Instantiate(_currentSpell, spellPosition, spellRotation);
            spellInstance.GetComponent<NetworkObject>().Spawn();
        }

        public void CastSpellOpposite()
        {
            const float spellDist = 8f;
            Vector3 centerEyePosition = HoloKitCameraManager.Instance.CenterEyePose.position;
            Vector3 centerEyeForward = HoloKitCameraManager.Instance.CenterEyePose.forward;
            Vector3 horizontalCenterEyeForward = Vector3.ProjectOnPlane(centerEyeForward, Vector3.up).normalized;

            Vector3 spellPosition = centerEyePosition + spellDist * horizontalCenterEyeForward;
            Vector3 spellForward = (centerEyePosition - spellPosition).normalized;
            Quaternion spellRotation = Quaternion.LookRotation(spellForward, Vector3.up);
            if (_currentSpell.PerpendicularToGround)
                spellRotation = MofaUtils.GetHorizontalRotation(spellRotation);

            var spellInstance = Instantiate(_currentSpell, spellPosition, spellRotation);
            spellInstance.GetComponent<NetworkObject>().Spawn();
        }

        private Quaternion GetCameraGravitationalRotation()
        {
            Vector3 cameraRotationEuler = HoloKitCameraManager.Instance.CenterEyePose.rotation.eulerAngles;
            return Quaternion.Euler(cameraRotationEuler.x, cameraRotationEuler.y, 0f);
        }
    }
}
