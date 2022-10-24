using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class LifeShield : NetworkBehaviour, IDamageable
    {
        public bool OnHitDelegation => true;

        public MagicSchool MagicSchool;

        [SerializeField] private AudioClip _beingHitSound;

        [SerializeField] private AudioClip _beingDestroyedSound;

        private readonly NetworkVariable<bool> _centerDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _topDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _leftDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _rightDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<int> _lastAttackerClientId = new(0, NetworkVariableReadPermission.Everyone);

        private AudioSource _audioSource;

        public static float DestroyDelay = 1f;

        // TODO: Test this value
        private const float LifeShieldFragmentRadius = 0.3f;

        private readonly Dictionary<LifeShieldArea, LifeShieldFragment> _fragments = new();

        public static event Action<ulong> OnCenterDestroyed;

        public static event Action<ulong> OnTopDestroyed;

        public static event Action<ulong> OnLeftDestroyed;

        public static event Action<ulong> OnRightDestroyed;

        public static event Action<ulong> OnSpawned;

        // The first ulong is the attackerClientId and the second is the ownerClientId
        public static event Action<ulong, ulong> OnDead;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<LifeShieldFragment>(out var fragment))
                {
                    _fragments.Add(fragment.Area, fragment);
                } 
            }
        }

        public override void OnNetworkSpawn()
        {
            ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).SetLifeShield(this);

            //Hide local player's shield
            if (OwnerClientId == NetworkManager.LocalClientId)
            {
                _fragments[LifeShieldArea.Center].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Top].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Left].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Right].transform.GetChild(0).gameObject.SetActive(false);
            }

            // TODO: Destroy fragments that have already been destroyed for late joined spectator.

            _centerDestroyed.OnValueChanged += OnCenterDestroyedFunc;
            _topDestroyed.OnValueChanged += OnTopDestroyedFunc;
            _leftDestroyed.OnValueChanged += OnLeftDestroyedFunc;
            _rightDestroyed.OnValueChanged += OnRightDestroyedFunc;

            OnSpawned?.Invoke(OwnerClientId);
        }

        public override void OnNetworkDespawn()
        {
            _centerDestroyed.OnValueChanged -= OnCenterDestroyedFunc;
            _topDestroyed.OnValueChanged -= OnTopDestroyedFunc;
            _leftDestroyed.OnValueChanged -= OnLeftDestroyedFunc;
            _rightDestroyed.OnValueChanged -= OnRightDestroyedFunc;
        }

        public void OnDamaged(Transform bulletTransform, ulong attackerClientId)
        {
            int hitFragmentCount = 0;
            foreach (var fragment in _fragments.Values)
            {
                if (Vector3.Distance(fragment.transform.position, bulletTransform.position) < LifeShieldFragmentRadius)
                {
                    _lastAttackerClientId.Value = (int)attackerClientId;
                    switch (fragment.Area)
                    {
                        case LifeShieldArea.Center:
                            if (_centerDestroyed.Value)
                            {
                                continue;
                            }
                            _centerDestroyed.Value = true;
                            GetComponent<Collider>().enabled = false;
                            bulletTransform.GetComponent<AttackSpell>().OnHitFunc();
                            return;
                        case LifeShieldArea.Top:
                            if (_topDestroyed.Value)
                            {
                                continue;
                            }
                            hitFragmentCount++;
                            _topDestroyed.Value = true;
                            break;
                        case LifeShieldArea.Left:
                            if (_leftDestroyed.Value)
                            {
                                continue;
                            }
                            hitFragmentCount++;
                            _leftDestroyed.Value = true;
                            break;
                        case LifeShieldArea.Right:
                            if (_rightDestroyed.Value)
                            {
                                continue;
                            }
                            hitFragmentCount++;
                            _rightDestroyed.Value = true;
                            break;
                    }
                }
            }
            if (hitFragmentCount> 0)
            {
                bulletTransform.GetComponent<AttackSpell>().OnHitFunc();
            }
        }

        private void OnCenterDestroyedFunc(bool oldValue, bool newValue)
        {
            if (!newValue)
            {
                return;
            }

            if (IsServer)
            {
                _topDestroyed.Value = true;
                _leftDestroyed.Value = true;
                _rightDestroyed.Value = true;
            }
            OnCenterDestroyed?.Invoke(OwnerClientId);
            PlayHitSound();
            _fragments[LifeShieldArea.Center].transform.GetChild(0).gameObject.SetActive(false);

            // The entire shield has been destroyed at this point
            PlayDestroySound();
            OnDead?.Invoke((ulong)_lastAttackerClientId.Value, OwnerClientId);
            if (IsServer)
            {
                Destroy(gameObject, DestroyDelay);
            }
        }

        private void OnTopDestroyedFunc(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                OnTopDestroyed?.Invoke(OwnerClientId);
                PlayHitSound();
                _fragments[LifeShieldArea.Top].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void OnLeftDestroyedFunc(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                OnLeftDestroyed?.Invoke(OwnerClientId);
                PlayHitSound();
                _fragments[LifeShieldArea.Left].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void OnRightDestroyedFunc(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                OnRightDestroyed?.Invoke(OwnerClientId);
                PlayHitSound();
                _fragments[LifeShieldArea.Right].transform.GetChild(0).gameObject.SetActive(false);
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