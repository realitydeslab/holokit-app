using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioVfxController : MonoBehaviour
{
    public AudioProcess audioProcess;

    VisualEffect vfx;
    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        vfx.SetFloat("Amplitude", audioProcess.DbValue);
    }
}
