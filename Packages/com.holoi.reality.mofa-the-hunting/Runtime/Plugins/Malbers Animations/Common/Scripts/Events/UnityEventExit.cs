using UnityEngine;

namespace MalbersAnimations.Events
{
    /// <summary>Simple Event Raiser On Disable</summary>
    [AddComponentMenu("Malbers/Events/Unity Event Exit")]
    public class UnityEventExit : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent OnDisableEvent;
        public void OnDisable() => OnDisableEvent.Invoke();

        public string Description = "";
        [HideInInspector] public bool ShowDescription = false;
        [ContextMenu("Show Description")]
        internal void EditDescription() => ShowDescription ^= true;
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UnityEventExit))]
    public class UnityEventExitInspector : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty  OnDisableEvent, ShowDescription, Description;
         
        private GUIStyle style;


        private void OnEnable()
        {
            ShowDescription = serializedObject.FindProperty("ShowDescription");
            Description = serializedObject.FindProperty("Description");
            OnDisableEvent = serializedObject.FindProperty("OnDisableEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (ShowDescription.boolValue)
            {
                if (style == null)
                    style = new GUIStyle(MTools.StyleBlue)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold, 
                        alignment = TextAnchor.MiddleLeft, 
                        stretchWidth = true
                    };

                style.normal.textColor = UnityEditor.EditorStyles.label.normal.textColor;
                Description.stringValue = UnityEditor.EditorGUILayout.TextArea(Description.stringValue, style);
            }
            UnityEditor.EditorGUILayout.PropertyField(OnDisableEvent,new GUIContent("On Disable"));
            serializedObject.ApplyModifiedProperties();
        } 
    }
#endif
}