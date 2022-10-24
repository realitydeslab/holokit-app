using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AvatarPlacementController : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] VisualEffect _placementVFX;
    [SerializeField] VisualEffect _birthVFX;

    void Start()
    {
        _placementVFX.enabled = true;
        _birthVFX.enabled = false;

    }

    public void OnBirth()
    {
        _animator.SetTrigger("Birth");
        _birthVFX.enabled = true;
    }
}