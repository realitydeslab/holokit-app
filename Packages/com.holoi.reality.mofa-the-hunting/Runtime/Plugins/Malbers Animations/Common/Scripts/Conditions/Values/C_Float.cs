using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_Float : MCondition
    {
        public override string DisplayName => "Values/Float";

        public FloatReference Target;
        public ComparerInt Condition;
        public FloatReference Value;

        public void SetTarget(float targ) => Target.Value = targ;
        public void SetValue(float targ) => Value.Value = targ;

        public void SetTarget(FloatVar targ) => Target.Value = targ.Value;
        public void SetValue(FloatVar targ) => Value.Value = targ.Value;

        public override bool _Evaluate() => Target.Value.CompareFloat(Value.Value, Condition);

        public override void SetTarget(Object target)
        {
            if (target is FloatVar) this.Target.Value = target as FloatVar;
        }

        private void Reset() => Name = "New Float Comparer";
    }
}
