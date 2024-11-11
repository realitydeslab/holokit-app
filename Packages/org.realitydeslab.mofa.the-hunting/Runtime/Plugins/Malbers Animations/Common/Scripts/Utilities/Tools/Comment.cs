using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Utilities/Tools/Comment")]
    /// <summary>Adding Coments on the Inspector</summary>.
    public class Comment : MonoBehaviour
    {
        [Multiline] public string text;
        public Object reference;
        public bool ShowDescription;

        [ContextMenu("Show Reference")]
        void ShowReference() => ShowDescription ^= true;


    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Comment)),UnityEditor.CanEditMultipleObjects]
    public class CommentEditor : UnityEditor.Editor
    {
        private GUIStyle style;
        private UnityEditor.SerializedProperty text, ShowDescription, reference;

        private void OnEnable()
        {
            text = serializedObject.FindProperty("text");
            ShowDescription = serializedObject.FindProperty("ShowDescription");
            reference = serializedObject.FindProperty("reference");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (style == null)
                style = new GUIStyle(MTools.StyleGray)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true

                };
            style.normal.textColor = UnityEditor.EditorStyles.boldLabel.normal.textColor;
            // Color.white;

            if (ShowDescription.boolValue)
            {
                UnityEditor.EditorGUILayout.PropertyField(reference, GUIContent.none);
            }
            using (new GUILayout.VerticalScope(MTools.StyleGray))
                text.stringValue = UnityEditor.EditorGUILayout.TextArea(text.stringValue, style);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}