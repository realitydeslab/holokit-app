using MalbersAnimations.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalStrafe : MAnimalCondition
    {
        public override string DisplayName => "Animal/Strafe";

        public enum StrafeCondition { Strafing, CanSrafe}
        public StrafeCondition Condition;
        public StanceID Value;

        public void _SetStanceID(StanceID v) => Value = v;


        public override bool _Evaluate()
        {
            if (Target)
            {
                switch (Condition)
                {
                    case StrafeCondition.Strafing: return Target.Strafe;
                    case StrafeCondition.CanSrafe: return Target.CanStrafe && Target.ActiveState.CanStrafe;
                }
            }
            return false;
        }


        private void Reset() => Name = "Can the Animal Strafe?";

    }
}
