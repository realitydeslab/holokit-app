using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUITest : MonoBehaviour
{
    Animator _animator;
    [SerializeField]bool toa = false;
    [SerializeField] bool tob = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    void Start()
    {

    }

    private void Update()
    {
        if (toa)
        {
            toa = false;
            PlayToAttack();
        }
        if (tob)
        {
            tob = false;
            PlayToFunction();
        }
    }

    public void PlayToAttack()
    {
        _animator.SetTrigger("To Attack");
    }
    public void PlayToFunction()
    {
        _animator.SetTrigger("To Function");
    }

    public void SetLife(int health) { }
}
