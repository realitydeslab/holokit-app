using System;
using UnityEngine;

namespace MalbersAnimations
{
    public class CreateMonoAttribute : PropertyAttribute
    {
        public string name; 
        public CreateMonoAttribute(string name) => this.name = name;
        public CreateMonoAttribute() => this.name = string.Empty;

    }


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CreateMonoAttribute), true)]
    public class CreateMonoDrawer : UnityEditor.PropertyDrawer
    {
        private GUIContent plus;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            label = UnityEditor.EditorGUI.BeginProperty(position, label, property);
            position = UnityEditor.EditorGUI.PrefixLabel(position, label);

            if (plus == null)
            {
                plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");
                plus.tooltip = "Create";
            }

            var element = property.objectReferenceValue;
            var so = property.serializedObject.targetObject as MonoBehaviour;

            var attr = attribute as CreateMonoAttribute;


            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = UnityEditor.EditorGUI.indentLevel;
            UnityEditor.EditorGUI.indentLevel = 0;

            if (element == null && so != null)
            {
                position.width -= 22;
                UnityEditor.EditorGUI.PropertyField(position, property, GUIContent.none);
                var AddButtonRect = new Rect(position) { x = position.width + position.x + 4, width = 20 };
                
                if (GUI.Button(AddButtonRect, plus, UnityEditor.EditorStyles.helpBox))
                {
                    var NewMono = so.gameObject;

                    if (!string.IsNullOrEmpty(attr.name))
                    {
                        NewMono = new GameObject(attr.name);
                        NewMono.transform.parent = so.transform;
                        NewMono.transform.ResetLocal();
                    }

                    var mono = NewMono.AddComponent(MTools.GetPropertyType(property));
                    property.objectReferenceValue = mono;
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();

                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                UnityEditor.EditorGUI.PropertyField(position, property, GUIContent.none);
            }

            UnityEditor.EditorGUI.indentLevel = indent;

            UnityEditor.EditorGUI.EndProperty();
        }
    }
#endif
}