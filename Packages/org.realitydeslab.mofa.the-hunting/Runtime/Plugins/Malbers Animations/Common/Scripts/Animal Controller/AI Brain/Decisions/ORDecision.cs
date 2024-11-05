using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/OR Decision", order = 100)]
    public class ORDecision : MAIDecision
    {
        public override string DisplayName => "General/OR";
        [HideInInspector] public int list_index;

        public List<MAIDecision> decisions = new List<MAIDecision>();
        public List<bool> invert = new List<bool>();
        public bool debug;

        public override void PrepareDecision(MAnimalBrain brain, int Index)
        {
            if (invert.Count != decisions.Count) invert.Resize(decisions.Count);

            foreach (var d in decisions) d.PrepareDecision(brain, Index);
        }

        public override bool Decide(MAnimalBrain brain,int Index)
        {
            for (int i = 0; i < decisions.Count; i++)
            {
                bool Decision = decisions[i].Decide(brain, Index);
                if (invert[i]) Decision = !Decision;
                if (debug) Debug.Log($"[{brain.Animal.name}] -> [{(invert[i] ? "NOT " : " ")}{decisions[i].name}] -> [{Decision}]", this);
                if (Decision) return true;
            }
            return false;
        }
        public override void FinishDecision(MAnimalBrain brain, int Index)
        {
            foreach (var d in decisions) d?.FinishDecision(brain, Index);
        }

        public override void DrawGizmos(MAnimalBrain brain)
        {
            foreach (var d in decisions) d?.DrawGizmos(brain);
        }

        void Reset() { Description = "At least ONE decisions on the list  must be TRUE in order to sent a True Decision"; }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ORDecision))]
    public class ORDecisionEd : ANDDecisionEd
    {
        ORDecision ORD;

        protected override void FindTarget() => ORD = (ORDecision) target;
        protected override void ResizeInvert() => ORD.invert.Resize(ORD.decisions.Count);
        //protected override void ResizeDecisionList() => ORD.decisions.Resize(ORD.decisions.Count + 1);
        protected override string ListLabel => "OR";
    }
#endif
}
