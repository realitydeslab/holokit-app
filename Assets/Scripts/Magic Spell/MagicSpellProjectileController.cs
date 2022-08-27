using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MagicSpellProjectileController : NetworkBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private bool _hitOnce = true;

    [SerializeField] private AudioClip _hitSound;

    [SerializeField] private float _radius;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            List<LagCompensatedHit> hits = new();
            Runner.LagCompensation.OverlapSphere(transform.position, _radius, Object.InputAuthority, hits);
            if (hits.Count > 0)
            {
                //hits[0].GameObject.GetComponent<>
            }
        }
    }
}

