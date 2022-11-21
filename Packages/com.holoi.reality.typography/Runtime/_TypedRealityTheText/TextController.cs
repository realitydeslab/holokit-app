
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        public bool isUpdated = false;
        TheTextRealityManager _manager;

        void Start()
        {
            _manager = FindObjectOfType<TheTextRealityManager>();
        }

        void Update()
        {
            if (isUpdated)
            {
                _vfx.SetVector3("ThumbPosition", _manager.ThumbJoint.position);
                _vfx.SetVector3("IndexPosition", _manager.IndexJoint.position);
            }
            else
            {

            }
        }
    }
}
