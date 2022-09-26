using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Holoi.Reality.MOFO
{
    public class VfxAlphaController : NetworkBehaviour
    {
        [SerializeField] private VisualEffect _vfx1;

        [SerializeField] private VisualEffect _vfx2;

        [SerializeField] private GameObject _localHand;

        private bool _isVfx1Active = true;

        private float _accumulatedTime;

        private void Start()
        {
            _vfx2.GetComponent<Animator>().SetTrigger("Fade Out");
        }

        public void TransitToVfx1()
        {
            _vfx1.GetComponent<Animator>().SetTrigger("Fade In");
            _vfx2.GetComponent<Animator>().SetTrigger("Fade Out");
        }

        public void TransitToVfx2()
        {
            _vfx1.GetComponent<Animator>().SetTrigger("Fade Out");
            _vfx2.GetComponent<Animator>().SetTrigger("Fade In");
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (Vector3.Distance(_vfx1.transform.position, _localHand.transform.position) < 0.4f)
                {
                    _accumulatedTime += Time.fixedDeltaTime;
                    if (_accumulatedTime > 5f)
                    {
                        if (_isVfx1Active)
                        {
                            OnTransitToVfx2ClientRpc();
                            _accumulatedTime = 0f;
                            _isVfx1Active = false;
                        }
                        else
                        {
                            OnTransitToVfx1ClientRpc();
                            _accumulatedTime = 0f;
                            _isVfx1Active = true;
                        }
                    }
                }
            }
        }

        [ClientRpc]
        private void OnTransitToVfx1ClientRpc()
        {
            TransitToVfx1();
        }

        [ClientRpc]
        private void OnTransitToVfx2ClientRpc()
        {
            TransitToVfx2();
        }
    }
}
