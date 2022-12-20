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

        public bool IsDestroyed => CenterDestroyed.Value;

        public NetworkVariable<bool> CenterDestroyed = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> TopDestroyed = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> LeftDestroyed = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> RightDestroyed = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<byte> _lastAttackerClientId = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

        public static event Action<LifeShield> OnSpawned;

        // The first ulong is the attackerClientId and the second is the ownerClientId
        public static event Action<ulong, ulong> OnBeingHit;

        // The first ulong is the attackerClientId and the second is the ownerClientId
        public static event Action<ulong, ulong> OnBeingDestroyed;

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
           
            CenterDestroyed.OnValueChanged += OnCenterDestroyedFunc;
            TopDestroyed.OnValueChanged += OnTopDestroyedFunc;
            LeftDestroyed.OnValueChanged += OnLeftDestroyedFunc;
            RightDestroyed.OnValueChanged += OnRightDestroyedFunc;

            // TODO: Destroy fragments that have already been destroyed for late joined spectator.

            OnSpawned?.Invoke(this);
        }

        public override void OnNetworkDespawn()
        {
            CenterDestroyed.OnValueChanged -= OnCenterDestroyedFunc;
            TopDestroyed.OnValueChanged -= OnTopDestroyedFunc;
            LeftDestroyed.OnValueChanged -= OnLeftDestroyedFunc;
            RightDestroyed.OnValueChanged -= OnRightDestroyedFunc;
        }

        public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
        {
            base.OnNetworkObjectParentChanged(parentNetworkObject);
            if (parentNetworkObject != null && parentNetworkObject.TryGetComponent<MofaPlayer>(out var mofaPlayer))
            {
                transform.localPosition = mofaPlayer.CenterEyeToLifeShieldOffset;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }    
        }

        // Host only
        public void OnDamaged(LifeShieldArea area, ulong attackerClientId)
        {
            OnBeingHitClientRpc((byte)attackerClientId);
            _lastAttackerClientId.Value = (byte)attackerClientId;
            switch (area)
            {
                case LifeShieldArea.Center:
                    CenterDestroyed.Value = true;
                    TopDestroyed.Value = true;
                    LeftDestroyed.Value = true;
                    RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Top:
                    TopDestroyed.Value = true;
                    break;
                case LifeShieldArea.Left:
                    LeftDestroyed.Value = true;
                    break;
                case LifeShieldArea.Right:
                    RightDestroyed.Value = true;
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
                OnBeingDestroyed?.Invoke((ulong)_lastAttackerClientId.Value, OwnerClientId);
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
                CenterDestroyed.Value = false;
                TopDestroyed.Value = false;
                LeftDestroyed.Value = false;
                RightDestroyed.Value = false;
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