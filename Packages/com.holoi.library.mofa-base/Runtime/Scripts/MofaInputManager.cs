using System;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.HoloKitApp.WatchConnectivity;
using Holoi.Library.MOFABase.WatchConnectivity;

namespace Holoi.Library.MOFABase
{
    public class MofaInputManager : MonoBehaviour
    {
        public bool IsActive => _isActive;

        public float BasicSpellChargePercentage
        {
            get
            {
                return _basicSpellCharge / (_basicSpell.ChargeTime * _basicSpell.MaxChargeCount);
            }
        }

        public float SecondarySpellChargePercentage
        {
            get
            {
                if (_secondarySpell != null)
                {
                    return _secondarySpellCharge / _secondarySpell.ChargeTime;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public int SecondarySpellMaxUseCount => _secondarySpell.MaxUseCount;

        public int SecondarySpellUseCount => _secondarySpellUseCount;

        public MofaWatchState CurrentWatchState => _currentWatchState;

        public float Distance => _distance;

        private bool _isActive;

        private Spell _basicSpell;

        private Spell _secondarySpell;

        private float _basicSpellCharge;

        private float _secondarySpellCharge;

        private int _secondarySpellUseCount;

        private MofaWatchState _currentWatchState;

        private Vector3 _lastFramePosition;

        // The moving distance of the local player in a round.
        private float _distance;

        private MofaBaseRealityManager _mofaBaseRealityManager;

        private Transform _centerEyePose;

        public static event Action<SpellType> OnSpawnSpellFailed;

        private void Start()
        {
            // Only players need inputs
            if (!HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                Destroy(gameObject);
                return;
            }

            // Find necessary references
            _mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            _centerEyePose = HoloKitCamera.Instance.CenterEyePose;
            // Setup MofaWatchConnectivity
            MofaWatchConnectivityAPI.Initialize();
            // MofaWatchConnectivityManager should take control first
            MofaWatchConnectivityAPI.TakeControlWatchConnectivitySession();
            // We then update the control on Watch side so that MofaWatchConnectivityManager won't miss messages.
            HoloKitAppWatchConnectivityAPI.UpdateCurrentReality(WatchReality.MOFATheTraining);

            MofaWatchConnectivityAPI.OnStartRoundMessageReceived += OnStartRoundMessageReceived;
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnWatchTriggered;

            SetupSpells();
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedIndividualStats += OnReceivedIndividualStats;
            LifeShield.OnDestroyed += OnLifeShieldDestroyed;
            LifeShield.OnRenovated += OnLifeShieldRenovated;

            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
            HoloKitAppUIEventManager.OnBoosted += OnBoosted;
        }

        private void OnDestroy()
        {
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived -= OnStartRoundMessageReceived;
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered -= OnWatchTriggered;

            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
            MofaBaseRealityManager.OnReceivedIndividualStats -= OnReceivedIndividualStats;
            LifeShield.OnDestroyed -= OnLifeShieldDestroyed;
            LifeShield.OnRenovated -= OnLifeShieldRenovated;

            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
            HoloKitAppUIEventManager.OnBoosted -= OnBoosted;
        }

        private void SetupSpells()
        {
            var preferencedMagicSchool = HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject();
            int count = 0;
            foreach (var spell in _mofaBaseRealityManager.SpellList.List)
            {
                if (spell.MagicSchool.TokenId.Equals(preferencedMagicSchool.TokenId))
                {
                    if (spell.SpellType == SpellType.Basic)
                    {
                        _basicSpell = spell;
                        count++;
                    }
                    else
                    {
                        _secondarySpell = spell;
                        count++;
                    }
                }
                if (count == 2)
                {
                    return;
                }
            }
        }

        private void OnPhaseChanged(MofaPhase newPhase)
        {
            switch (newPhase)
            {
                case MofaPhase.Waiting:
                    break;
                case MofaPhase.Countdown:
                    Reset();
                    MofaWatchConnectivityAPI.SyncRoundStartToWatch();
                    break;
                case MofaPhase.Fighting:
                    _isActive = true;
                    MofaWatchConnectivityAPI.QueryWatchState();
                    break;
                case MofaPhase.RoundOver:
                    _isActive = false;
                    break;
                case MofaPhase.RoundResult:
                    break;
                case MofaPhase.RoundData:
                    break;
            }
        }

        private void OnReceivedIndividualStats(MofaIndividualStats individualStats)
        {
            MofaWatchConnectivityAPI.SyncRoundResultToWatch(individualStats.IndividualRoundResult,
                                                            individualStats.Kill,
                                                            individualStats.HitRate,
                                                            individualStats.Distance);
        }

        private void Update()
        {
            if (_mofaBaseRealityManager.CurrentPhase == MofaPhase.Fighting)
            {
                _distance = Vector3.Distance(_lastFramePosition, _centerEyePose.position);
                _lastFramePosition = _centerEyePose.position;
            }
        }

        private void FixedUpdate()
        {
            if (!IsActive)
            {
                return;
            }

            if (_basicSpellCharge < _basicSpell.ChargeTime * _basicSpell.MaxChargeCount)
            {
                _basicSpellCharge += Time.fixedDeltaTime;
                if (_basicSpellCharge > _basicSpell.ChargeTime * _basicSpell.MaxChargeCount)
                {
                    _basicSpellCharge = _basicSpell.ChargeTime * _basicSpell.MaxChargeCount;
                }
            }

            if (_currentWatchState == MofaWatchState.Ground)
            {
                if (_secondarySpellCharge < _secondarySpell.ChargeTime)
                {
                    _secondarySpellCharge += Time.fixedDeltaTime;
                    if (_secondarySpellCharge > _secondarySpell.ChargeTime)
                    {
                        _secondarySpellCharge = _secondarySpell.ChargeTime;
                    }
                }
            }
            else
            {
                if (_secondarySpellCharge > 0f)
                {
                    _secondarySpellCharge -= Time.fixedDeltaTime;
                }
            }
        }

        public void Reset()
        {
            _basicSpellCharge = 0f;
            _secondarySpellCharge = 0f;
            _secondarySpellUseCount = 0;
            _distance = 0;
        }

        private void SpawnBasicSpell()
        {
            if (!IsActive)
            {
                Debug.Log("[MofaInputManager] Not active");
                return;
            }

            if (_basicSpellCharge < _basicSpell.ChargeTime)
            {
                Debug.Log("[MofaInputManager] Basic spell not charged");
                OnSpawnSpellFailed?.Invoke(SpellType.Basic);
            }
            else
            {
                _mofaBaseRealityManager.SpawnSpellServerRpc(_basicSpell.Id,
                    HoloKitCamera.Instance.CenterEyePose.position,
                    HoloKitCamera.Instance.CenterEyePose.rotation,
                    NetworkManager.Singleton.LocalClientId);
                _basicSpellCharge -= _basicSpell.ChargeTime;
            }
        }

        private void SpawnSecondarySpell()
        {
            if (!IsActive)
            {
                Debug.Log("[MofaInputManager] Not active");
                return;
            }

            if (_secondarySpellUseCount > _secondarySpell.MaxUseCount)
            {
                Debug.Log("[MofaInputManager] Exceed secondary spell use count");
                OnSpawnSpellFailed?.Invoke(SpellType.Secondary);
                return;
            }

            _mofaBaseRealityManager.SpawnSpellServerRpc(_secondarySpell.Id,
                HoloKitCamera.Instance.CenterEyePose.position,
                HoloKitCamera.Instance.CenterEyePose.rotation,
                NetworkManager.Singleton.LocalClientId);
            _secondarySpellCharge -= _secondarySpell.ChargeTime;
            _secondarySpellUseCount++;
        }

        private void OnLifeShieldDestroyed(ulong _, ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                _isActive = false;
            }
        }

        private void OnLifeShieldRenovated(ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                if (_mofaBaseRealityManager.CurrentPhase == MofaPhase.Fighting)
                {
                    _isActive = true;
                    MofaWatchConnectivityAPI.QueryWatchState();
                }
            }
        }

        private void OnTriggered()
        {
            if (_mofaBaseRealityManager.CurrentPhase == MofaPhase.Fighting)
            {
                if (IsActive)
                {
                    SpawnBasicSpell();
                }
                else
                {
                    Debug.Log("[MofaInputManager] You cannot cast spells when you are dead :(");
                }
            }
            else if (_mofaBaseRealityManager.CurrentPhase == MofaPhase.Waiting || _mofaBaseRealityManager.CurrentPhase == MofaPhase.RoundData)
            {
                _mofaBaseRealityManager.TryStartRound();
            }
        }

        private void OnBoosted()
        {
            if (IsActive)
            {
                SpawnSecondarySpell();
            }
        }

        #region Apple Watch
        private void OnStartRoundMessageReceived()
        {
            _mofaBaseRealityManager.TryStartRound();
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            _currentWatchState = watchState;
        }

        private void OnWatchTriggered()
        {
            if (IsActive)
            {
                if (_secondarySpellCharge >= _secondarySpell.ChargeTime)
                {
                    SpawnSecondarySpell();
                }
                else
                {
                    SpawnBasicSpell();
                }
            }
        }
        #endregion
    }
}