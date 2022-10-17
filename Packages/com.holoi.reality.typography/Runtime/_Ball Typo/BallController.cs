using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void OnCollisionEnter(Collision collision)
        {
            var hitPos = collision.transform.position;
            _vfx.SetVector3("Hit Position_position", hitPos);
            _vfx.SendEvent("OnExplode");
        }
    }
}
