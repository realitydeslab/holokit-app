using UnityEngine;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class KoreanFingerTextManager : MonoBehaviour
    {
        [SerializeField] HoloKitHandTracker _HHT;
        [SerializeField] GameObject[] _vfxs = new GameObject[5];
        [SerializeField] Transform[] _softTips = new Transform[5];

        void Start()
        {

        }

        void FixedUpdate()
        {

            for (int i = 0; i < 5; i++)
            {
                // particel vfx
                _vfxs[i].transform.position = _softTips[i].position;

            }

        }
        public void HideJoint()
        {
            if (_HHT.Visible)
            {
                _HHT.Visible = false;

            }
            else
            {
                _HHT.Visible = true;

            }
        }
    }
}
