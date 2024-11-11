using System;
using UnityEngine;

namespace MalbersAnimations
{
    public class CreateScriptableAssetAttribute : PropertyAttribute
    {
        public bool isAsset = true; 
        public CreateScriptableAssetAttribute(bool isAsset) => this.isAsset = isAsset;
        public CreateScriptableAssetAttribute() => this.isAsset = true;

    }


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CreateScriptableAssetAttribute), true)]
    public class CreateAssetDrawer : UnityEditor.PropertyDrawer
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

            var attr = attribute as CreateScriptableAssetAttribute;


            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = UnityEditor.EditorGUI.indentLevel;
            UnityEditor.EditorGUI.indentLevel = 0;

            if (element == null)
            {
                position.width -= 22;
                UnityEditor.EditorGUI.PropertyField(position, property, GUIContent.none);
                var AddButtonRect = new Rect(position) { x = position.width + position.x + 4, width = 20 };
                 
                
                if (GUI.Button(AddButtonRect, plus, UnityEditor.EditorStyles.helpBox))
                {
                    if (attr.isAsset)
                        MTools.CreateScriptableAsset(property, MTools.GetPropertyType(property), MTools.GetSelectedPathOrFallback());
                    else
                        MTools.CreateScriptableAssetInternal(property, MTools.GetPropertyType(property));

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