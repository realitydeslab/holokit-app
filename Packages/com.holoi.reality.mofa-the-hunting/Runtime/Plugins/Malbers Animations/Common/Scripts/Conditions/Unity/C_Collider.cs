using UnityEngine;
 


namespace MalbersAnimations.Conditions
{
    public enum ColCondition {Enabled ,Equal, Trigger, PhysicMaterial, Box, Capsule, Sphere, MeshCollider, Layer}

    [System.Serializable]
    public class C_Collider : MCondition
    {
        public override string DisplayName => "Unity/Collider";

        [Tooltip("Target to check for the condition ")]
        [RequiredField] public Collider Target;
        [Tooltip("Conditions types")]
        public ColCondition Condition;
        [Tooltip("Transform Value to compare with")]
        [Hide("Condition", 1)]
        public Collider Value;
        [Hide("Condition", 3)]
        public PhysicMaterial Material;
        [Hide("Condition",  8)]
        public LayerMask Mask;
         

        public override bool _Evaluate()
        {
            if (Target != null)
            {
                switch (Condition)
                {
                    case ColCondition.Enabled: return Target.enabled;
                    case ColCondition.Equal: return Target == Value;
                    case ColCondition.Trigger: return Target.isTrigger;
                    case ColCondition.PhysicMaterial: return Target.sharedMaterial == Material;
                    case ColCondition.Box: return Target is BoxCollider;
                    case ColCondition.Capsule: return Target is CapsuleCollider;
                    case ColCondition.Sphere: return Target is SphereCollider;
                    case ColCondition.MeshCollider: return Target is MeshCollider;
                    case ColCondition.Layer: return Mask == (Mask | (1 << Target.gameObject.layer));
                         
                    default:
                        break;
                }
            }
            return false;
        }

        public override void SetTarget(Object target)
        {
            if (target is Collider) this.Target = target as Collider;
        }
         
        private void Reset() => Name = "New Collider Condition";
    }

}
