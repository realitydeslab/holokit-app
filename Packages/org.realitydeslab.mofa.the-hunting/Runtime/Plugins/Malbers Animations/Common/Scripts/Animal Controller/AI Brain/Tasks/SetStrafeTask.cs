using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Set Strafe")]
    public class SetStrafeTask : MTask
    {
        public override string DisplayName => "Animal/Set Strafe";
        void Reset() => Description = "Enable/Disable Strafing on the Animal Controller";


        [Space, Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        public BoolReference strafe = new BoolReference(true);

        public override void StartTask(MAnimalBrain brain, int index)
        {
            var animl = brain.Animal;
            if (affect == Affected.Target) animl = brain.TargetAnimal;

            if (animl) setStrafe(animl);

            brain.TaskDone(index); //Set Done to this task
        }

        public void setStrafe(MAnimal animal) => animal.Strafe = strafe.Value;
    }
}