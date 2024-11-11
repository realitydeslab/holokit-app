using MalbersAnimations.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Scriptables
{
    [AddComponentMenu("Malbers/Variables/Transform Listener")]
    public class TransformListener : MonoBehaviour
    {
        [RequiredField] public TransformVar value;
        [Tooltip("Invokes the current value on Enable")]
        public bool InvokeOnEnable = true;

        [Tooltip("Show Invoke Null Event if the ")]
        public bool InvokeNull = true;

        public TransformEvent OnValueChanged = new TransformEvent();
        public UnityEvent OnValueNull = new UnityEvent();


        void OnEnable()
        {
            value.OnValueChanged += Invoke;
            if (InvokeOnEnable) Invoke(value.Value);
        }

        void OnDisable()
        {
            value.OnValueChanged -= Invoke;
            Invoke(value.Value);
        }

        /// <summary> Used to use turn Objects to True or false </summary>
        public virtual void Invoke(Transform value)
        {
            OnValueChanged.Invoke(value);
            if (InvokeNull && !value) OnValueNull.Invoke();
        }
    }

#if UNITY_EDITOR 
    //INSPECTOR
    [UnityEditor.CustomEditor(typeof(TransformListener)), UnityEditor.CanEditMultipleObjects]
    public class TransformListenerEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty value, InvokeOnEnable, OnValueChanged, InvokeNull, OnValueNull;
        TransformListener M;

        void OnEnable()
        {
            M = target as TransformListener;
            value = serializedObject.FindProperty("value");
            InvokeOnEnable = serializedObject.FindProperty("InvokeOnEnable");
            OnValueChanged = serializedObject.FindProperty("OnValueChanged");
            InvokeNull = serializedObject.FindProperty("InvokeNull");
            OnValueNull = serializedObject.FindProperty("OnValueNull");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(value);
            if (value.objectReferenceValue && Application.isPlaying)
            {
                UnityEditor.EditorGUI.BeginDisabledGroup(true);
                UnityEditor.EditorGUILayout.ObjectField("Value [Runtime] ", M.value.Value, typeof(Transform), false);
                Repaint();
                UnityEditor.EditorGUI.EndDisabledGroup();
            }
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(InvokeOnEnable);
            UnityEditor.EditorGUIUtility.labelWidth = 70;
            UnityEditor.EditorGUILayout.PropertyField(InvokeNull);
            UnityEditor.EditorGUIUtility.labelWidth = 0;
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(OnValueChanged);

            if (InvokeNull.boolValue)
                UnityEditor.EditorGUILayout.PropertyField(OnValueNull);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
