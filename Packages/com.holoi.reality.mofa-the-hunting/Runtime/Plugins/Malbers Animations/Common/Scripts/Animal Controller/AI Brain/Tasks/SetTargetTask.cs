using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Set Target")]
    public class SetTargetTask : MTask
    {
        public override string DisplayName => "Movement/Set Target";


        public enum TargetToFollow { Transform, GameObject, RuntimeGameObjects, ClearTarget, Name }

        [Space]
        public TargetToFollow targetType = TargetToFollow.Transform;

        [RequiredField] public TransformVar TargetT;
        [RequiredField] public GameObjectVar TargetG;
        [RequiredField] public RuntimeGameObjects TargetRG;
        public GetRuntimeGameObjects.RuntimeSetTypeGameObject rtype = GetRuntimeGameObjects.RuntimeSetTypeGameObject.Random;

        public IntReference RTIndex = new IntReference();
        public StringReference RTName = new StringReference();

        [Tooltip("When a new target is assinged it also sets that the Animal should move to that target")]
        public bool MoveToTarget = true;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            if (MoveToTarget) brain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved

            switch (targetType)
            {
                case TargetToFollow.Transform:
                    brain.AIControl.SetTarget(TargetT.Value, MoveToTarget);
                    break;
                case TargetToFollow.GameObject:
                    brain.AIControl.SetTarget(TargetG.Value.transform, MoveToTarget);
                    break;
                case TargetToFollow.RuntimeGameObjects:
                    switch (rtype)
                    {
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.First:
                            brain.AIControl.SetTarget(TargetRG.Item_GetFirst().transform, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Random:
                            brain.AIControl.SetTarget(TargetRG.Item_GetRandom().transform, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Index:
                            brain.AIControl.SetTarget(TargetRG.Item_Get(RTIndex).transform, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.ByName:
                            brain.AIControl.SetTarget(TargetRG.Item_Get(RTName).transform, true);
                            break;
                        case GetRuntimeGameObjects.RuntimeSetTypeGameObject.Closest:
                            brain.AIControl.SetTarget(TargetRG.Item_GetClosest(brain.Animal.gameObject).transform, true);
                            break;
                        default:
                            break;
                    }
                    break;
                case TargetToFollow.ClearTarget:
                    brain.AIControl.ClearTarget();
                    break;
                case TargetToFollow.Name:
                    var GO = GameObject.Find(RTName);
                    if (GO != null)
                    {
                        brain.AIControl.SetTarget(GO.transform, MoveToTarget);
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
        void Reset() { Description = "Set a new Target to the AI Animal Control, it uses Run time sets Transforms or GameObjects"; }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SetTargetTask))]
    public class SetTargetTaskEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty Description, MessageID, targetType, TargetT, TargetG, TargetRG, rtype, RTIndex, RTName, MoveToTarget;

        private void OnEnable()
        {
            Description = serializedObject.FindProperty("Description");
            MessageID = serializedObject.FindProperty("MessageID");
            targetType = serializedObject.FindProperty("targetType");
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

            UnityEditor.EditorGUILayout.PropertyField(targetType);

            var tt = (SetTargetTask.TargetToFollow)targetType.intValue;

            switch (tt)
            {
                case SetTargetTask.TargetToFollow.Transform:
                    UnityEditor.EditorGUILayout.PropertyField(TargetT, new GUIContent("Transform Hook"));
                    break;
                case SetTargetTask.TargetToFollow.GameObject:
                    UnityEditor.EditorGUILayout.PropertyField(TargetG, new GUIContent("GameObject Hook"));
                    break;
                case SetTargetTask.TargetToFollow.RuntimeGameObjects:
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
                case SetTargetTask.TargetToFollow.ClearTarget:
                    break;
                case SetTargetTask.TargetToFollow.Name:
                    UnityEditor.EditorGUILayout.PropertyField(RTName, new GUIContent("GameObject name"));

                    break;
                default:
                    break;
            }

            if (tt != SetTargetTask.TargetToFollow.ClearTarget)  
                UnityEditor.EditorGUILayout.PropertyField(MoveToTarget);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
