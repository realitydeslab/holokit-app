using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;

namespace MalbersAnimations.Controller.AI
{

    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Quick Align")]
    public class QuickAlignTask : MTask
    {
        public enum AlignTo { TransformHook, GameObjectHook, CurrentTarget}

        public override string DisplayName => "General/Quick Align";

        public AlignTo alignTo = AlignTo.TransformHook;

        [FormerlySerializedAs("AlignTarget")]
        [Hide("alignTo", (int)AlignTo.TransformHook)]
       
        public TransformVar TransformHook;
        [Hide("alignTo", (int)AlignTo.GameObjectHook)]
       
        public GameObjectVar GameObjectHook;
        [Tooltip("Align time to rotate towards the Target")]
        public float alignTime = 0.3f;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            switch (alignTo)
            {
                case AlignTo.TransformHook:
                    if (TransformHook != null || TransformHook.Value == null)
                        brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, TransformHook.Value, alignTime));
                    else
                        Debug.LogWarning($"The Hook Target is empty or Null",this);
                    break;
                case AlignTo.GameObjectHook:
                    if (GameObjectHook != null || GameObjectHook.Value == null)
                        brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, GameObjectHook.Value.transform, alignTime));
                    else
                        Debug.LogWarning($"The Hook is empty or Null",this);
                    break;
                case AlignTo.CurrentTarget:
                    if (brain.Target)
                        brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, brain.Target, alignTime));
                    else
                        Debug.LogWarning($"The Hook is empty or Null", this);
                    break;
                default:
                    break;
            }

            brain.TaskDone(index);
        }

        void Reset() { Description = "Makes the Animal do a quick alignment towards an object"; }
    }
}