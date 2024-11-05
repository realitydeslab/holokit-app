using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations
{
    [CustomEditor(typeof(MalbersInput))/*, CanEditMultipleObjects*/]
    public class MalbersInputEditor : MInputEditor
    {
        protected SerializedProperty Horizontal, Vertical, UpDown;
     //   private MalbersInput M;

        protected override void OnEnable()
        {
            base.OnEnable();

            Horizontal = serializedObject.FindProperty("Horizontal");
            Vertical = serializedObject.FindProperty("Vertical");
            UpDown = serializedObject.FindProperty("UpDown");

           // M = ((MalbersInput)target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Inputs connected to component functions via UnityEvents");

            if (Application.isPlaying)
            {
                showOnPlayMode =
                    GUILayout.Toggle(showOnPlayMode,
                    new GUIContent("Show Buttons on Play Mode", "This makes the Inspector bit faster"), EditorStyles.miniButton);
            }

            if (!Application.isPlaying || (showOnPlayMode && Application.isPlaying))
            {

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(IgnoreOnPause);
                    EditorGUILayout.PropertyField(ResetOnFocusLost);
                }

                DrawRewired();

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(Horizontal, new GUIContent("Horizontal", "Axis for the Horizontal Movement"));
                    EditorGUILayout.PropertyField(Vertical, new GUIContent("Vertical", "Axis for the Forward/Backward Movement"));
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(UpDown, new GUIContent("UpDown", "Axis for the Up and Down Movement"));

                        if (GUILayout.Button(new GUIContent("Create", "Creates 'UpDown' on the Input Manager"), GUILayout.Width(55)))
                            CreateInputAxe();
                    }
                }

                DrawListAnEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }


        //CREATE UP DOWN AXIS
        public static void CreateInputAxe()
        {
            var InputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            var axesProperty = InputManager.FindProperty("m_Axes");

            AddInputAxis(axesProperty, "UpDown", "c", "space", "", "", 1000, 0.001f, 3, true, false, AxisType.KeyMouseButton, AxisNumber.X);

            InputManager.ApplyModifiedProperties();
        }

        private static void AddInputAxis(SerializedProperty axesProperty, string name, string negativeButton, string positiveButton,
                                string altNegativeButton, string altPositiveButton, float gravity, float dead, float sensitivity, bool snap, bool invert, AxisType axisType, AxisNumber axisNumber)
        {
            var property = FindAxisProperty(axesProperty, name);

            if (property != null)
            {
                property.FindPropertyRelative("m_Name").stringValue = name;
                property.FindPropertyRelative("negativeButton").stringValue = negativeButton;
                property.FindPropertyRelative("positiveButton").stringValue = positiveButton;
                property.FindPropertyRelative("altNegativeButton").stringValue = altNegativeButton;
                property.FindPropertyRelative("altPositiveButton").stringValue = altPositiveButton;
                property.FindPropertyRelative("gravity").floatValue = gravity;
                property.FindPropertyRelative("dead").floatValue = dead;
                property.FindPropertyRelative("sensitivity").floatValue = sensitivity;
                property.FindPropertyRelative("snap").boolValue = snap;
                property.FindPropertyRelative("invert").boolValue = invert;
                property.FindPropertyRelative("type").intValue = (int)axisType;
                property.FindPropertyRelative("axis").intValue = (int)axisNumber;
            }
        }

        private static SerializedProperty FindAxisProperty(SerializedProperty axesProperty, string name)
        {
            SerializedProperty foundProperty = null;

            for (int i = 0; i < axesProperty.arraySize; ++i)
            {
                var property = axesProperty.GetArrayElementAtIndex(i);
                if (property.FindPropertyRelative("m_Name").stringValue.Equals(name))
                {
                    foundProperty = property;
                    break;
                }
            }

            if (foundProperty == null)  // If no property was found then create a new one.
            {
                axesProperty.InsertArrayElementAtIndex(axesProperty.arraySize);
                foundProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);
                Debug.Log("Added Input");
            }

            return foundProperty;
        }
    }
}