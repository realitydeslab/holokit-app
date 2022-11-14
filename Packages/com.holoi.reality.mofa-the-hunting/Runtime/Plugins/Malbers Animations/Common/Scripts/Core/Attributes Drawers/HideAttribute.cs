using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public sealed class HideAttribute : PropertyAttribute
    {
        public string Variable = "";
        public bool Inverse = false;
        public int[] EnumValue;
       // public bool useOR = false; //Todo: Do thissss lateer?!?!

        public HideAttribute(string conditionalSourceField)
        {
            this.Variable = conditionalSourceField;
            this.Inverse = false;
        }

        public HideAttribute(string conditionalSourceField, bool inverse)
        {
            this.Variable = conditionalSourceField;
            this.Inverse = inverse;
        }

        public HideAttribute(string conditionalSourceField, bool inverse, params int[] EnumValue)
        {
            this.Variable = conditionalSourceField;
            this.Inverse = inverse;
            this.EnumValue = EnumValue;
        }

        public HideAttribute(string conditionalSourceField,   params int[] EnumValue)
        {
            this.Variable = conditionalSourceField;
            this.Inverse = false;
            this.EnumValue = EnumValue;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HideAttribute))]
    public class HidePropertyDrawer : PropertyDrawer
    { 
        private bool enabled;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideAttribute condHAtt = (HideAttribute)attribute;

            enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideAttribute condHAtt = (HideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }



        private bool GetConditionalHideAttributeResult(HideAttribute condHAtt, SerializedProperty property)
        {
            bool enabled =  true;

            //Handle primary property
            SerializedProperty sourcePropertyValue;
            //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays

            if (!property.isArray)
            {
                //returns the property path of the property we want to apply the attribute to
                string propertyPath = property.propertyPath; 
                
                //changes the path to the conditionalsource property path
                string conditionPath = propertyPath.Replace(property.name, condHAtt.Variable); 
                
                sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                if (sourcePropertyValue == null)
                {
                    //original implementation (doens't work with nested serializedObjects)
                    sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.Variable);
                }
            }
            else
            {
                //original implementation (doens't work with nested serializedObjects)
                sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.Variable);
            }


            if (sourcePropertyValue != null)
            {
                enabled = CheckPropertyType(sourcePropertyValue, condHAtt.EnumValue);
            }
            
            //wrap it all up
            if (condHAtt.Inverse) enabled = !enabled;
            return enabled;
        }

        private bool CheckPropertyType(SerializedProperty sourcePropertyValue, int[] EnumValue)
        {
            //Note: add others for custom handling if desired
            switch (sourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return sourcePropertyValue.boolValue;
                case SerializedPropertyType.ObjectReference:
                    return sourcePropertyValue.objectReferenceValue != null;
                case SerializedPropertyType.Enum:
                    for (int i = 0; i < EnumValue.Length; i++)
                    {
                        if (sourcePropertyValue.enumValueIndex == EnumValue[i]) 
                            return true;
                    }
                    return false;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
                    return true;
            }
        }
    }
#endif
}