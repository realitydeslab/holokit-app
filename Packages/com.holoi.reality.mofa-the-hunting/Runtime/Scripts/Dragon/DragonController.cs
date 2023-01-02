using System;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;
using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using MalbersAnimations.Events;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public partial class DragonController : NetworkBehaviour
    {
        public bool IsFlying => _animal.activeState.ID == _fly;

        [Header("Reference")]
        [SerializeField] private MAnimal _animal;

        [SerializeField] private Aim _aim;

        [SerializeField] private Material _bodyMaterial;

        [SerializeField] private Material _wingMaterial;

        [SerializeField] private Material _eyeMaterial;

        [SerializeField] private VisualEffect _dragonParticleVfx;

        [SerializeField] private VisualEffect _dragonExplodeVfx;

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

        [Header("Debug")]
        [SerializeField] private bool _isDead;

        private NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private bool _readyForControl = false;

        //public AimMode AimMode
        //{
        //    get => _aimMode;
        //    set
        //    {
        //        _aimMode = value;
        //        if (value == AimMode.Camera)
        //        {
        //            _aim.AimTarget = null;
        //            _aim.UseCamera = true;
        //            _aim.MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).PlayerDict[0].transform;
        //        }
        //        else
        //        {
        //            var mofaBaseRealityManager = (MofaBaseRealityManager)HoloKitApp.Instance.RealityManager;
        //            if (mofaBaseRealityManager.PlayerDict.ContainsKey(1))
        //            {
        //                _aim.AimTarget = mofaBaseRealityManager.PlayerDict[1].LifeShield.transform;
        //            }
        //            else
        //            {
        //                _aim.AimTarget = mofaBaseRealityManager.PlayerDict[0].transform;
        //            }
        //            _aim.UseCamera = false;
        //            //_aim.MainCamera = null;
        //        }
        //        Debug.Log($"[TheDragonController] AimMode changed to {_aimMode}");
        //    }
        //}

        public static event Action OnDragonSpawned;

        private void Start()
        {
            //AimMode = AimMode.Camera;

            //_aim.UseCamera = false;
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

            EntranceAnimation();
        }

        private void Update()
        {
            if (!_readyForControl)
            {
                if (_animal.activeState.ID == _idle)
                {
                    _readyForControl = true;
                    Debug.Log("Dragon now is ready for control");
                }  
            }

            if (_isDead)
            {
                _isDead = false;
                _currentHealth.Value = 0;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _currentHealth.OnValueChanged -= OnCurrentHealthValueChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void OnDamaged(int damage, ulong attackerClientId)
        {
            _currentHealth.Value -= damage;
        }

        private void OnCurrentHealthValueChanged(int oldValue, int newValue)
        {
            if (oldValue > newValue)
            {
                //Debug.Log($"[DragonHealth] oldValue: {oldValue}, newValue: {newValue}");
                if (newValue <= 0)
                {
                    DeathAnimation();
                }
            }
        }

        public void SetTarget(Transform targetTransform)
        {
            _aim.AimTarget = targetTransform;
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

        private void EntranceAnimation()
        {
            float flyDistance = 4f;
            float flyDuration = 5;
            SwitchToFly();
            // Fly forward
            if (IsServer)
            {
                LeanTween.move(gameObject, transform.position + transform.forward * flyDistance, flyDuration);
            }
            _bodyMaterial.SetInt("_IsClip", 1);
            _wingMaterial.SetInt("_IsClip", 1);
            _eyeMaterial.SetInt("_IsClip", 1);
            _dragonParticleVfx.SetBool("IsClip", true);
            var mofaHuntingRealityManager = (MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager;
            LeanTween.value(0f, 1f, flyDuration)
                .setOnUpdate((float _) =>
                {
                    if (mofaHuntingRealityManager.PortalController != null)
                    {
                        Vector3 portalForward = mofaHuntingRealityManager.PortalController.transform.forward;
                        float distance = Vector3.ProjectOnPlane(mofaHuntingRealityManager.PortalController.transform.position, Vector3.up).magnitude;
                        Vector4 clipVector4 = new(-portalForward.x, -portalForward.y, -portalForward.z, distance);
                        _bodyMaterial.SetVector("_Clip_Plane", clipVector4);
                        _wingMaterial.SetVector("_Clip_Plane", clipVector4);
                        _eyeMaterial.SetVector("_Clip_Plane", clipVector4);
                        _dragonParticleVfx.SetVector4("Clip Plane", clipVector4);
                    }
                })
                .setOnComplete(() =>
                {
                    _bodyMaterial.SetInt("_IsClip", 0);
                    _wingMaterial.SetInt("_IsClip", 0);
                    _eyeMaterial.SetInt("_IsClip", 0);
                    _dragonParticleVfx.SetBool("IsClip", false);
                    _dragonParticleVfx.Reinit();
                    SwitchToGround();
                });
        }

        private void DeathAnimation()
        {
            _animal.State_Activate(_death);
          
            float duration = 5f;
            Debug.Log("body isclip: " + _bodyMaterial.GetInt("_IsClip"));
            _bodyMaterial.SetInt("_IsClip", 1);
            _wingMaterial.SetInt("_IsClip", 1);
            _eyeMaterial.SetInt("_IsClip", 1);
            Debug.Log("body isclip: " + _bodyMaterial.GetInt("_IsClip"));
            _dragonParticleVfx.SetBool("IsClip", true);
            LeanTween.value(3f, -2f, duration)
                .setOnUpdate((float height) =>
                {
                    if (gameObject == null) { return; }
                    Vector4 clipVector4 = new(0f, 1f, 0f, transform.position.y + height);
                    _bodyMaterial.SetVector("_Clip_Plane", clipVector4);
                    _wingMaterial.SetVector("_Clip_Plane", clipVector4);
                    _eyeMaterial.SetVector("_Clip_Plane", clipVector4);
                    _dragonParticleVfx.SetVector4("Clip Plane", clipVector4);
                })
                .setOnComplete(() =>
                {
                    if (IsServer)
                    {
                        ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).OnDragonDead();
                        Destroy(gameObject, 3f);
                    }
                });
        }

        [ClientRpc]
        public void OnDragonBeingHitClientRpc(Vector3 hitPosition)
        {
            _dragonExplodeVfx.SetVector3("HitPosition", hitPosition);
            _dragonExplodeVfx.SendEvent("OnHit");
        }

        #region Network Callbacks
        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        public void Movement_UseCameraInputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.UseCameraInput = value;
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        public void Movement_SetUpDownAxisClientRpc(float value)
        {
            if (!IsOwner)
            {
                _animal.SetUpDownAxis(value);
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
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
