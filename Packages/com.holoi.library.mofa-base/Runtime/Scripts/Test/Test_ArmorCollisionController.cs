using UnityEngine;
using HoloKit;

namespace Holoi.Library.MOFABase.Test
{
    public class Test_ArmorCollisionController : MonoBehaviour, IDamageable
    {
        public void OnDamaged(ulong attackerClientId)
        {
            
        }

        private void LateUpdate()
        {
            transform.SetPositionAndRotation(HoloKitCamera.Instance.CenterEyePose.position, HoloKitCamera.Instance.CenterEyePose.rotation);
        }
    }
}
