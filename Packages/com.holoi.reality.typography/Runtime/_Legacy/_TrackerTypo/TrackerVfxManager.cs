using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class TrackerVfxManager : MonoBehaviour
    {
        [SerializeField] Transform _serverCenterEye;

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<VisualEffect>().SetVector3("CenterEye Forward_position", _serverCenterEye.forward);
            }
        }
    }
}
