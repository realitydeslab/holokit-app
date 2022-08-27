using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MagicSpellLifetimeController : NetworkBehaviour
{
    [SerializeField] private float _lifetime;

    [SerializeField] private AudioClip _spawnSound;

    public TickTimer TickTimer { get; set; }

    public override void Spawned()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = _spawnSound;
        audioSource.Play();

        if (Object.HasStateAuthority)
        {
            TickTimer = TickTimer.CreateFromSeconds(Runner, _lifetime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (TickTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }
    }
}
