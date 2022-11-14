using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    /// <summary>  Allows the Animal to Exit a State to another. Using the Animation Time </summary>
    public class StateExitBehaviour : StateMachineBehaviour
    {
        private MAnimal animal;
        [Tooltip("State the Animal will exit to, when the time as passed. If null it will not force the next state")]
        public StateID ExitState;
        public IntReference StateExitStatus = new IntReference();
        [Range(0, 1)]
        public float m_time = 0.8f;
        private bool isOn;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal == null) animal = animator.FindComponent<MAnimal>();
            isOn = false;

            if (animal)
                animal.State_SetExitStatus(StateExitStatus);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo state, int layerIndex)
        {
            if (animal)
            {
                var time = state.normalizedTime % 1;

                if (!isOn && (time >= m_time))
                {
                    isOn = true;

                    animal.State_AllowExit(ExitState != null ? ExitState.ID : -1);
                }
            }
        }
    }
}