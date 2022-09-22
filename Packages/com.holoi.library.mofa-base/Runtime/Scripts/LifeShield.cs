using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using System;

namespace Holoi.Mofa.Base
{
    public class LifeShield : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<bool> TopDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> BotDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> LeftDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> RightDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> LastAttackerClientId = new(0, NetworkVariableReadPermission.Everyone);

        [SerializeField] private Material _blueMaterial;

        [SerializeField] private Material _redMaterial;

        [SerializeField] private AudioClip _beingHitSound;

        [SerializeField] private AudioClip _beingDestroyedSound;

        public Vector3 CenterEyeOffset;

        private AudioSource _audioSource;

        public static float DestroyDelay = 1f;

        private readonly Dictionary<LifeShieldArea, LifeShieldFragment> _fragments = new();

        public static event Action<ulong> OnDead;

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
            Debug.Log($"[LifeShield] OnNetworkSpawn with ownership {OwnerClientId}");

            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetLifeShield(this);

            // Setup color
            if (mofaRealityManager.Players[OwnerClientId].Team.Value == MofaTeam.Blue)
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
                _fragments[LifeShieldArea.Top].GetComponent<MeshRenderer>().enabled = false;
                _fragments[LifeShieldArea.Bot].GetComponent<MeshRenderer>().enabled = false;
                _fragments[LifeShieldArea.Left].GetComponent<MeshRenderer>().enabled = false;
                _fragments[LifeShieldArea.Right].GetComponent<MeshRenderer>().enabled = false;
            }

            // TODO: Destroy fragments that have already been destroyed for late joined spectator.


            TopDestroyed.OnValueChanged += OnTopDestroyed;
            BotDestroyed.OnValueChanged += OnBotDestroyed;
            LeftDestroyed.OnValueChanged += OnLeftDestroyed;
            RightDestroyed.OnValueChanged += OnRightDestroyed;
        }

        public override void OnNetworkDespawn()
        {
            TopDestroyed.OnValueChanged -= OnTopDestroyed;
            BotDestroyed.OnValueChanged -= OnBotDestroyed;
            LeftDestroyed.OnValueChanged -= OnLeftDestroyed;
            RightDestroyed.OnValueChanged -= OnRightDestroyed;
        }

        private void OnTopDestroyed(bool oldValue, bool newValue)
        {
            if (IsServer)
            {
                if (_fragments[LifeShieldArea.Top].TryGetComponent<Collider>(out var topCollider))
                {
                    topCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.TopLeft].TryGetComponent<Collider>(out var topLeftCollider))
                {
                    topLeftCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.TopRight].TryGetComponent<Collider>(out var topRightCollider))
                {
                    topRightCollider.enabled = false;
                }
            }
            _fragments[LifeShieldArea.Top].GetComponent<MeshRenderer>().enabled = false;
            PlayHitSound();
            IsDead();
        }

        private void OnBotDestroyed(bool oldValue, bool newValue)
        {
            if (IsServer)
            {
                if (_fragments[LifeShieldArea.Bot].TryGetComponent<Collider>(out var botCollider))
                {
                    botCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.BotLeft].TryGetComponent<Collider>(out var botLeftCollider))
                {
                    botLeftCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.BotRight].TryGetComponent<Collider>(out var botRightCollider))
                {
                    botRightCollider.enabled = false;
                }
            }
            _fragments[LifeShieldArea.Bot].GetComponent<MeshRenderer>().enabled = false;
            PlayHitSound();
            IsDead();
        }

        private void OnLeftDestroyed(bool oldValue, bool newValue)
        {
            if (IsServer)
            {
                if (_fragments[LifeShieldArea.Left].TryGetComponent<Collider>(out var leftCollider))
                {
                    leftCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.TopLeft].TryGetComponent<Collider>(out var topLeftCollider))
                {
                    topLeftCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.BotLeft].TryGetComponent<Collider>(out var botLeftCollider))
                {
                    botLeftCollider.enabled = false;
                }
            }
            _fragments[LifeShieldArea.Left].GetComponent<MeshRenderer>().enabled = false;
            PlayHitSound();
            IsDead();
        }

        private void OnRightDestroyed(bool oldValue, bool newValue)
        {
            if (IsServer)
            {
                if (_fragments[LifeShieldArea.Right].TryGetComponent<Collider>(out var rightCollider))
                {
                    rightCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.TopRight].TryGetComponent<Collider>(out var topRightCollider))
                {
                    topRightCollider.enabled = false;
                }
                if (_fragments[LifeShieldArea.BotRight].TryGetComponent<Collider>(out var botRightCollider))
                {
                    botRightCollider.enabled = false;
                }
            }
            _fragments[LifeShieldArea.Right].GetComponent<MeshRenderer>().enabled = false;
            PlayHitSound();
            IsDead();
        }

        private void IsDead()
        {
            if (TopDestroyed.Value && BotDestroyed.Value && LeftDestroyed.Value && RightDestroyed.Value)
            {
                OnDead?.Invoke(OwnerClientId);
                PlayDestroySound();
                if (IsServer)
                {
                    var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                    if (mofaRealityManager.Phase.Value == MofaPhase.Fighting || mofaRealityManager.Phase.Value == MofaPhase.RoundOver)
                    {
                        mofaRealityManager.Players[(ulong)LastAttackerClientId.Value].KillCount.Value++;
                        mofaRealityManager.Players[OwnerClientId].DeathCount.Value++;
                    }
                    Destroy(gameObject, DestroyDelay);
                }
            }
        }

        private void PlayHitSound()
        {
            if (_beingHitSound != null)
            {
                _audioSource.clip = _beingHitSound;
                _audioSource.Play();
            }
        }

        private void PlayDestroySound()
        {
            if (_beingDestroyedSound != null)
            {
                _audioSource.clip = _beingDestroyedSound;
                _audioSource.Play();
            }
        }
    }
}