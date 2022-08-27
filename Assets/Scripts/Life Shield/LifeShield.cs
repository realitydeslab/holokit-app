using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class LifeShield : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnTopDestroyedChanged))]
    public bool TopDestroyed { get; set; }

    [Networked(OnChanged = nameof(OnBotDestroyedChanged))]
    public bool BotDestroyed { get; set; }

    [Networked(OnChanged = nameof(OnLeftDestroyedChanged))]
    public bool LeftDestroyed { get; set; }

    [Networked(OnChanged = nameof(OnRightDestroyedChanged))]
    public bool RightDestroyed { get; set; }

    public static event Action<PlayerRef> OnTopDestroyed;

    public static event Action<PlayerRef> OnBotDestroyed;

    public static event Action<PlayerRef> OnLeftDestroyed;

    public static event Action<PlayerRef> OnRightDestroyed;

    public static event Action<PlayerRef, PlayerRef> OnDeath;

    private static void OnTopDestroyedChanged(Changed<LifeShield> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.OnTopDestroyedChanged();
        }
    }

    private static void OnBotDestroyedChanged(Changed<LifeShield> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.OnBotDestroyedChanged();
        }
    }

    private static void OnLeftDestroyedChanged(Changed<LifeShield> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.OnLeftDestroyedChanged();
        }
    }

    private static void OnRightDestroyedChanged(Changed<LifeShield> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.OnRightDestroyedChanged();
        }
    }

    private void OnTopDestroyedChanged()
    {
        OnTopDestroyed?.Invoke(Object.InputAuthority);
        IsStillAlive();
    }

    private void OnBotDestroyedChanged()
    {
        OnBotDestroyed?.Invoke(Object.InputAuthority);
        IsStillAlive();
    }

    private void OnLeftDestroyedChanged()
    {
        OnLeftDestroyed?.Invoke(Object.InputAuthority);
        IsStillAlive();
    }

    private void OnRightDestroyedChanged()
    {
        OnRightDestroyed?.Invoke(Object.InputAuthority);
        IsStillAlive();
    }

    private void IsStillAlive()
    {
        if (TopDestroyed && BotDestroyed && LeftDestroyed && RightDestroyed)
        {
            
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
