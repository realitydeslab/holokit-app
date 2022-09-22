using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryMyselfAfterSomeTime : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime = 1f;

    void OnEnable()
    {
        Destroy(this.gameObject, _lifeTime);
    }
}
