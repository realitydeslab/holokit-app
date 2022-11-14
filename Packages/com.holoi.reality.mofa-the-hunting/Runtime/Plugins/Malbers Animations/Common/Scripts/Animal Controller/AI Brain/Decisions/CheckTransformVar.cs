using MalbersAnimations.Scriptables;
using UnityEngine;


namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Check Transform Var", order = 6)]
    public class CheckTransformVar : MAIDecision
    {
        public override string DisplayName => "Variables/Check Transform Var";

        public TransformVar Var;
        public CompareTransformVar compare;
        [Hide("compare",2 )]
        public RuntimeGameObjects Set;

        public override bool Decide(MAnimalBrain brain, int Index)
        {
            if (Var)
            {
                switch (compare)
                {
                    case CompareTransformVar.IsNull:
                        return Var.Value == null;
                    case CompareTransformVar.IsCurrentTarget:
                        return Var.Value == brain.Target;
                    case CompareTransformVar.IsInRuntimeSet:
                        if (Set)
                            return Set.items.Contains(Var.Value.gameObject);
                        else
                            return false;
                    default:
                        break;
                }
            }
            return false;
        }

        [SerializeField, HideInInspector] private bool showSet;
        private void OnValidate()
        {
            showSet = compare == CompareTransformVar.IsInRuntimeSet;

        }
        public enum CompareTransformVar { IsNull, IsCurrentTarget,  IsInRuntimeSet }
    }
}
