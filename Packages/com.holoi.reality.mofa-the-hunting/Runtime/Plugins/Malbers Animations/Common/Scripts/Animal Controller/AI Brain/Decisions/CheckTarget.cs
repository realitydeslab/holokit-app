using UnityEngine;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Check Target",order = -100)]
    public class CheckTarget : MAIDecision
    {
        public override string DisplayName => "Movement/Check Target";
        [Space,Tooltip("(OPTIONAL)Use it if you want to know if we have arrived to a specific Target")]
        public string TargetName = string.Empty;

        public CompareTarget compare = CompareTarget.IsNull;

        public RuntimeGameObjects set;
        public TransformVar transform;
        public string m_name;


        public override bool Decide(MAnimalBrain brain, int index)
        {
            switch (compare)
            {
                case CompareTarget.IsNull:
                    return brain.Target == null;
                case CompareTarget.isTransformVar:
                    return transform.Value != null ? brain.Target == transform.Value : false;
                case CompareTarget.IsInRuntimeSet:
                    return set != null ? set.Items.Contains(brain.Target.gameObject) : false;
                case CompareTarget.HasName:
                    return string.IsNullOrEmpty(m_name) ? brain.Target.name.Contains(m_name) : false;
                default:
                    break;
            }
            return false;
        }


        [SerializeField, HideInInspector] private bool showSet, showname,showTrans;
        private void OnValidate()
        {
            showSet = compare == CompareTarget.IsInRuntimeSet;
            showname = compare == CompareTarget.HasName;
            showTrans = compare == CompareTarget.isTransformVar;
        }



        public enum CompareTarget {IsNull, isTransformVar, IsInRuntimeSet, HasName,  }
    }
}