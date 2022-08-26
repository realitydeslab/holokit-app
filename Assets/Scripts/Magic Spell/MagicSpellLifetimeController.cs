using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MagicSpellLifetimeController : NetworkBehaviour
{
    [SerializeField] private float _lifetime;

    // The time delay to destroy the object after it expired
    [SerializeField] private float _destroyDelay;

    [SerializeField] private AudioClip _spawnSound;

    [Networked(OnChanged = nameof(OnExpiredChanged))]
    public NetworkBool Expired { get; set; }

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
                if (Expired)
                {
                    Runner.Despawn(Object);
                }
                else
                {
                    Expired = true;
                    TickTimer = TickTimer.CreateFromSeconds(Runner, _destroyDelay);
                }
            }
        }
    }

    public static void OnExpiredChanged(Changed<MagicSpellLifetimeController> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.OnExpiredChanged();
        }
    }

    private void OnExpiredChanged()
    {
        // TODO: Play the fadeaway effect
        PlayDieVisual();
    }

    private void PlayDieVisual()
    {
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Die");
    }
}
