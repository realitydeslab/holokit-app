using UnityEngine;
using MalbersAnimations.Events;

namespace MalbersAnimations.Utilities
{
    /// <summary>Converts a value to string </summary>
    [AddComponentMenu("Malbers/UI/Value to String")]
    public class ValueToString : MonoBehaviour
    {
        [Tooltip("Decimal values after the coma 0.0")]
        public int decimals = 2;
        [Tooltip("String to add before the value")]
        public string Prefix;
        [Tooltip("String to add after the value")]
        public string Suffix;

        public StringEvent toString = new StringEvent();

        public virtual void ToString(float value) => toString.Invoke(Prefix + value.ToString("F" + decimals) + Suffix);
        public virtual void ToString(int value) => toString.Invoke(Prefix + value.ToString() + Suffix);
        public virtual void ToString(bool value) => toString.Invoke(Prefix + value.ToString() + Suffix);
        public virtual void ToString(string value) => toString.Invoke(Prefix + value + Suffix);
        public virtual void SetPrefix(string value) => Prefix = value;
        public virtual void SetSufix(string value) => Suffix = value;
        public virtual void ToString(Object value) => toString.Invoke(Prefix + value.name + Suffix);
        public virtual void ToString(Vector3 value) => toString.Invoke(Prefix + value.ToString() + Suffix);
        public virtual void ToString(Vector2 value) => toString.Invoke(Prefix + value.ToString() + Suffix);
    }


#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ValueToString))]
    public class ValueToStringEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty decimals, Prefix, Suffix, toString;
        private void OnEnable()
        {
            decimals= serializedObject.FindProperty("decimals");
            Prefix = serializedObject.FindProperty("Prefix");
            Suffix = serializedObject.FindProperty("Suffix");
            toString = serializedObject.FindProperty("toString");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Convert any value to a string");

          //  UnityEditor.EditorGUILayout.PropertyField(decimals);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUIUtility.labelWidth = 50;
            UnityEditor.EditorGUILayout.PropertyField(Prefix, GUILayout.MinWidth(40));
            UnityEditor.EditorGUILayout.PropertyField(Suffix);
          
            if (GUILayout.Button(new GUIContent( decimals.intValue.ToString(),"Float Decimal Values"), UnityEditor.EditorStyles.miniButton, GUILayout.Width(24)))
            {
                decimals.intValue = (decimals.intValue + 1) % 11;
            }
            UnityEditor.EditorGUIUtility.labelWidth = 0;
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(toString);
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}