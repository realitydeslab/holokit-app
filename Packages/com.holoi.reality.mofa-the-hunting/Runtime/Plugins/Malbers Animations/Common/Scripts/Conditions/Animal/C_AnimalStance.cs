using MalbersAnimations.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalStance : MAnimalCondition
    {
        public override string DisplayName => "Animal/Stances";

        public enum StanceCondition { CurrentStance, DefaultStance}
        public StanceCondition Condition;
        public StanceID Value;

        public void SetValue(StanceID v) => Value = v;


        public override bool _Evaluate()
        {
            if (Target)
            {
                switch (Condition)
                {
                    case StanceCondition.CurrentStance:
                        return Target.Stance == Value;
                    case StanceCondition.DefaultStance:
                        return Target.DefaultStanceID == Value;
                }
            }
            return false;
        }

        //private void Reset() => Name = "New Animal Stance Condition";

        //[HideInInspector, SerializeField] private bool showName, showValue, showCompare;
        //protected override void OnValidate()
        //{
        //    base.OnValidate();

        //    showName = Condition == ModeCondition.PlayingAbility || Condition == ModeCondition.HasAbility;
        //}

    }
}
