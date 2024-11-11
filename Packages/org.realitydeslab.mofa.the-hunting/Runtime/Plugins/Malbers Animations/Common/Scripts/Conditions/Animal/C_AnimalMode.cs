using MalbersAnimations.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalMode : MAnimalCondition
    {
        public override string DisplayName => "Animal/Modes";
        public enum ModeCondition { PlayingMode, PlayingAbility, HasMode, HasAbility, Enabled }
        public ModeCondition Condition;
        public ModeID Value;
        [Hide("Condition",1,3)]
        public string AbilityName;

        private Mode mode;

        private void OnEnable()
        {
           if (Target) mode = Target.Mode_Get(Value);
        }


        public void SetValue(ModeID v) => Value = v;


        public override bool _Evaluate()
        {
            if (Target != null && mode != null)
            {
                switch (Condition)
                {
                    case ModeCondition.PlayingMode:
                        return Target.IsPlayingMode && (Value == null || Target.ActiveMode.ID == Value);
                    case ModeCondition.PlayingAbility:
                        return Target.IsPlayingMode && (string.IsNullOrEmpty(AbilityName) || Target.ActiveMode.ActiveAbility.Name == AbilityName);
                    case ModeCondition.HasMode:
                        return mode != null;
                    case ModeCondition.HasAbility:
                        return mode != null && mode.Abilities.Exists(x => x.Name == AbilityName);
                    case ModeCondition.Enabled:
                        return mode != null && mode.Active;
                }
            }
            return false;
        }

        private void Reset() => Name = "New Animal Mode Condition";

       

    }
}
