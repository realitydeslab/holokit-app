using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Empty")]
    public class EmptyTask : MTask
    {
        public override string DisplayName => "General/Empty";

        public bool DoneOnStart = false;


        public override void StartTask(MAnimalBrain brain, int index)
        {
            if (DoneOnStart) brain.TaskDone(index);
        }

        private void Reset()
        => Description = "Use this task to Only Invoke [On Task Started] event and [OnTaskone] Events. Useful to invoke methods outside the brain.";

    }
}
