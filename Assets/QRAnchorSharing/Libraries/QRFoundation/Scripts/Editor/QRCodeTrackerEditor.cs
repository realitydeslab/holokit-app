using UnityEngine;
using UnityEditor;

namespace QRFoundation
{
    [CustomEditor(typeof(QRCodeTracker))]
    public class QRCodeTrackerEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        SerializedProperty onCodeDetectedEventProp;
        SerializedProperty onCodeRegisteredEventProp;
        SerializedProperty onCodeLostEventProp;

        void OnEnable()
        {
            onCodeDetectedEventProp = serializedObject.FindProperty("onCodeDetected");
            onCodeRegisteredEventProp = serializedObject.FindProperty("onCodeRegistered");
            onCodeLostEventProp = serializedObject.FindProperty("onCodeLost");
        }

        public override void OnInspectorGUI()
        {
            QRCodeTracker module = (QRCodeTracker)target;

            string[] options = new string[]
            {
                "Fixed size", "Content mapping", "Custom function",
            };
            module.sizeDetermination = (SizeDeterminationMode) EditorGUILayout.Popup("Size determination strategy", (int) module.sizeDetermination, options);

            switch (module.sizeDetermination)
            {
                case SizeDeterminationMode.Fixed:
                    module.codeWidth = EditorGUILayout.FloatField(
                        new GUIContent(
                        "Code width",
                        "The pysical width of the QR code in meters."),
                        module.codeWidth
                    );
                    break;
                case SizeDeterminationMode.Map:
                    EditorGUILayout.HelpBox("Determine the real-world width of the QR code based on a text that is contained in it.", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Search string");
                    GUILayout.Label("Resulting size (meters)");
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < module.codeWidths.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        module.codeWidths[i].contains = EditorGUILayout.TextField(module.codeWidths[i].contains);
                        module.codeWidths[i].width = EditorGUILayout.FloatField(module.codeWidths[i].width);
                        if (GUILayout.Button("x"))
                        {
                            module.codeWidths.Remove(module.codeWidths[i]);
                            i--;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Add more"))
                    {
                        module.codeWidths.Add(new WidthMapping { contains = "", width = 0 });
                    }

                    break;
                case SizeDeterminationMode.Function:
                    EditorGUILayout.HelpBox("This mode enables you to write custom logic for determining the code width. Simply assign your own function to the public \"getWidthDelegate\" property of this component. It will receive the QR code's content as a string and has to return the width in meters.", MessageType.Info);
                    break;
            }

            GUILayout.Space(10);

            string[] invertOptions = new string[]
            {
                "Invert", "Don't invert", "Only invert"
            };
            module.inversionAttempt = (InversionAttempt)EditorGUILayout.Popup(
                new GUIContent(
                    "Inversion attempt",
                    "Whether the scanner should also look for light on dark QR codes. If your use case does not include those, you can deactivate this option for a small performance increase"),
                (int)module.inversionAttempt,
                invertOptions
            );

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(
                "Tracked code prefab",
                "A GameObject that will be spawned with the position and orientation of the QR code when it is registered."
            ));
            module.prefab = (GameObject)EditorGUILayout.ObjectField(module.prefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();


            module.showLifecycleEvents = EditorGUILayout.Foldout(module.showLifecycleEvents, "Lifecycle events");

            if (module.showLifecycleEvents)
            {
                //serializedObject.Update();
                //DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
                //serializedObject.ApplyModifiedProperties();

                EditorGUILayout.PropertyField(
                    onCodeDetectedEventProp,
                    new GUIContent(
                        "On code detected"
                    )
                );
                EditorGUILayout.PropertyField(
                    onCodeRegisteredEventProp,
                    new GUIContent(
                        "On code registered"
                    )
                );
                EditorGUILayout.PropertyField(
                    onCodeLostEventProp,
                    new GUIContent(
                        "On code lost"
                    )
                );
            }

            module.showAdvanedSettings = EditorGUILayout.Foldout(module.showAdvanedSettings, "Advanced settings");

            if (module.showAdvanedSettings)
            {
                module.codeLossTimeout = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Code lost timeout",
                        "The amount of seconds that need to pass without a successful scan before treating the code as lost and switching back to SEARCHING mode."),
                     module.codeLossTimeout
                );

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

                module.searchScanInterval = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Search scan interval",
                        "How many seconds have to pass at least between two consecutive scans in SEARCHING mode, i.e. when no code has been recognized so far."),
                    module.searchScanInterval
                );

                module.stabilizeScanInterval = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Stabilize scan interval",
                        "How many seconds have to pass at least between two consecutive scans in STABILIZING mode, i.e. when a code has just been found, but not yet localized in 3D space. It's recommended to keep this value at 0 to scan as often as possible!"),
                    module.stabilizeScanInterval
                );

                module.refineScanInterval = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Refine scan interval",
                        "How many seconds have to pass at least between two consecutive scans in REFINING mode, i.e. when a code has already been fully registered but may need correction over time. It's recommended to keep this value at 0 if you can afford the performance impact."),
                    module.refineScanInterval
                );

                module.truncateTimeout = EditorGUILayout.FloatField(
                    new GUIContent(
                        "Stabilize timeout",
                        "When this amount of seconds has passed in STABILIZING mode, the code is registered based on the current samples regardless of the precision."),
                    module.truncateTimeout
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
