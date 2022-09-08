using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityThumbnailContainer : MonoBehaviour
{
    public float positionOffset;

    private void Update()
    {
        transform.position = new Vector3(-1 * positionOffset, 0,0);
    }
}
