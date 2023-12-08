using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations.Scriptables
{
    public abstract class ScriptableVar: ScriptableObject
    {
#if UNITY_EDITOR
        [TextArea(3, 20)]
        public string Description = "";
#endif
        [HideInInspector] public bool debug = false;
    }

    /// <summary> Base for all Local Scritable Reference Variables </summary>
    public abstract class ReferenceVar
    {
        public bool UseConstant = true;
    }


#if UNITY_EDITOR
    public class VariableEditor : Editor
    {
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        protected SerializedProperty value, Description, debug;

        private void OnEnable()
        {
            value = serializedObject.FindProperty("value");
            Description = serializedObject.FindProperty("Description");
            debug = serializedObject.FindProperty("debug");
        }

        public virtual void PaintInspectorGUI(string title)
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription(title);


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(value, new GUIContent("Value", "The current value"));
                    MalbersEditor.DrawDebugIcon(debug);
                }
                EditorGUILayout.PropertyField(Description);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}