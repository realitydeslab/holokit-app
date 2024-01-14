using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawnLogger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"[PrefabSpawnLogger] Start {gameObject.name}");
    }
}
