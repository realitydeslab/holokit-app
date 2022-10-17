//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.VFX;

//public class Timer : MonoBehaviour
//{
//    VisualEffect vfx;
//    float currentTime = 0;
//    float totalTime = 0;
//    [SerializeField]
//    float defaultTotalTime = 60;

//    public bool startTimer = false;
//    // Start is called before the first frame update
//    void Start()
//    {
//        vfx = GetComponent<VisualEffect>();
//        Reset(defaultTotalTime);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (startTimer)
//        {
//            TimeCount();
//        }
//    }

//    void TimeCount()
//    {
//        if (currentTime > 0)
//        {
//            currentTime -= Time.deltaTime;

//        }
//        else
//        {
//            currentTime = 0;
//        }

//        SetVfx();
//    }

//    void SetVfx()
//    {
//        vfx.SetFloat("Time", currentTime);
//        //vfx.SetFloat("Process", currentTime / totalTime);
//    }
//    public void Reset(float InitialTime)
//    {
//        totalTime = InitialTime;
//        currentTime = InitialTime;
//    }

//    public void StartTimer()
//    {
//        Reset(defaultTotalTime);
//        startTimer = true;
//    }
//}
