using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    public class ExposeScriptableAssetAttribute : PropertyAttribute
    { }
   


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ExposeScriptableAssetAttribute), true)]
    public class ExposeScriptableAssetDrawer : PropertyDrawer
    {
        private Editor editor = null;
        private GUIContent plus;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hasProperty = property.objectReferenceValue != null;

          

            if (!hasProperty) { position.width -= 22; }

            //Draw Label
            if (hasProperty) EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, label,true);
            if (hasProperty) EditorGUI.indentLevel--;

            if (hasProperty)
            {
                EditorGUI.indentLevel++;
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
                EditorGUI.indentLevel--;


                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    if (!editor)
                        Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);

                    editor.OnInspectorGUI();

                    EditorGUI.indentLevel--;

                }
            }
            else
            {

                if (plus == null)
                {
                    plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");
                    plus.tooltip = "Create";
                }

                var AddButtonRect = new Rect(position) { x = position.width + position.x + 4, width = 20 };

                if (GUI.Button(AddButtonRect, plus, UnityEditor.EditorStyles.helpBox))
                {
                    MTools.CreateScriptableAsset(property, MTools.GetPropertyType(property), MTools.GetSelectedPathOrFallback());
                    GUIUtility.ExitGUI();
                }
            }
        }
    }
#endif
}