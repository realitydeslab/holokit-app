using MalbersAnimations.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Scriptables
{
    [AddComponentMenu("Malbers/Variables/Transform Comparer")]
    public class TransformComparer : MonoBehaviour
    {
        public enum TransformCondition { Null, Equal, ChildOf, ParentOf, Name }


        public TransformReference value;
        public TransformCondition Condition;
        public TransformReference compareTo;
        public StringReference T_Name;

        [Tooltip("Invokes the current value on Enable")]
        public bool InvokeOnEnable = true;

        public UnityEvent Then = new UnityEvent();
        public UnityEvent Else = new UnityEvent();


        void OnEnable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged += Invoke;
            if (compareTo.Variable != null) compareTo.Variable.OnValueChanged += Invoke;

            if (InvokeOnEnable) Invoke(value.Value);
        }

        void OnDisable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged -= Invoke;
            if (compareTo.Variable != null) compareTo.Variable.OnValueChanged -= Invoke;

          //  Invoke(value.Value);
        }

        /// <summary> Used to use turn Objects to True or false </summary>
        public virtual void Invoke(Transform _)
        {
            switch (Condition)
            {
                case TransformCondition.Null:
                    Response(value.Value == null);
                    break;
                case TransformCondition.Equal:
                    Response(value.Value == compareTo.Value);
                    break;
                case TransformCondition.ChildOf:
                    if (value.Value) Response(value.Value.IsChildOf(compareTo.Value));
                    break;
                case TransformCondition.ParentOf:
                    if (compareTo.Value) Response(compareTo.Value.IsChildOf(value));
                    break;
                case TransformCondition.Name:
                    if (value.Value) Response(value.Value.name.Contains(T_Name));
                    break;
                default:
                    break;
            }
        }


        private void Response(bool value)
        {
            if (value) Then.Invoke(); else Else.Invoke();
        }
    }

#if UNITY_EDITOR 
    //INSPECTOR
    [UnityEditor.CustomEditor(typeof(TransformComparer)), UnityEditor.CanEditMultipleObjects]
    public class TransformComparerEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty value, Then, Else, Condition, compareTo, T_Name, InvokeOnEnable;
        TransformComparer M;

        void OnEnable()
        {
            M = target as TransformComparer;
            value = serializedObject.FindProperty("value");
            Then = serializedObject.FindProperty("Then");
            Else = serializedObject.FindProperty("Else");
            Condition = serializedObject.FindProperty("Condition");
            InvokeOnEnable = serializedObject.FindProperty("InvokeOnEnable");
            compareTo = serializedObject.FindProperty("compareTo");
            T_Name = serializedObject.FindProperty("T_Name");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(value);

            UnityEditor.EditorGUILayout.PropertyField(Condition);

            if (Condition.intValue != 0 && Condition.intValue != 4)
            {
                UnityEditor.EditorGUILayout.PropertyField(compareTo);
            }

            if (Condition.intValue == 4)
                UnityEditor.EditorGUILayout.PropertyField(T_Name,new GUIContent("Transform Name"));

            UnityEditor.EditorGUILayout.PropertyField(InvokeOnEnable);

            UnityEditor.EditorGUILayout.Space();

            UnityEditor.EditorGUILayout.PropertyField(Then);
            UnityEditor.EditorGUILayout.PropertyField(Else);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
