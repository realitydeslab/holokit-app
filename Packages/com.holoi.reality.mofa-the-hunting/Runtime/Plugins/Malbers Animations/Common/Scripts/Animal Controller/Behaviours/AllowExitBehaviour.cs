using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    
    public class AllowExitBehaviour : StateMachineBehaviour
    {
        private MAnimal animal;
        [Tooltip("State the Animal will exit to.\n If is left to null. it wont force a next state; AC will decide which will be the next State to Activate")]
        public StateID NextState;
        [Tooltip("The Animal will set an exit Status, so the Next State know what was the last animation played from the previous state")]
        public IntReference ExitStatus = new IntReference();
        [Tooltip("After this normalized time has elapsed, The State will be force to exit.")]
        [Range(0, 1)]
        public float m_time = 0.8f;
        private bool isOn;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal == null) animal = animator.FindComponent<MAnimal>();
            isOn = false;
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo state, int layerIndex)
        {
         //   Debug.Log($"Time {state.normalizedTime:F3}  >= {m_time} ");  

            if (animal && !isOn && (state.normalizedTime/* % 1*/ >= m_time))
            {
                isOn = true;

              //  Debug.Log("on");

                var nextSt = NextState ? NextState.ID : -1;
                animal.State_Allow_Exit(nextSt, ExitStatus);
            }
        }

//#if UNITY_EDITOR    //INSPECTOR
//        [UnityEditor.CustomEditor(typeof(AllowExitBehaviour))]
//        public class AllowExitBehaviourEditor : UnityEditor.Editor
//        {
//            private UnityEditor.SerializedProperty Raise;

//            void OnEnable()
//            {
               
//            }


//            public override void OnInspectorGUI()
//            {
                
//            }
//        }
//#endif
    }
}