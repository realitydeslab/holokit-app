using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public enum AimMode
    {
        Camera = 0,
        Target = 1
    }

    public class TheDragonController : NetworkBehaviour
    {
        [SerializeField] private MAnimal _animal;

        [SerializeField] private Aim _aim;

        [Header("ModeID")]
        [SerializeField] private ModeID _attack1;

        [SerializeField] private ModeID _attack1Air;

        [SerializeField] private ModeID _attack2;

        [SerializeField] private ModeID _action;

        [Header("StateID")]
        [SerializeField] private StateID _fly;

        [SerializeField] private StateID _death;

        [SerializeField] private StateID _idle;

        [SerializeField] private StateID _fall;

        [Header("Parameters")]
        [SerializeField] private int _maxHealth = 50;

        private NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public AimMode AimMode
        {
            get => _aimMode;
            set
            {
                _aimMode = value;
                if (value == AimMode.Camera)
                {
                    _aim.AimTarget = null;
                    _aim.UseCamera = true;
                    _aim.MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform; 
                }
                else
                {
                    _aim.AimTarget = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
                    _aim.UseCamera = false;
                    _aim.MainCamera = null;
                }
            }
        }

        private AimMode _aimMode = AimMode.Camera;

        public static event Action OnDragonSpawned;

        private void Awake()
        {
            _animal.m_MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
        }

        private void Start()
        {
            AimMode = AimMode.Camera;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetTheDragonController(this);
            OnDragonSpawned?.Invoke();
            if (IsServer)
            {
                _currentHealth.Value = _maxHealth;
            }
            _currentHealth.OnValueChanged += OnCurrentHealthValueChanged;

            StartCoroutine(EntranceAnimation());
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _currentHealth.OnValueChanged -= OnCurrentHealthValueChanged;
        }

        public void OnDamaged(int damage, ulong attackerClientId)
        {
            _currentHealth.Value -= damage;
        }

        private void OnCurrentHealthValueChanged(int oldValue, int newValue)
        {
            if (oldValue > newValue)
            {
                Debug.Log($"[DragonHealth] oldValue: {oldValue}, newValue: {newValue}");
                if (newValue <= 0)
                {
                    // TODO: dragon death animation
                    _animal.State_Activate(_death);
                    if (IsServer)
                    {
                        StartCoroutine(((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).OnDragonDead());
                        Destroy(gameObject, 5f);
                    }
                }
            }
        }

        private void SwitchToFly()
        {
            _animal.State_Activate(_fly);
        }

        private void SwitchToGround()
        {
            _animal.State_AllowExit(_fly);
            _animal.State_Activate(_fall);
        }

        private IEnumerator EntranceAnimation()
        {
            SwitchToFly();
            // Fly forward
            if (IsServer)
            {
                LeanTween.move(gameObject, transform.position + transform.forward * 2f, 3f);
            }
            yield return new WaitForSeconds(3.5f);
            SwitchToGround();
        }

        #region Network Callbacks
        [ClientRpc]
        public void Movement_UseCameraInputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.UseCameraInput = value;
            }
        }

        [ClientRpc]
        public void Movement_SetUpDownAxisClientRpc(float value)
        {
            if (!IsOwner)
            {
                _animal.SetUpDownAxis(value);
            }
        }

        [ClientRpc]
        public void Movement_SetInputAxisClientRpc(Vector2 value)
        {
            if (!IsOwner)
            {
                _animal.SetInputAxis(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack1ClientRpc()
        {
            Debug.Log("Mode_Pin_Attack1ClientRpc");
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack1);
            }
        }

        [ClientRpc]
        public void Mode_Pin_InputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Input(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_AbilityClientRpc(int value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Ability(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack1AirClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack1Air);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack2ClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack2);
            }
        }

        [ClientRpc]
        public void Mode_InterruptClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Interrupt();
            }
        }

        [ClientRpc]
        public void Mode_Pin_ActionClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_action);
            }
        }

        [ClientRpc]
        public void Mode_Pin_TimeClientRpc(float value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Time(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_StatusClientRpc(int value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Status(value);
            }
        }

        [ClientRpc]
        public void State_Pin_FlyClientRpc()
        {
            if (!IsOwner)
            {
                _animal.State_Pin(_fly);
            }
        }

        [ClientRpc]
        public void State_Pin_ByInputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.State_Pin_ByInput(value);
            }
        }

        [ClientRpc]
        public void SpeedDownClientRpc()
        {
            if (!IsOwner)
            {
                _animal.SpeedDown();
            }
        }

        [ClientRpc]
        public void SpeedUpClientRpc()
        {
            if (!IsOwner)
            {
                _animal.SpeedUp();
            }
        }
        #endregion
    }
}
