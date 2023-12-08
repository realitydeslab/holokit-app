using MalbersAnimations.Controller;

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_AnimalState : MAnimalCondition
    {
        public override string DisplayName => "Animal/States";


        public enum StateCondition { ActiveState, Enabled, HasState , Pending, SleepFromMode, SleepFromState, SleepFromStance }
        public StateCondition Condition = StateCondition.ActiveState;
        public StateID Value;

        public void SetValue(StateID v) => Value = v;

        private State st;

        private void OnEnable()
        {
            if (Target) st = Target.State_Get(Value);
        }

        public override bool _Evaluate()
        {
            if (Target)
            {
                switch (Condition)
                {
                    case StateCondition.ActiveState: return Target.ActiveStateID == Value;     //Check if the Active state is the one with this ID
                    case StateCondition.HasState: return st != null;                            //Check if the State exist on the Current Animal
                    case StateCondition.Enabled: return st.Active;
                    case StateCondition.Pending: return st.IsPending;
                    case StateCondition.SleepFromMode: return st.IsSleepFromMode;
                    case StateCondition.SleepFromState: return st.IsSleepFromState;
                    case StateCondition.SleepFromStance: return st.IsSleepFromStance;
                }
            }
            return false;
        }
        private void Reset()
        {
            Name = "New Animal State Condition";

            Target = this.FindComponent<MAnimal>();
        }
    }
}
