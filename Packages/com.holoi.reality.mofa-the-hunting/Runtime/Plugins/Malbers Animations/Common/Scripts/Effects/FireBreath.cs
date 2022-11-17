using UnityEngine;
using UnityEngine.VFX;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Utilities/Effects - Audio/FireBreath")]
    public class FireBreath : MonoBehaviour
    {
        [SerializeField] private VisualEffect _vfx;

        [SerializeField] private Collider _collider;

        public void Activate(bool value)
        {
            if (value)
            {
                _vfx.SendEvent("OnStart");
                _collider.enabled = true;
            }
            else
            {
                _vfx.SendEvent("OnStop");
                _collider.enabled = false;
            }
        }
    }
}
