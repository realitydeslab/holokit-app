using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltManager : MonoBehaviour
{
    public Vector3 speed = new Vector3(0, 0, 5);
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.rotation * speed;
    }

    void Update()
    {

    }
}
