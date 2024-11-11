using MalbersAnimations.Utilities;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Effects")]
    public class EffectBehaviour : StateMachineBehaviour
    {
        public List<EffectItem> effects = new List<EffectItem>();
        private EffectManager effectManager;


        override public void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (effectManager == null) effectManager = anim.GetComponentInChildren<EffectManager>();

            if (effectManager)
            {
                foreach (var item in effects) item.sent = false;
            }
        }

        override public void OnStateUpdate(Animator anim, AnimatorStateInfo state, int layer)
        {
            if (effectManager)
            {
                var time = state.normalizedTime % 1;

                foreach (var e in effects)
                {
                    if (!e.sent && (time >= e.Time))
                    {
                        if (e.action == EffectOption.Play)
                            effectManager.PlayEffect(e.ID);
                        else
                            effectManager.StopEffect(e.ID);
                        e.sent = true;
                    }
                }
            }
        }

        override public void OnStateExit(Animator anim, AnimatorStateInfo state, int layer)
        {
            if (effectManager)
            {
                if (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == state.fullPathHash) return; //means is transitioning to it self

                foreach (var e in effects)
                {
                    if (e.Time == 1 || (e.ExecuteOnExit && !e.sent))
                    {
                        if (e.action == EffectOption.Play)
                        {
                            effectManager.PlayEffect(e.ID);
                        }
                        else
                        {
                            effectManager.StopEffect(e.ID);
                        }
                    }
                    e.sent = true;
                }
            }
        }

        private void OnValidate()
        {
            foreach (var item in effects)
            {
                item.name = $"{item.action} Effect [{item.ID}]";

                if (item.Time == 0)
                    item.name += $"  -  [On Enter]";
                else if (item.Time == 1)
                    item.name += $"  -  [On Exit]";
                else
                    item.name += $"  -  [OnTime] ({item.Time:F2})";

                item.showExecute = item.Time != 1  && item.Time != 0;
            }
        }
    }




    [System.Serializable]
    public class EffectItem
    {
        [HideInInspector] public string name;
        [HideInInspector] public bool showExecute;
        [Tooltip("ID of the Effect")]
        public int ID = 1;                           //ID of the Attack Trigger to Enable/Disable during the Attack Animation
        public EffectOption action = EffectOption.Play;
        [Range(0, 1)]
        public float Time = 0;

        [Tooltip("If the animation is interrupted Play or Stop the Effect on Exit")]
        [Hide(nameof(showExecute))]
        public bool ExecuteOnExit = true;
        public bool sent { get; set; }
    }

    public enum EffectOption { Play, Stop }



#if UNITY_EDITOR

#endif
}