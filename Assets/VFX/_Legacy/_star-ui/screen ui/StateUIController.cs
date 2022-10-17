using UnityEngine;

public class StateUIController : MonoBehaviour
{
    private Animator _animator;

    private bool _isFunctional;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayToAttack()
    {
        if (_isFunctional)
        {
            _animator.SetTrigger("To Attack");
            _isFunctional = false;
        }
    }

    public void PlayToFunction()
    {
        if (!_isFunctional)
        {
            _animator.SetTrigger("To Function");
            _isFunctional = true;
        }
    }
}
