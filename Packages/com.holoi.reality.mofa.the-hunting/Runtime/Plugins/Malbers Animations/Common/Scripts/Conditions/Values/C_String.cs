using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_String : MCondition
    {
        public enum stringCondition { Equal, Contains}

        public override string DisplayName => "Values/String";

        public StringReference Target;
        public stringCondition Condition;
        public StringReference Value;

        public void SetTarget(string targ) => Target.Value = targ;
        public void SetValue(string targ) => Value.Value = targ;

        public void SetTarget(StringVar targ) => Target.Value = targ.Value;
        public void SetValue(StringVar targ) => Value.Value = targ.Value;

        public override bool _Evaluate()
        {
            switch (Condition)
            {
                case stringCondition.Equal:  return Target.Value == Value.Value;
                case stringCondition.Contains: return Target.Value.Contains(Value.Value);
            }
            return false;
        }

        public override void SetTarget(Object target)
        {
            if (target is StringVar) this.Target.Value = target as StringVar;
        }

        private void Reset() => Name = "New String Comparer";
    }
}
