using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ThumbnailDragonController : MonoBehaviour
{
    Animator _animator;

    // debug: 
    public bool _fireBall;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_fireBall)
        {
            _animator.SetTrigger("Fire Ball");
            _fireBall = false;
        }
    }

    public void PlaySound()
    {

    }
}
