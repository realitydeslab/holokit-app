using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Mofa.Base
{
    public class LifeShield : NetworkBehaviour
    {
        public NetworkVariable<bool> TopDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        public NetworkVariable<bool> BotDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        public NetworkVariable<bool> LeftDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        public NetworkVariable<bool> RightDestroyed = new(false, NetworkVariableReadPermission.Everyone);

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
            // TODO: Destroy fragments that have already been destroyed for late joined spectator.

            TopDestroyed.OnValueChanged += OnTopDestroyed;
            BotDestroyed.OnValueChanged += OnBotDestroyed;
            LeftDestroyed.OnValueChanged += OnLeftDestroyed;
            RightDestroyed.OnValueChanged += OnRightDestroyed;
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