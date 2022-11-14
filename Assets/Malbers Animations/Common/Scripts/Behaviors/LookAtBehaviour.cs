using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations.Utilities
{
    /// <summary> Enable Disable the LookAt Logic by Priority </summary>
    public interface ILookAtActivation
    {
        void EnableByPriority(int layer);
        void DisableByPriority(int layer);
        void ResetByPriority(int layer);
    }
        public enum EnterExit  { OnEnter, OnExit, OnTime}
        public enum LookAtState { DoNothing, Enable, Disable , Reset}

    public class LookAtBehaviour : StateMachineBehaviour
    {

        [Range(0, 1)]
        public float Time = 0;

        [HideInInspector] public bool showTime;

        private ILookAtActivation lookat;
        public EnterExit when = EnterExit.OnEnter;
        public LookAtState OnEnter = LookAtState.Enable;

        private bool sent;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (lookat == null) lookat = animator.FindInterface<ILookAtActivation>();
            sent = false;
            if (when == EnterExit.OnEnter) CheckLookAt(animator, layerIndex);
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (when == EnterExit.OnTime && !sent)
            {
                var time = stateInfo.normalizedTime;

                if (time >= Time)
                {
                    CheckLookAt(animator, layerIndex);
                    sent = true;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (when == EnterExit.OnExit) CheckLookAt(animator, layerIndex);
        }


        private void CheckLookAt(Animator animator, int layerIndex)
        {
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash) return;

            if (lookat != null)
            {
                switch (OnEnter)
                {
                    case LookAtState.Reset: lookat.ResetByPriority(layerIndex + 1); break;
                    case LookAtState.Enable: lookat.EnableByPriority(layerIndex + 1); break;
                    case LookAtState.Disable: lookat.DisableByPriority(layerIndex + 1); break;
                    default: break;
                }
            }
        }
         
    }

   


#if UNITY_EDITOR
    [CustomEditor(typeof(LookAtBehaviour))]
    public class LookAtBehaviourED : Editor
    {
        SerializedProperty OnEnter, stateInfo, Time;
        void OnEnable()
        {
            OnEnter = serializedObject.FindProperty("OnEnter");
            stateInfo = serializedObject.FindProperty("when");
            Time = serializedObject.FindProperty("Time");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Enable/Disable the Look At logic by layer priority");
            EditorGUILayout.PropertyField(stateInfo);
            EditorGUILayout.PropertyField(OnEnter, new GUIContent("Status"));

            if (stateInfo.intValue == (int)EnterExit.OnTime)
            EditorGUILayout.PropertyField(Time);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}