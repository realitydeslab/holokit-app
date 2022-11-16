using UnityEngine;

public class BoltMagic : MonoBehaviour
{
    [SerializeField] float _liftime = 3f; 
    
    private void Start()
    {
        Destroy(gameObject, _liftime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetTrigger("Hit");
        Destroy(gameObject, 0.3f);
    }
}
