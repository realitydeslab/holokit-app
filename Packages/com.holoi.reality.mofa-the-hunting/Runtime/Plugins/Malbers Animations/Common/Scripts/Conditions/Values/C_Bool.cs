using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_Bool : MCondition
    {
        public override string DisplayName => "Values/Boolean";

        public BoolReference Target;
        public BoolReference Value;

        public void SetTarget(bool targ) => Target.Value = targ;
        public void SetValue(bool targ) => Value.Value = targ;

        public void SetTarget(BoolVar targ) => Target.Value = targ.Value;
        public void SetValue(BoolVar targ) => Value.Value = targ.Value;

        public override bool _Evaluate() => Target.Value == Value.Value;


        public override void SetTarget(Object target)
        {
            if (target is BoolVar) this.Target.Value = target as BoolVar;
        }

        private void Reset() => Name = "New Bool Comparer";
    }
}
