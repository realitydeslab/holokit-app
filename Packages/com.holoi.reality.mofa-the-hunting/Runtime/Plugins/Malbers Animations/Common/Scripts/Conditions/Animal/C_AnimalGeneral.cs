using MalbersAnimations.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalGeneral : MAnimalCondition
    {
        public override string DisplayName => "Animal/General";

        public enum AnimalCondition { Grounded, RootMotion,FreeMovement,AlwaysForward, Sleep, AdditivePosition, AdditiveRotation, }
        public AnimalCondition Condition;

        public override bool _Evaluate()
        {
            if (Target)
            {
                switch (Condition)
                {
                    case AnimalCondition.Grounded: return Target.Grounded;
                    case AnimalCondition.RootMotion: return Target.RootMotion;
                    case AnimalCondition.FreeMovement: return Target.FreeMovement;
                    case AnimalCondition.AlwaysForward: return Target.AlwaysForward;
                    case AnimalCondition.Sleep: return Target.Sleep;
                    case AnimalCondition.AdditivePosition: return Target.UseAdditivePos;
                    case AnimalCondition.AdditiveRotation: return Target.UseAdditiveRot;
                }
            }
            return false;
        }


        private void Reset() => Name = "New Animal Condition";

    }
}
