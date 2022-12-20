using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAvatarAnimationEventHandler : MonoBehaviour
    {
        private MofaAIPlayer _mofaAIPlayer;

        private void Start()
        {
            _mofaAIPlayer = GetComponentInParent<MofaAIPlayer>();
        }

        public void FootL()
        {
            _mofaAIPlayer.AnimationEventReceiver_FootL();
        }

        public void FootR()
        {
            _mofaAIPlayer.AnimationEventReceiver_FootR();
        }

        public void Land()
        {
            _mofaAIPlayer.AnimationEventReceiver_Land();
        }

        public void Hit()
        {
            _mofaAIPlayer.AnimationEventReceiver_Hit();
        }

        public void Shoot()
        {
            _mofaAIPlayer.AnimationEventReceiver_Shoot();
        }
    }
}
