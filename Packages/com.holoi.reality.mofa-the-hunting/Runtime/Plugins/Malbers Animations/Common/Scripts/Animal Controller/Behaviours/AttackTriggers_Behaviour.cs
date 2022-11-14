using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/Attack Triggers")]
    public class AttackTriggers_Behaviour : StateMachineBehaviour
    {
       public List<AttacksBehavior> triggers;
        [Tooltip("Damage Multiplier to apply regarding the animation.. (A finisher can cause more Damage)")]
        public float Multiplier = 1f;

       private IMDamagerSet[] damagers;

        override public void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (damagers == null) damagers = anim.GetComponents<IMDamagerSet>();


            //Let know everybody that an attack trigger animation has started (Needed for the Weapon Manager)
            if (damagers != null)
                foreach (var d in damagers)
                    d.DamagerAnimationStart(stateInfo.fullPathHash);

            foreach (var item in triggers)
            {
                item.isOff = false;
                item.isOn = false;
            }
        }

        override public void OnStateUpdate(Animator anim, AnimatorStateInfo state, int layer)
        {
            var time = state.normalizedTime % 1;

            foreach (var atk in triggers)
            {
                if (!atk.isOn && (time >= atk.AttackActivation.minValue))
                {
                    foreach (var d in damagers) d.ActivateDamager(atk.AttackTrigger, Multiplier);
                    atk.isOn = true;
                }

                if (!atk.isOff && (time >= atk.AttackActivation.maxValue))
                {
                    //means is transitioning to it self so do not OFF it
                    if (anim.IsInTransition(layer) && anim.GetNextAnimatorStateInfo(layer).fullPathHash == state.fullPathHash) return; 
                    foreach (var d in damagers) d.ActivateDamager(0,Multiplier);
                    atk.isOff = true;
                }
            }
        }

        override public void OnStateExit(Animator anim, AnimatorStateInfo state, int layer)
        {
            if (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == state.fullPathHash) return; //means is transitioning to it self

            foreach (var atk in triggers)
            {

                if (!atk.isOff)
                    foreach (var d in damagers) d.ActivateDamager(0, Multiplier);  //Double check that the Trigger is OFF

                atk.isOn = atk.isOff = false;                                               //Reset the ON/OFF variables
            }


            //Let know everybody that an attack trigger animation has started (Needed for the Weapon Manager)
            if (damagers != null)
                foreach (var d in damagers)
                    d.DamagerAnimationEnd(state.fullPathHash);
        }


        private void OnValidate()
        {
            foreach (var item in triggers)
            {
                item.name = $"Trigger [{item.AttackTrigger}] → ({item.AttackActivation.minValue}) - ({item.AttackActivation.maxValue})";
            }
        }
    }


    

    [System.Serializable]
    public class AttacksBehavior
    {
        [HideInInspector] public string name;
        [Tooltip("0: Disable All Attack Triggers\n-1: Enable All Attack Triggers\nx: Enable the Attack Trigger by its index")]
        public int AttackTrigger = 1;                           //ID of the Attack Trigger to Enable/Disable during the Attack Animation

        [Tooltip("Range on the Animation that the Attack Trigger will be Active")]
        [MinMaxRange(0, 1)]
        public RangedFloat AttackActivation = new RangedFloat(0.3f, 0.6f);


        public bool isOn { get; set; }
        public bool isOff { get; set; }
    }
}