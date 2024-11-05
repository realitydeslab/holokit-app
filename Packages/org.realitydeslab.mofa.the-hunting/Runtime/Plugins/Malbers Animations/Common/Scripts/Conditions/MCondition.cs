using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Conditions
{
    /// <summary>Conditions to Run on a Object </summary>
    [System.Serializable]
    //public abstract class MCondition<T>  where T : Object
    public abstract class  MCondition : MonoBehaviour 
    {
        /// <summary>Path displayed on the creation menu</summary>
        public abstract string DisplayName { get; }

        [HideInInspector,Tooltip("Name-Description of the Condition")]
        public string Name = "Condition";
        [HideInInspector, Tooltip("Inverts the result of the condition")]
        public bool invert;
        [HideInInspector, Tooltip("Or = true . And = False")]
        public bool OrAnd;
         

        /// <summary>Evaluate a condition using the Target</summary>
        public abstract bool _Evaluate();

        /// <summary>Set target on the Conditions</summary>
        public abstract void SetTarget(Object target);


        public bool Evaluate()
        {
            return invert ? !_Evaluate() : _Evaluate();
        }

        protected virtual void OnValidate()
        {
            //this.hideFlags = HideFlags.HideInInspector;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(MCondition))]
    public class MConditionEditor : Editor
    {
        protected SerializedObject so;
        protected SerializedProperty TTarget, Condition, Value, invert;

        protected virtual void OnEnable()
        {
            so = serializedObject;
            TTarget = so.FindProperty("Target");
            Condition = so.FindProperty("Condition");
            Value = so.FindProperty("Value");
            
            invert = so.FindProperty("invert");
        }

        public virtual void CustomInspector() { }

        protected void Field(SerializedProperty prop)
        {
            if (prop != null) EditorGUILayout.PropertyField(prop);
        }

        protected void Field(SerializedProperty prop, GUIContent cc)
        {
            if (prop != null) EditorGUILayout.PropertyField(prop, cc);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Field(TTarget);
            Field(Condition, new GUIContent($"Condition: {(!invert.boolValue ? "[Is]" : "[Is NOT]")}"));

            CustomInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}