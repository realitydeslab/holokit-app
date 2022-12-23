using UnityEngine;
using HoloKit;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_WallCollisionController : MonoBehaviour, IDamageable
    {
        private void Start()
        {
            transform.position = HoloKitCamera.Instance.CenterEyePose.position + 8f * HoloKitCamera.Instance.CenterEyePose.forward;
            transform.LookAt(HoloKitCamera.Instance.CenterEyePose);

            Vector3 rotationEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y, 0f);
        }

        public void OnDamaged(ulong attackerClientId)
        {
            
        }
    }
}
