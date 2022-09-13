using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;

namespace Holoi.Mofa.Base
{
    public class LifeShield : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<bool> TopDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> BotDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> LeftDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> RightDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [SerializeField] private Material _blueMaterial;

        [SerializeField] private Material _redMaterial;

        [SerializeField] private AudioClip _hitSound;

        private AudioSource _audioSource;

        private readonly Dictionary<LifeShieldArea, LifeShieldFragment> _fragments = new();

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var fragment = transform.GetChild(i).GetComponent<LifeShieldFragment>();
                _fragments.Add(fragment.Area, fragment);
            }
        }

        public override void OnNetworkSpawn()
        {
            TopDestroyed.OnValueChanged += OnTopDestroyed;
            BotDestroyed.OnValueChanged += OnBotDestroyed;
            LeftDestroyed.OnValueChanged += OnLeftDestroyed;
            RightDestroyed.OnValueChanged += OnRightDestroyed;

            // Setup color
            var realityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            if (realityManager.Players[NetworkManager.LocalClientId].Team.Value == MofaTeam.Blue)
            {
                _fragments[LifeShieldArea.Top].GetComponent<MeshRenderer>().material = _blueMaterial;
                _fragments[LifeShieldArea.Bot].GetComponent<MeshRenderer>().material = _blueMaterial;
                _fragments[LifeShieldArea.Left].GetComponent<MeshRenderer>().material = _blueMaterial;
                _fragments[LifeShieldArea.Right].GetComponent<MeshRenderer>().material = _blueMaterial;
            }
            else
            {
                _fragments[LifeShieldArea.Top].GetComponent<MeshRenderer>().material = _redMaterial;
                _fragments[LifeShieldArea.Bot].GetComponent<MeshRenderer>().material = _redMaterial;
                _fragments[LifeShieldArea.Left].GetComponent<MeshRenderer>().material = _redMaterial;
                _fragments[LifeShieldArea.Right].GetComponent<MeshRenderer>().material = _redMaterial;
            }

            // Hide local player's shield
            if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {

            }

            // TODO: Destroy fragments that have already been destroyed for late joined spectator.

        }

        private void OnTopDestroyed(bool oldValue, bool newValue)
        {
            PlayHitSound();
        }

        private void OnBotDestroyed(bool oldValue, bool newValue)
        {
            PlayHitSound();
        }

        private void OnLeftDestroyed(bool oldValue, bool newValue)
        {
            PlayHitSound();
        }

        private void OnRightDestroyed(bool oldValue, bool newValue)
        {
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            _audioSource.clip = _hitSound;
            _audioSource.Play();
        }
    }
}