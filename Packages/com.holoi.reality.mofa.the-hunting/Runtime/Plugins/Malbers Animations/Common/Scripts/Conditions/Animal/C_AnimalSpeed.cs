using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalSpeed : MAnimalCondition
    {
        public enum SpeedCondition { VerticalSpeed, CurrentSpeedSet, CurrentSpeedModifier,  ActiveIndex, Sprinting , CanSprint }

        public SpeedCondition Condition;
        [Hide("showCompare", false)]
        public ComparerInt compare = ComparerInt.Equal;

        [Hide("showValue",false)]
        public float Value = 0;
        
        [Hide("showName",false)]
        public string SpeedName;

        public override string DisplayName => "Animal/Speeds";

        public override bool _Evaluate()
        {
            if (Target)
            {
                switch (Condition)
                {
                    case SpeedCondition.VerticalSpeed:
                        return Target.VerticalSmooth.CompareFloat(Value, compare);
                    case SpeedCondition.CurrentSpeedSet:
                        return Target.CurrentSpeedSet.name == SpeedName;
                    case SpeedCondition.CurrentSpeedModifier:
                        return Target.CurrentSpeedModifier.name == SpeedName;
                    case SpeedCondition.ActiveIndex:
                        return Target.CurrentSpeedIndex == Value;
                    case SpeedCondition.Sprinting:
                        return Target.Sprint;
                    case SpeedCondition.CanSprint:
                        return Target.CanSprint;
                }

            }
            return false;
        }

        [HideInInspector, SerializeField] private bool showName,showValue,showCompare;

        protected override void OnValidate()
        {
            base.OnValidate();

            showName = Condition == SpeedCondition.CurrentSpeedModifier || Condition == SpeedCondition.CurrentSpeedSet;
            showValue = Condition == SpeedCondition.ActiveIndex || Condition == SpeedCondition.VerticalSpeed;
            showCompare = Condition ==   SpeedCondition.VerticalSpeed;
        }

        private void Reset()
        {
            Name = "New Animal Speed Condition";
            Target = this.FindComponent<MAnimal>();
        }
    }
}
