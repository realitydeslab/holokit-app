using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MofaUIPositionLogger : MonoBehaviour
{
    private const float LOG_INTERVAL = 1;

    private float m_LastLogTime;

    private void Update()
    {
        if (Time.time - m_LastLogTime > LOG_INTERVAL)
        {
            m_LastLogTime = Time.time;
            Log();
        }
    }

    private void Log()
    {
        Debug.Log($"[MofaUIPositionLogger] {gameObject.name}: localPosition: {transform.localPosition}, localRotation: {transform.localRotation}");
        GameObject child1 = transform.GetChild(0).gameObject;
        Debug.Log($"[MofaUIPositionLogger] {child1.name}: localPosition: {child1.transform.localPosition}, localRotation: {child1.transform.localRotation}");
        GameObject child2 = child1.transform.GetChild(0).gameObject;
        Debug.Log($"[MofaUIPositionLogger] {child2.name}: localPosition: {child2.transform.localPosition}, localRotation: {child2.transform.localRotation}");
    }
}
