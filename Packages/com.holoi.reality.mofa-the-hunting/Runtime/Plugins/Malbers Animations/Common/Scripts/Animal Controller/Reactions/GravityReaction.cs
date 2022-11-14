using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller.Reactions
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Malbers Animations/Animal Reactions/Gravity Reaction"/*, order = 10*/)]
    public class GravityReaction : MReaction
    {
        public Gravity_Reaction type = Gravity_Reaction.Enable;
        [Hide("type", (int)Gravity_Reaction.Enable, (int)Gravity_Reaction.GroundChangesGravity)]
        public bool Value;

        protected override void _React(MAnimal animal)
        {
            switch (type)
            {
                case Gravity_Reaction.Enable:
                    animal.UseGravity = Value;
                    break;
                case Gravity_Reaction.Reset:
                    animal.ResetGravityDirection();
                    break;
                case Gravity_Reaction.GroundChangesGravity:
                    animal.GroundChangesGravity(Value);
                    break;
                case Gravity_Reaction.SnapAlignment:
                    animal.AlignToGravity();
                    break;
                default:
                    break;
            }
        }

        protected override bool _TryReact(MAnimal animal)
        {
            _React(animal);
            return true;
        }

        public enum Gravity_Reaction
        {
            Enable,
            Reset,
            GroundChangesGravity,
            SnapAlignment,
        }



        /// 
        /// VALIDATIONS
        /// 
        private void OnEnable() { Validation(); }
        private void OnValidate() { Validation(); }

        private const string reactionName = "Gravity → ";

        void Validation()
        {
            fullName = reactionName + type.ToString(); 
            switch (type)
            {
                case Gravity_Reaction.Enable:
                    description = (Value ? "Enable" : "Disable") + " the gravity on the Animal";
                    fullName += " [" + Value + "]";
                    break;
                case Gravity_Reaction.Reset:
                    description = "Resets the gravity on the Animal to Vector3.Down";
                    break;
                case Gravity_Reaction.GroundChangesGravity:
                    description = "The Gravity Direction is set by the Ground Normal";
                    fullName += " [" + Value + "]";
                    break;
                case Gravity_Reaction.SnapAlignment:
                    description = "Align the Animal to the Gravity direction with no smoothness";
                    break;
                default:
                    break;
            }
        }
    }
}
