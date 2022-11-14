using MalbersAnimations.Controller;
using UnityEngine;


namespace MalbersAnimations.Conditions
{
    [System.Serializable] 
    public abstract class MAnimalCondition : MCondition 
    {
       [RequiredField] public MAnimal Target;

        public virtual void _SetAnimal(MAnimal n) => Target = n;

        public override void SetTarget(Object target)
        {
            if (target is MAnimal) this.Target = target as MAnimal;
        }

    } 
}
