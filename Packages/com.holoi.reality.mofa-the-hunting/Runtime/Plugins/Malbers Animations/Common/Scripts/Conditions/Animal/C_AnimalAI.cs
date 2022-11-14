using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalAI : MCondition
    {
        public override string DisplayName => "Animal/Animal AI";

        [RequiredField] public MAnimalAIControl AI;
        public enum AnimalAICondition { enabled,  HasTarget, HasNextTarget, Arrived, Waiting ,InOffMesh, CurrentTarget, NextTarget }
        public AnimalAICondition Condition;
        [Hide("Condition",6,7)]
        public Transform Target;

        public override bool _Evaluate()
        {
            if (AI)
            {
                switch (Condition)
                {
                    case AnimalAICondition.enabled: return AI.enabled;
                    case AnimalAICondition.HasTarget: return AI.Target != null;
                    case AnimalAICondition.HasNextTarget: return AI.NextTarget != null;
                    case AnimalAICondition.Arrived: return AI.HasArrived;
                    case AnimalAICondition.InOffMesh: return AI.InOffMeshLink;
                    case AnimalAICondition.CurrentTarget: return AI.Target == Target;
                    case AnimalAICondition.Waiting: return AI.IsWaiting;
                    case AnimalAICondition.NextTarget: return AI.NextTarget == Target;
                }
            }
            return false;
        }


        private void Reset() => Name = "New Animal AI Condition";

        [HideInInspector, SerializeField] bool showTarg;
        protected override void OnValidate()
        {
            base.OnValidate();
            showTarg = Condition == AnimalAICondition.CurrentTarget || Condition == AnimalAICondition.NextTarget;
        }

        public override void SetTarget(Object target)
        {
            if (target is MAnimalAIControl) this.AI = target as MAnimalAIControl;
        }
    }
}
