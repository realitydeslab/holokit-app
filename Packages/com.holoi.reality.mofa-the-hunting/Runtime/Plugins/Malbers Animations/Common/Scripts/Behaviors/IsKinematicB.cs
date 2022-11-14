using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    public class IsKinematicB : StateMachineBehaviour
    {
        public enum OnEnterOnExit { OnEnter, OnExit, OnEnterOnExit, OnTime }
        public OnEnterOnExit SetKinematic = OnEnterOnExit.OnEnterOnExit;

        [Tooltip("Changes the Kinematic property of the RigidBody On Enter/OnExit")]
        public bool isKinematic = true;

        [Tooltip("Time to change the the RB to kinematic or not")]
        [Range(0f, 1f)]
        public float Time = 0.5f;

        CollisionDetectionMode current;

        private bool sent = false;

        Rigidbody rb;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (rb == null) rb = animator.GetComponent<Rigidbody>();

            sent = false;

            if (SetKinematic == OnEnterOnExit.OnEnter)
            {
                Set_RB_Kinematic(isKinematic);
            }
            else if (SetKinematic == OnEnterOnExit.OnEnterOnExit)
            {
                Set_RB_Kinematic(true);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!sent && SetKinematic == OnEnterOnExit.OnTime && stateInfo.normalizedTime >= Time)
            {
                Set_RB_Kinematic(isKinematic);
                sent = true;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            switch (SetKinematic)
            {
                case OnEnterOnExit.OnExit:            Set_RB_Kinematic(isKinematic); break;
                case OnEnterOnExit.OnEnterOnExit:     Set_RB_Kinematic(false); break;
                case OnEnterOnExit.OnTime: if (!sent) Set_RB_Kinematic(isKinematic); break;
                default: break;
            } 
        }
        private void Set_RB_Kinematic(bool value)
        {
            if (value)
            {
                current = rb.collisionDetectionMode;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
                rb.collisionDetectionMode = current;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(IsKinematicB))] 
    public class IsKinematicBED : Editor
    {
        SerializedProperty SetKinematic, isKinematic, Time;
        void OnEnable()
        {

            SetKinematic = serializedObject.FindProperty("SetKinematic");
            isKinematic = serializedObject.FindProperty("isKinematic");
            Time = serializedObject.FindProperty("Time");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new GUILayout.HorizontalScope())
            {
                EditorGUIUtility.labelWidth = 50;
                EditorGUILayout.PropertyField(SetKinematic, new GUIContent("Set: "));

                if (SetKinematic.intValue != 2)
                {
                    EditorGUIUtility.labelWidth = 70;
                    EditorGUILayout.PropertyField(isKinematic, new GUIContent("Kinematic:"), GUILayout.Width(100));
                }
                EditorGUIUtility.labelWidth = 0;
            }
            
            if (SetKinematic.intValue == 3)
            {
                EditorGUILayout.PropertyField(Time);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}