using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltMagic : MonoBehaviour
{
    [SerializeField] float _liftime = 3f; 
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, _liftime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter");
        GetComponent<Animator>().SetTrigger("Hit");
    }
}
