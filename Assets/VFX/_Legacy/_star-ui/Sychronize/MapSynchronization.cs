using UnityEngine;

public class MapSynchronization : MonoBehaviour
{
    Animator _animator;
    void Start()
    {
        _animator.GetComponent<Animator>();
    }

    public void Done()
    {
        _animator.SetTrigger("done");
    }
}
