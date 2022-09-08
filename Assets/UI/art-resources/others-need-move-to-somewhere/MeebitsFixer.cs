using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeebitsFixer : MonoBehaviour
{
    [SerializeField] Avatar _avatar;
    private void Awake()
    {
        GetComponent<Animator>().avatar = _avatar;
    }
}
