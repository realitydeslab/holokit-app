using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Prefab Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Variables/Transform", order = 3000)]
    public class TransformVar : ScriptableVar
    {
       [SerializeField] private Transform value;

        /// <summary>Invoked when the value changes </summary>
        public Action<Transform> OnValueChanged = delegate { };

        /// <summary> Value of the Bool variable</summary>
        public virtual Transform Value
        {
            get => value;
            set
            {
                if (value != this.value) // Avoid Stack Overflow
                {
                    this.value = value;
                    OnValueChanged(value);         //If we are using OnChange event Invoked
#if UNITY_EDITOR
                    if (debug) Debug.Log($"<B>{name} -> [<color=white> {(value != null ? value.ToString() : "NULL")} </color>] </B>", this);
#endif
                }
            }
        }

        public virtual void SetValue(TransformVar var) => Value = var.Value;
        public virtual void SetNull() => Value = null;
        public virtual void SetValue(Transform var) => Value = var;
        public virtual void SetValue(GameObject var) => Value = var.transform;
        public virtual void SetValue(Component var) => Value = var.transform;
    }

    [Serializable]
    public class TransformReference : ReferenceVar
    {
        public Transform ConstantValue;
        [RequiredField] public TransformVar Variable;

        public TransformReference() => UseConstant = true;
        public TransformReference(Transform value) => Value = value;

        public TransformReference(TransformVar value)
        {
            Variable = value;
            UseConstant = false;
        }

        public Transform Value
        {
            get
            { 
                return UseConstant ? ConstantValue : (Variable != null ? Variable.Value : null);
            }

            set
            {
                if (UseConstant || Variable == null)
                {
                    UseConstant = true;
                    ConstantValue = value;
                }
                else
                    Variable.Value = value;
            }
        }

        public Vector3 position => Value.position;
        public Quaternion rotation => Value.rotation;

        public static implicit operator Transform(TransformReference reference) => reference.Value;
        public static implicit operator TransformReference(Transform reference) => new TransformReference(reference);
    }

#if UNITY_EDITOR

    [UnityEditor.CanEditMultipleObjects, UnityEditor.CustomEditor(typeof(TransformVar))]
    public class TransformVarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new GUILayout.HorizontalScope())
            {
                MalbersEditor.DrawDescription("Runtime Transform Reference");
                MalbersEditor.DrawDebugIcon(debug);
            }

            UnityEditor.EditorGUILayout.Space();

            var go = value.objectReferenceValue as Transform;

            if (Application.isPlaying)
            {
                UnityEditor.EditorGUILayout.ObjectField("Value ", go, typeof(GameObject), false);
            }
            else
            {
                UnityEditor.EditorGUILayout.LabelField("<Runtime Only>", UnityEditor.EditorStyles.boldLabel);
            }


            UnityEditor.EditorGUILayout.PropertyField(Description);


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}
