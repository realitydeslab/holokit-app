
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class TextController : MonoBehaviour
    {
        [HideInInspector] public bool isUpdated = false;

        [HideInInspector] public float AnimationProcess = 0;

        [SerializeField] VisualEffect _vfx;

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
                _vfx.SetFloat("AnimationProcess", AnimationProcess);
            }
            else
            {

            }
        }
    }
}
