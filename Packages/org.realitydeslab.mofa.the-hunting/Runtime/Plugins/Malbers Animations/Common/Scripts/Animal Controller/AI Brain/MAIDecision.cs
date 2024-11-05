using MalbersAnimations.Scriptables;
using System;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{
    public abstract class MAIDecision : BrainBase
    {
        /// <summary>What name will be displayed while adding a new Decision</summary>
        public abstract string DisplayName { get; }

        public enum WSend { None, SendTrue, SendFalse }

        [Space, Tooltip("ID Used for sending messages to the Brain to see if the Decision was TRUE or FALSE")]
        public IntReference DecisionID = new IntReference(0);
        [Tooltip("What to send to to the Brain.OnDecisionSucceeded() if a Decision is executed")]
        public WSend send = WSend.None;

        /// <summary>Execute the Decide method every x Seconds</summary>
        [Tooltip("Execute the Decide method every x Seconds to improve performance")]
        public FloatReference interval = new FloatReference(0.2f);

        [Tooltip("Check for decisions after all tasks are done")]
        public bool WaitForAllTasks;

        [Tooltip("Check for decisions after an specific task is done. (Using this Index) -1 will be ignored")]
        [Min(-1)] public int waitForTask = -1;

        /// <summary>Decides which State to take on the Transition based on the Return value</summary>
        /// <param name="brain">The Animal using the Decision</param>
        /// <param name="Index">Index of the Transition on the AI State</param>
        public abstract bool Decide(MAnimalBrain brain, int Index);


        /// <summary>Prepares a Decision the an AI State Starts (OPTIONAL)</summary>
        /// <param name="brain">The Animal using the Decision</param>
        /// <param name="Index">Index of the Transition on the AI State</param>
        public virtual void PrepareDecision(MAnimalBrain brain, int Index) { }

        public virtual void FinishDecision(MAnimalBrain brain, int Index) { }

        //   public virtual void RemoveListeners(MAnimalBrain brain, int Index) { }

        public virtual void OnAnimalEventDecisionListen(MAnimalBrain brain, MAnimal animal, int Index) { }
 
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MAIDecision))]
    public class MAIDecisionEditor : UnityEditor.Editor
    {
        protected UnityEditor.SerializedProperty Description, DecisionID, send ,  interval, WaitForTask, WaitForAllTasks;

        protected virtual void OnEnable()
        {
            Description = serializedObject.FindProperty("Description");
            DecisionID = serializedObject.FindProperty("DecisionID");
            send = serializedObject.FindProperty("send");
            interval = serializedObject.FindProperty("interval");
            WaitForTask = serializedObject.FindProperty("waitForTask");
            WaitForAllTasks = serializedObject.FindProperty("WaitForAllTasks"); 
        }

        public void DefaultParameters()
        {
            UnityEditor.EditorGUILayout.PropertyField(Description);
            UnityEditor.EditorGUILayout.PropertyField(send);
            UnityEditor.EditorGUILayout.PropertyField(DecisionID);
            UnityEditor.EditorGUILayout.PropertyField(interval);
            UnityEditor.EditorGUILayout.PropertyField(WaitForAllTasks);
            UnityEditor.EditorGUILayout.PropertyField(WaitForTask);
            UnityEditor.EditorGUILayout.Space();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DefaultParameters();

            DecisionParameters();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DecisionParameters()
        { }
    }
#endif
}
