
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class RainbowController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        public float Width = 0.05f;

        void Start()
        {
        }

        void Update()
        {
            _vfx.SetFloat("Width", Width);
        }

    }
}
