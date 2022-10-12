using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.QuantumBuddhas
{
    public class QuantumBuddhasSceneManager : MonoBehaviour
    {
        public List<VisualEffect> _vfxs = new List<VisualEffect>();
        int _amount = 0;
        int _index = 0;

        void Start()
        {
            _amount = _vfxs.Count;
        }

        void Update()
        {
        
        }

        public void SwitchToNextVFX()
        {
            _index++;
            if (_index == _vfxs.Count) _index = 0;
            foreach (var vfx in _vfxs)
            {
                vfx.gameObject.SetActive(false);
            }
            _vfxs[_index].gameObject.SetActive(true);
        }
    }
}