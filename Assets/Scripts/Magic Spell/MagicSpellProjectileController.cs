using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//public struct HitInfo : INetworkStruct
//{
//    public NetworkId NetworkId;
//    public LifeShieldArea LifeShieldArea;
//}

//[RequireComponent(typeof(NetworkTransform))]
//[RequireComponent(typeof(Collider))]
//public class MagicSpellProjectileController : NetworkBehaviour
//{
//    [SerializeField] private float _speed;

//    // The time delay to destroy the object after it hit something
//    [SerializeField] private float _destroyDelay;

//    [SerializeField] private bool _hitOnce = true;

//    [SerializeField] private AudioClip _hitSound;

//    [Networked(OnChanged = nameof(OnHitInfoChanged))]
//    private NetworkLinkedList<HitInfo> _hitInfos { get; }

//    private LifetimeController _lifetimeController;

//    public static event Action OnLocalPlayerHit;

//    public static event Action OnLocalPlayerHitCenter;

//    public override void Spawned()
//    {
//        if (Object.HasStateAuthority)
//        {
//            _lifetimeController = GetComponent<LifetimeController>();
//        }
//    }

//    public override void FixedUpdateNetwork()
//    {
//        if (Object.HasStateAuthority)
//        {
//            if (!_lifetimeController.Expired)
//            {
//                if (!_hitOnce || (_hitOnce && _hitInfos.Count == 0))
//                {
//                    transform.position += _speed * Runner.DeltaTime * transform.forward;
//                }
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (Object != null && Object.IsValid && Object.HasStateAuthority)
//        {
//            if (!_lifetimeController.Expired)
//            {
//                if (other.TryGetComponent<IDamageable>(out var damageable))
//                {
//                    SimulationBehaviour simulationBehaviour;
//                    if (damageable.LifeShieldArea == LifeShieldArea.None)
//                    {
//                        // Not life shield
//                        simulationBehaviour = other.GetComponent<SimulationBehaviour>();
//                    }
//                    else
//                    {
//                        // Life shield
//                        simulationBehaviour = other.GetComponentInParent<SimulationBehaviour>();
//                    }
//                    if (simulationBehaviour == null)
//                        return;
//                    if (Object.StateAuthority != simulationBehaviour.Object.StateAuthority)
//                    {
//                        if (damageable.LifeShieldArea == LifeShieldArea.Center)
//                        {
//                            OnLocalPlayerHitCenter?.Invoke();
//                        }
//                        else
//                        {
//                            OnLocalPlayerHit?.Invoke();
//                        }
//                        if (_hitOnce)
//                            GetComponent<Collider>().enabled = false;
//                        _hitInfos.Add(new HitInfo
//                        {
//                            NetworkId = simulationBehaviour.Object.Id,
//                            LifeShieldArea = damageable.LifeShieldArea
//                        });
//                    }
//                }
//            }
//        }
//    }

//    public static void OnHitInfoChanged(Changed<ProjectileController> changed)
//    {
//        changed.Behaviour.PlayHitVisual();
//        changed.Behaviour.PlayHitSound();
//        if (App.Instance.GetPlayer().Object.StateAuthority != changed.Behaviour.Object.StateAuthority)
//        {
//            changed.LoadOld();
//            int oldCount = changed.Behaviour._hitInfos.Count;
//            changed.LoadNew();
//            HitInfo[] hitInfos = new HitInfo[changed.Behaviour._hitInfos.Count - oldCount];
//            for (int i = 0; i < changed.Behaviour._hitInfos.Count - oldCount; i++)
//            {
//                hitInfos[i] = changed.Behaviour._hitInfos.Get(oldCount + i);
//            }
//            changed.Behaviour.OnHitInfoChanged(hitInfos);
//        }
//    }

//    private void PlayHitVisual()
//    {
//        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
//    }

//    private void PlayHitSound()
//    {
//        var audioSource = GetComponent<AudioSource>();
//        audioSource.clip = _hitSound;
//        audioSource.Play();
//    }

//    private void OnHitInfoChanged(HitInfo[] hitInfos)
//    {
//        App.Instance.GameManager.OnHitInfoUpdated(Object.StateAuthority, hitInfos);
//    }
//}

