using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissloveControl : MonoBehaviour
{
    float initTime = 0;
    public float totalTime = 1.5f;
    //public float totolHeight = 1.8f;
    public Vector2 heightRange = new Vector2(0,1);
    Material[] mats;

    private void OnEnable()
    {
        initTime = Time.time;
    }
    void Start()
    {
        initTime = Time.time;
        mats = GetComponent<SkinnedMeshRenderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        var t = Time.time - initTime;
        var percentT = t / totalTime;
        //var threshold = percentT * totolHeight;
        var threshold = heightRange.x + ((heightRange.y - heightRange.x) * percentT);
        foreach (var m in mats)
        {
            m.SetFloat("_Heiglt_Clip", threshold);
        }
    }
}
