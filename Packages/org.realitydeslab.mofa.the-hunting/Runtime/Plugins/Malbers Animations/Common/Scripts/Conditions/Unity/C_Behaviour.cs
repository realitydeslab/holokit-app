using UnityEngine;
  

namespace MalbersAnimations.Conditions
{
    public enum ComponentCondition {Enabled, ActiveAndEnabled }

    [System.Serializable]
    public class C_Behaviour : MCondition
    {
        public override string DisplayName => "Unity/Behavior";

        [Tooltip("Target to check for the condition ")]
        [RequiredField] public Behaviour Target;
        [Tooltip("Conditions types")]
        public ComponentCondition Condition;
        

        public override bool _Evaluate()
        {
            if (Target != null)
            {
                switch (Condition)
                {
                    case ComponentCondition.Enabled: return Target.enabled;
                    case ComponentCondition.ActiveAndEnabled: return Target.isActiveAndEnabled;
                }
            }
            return false;
        }

        public override void SetTarget(Object target)
        {
            if (target is Behaviour) this.Target = target as Behaviour;
        }


        private void Reset() => Name = "New Behaviour Condition";
    }

}
