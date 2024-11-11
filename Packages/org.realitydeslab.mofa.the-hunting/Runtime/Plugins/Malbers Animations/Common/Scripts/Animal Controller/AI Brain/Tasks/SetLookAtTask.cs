using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;
namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Look At-Aim" , fileName = "new Aim Task")]
    public class SetLookAtTask : MTask
    {
        public override string DisplayName => "General/Set Look At-Aim";
        public enum LookAtOption1 { CurrentTarget, TransformVar}
        public enum LookAtOption2 { AIAnimal, TransformVar}

        [Tooltip("Check the Look At Component on the Target or on Self")]
        [UnityEngine.Serialization.FormerlySerializedAs("SetLookAtOn")]
        public Affected SetAimOn = Affected.Self;

        [Hide("SetAimOn", (int)Affected.Self)]
        public LookAtOption1 LookAtTargetS = LookAtOption1.CurrentTarget;
        [Hide("SetAimOn", (int)Affected.Target)]
        public LookAtOption2 LookAtTargetT = LookAtOption2.AIAnimal;
        [Hide("showTransformVar")]
        public TransformVar TargetVar;

        [Tooltip("If true .. it will Look for a gameObject on the Target with the Tag[tag].... else it will look for the gameObject name")]
        public bool UseTag = false; 

        [Hide("UseTag",true), Tooltip("Search for the Target Child gameObject name")]
        public string BoneName = "Head";
        [Hide("UseTag"), Tooltip("Look for a child gameObject on the Target with the Tag[tag]")]
        public Tag tag;
        [Tooltip("When the Task ends it will Remove the Target on the Aim Component")]
        public bool DisableOnExit = true;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            Transform child = null;

            if (SetAimOn == Affected.Self)
            {
                switch (LookAtTargetS)
                {
                    case LookAtOption1.CurrentTarget:
                        child = UseTag ? GetGameObjectByTag(brain.Target) : GetChildByName(brain.Target);
                        break;
                    case LookAtOption1.TransformVar:
                        child = UseTag ? GetGameObjectByTag(TargetVar.Value) : GetChildByName(TargetVar.Value);
                        break;
                    default:
                        break;
                }

                brain.Animal.FindInterface<IAim>()?.SetTarget(child);
            }
            else
            {
                if (LookAtTargetT == LookAtOption2.AIAnimal)
                {
                    child = UseTag ? GetGameObjectByTag(brain.Animal.transform) : GetChildByName(brain.Animal.transform);
                }
                else
                {
                    child = UseTag ? GetGameObjectByTag(TargetVar.Value) : GetChildByName(TargetVar.Value);
                }

                if (brain.Target) 
                    brain.Target.FindInterface<IAim>()?.SetTarget(child);
            }

            brain.TaskDone(index);
        }


        private Transform GetChildByName(Transform Target)
        {
            if (Target && !string.IsNullOrEmpty(BoneName))
            {
               var child = Target.FindGrandChild(BoneName);
               if (child != null)  return child;
            }
            return Target;
        }

        private Transform GetGameObjectByTag(Transform Target)
        {
            if (Target )
            {
                var allTags = Target.root.GetComponentsInChildren<Tags>();

                if (allTags == null) return null;

                foreach (var item in allTags)
                {
                    if (item.HasTag(tag))
                        return item.transform;
                }
            }
            return null;
        }

        public override void ExitAIState(MAnimalBrain brain, int index)
        {
            if (DisableOnExit)
            {
                brain.Animal.FindInterface<IAim>()?.SetTarget(null);
                if (brain.Target) brain.Target.FindInterface<IAim>()?.SetTarget(null);
            }
        }



        [HideInInspector] public bool showTransformVar = false;
        private void OnValidate()
        {
            showTransformVar =
                (LookAtTargetS == LookAtOption1.TransformVar && SetAimOn == Affected.Self) ||
                (LookAtTargetT == LookAtOption2.TransformVar && SetAimOn == Affected.Target);
        }


        private void Reset() { Description = "Find a child gameObject with the name given on the Target and set it as the Target for the Look At and the Aim Component on the Animal "; }
    }
}
