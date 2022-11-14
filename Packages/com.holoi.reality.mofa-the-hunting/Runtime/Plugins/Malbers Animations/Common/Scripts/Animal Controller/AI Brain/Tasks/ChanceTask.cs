using UnityEngine;
namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Chance Task")]
    public class ChanceTask : MTask
    {
        public override string DisplayName => "General/Chance";
        private static readonly int ChanceKey = "ChanceTask".GetHashCode();

        [Range(0,1), Tooltip("Chance this Task can execute another Task when the AI State start")]
        public float Chance = 1;

        [Tooltip("Task to execute if the chance succeded")]
      //  [CreateScriptableAsset]
        public MTask Task;



        public override void StartTask(MAnimalBrain brain, int index) 
        {
            var RandomChance = UnityEngine.Random.Range(0f, 1f);

            var canExecute = Chance >= RandomChance;

            brain.TasksVars[index].boolValue = canExecute; //Store the Result in the Task Vars Value on the Brain
           // brain.TasksVars[index].AddVar(ChanceKey, canExecute); //Store the Result in the Task Vars Value on the Brain


            if (brain.debug)
                Debug.Log($"Probability to execute <B>[{Task.name}]</B>. Value:<B>[{RandomChance:F2}]</B> >= Limit:<B>[{Chance:F2}] ?</B>. Result: [<B>{canExecute}]</B>");

            if (canExecute)
            {
                Task.StartTask(brain, index);
            }
            else
            {
                brain.TaskDone(index);
            }
        }

        public override void UpdateTask(MAnimalBrain brain, int index)
        {
           if (brain.TasksVars[index].boolValue)
           //if (brain.TasksVars[index].GetBool(ChanceKey))
            {
                Task.UpdateTask(brain, index);
            }
        }

        public override void ExitAIState(MAnimalBrain brain, int index)
        {
            if (brain.TasksVars[index].boolValue)
            //if (brain.TasksVars[index].GetBool(ChanceKey))
            {
                Task.ExitAIState(brain, index);
            }
        }

        void Reset()  { Description = "Gives a Percent Chance to execute another task"; }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ChanceTask))]
    public class ChanceTaskEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty Description, MessageID, Task, Chance, WaitForPreviousTask;

        private void OnEnable()
        {
            WaitForPreviousTask = serializedObject.FindProperty("WaitForPreviousTask");
            Description = serializedObject.FindProperty("Description");
            MessageID = serializedObject.FindProperty("MessageID");
            Chance = serializedObject.FindProperty("Chance");
            Task = serializedObject.FindProperty("Task");

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(Description);
            UnityEditor.EditorGUILayout.PropertyField(MessageID);
            UnityEditor.EditorGUILayout.PropertyField(WaitForPreviousTask);
            UnityEditor.EditorGUILayout.Space();

            UnityEditor.EditorGUILayout.PropertyField(Chance);
            // UnityEditor.EditorGUILayout.PropertyField(Task);

            UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            UnityEditor.EditorGUI.indentLevel++;
            MTools.DrawScriptableObject(Task, true ,false, "Task");
            UnityEditor.EditorGUI.indentLevel--;
            UnityEditor.EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
