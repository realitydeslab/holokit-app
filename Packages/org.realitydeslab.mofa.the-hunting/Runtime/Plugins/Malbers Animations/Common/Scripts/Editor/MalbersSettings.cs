using UnityEngine;
using UnityEditor;

/// <summary> This Class is use for creating Layers and Tags </summary>
namespace MalbersAnimations
{
    [InitializeOnLoad]
    public class MalbersSettings : Editor
    {
        static MalbersSettings()
        {
            CreateLayer("Animal", 20);
            CreateLayer("Enemy", 23);
            CreateLayer("Item", 30);
            CreateTag("Fly");
            CreateTag("Climb");
            CreateTag("WallRun");
            CreateTag("Stair");
            CreateInputAxe();
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

            if (property == null)
            {
                axesProperty.InsertArrayElementAtIndex(axesProperty.arraySize);
                property = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

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

                Debug.Log($"Added Input [{name}]");
            }
        }

        private static SerializedProperty FindAxisProperty(SerializedProperty axesProperty, string name)
        {
            for (int i = 0; i < axesProperty.arraySize; ++i)
            {
                var property = axesProperty.GetArrayElementAtIndex(i);
                if (property.FindPropertyRelative("m_Name").stringValue.Equals(name))
                {
                    return property;
                }
            }
            return null;
        }


        static void CreateLayer(string LayerName, int index)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            if (layers == null || !layers.isArray)
            {
                Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
                Debug.LogWarning("Layers is null: " + (layers == null));
                return;
            }

            if (LayerMask.GetMask(LayerName) == 0)
            {
                var layerEnemy = layers.GetArrayElementAtIndex(index);

                if (layerEnemy.stringValue == string.Empty)
                {
                    Debug.Log("Setting up layers.  Layer " + "[" + index + "]" + " is now called " + "[" + LayerName + "]");
                    layerEnemy.stringValue = LayerName;
                    tagManager.ApplyModifiedProperties();
                }
            }
        }

        static void CreateTag(string s)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(s)) { found = true; break; }
            }

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = s;
                Debug.Log("Tag: <B>" + s + "</B> Added");
                tagManager.ApplyModifiedProperties();
            }
        }
    }
}