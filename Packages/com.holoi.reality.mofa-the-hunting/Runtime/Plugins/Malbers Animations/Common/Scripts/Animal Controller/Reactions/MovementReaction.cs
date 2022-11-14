using MalbersAnimations.Scriptables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MalbersAnimations.Controller.Reactions
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Malbers Animations/Animal Reactions/Movement Reaction"/*, order = 4*/)]
    public class MovementReaction : MReaction
    {
        public Move_Reaction type = Move_Reaction.Sleep;
        public bool Value;
        //public Vector3Reference Direction;
        public TransformReference Destination;

        protected override void _React(MAnimal animal)
        {
            switch (type)
            {
                case Move_Reaction.UseCameraInput:
                    animal.UseCameraInput = Value;
                    break;
                case Move_Reaction.Sleep:
                    animal.Sleep = Value;
                    break;
                case Move_Reaction.LockInput:
                    animal.LockInput = Value;
                    break;
                case Move_Reaction.LockMovement:
                    animal.LockMovement = Value;
                    break;
                case Move_Reaction.AlwaysForward:
                    animal.AlwaysForward = Value;
                    break;
                case Move_Reaction.UseCameraUp:
                    animal.UseCameraUp = Value;
                    break;
                case Move_Reaction.LockForward:
                    animal.LockForwardMovement = Value;
                    break;
                case Move_Reaction.LockHorizontal:
                    animal.LockHorizontalMovement = Value;
                    break;
                case Move_Reaction.LockUpDown:
                    animal.LockUpDownMovement = Value;
                    break;
                case Move_Reaction.Teleport:
                    animal.Teleport(Destination);
                    break;
                case Move_Reaction.TeleportWithRotation:
                    animal.TeleportRot(Destination);
                    break;
            }
        }

        protected override bool _TryReact(MAnimal animal)
        {
            _React(animal);
            return true;
        }

        public enum Move_Reaction
        {
            UseCameraInput,
            Sleep,
            LockInput,
            LockMovement,
            AlwaysForward,
            UseCameraUp,
            LockForward,
            LockHorizontal,
            LockUpDown,
            Teleport,
            TeleportWithRotation
        } 


        /// 
        /// VALIDATIONS
        /// 


        private void OnEnable() { Validation(); }
        private void OnValidate() { Validation(); }

        private const string reactionName = "Move → ";

        void Validation()
        {
            fullName = reactionName + type.ToString() + " [" + Value + "]";
            switch (type)
            {
                case Move_Reaction.UseCameraInput:
                    description = "Enable/Disable the Camera Input movement type";
                    break;
                case Move_Reaction.Sleep:
                    description = "Sets the Animal to Sleep. The Controller will be disable internally, all inputs and movement will ignored";
                    break;
                case Move_Reaction.LockInput:
                    description = "Locks Input on the Animal, Ignore inputs like Jumps, Attacks , Actions etc";
                    break;
                case Move_Reaction.LockMovement:
                    description = "Locks the Movement on the Animal";
                    break;
                case Move_Reaction.AlwaysForward:
                    description = "The animal will always go forward. useful for flying";
                    break;
                case Move_Reaction.UseCameraUp:
                    description = "Use the Camera Up Vector to Move while flying or Swiming UnderWater";
                    break;
                case Move_Reaction.LockHorizontal:
                    description = "Sets to Zero the X value on the Movement Axis when this is set to true";
                    break;
                case Move_Reaction.LockUpDown:
                    description = "Sets to Zero the Y value on the Movement Axis when this is set to true";
                    break;
                case Move_Reaction.LockForward:
                    description = "Sets to Zero the Z value on the Movement Axis when this is set to true";
                    break;
                case Move_Reaction.Teleport:
                    description = "Teleports the Animal to a new Transform Location. Rotation is skipped";
                    break;
                case Move_Reaction.TeleportWithRotation:
                    description = "Teleports the Animal to a new Transform Location. The animal inherit the transform Rotation";
                    break;
                //case Move_Reaction.Move:
                //    description = "Moves the Animal using a Direction. Using the Method";
                //    fullName = reactionName + type.ToString() + " [" + Direction.Value + "]";
                //    break;
                default:
                    break;
            }
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MovementReaction))]
    public class MovementReactionEditor : Editor
    {
        SerializedProperty type, Value, Destination;

        private void OnEnable()
        {
            type = serializedObject.FindProperty("type");
            Value = serializedObject.FindProperty("Value");
            Destination = serializedObject.FindProperty("Destination");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(type);
            
            switch ((MovementReaction.Move_Reaction)type.intValue)
            {  
                case MovementReaction.Move_Reaction.Teleport:
                    EditorGUILayout.PropertyField(Destination);
                    break;
                case MovementReaction.Move_Reaction.TeleportWithRotation:
                    EditorGUILayout.PropertyField(Destination);
                    break;
                default:
                    EditorGUILayout.PropertyField(Value);
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
