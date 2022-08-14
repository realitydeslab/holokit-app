using UnityEngine;
using UnityEditor;
using QRFoundation;

namespace QRFoundation
{
    [CustomEditor(typeof(QRAnchorReceiver))]
    public class QRAnchorReceiverEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        SerializedProperty onCodeDetectedEventProp;
        SerializedProperty onAnchorReceivedEventProp;
        SerializedProperty onPoseReceivedEventProp;
        SerializedProperty onStabilizeFailureEventProp;

        void OnEnable()
        {
            onCodeDetectedEventProp = serializedObject.FindProperty("onCodeDetected");
            onAnchorReceivedEventProp = serializedObject.FindProperty("onAnchorReceived");
            onPoseReceivedEventProp = serializedObject.FindProperty("onPoseReceived");
            onStabilizeFailureEventProp = serializedObject.FindProperty("onStabilizeFailure");
        }

        public override void OnInspectorGUI()
        {
            QRAnchorReceiver module = (QRAnchorReceiver)target;

            string[] options = new string[]
            {
                "Full rotation", "Align with Y axis",
            };
            module.rotationMode = (RotationMode) EditorGUILayout.Popup("Rotation mode", (int) module.rotationMode, options);

            GUILayout.Space(10);

            string[] modeOptions = new string[]
            {
                "Managed anchor", "Pose",
            };
            module.outputMode = (OutputMode)EditorGUILayout.Popup("Output mode", (int)module.outputMode, modeOptions);

            //string[] invertOptions = new string[]
            //{
            //    "Invert", "Don't invert", "Only invert"
            //};
            //module.inversionAttempt = (InversionAttempt)EditorGUILayout.Popup(
            //    new GUIContent(
            //        "Inversion attempt",
            //        "Whether the scanner should also look for light on dark QR codes. If your use case does not include those, you can deactivate this option for a small performance increase"),
            //    (int)module.inversionAttempt,
            //    invertOptions
            //);

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label(new GUIContent(
            //    "Shared prefab",
            //    "A GameObject that will be spawned with the position and orientation of the QR code when it is registered."
            //));
            //module.prefab = (GameObject)EditorGUILayout.ObjectField(module.prefab, typeof(GameObject), false);
            //EditorGUILayout.EndHorizontal();

            //serializedObject.ApplyModifiedProperties();
            //serializedObject.Update();
            //DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
            if (module.outputMode == OutputMode.ManagedAnchor)
            {
                EditorGUILayout.HelpBox("The event will fire each time a shared anchor has been received and is no longer pending. You have attach an AR Anchor Manager script to a game object (usually the AR Session Origin) and link it as the \"AR session origin\" property below.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(
                    "AR session origin",
                    "The game object containing the AR Anchor Manager."
                ));
                module.sessionOrigin = (GameObject)EditorGUILayout.ObjectField(module.sessionOrigin, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(
                    onAnchorReceivedEventProp,
                    new GUIContent(
                        "On anchor received"
                    )
                );
            } else if (module.outputMode == OutputMode.Pose)
            {
                EditorGUILayout.HelpBox("The event will fire each time a shared pose has been received. In contains the metadata string and the pose. This enables you to write custom logic on how to handle the anchor.", MessageType.Info);
                EditorGUILayout.PropertyField(
                    onPoseReceivedEventProp,
                    new GUIContent(
                        "On pose received"
                    )
                );
            }
            module.showLifecycleEvents = EditorGUILayout.Foldout(module.showLifecycleEvents, "Lifecycle events");

            if (module.showLifecycleEvents)
            {
                EditorGUILayout.PropertyField(
                    onCodeDetectedEventProp,
                    new GUIContent(
                        "On code detected"
                    )
                );

                EditorGUILayout.PropertyField(
                    onStabilizeFailureEventProp,
                    new GUIContent(
                        "On stabilize failure"
                    )
                );
            }

            module.showAdvanedSettings = EditorGUILayout.Foldout(module.showAdvanedSettings, "Advanced settings");

            if (module.showAdvanedSettings)
            {
                module.requiredPrecision = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Required stability",
                        "The maximum allowed degree of fluctuation in the tracking before triggering registration. Higher values result in earlier, but less stable results. Values should be from around 1 to 2."),
                     module.requiredPrecision
                );

                module.maxScanResolution = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Max. scan resolution",
                        "For performance reasons, the camera image is downscaled before analyzing. This value determines the maximum amount of pixels (width x height) that should be targeted. Higher values result in more accurate tracking, at the cost of more CPU usage. However, the benefit of increasing this value is neglectable. Decrease it if you need better performance."),
                    module.maxScanResolution
                );

                module._searchScanInterval = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Search scan interval",
                        "How many seconds have to pass at least between two consecutive scans in SEARCHING mode, i.e. when no code has been recognized so far."),
                    module._searchScanInterval
                );

                module.stabilizeScanInterval = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Stabilize scan interval",
                        "How many seconds have to pass at least between two consecutive scans in STABILIZING mode, i.e. when a code has just been found, but not yet localized in 3D space. It's recommended to keep this value at 0 to scan as often as possible!"),
                    module.stabilizeScanInterval
                );

                module._truncateTimeout = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Stabilize timeout",
                        "When this amount of seconds has passed in STABILIZING mode, the code is registered based on the current samples regardless of the precision."),
                    module._truncateTimeout
                );

                module.debugMode = EditorGUILayout.Toggle(
                    new GUIContent(
                        "Debug mode",
                        "Turn debug outputs on or off."),
                    module.debugMode
                );

                GUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
