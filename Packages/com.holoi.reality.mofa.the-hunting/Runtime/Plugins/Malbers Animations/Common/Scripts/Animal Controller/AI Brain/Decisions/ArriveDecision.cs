using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Arrived to Target",order = -100)]
    public class ArriveDecision : MAIDecision
    {
        public override string DisplayName => "Movement/Has Arrived";
        [Space,Tooltip("(OPTIONAL)Use it if you want to know if we have arrived to a specific Target")]
        public string TargetName = string.Empty;

        public override bool Decide(MAnimalBrain brain, int index)
        {
            bool Result;

            if (string.IsNullOrEmpty(TargetName))
            {
                Result =
                    brain.AIControl.HasArrived;
            }
            else
            {
                Result =
                    brain.AIControl.HasArrived &&
                    (brain.Target.name == TargetName || brain.Target.root.name == TargetName); //If we are looking for an specific Target
            }

            return Result;
        }
    }
}