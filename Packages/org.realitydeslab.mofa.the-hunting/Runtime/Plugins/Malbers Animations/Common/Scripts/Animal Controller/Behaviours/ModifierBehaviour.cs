using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    public class ModifierBehaviour : StateMachineBehaviour
    {
        public AnimalModifier EnterModifier;
        public AnimalModifier ExitModifier;
        public AnimalModifier OnTimeModifier;
       
        [Range(0,1)]
        public float Time = 0f;
        private MAnimal animal;

        private bool sent;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<MAnimal>();
            sent = false;
            EnterModifier.Modify(animal);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExitModifier.Modify(animal);
            if (!sent) OnTimeModifier.Modify(animal);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!sent && stateInfo.normalizedTime >= Time)
            {
                OnTimeModifier.Modify(animal);
                sent = true;
            }
        }
    }
}