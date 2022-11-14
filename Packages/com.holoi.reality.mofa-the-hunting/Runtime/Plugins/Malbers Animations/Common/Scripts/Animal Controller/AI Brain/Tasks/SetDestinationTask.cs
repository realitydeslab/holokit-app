using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Set Destination")]
    public class SetDestinationTask : MTask
    {
        public override string DisplayName => "Movement/Set Destination";

        public enum DestinationType { Transform, GameObject, RuntimeGameObjects, Vector3 , Name }

        [Tooltip("Slow multiplier to set on the Destination")]
        public float SlowMultiplier = 0;
        [Space]
        public DestinationType targetType = DestinationType.Transform;

        [RequiredField] public TransformVar TargetT;
        [RequiredField] public Vector3Var Destination;
        [RequiredField] public GameObjectVar TargetG;
        [RequiredField] public RuntimeGameObjects TargetRG;
        
        public GetRuntimeGameObjects.RuntimeSetTypeGameObject rtype = GetRuntimeGameObjects.RuntimeSetTypeGameObject.Random;

        public IntReference RTIndex = new IntReference();
        public StringReference RTName = new StringReference();

        [Tooltip("When a new target is assinged it also sets that the Animal should move to that target")]
        public bool MoveToTarget = true;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            brain.AIControl.ClearTarget();

            brain.AIControl.CurrentSlowingDistance = brain.AIControl.StoppingDistance * SlowMultiplier;

            switch (targetType)
            {
                case DestinationType.Transform:
                    brain.AIControl.SetDestination(TargetT.Value.position, true);
                    break;
                case DestinationType.GameObject:
                    brain.AIControl.SetDestination(TargetG.Value.transform.position, true);
                    break;
                case DestinationType.RuntimeGameObjects:
                    switch (rtype)
                    {
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.First:
                            brain.AIControl.SetDestination(TargetRG.Item_GetFirst().transform.position, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Random:
                            brain.AIControl.SetDestination(TargetRG.Item_GetRandom().transform.position, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Index:
                            brain.AIControl.SetDestination(TargetRG.Item_Get(RTIndex).transform.position, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.ByName:
                            brain.AIControl.SetDestination(TargetRG.Item_Get(RTName).transform.position, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Closest:
                            brain.AIControl.SetDestination(TargetRG.Item_GetClosest(brain.Animal.gameObject).transform.position, true);
                            break;
                        default:
                            break;
                    }
                    break;
                case DestinationType.Vector3:
                    brain.AIControl.SetDestination(Destination.Value, true);
                    break;
                case DestinationType.Name:
                    var GO = GameObject.Find(RTName);
                    if (GO != null)
                    {
                        brain.AIControl.SetDestination(GO.transform.position, MoveToTarget);
                    }
                    else
                    {
                        Debug.Log("Using SetTarget.ByName() but there's no Gameobject with that name",this);
                    }
                    break;
                default:
                    break;
            }

            brain.TaskDone(index);
        }
        void Reset() { Description = "Set a new Destination to the AI Animal Control, it uses Run time sets Transforms or GameObjects"; }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SetDestinationTask))]
    public class SetDestinationTaskEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty Description, SlowMultiplier, MessageID, targetType, TargetT, TargetG, TargetRG, rtype, RTIndex, RTName, MoveToTarget, Destination;

        private void OnEnable()
        {
            Description = serializedObject.FindProperty("Description");
            SlowMultiplier = serializedObject.FindProperty("SlowMultiplier");
            MessageID = serializedObject.FindProperty("MessageID");
            targetType = serializedObject.FindProperty("targetType");
            Destination = serializedObject.FindProperty("Destination");
            TargetT = serializedObject.FindProperty("TargetT");
            TargetG = serializedObject.FindProperty("TargetG");
            TargetRG = serializedObject.FindProperty("TargetRG");
            rtype = serializedObject.FindProperty("rtype");
            RTIndex = serializedObject.FindProperty("RTIndex");
            RTName = serializedObject.FindProperty("RTName");
            MoveToTarget = serializedObject.FindProperty("MoveToTarget");

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(Description);
            UnityEditor.EditorGUILayout.PropertyField(MessageID);
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.HelpBox("All targets must be set at Runtime. Scriptable asset cannot have scenes References", UnityEditor.MessageType.Info);

            UnityEditor.EditorGUILayout.PropertyField(SlowMultiplier);
            UnityEditor.EditorGUILayout.PropertyField(targetType);

            var tt = (SetDestinationTask.DestinationType)targetType.intValue;

            switch (tt)
            {
                case SetDestinationTask.DestinationType.Transform:
                    UnityEditor.EditorGUILayout.PropertyField(TargetT, new GUIContent("Transform Hook"));
                    break;
                case SetDestinationTask.DestinationType.GameObject:
                    UnityEditor.EditorGUILayout.PropertyField(TargetG, new GUIContent("GameObject Hook"));
                    break;
                case SetDestinationTask.DestinationType.RuntimeGameObjects:
                    UnityEditor.EditorGUILayout.PropertyField(TargetRG, new GUIContent("Runtime Set"));
                    UnityEditor.EditorGUILayout.PropertyField(rtype, new GUIContent("Selection"));

                    var Sel = (GetRuntimeGameObjects.RuntimeSetTypeGameObject)rtype.intValue;
                    switch (Sel)
                    {
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Index:
                            UnityEditor.EditorGUILayout.PropertyField(RTIndex, new GUIContent("Element Index"));
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.ByName:
                            UnityEditor.EditorGUILayout.PropertyField(RTName, new GUIContent("Element Name"));
                            break;
                        default:
                            break;
                    }
                    break;
                case SetDestinationTask.DestinationType.Vector3:
                    UnityEditor.EditorGUILayout.PropertyField(Destination, new GUIContent("Global Vector3"));

                    break;
                case SetDestinationTask.DestinationType.Name:
                    UnityEditor.EditorGUILayout.PropertyField(RTName, new GUIContent("GameObject name"));

                    break;
                default:
                    break;
            }
            UnityEditor.EditorGUILayout.PropertyField(MoveToTarget);
            serializedObject.ApplyModifiedProperties();
        }
    }
    
#endif
}
