using UnityEngine;
using UnityEditor;
using QRFoundation;

namespace QRFoundation
{
    [CustomEditor(typeof(QRAnchorSender))]
    public class QRAnchorSenderEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        SerializedProperty onCodeUpdateEventProp;

        void OnEnable()
        {
            onCodeUpdateEventProp = serializedObject.FindProperty("onCodeUpdate");
        }

        public override void OnInspectorGUI()
        {
            QRAnchorSender module = (QRAnchorSender)target;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(
                "Sharing target",
                "The game object which's transform should be shared."
            ));
            module.target = (GameObject)EditorGUILayout.ObjectField(module.target, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            module.anchorId = EditorGUILayout.IntField(
                new GUIContent(
                    "Anchor ID",
                    "A number that distinguishes the current anchor to be shared. Leave at 0 if you don't maintain multiple anchors."),
                module.anchorId
            );

            module.metaData = EditorGUILayout.TextField(
                new GUIContent(
                    "Meta data",
                    "Additional string value that should be transmitted in the QR code"),
                module.metaData
            );

            module.updateInterval = EditorGUILayout.FloatField(
                new GUIContent(
                    "Update interval",
                    "Interval in seconds for updating the code. Lower values will lead to faster transmission, but if updates are too quick, the camera of the receiving device may not be able to capture it."),
                module.updateInterval
            );

            module.codeScale = EditorGUILayout.FloatField(
                new GUIContent(
                    "Code scale",
                    "The factor by which the code will be downscaled when displayed. A value of 1 means, that it will cover the entire smaller dimension of the screen."),
                module.codeScale
            );

            module.drawOnGui = EditorGUILayout.Toggle(
                new GUIContent(
                    "Draw on GUI",
                    "Render the QR code full-screen as GUI."),
                module.drawOnGui
            );

            EditorGUILayout.PropertyField(
                onCodeUpdateEventProp,
                new GUIContent(
                    "On code update"
                )
            );
        }
    }
}
