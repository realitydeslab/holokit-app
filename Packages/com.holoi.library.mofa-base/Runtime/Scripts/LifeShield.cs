using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class LifeShield : NetworkBehaviour
    {
        public MagicSchool MagicSchool;

        [SerializeField] private AudioClip _beingHitSound;

        public bool IsDestroyed => _centerDestroyed.Value;

        private readonly NetworkVariable<bool> _centerDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _topDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _leftDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<bool> _rightDestroyed = new(false, NetworkVariableReadPermission.Everyone);

        private readonly NetworkVariable<byte> _lastAttackerClientId = new(0, NetworkVariableReadPermission.Everyone);

        private readonly Dictionary<LifeShieldArea, LifeShieldFragment> _fragments = new();

        /// <summary>
        /// Get a reference of this so we can easily play being hit sound.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// The life shield will automatically renovate itself after being destroyed.
        /// </summary>
        public const float RenovateTime = 3f;

        public event Action OnCenterDestroyed;

        public event Action OnTopDestroyed;

        public event Action OnLeftDestroyed;

        public event Action OnRightDestroyed;

        // The first ulong is the attackerClientId and the second is the ownerClientId
        public static event Action<ulong, ulong> OnBeingHit;

        // The first ulong is the attackerClientId and the second is the ownerClientId
        public static event Action<ulong, ulong> OnDestroyed;

        /// <summary>
        /// This event is called when the life shield is renovated. The parameter
        /// is the ownerClientId.
        /// </summary>
        public static event Action<ulong> OnRenovated;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<LifeShieldFragment>(out var fragment))
                {
                    _fragments.Add(fragment.Area, fragment);
                } 
            }
            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnNetworkSpawn()
        {
            var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaBaseRealityManager.SetLifeShield(this);

            //Hide local player's shield
            if (HoloKit.HoloKitUtils.IsRuntime && OwnerClientId == NetworkManager.LocalClientId)
            {
                _fragments[LifeShieldArea.Center].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Top].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Left].transform.GetChild(0).gameObject.SetActive(false);
                _fragments[LifeShieldArea.Right].transform.GetChild(0).gameObject.SetActive(false);
            }
           
            _centerDestroyed.OnValueChanged += OnCenterDestroyedFunc;
            _topDestroyed.OnValueChanged += OnTopDestroyedFunc;
            _leftDestroyed.OnValueChanged += OnLeftDestroyedFunc;
            _rightDestroyed.OnValueChanged += OnRightDestroyedFunc;

            // TODO: Destroy fragments that have already been destroyed for late joined spectator.

        }

        public override void OnNetworkDespawn()
        {
            _centerDestroyed.OnValueChanged -= OnCenterDestroyedFunc;
            _topDestroyed.OnValueChanged -= OnTopDestroyedFunc;
            _leftDestroyed.OnValueChanged -= OnLeftDestroyedFunc;
            _rightDestroyed.OnValueChanged -= OnRightDestroyedFunc;
        }

        // Host only
        public void OnDamaged(LifeShieldArea area, ulong attackerClientId)
        {
            OnBeingHitClientRpc((byte)attackerClientId);
            _lastAttackerClientId.Value = (byte)attackerClientId;
            switch (area)
            {
                case LifeShieldArea.Center:
                    _centerDestroyed.Value = true;
                    _topDestroyed.Value = true;
                    _leftDestroyed.Value = true;
                    _rightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Top:
                    _topDestroyed.Value = true;
                    break;
                case LifeShieldArea.Left:
                    _leftDestroyed.Value = true;
                    break;
                case LifeShieldArea.Right:
                    _rightDestroyed.Value = true;
                    break;
            }
        }

        [ClientRpc]
        private void OnBeingHitClientRpc(byte attackerClientId)
        {
            OnBeingHit?.Invoke((ulong)attackerClientId, OwnerClientId);
        }

        private void OnCenterDestroyedFunc(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                OnCenterDestroyed?.Invoke();
                PlayHitSound();
                _fragments[LifeShieldArea.Center].gameObject.SetActive(false);

                // The entire shield has been destroyed at this point
                OnDestroyed?.Invoke((ulong)_lastAttackerClientId.Value, OwnerClientId);
                StartCoroutine(OnDestroyedInternal());
            }
        }

        /// <summary>
        /// Renovate life shield after a certian period of time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnDestroyedInternal()
        {
            yield return new WaitForSeconds(RenovateTime);
            if (IsServer)
            {
                _centerDestroyed.Value = false;
                _topDestroyed.Value = false;
                _leftDestroyed.Value = false;
                _rightDestroyed.Value = false;
            }
            _fragments[LifeShieldArea.Center].gameObject.SetActive(true);
            _fragments[LifeShieldArea.Top].gameObject.SetActive(true);
            _fragments[LifeShieldArea.Left].gameObject.SetActive(true);
            _fragments[LifeShieldArea.Right].gameObject.SetActive(true);
            OnRenovated?.Invoke(OwnerClientId);
        }

        private void OnTopDestroyedFunc(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                OnTopDestroyed?.Invoke();
                PlayHitSound();
                _fragments[LifeShieldArea.Top].gameObject.SetActive(false);
            }
        }

        private void OnLeftDestroyedFunc(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                OnLeftDestroyed?.Invoke();
                PlayHitSound();
                _fragments[LifeShieldArea.Left].gameObject.SetActive(false);
            }
        }

        private void OnRightDestroyedFunc(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                OnRightDestroyed?.Invoke();
                PlayHitSound();
                _fragments[LifeShieldArea.Right].gameObject.SetActive(false);
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
    }
}