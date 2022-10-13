using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;

public class DragonController : MonoBehaviour
{
    public void PlaySound(AnimationEvent animationEvent)
    {
        GetComponent<AudioSource>().Play();
        PHASESource ps = GetComponent<PHASESource>();
        ps.Play();
    }
}
