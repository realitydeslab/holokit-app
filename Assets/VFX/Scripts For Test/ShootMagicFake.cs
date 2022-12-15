using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMagicFake : MonoBehaviour
{
    public float speed = 5f;

    public Animator animator;

    public bool reset = false;

    public bool die = false;

    public string triggerName = "";

    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        GetComponent<Rigidbody>().velocity = Vector3.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            reset = false;
            Reset();
        }

        if (die)
        {
            die = false;
            PlayOnDeath();
        }
    }

    void PlayOnDeath()
    {
        animator.SetTrigger(triggerName);
    }

    void Reset()
    {
        animator.Rebind();
        animator.Update(0);
        transform.position = pos;
    }
}
