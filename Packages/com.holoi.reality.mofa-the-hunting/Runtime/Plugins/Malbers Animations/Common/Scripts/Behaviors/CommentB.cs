using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>  Adding Coments on the the Animator</summary>
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/")]
    public class CommentB : StateMachineBehaviour  {[Multiline] public string text; }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CommentB))]
    public class CommentBEditor : UnityEditor.Editor
    {
        private GUIStyle style;
        private UnityEditor.SerializedProperty text;

        private void OnEnable()
        {
            text = serializedObject.FindProperty("text");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (style == null)
                style = new GUIStyle(MTools.StyleGray)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true
                };

            style.normal.textColor = UnityEditor.EditorStyles.label.normal.textColor;

            UnityEditor.EditorGUILayout.BeginVertical(MTools.StyleBlue);
            text.stringValue = UnityEditor.EditorGUILayout.TextArea(text.stringValue, style);
            UnityEditor.EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}