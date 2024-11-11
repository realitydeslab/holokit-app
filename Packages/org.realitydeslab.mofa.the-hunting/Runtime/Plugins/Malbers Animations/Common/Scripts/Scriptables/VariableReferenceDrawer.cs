#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    [CustomPropertyDrawer(typeof(ReferenceVar),true)]
    public class VariableReferenceDrawer : PropertyDrawer
    {
        /// <summary>  Options to display in the popup to select constant or variable. </summary>
        private readonly string[] popupOptions =  { "Use Local", "Use Global" };

        /// <summary> Cached style to use to draw the popup button. </summary>
        private GUIStyle popupStyle;
        private GUIStyle AddStyle;
        private GUIContent plus;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) { imagePosition = ImagePosition.ImageOnly };

            if (AddStyle == null)
                AddStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) { imagePosition = ImagePosition.ImageOnly };

            if (plus == null) plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");

            position.y -= 0;

            label = EditorGUI.BeginProperty(position, label, property);
            {
                Rect variableRect = new Rect(position);
                position = EditorGUI.PrefixLabel(position, label);


                float height = EditorGUIUtility.singleLineHeight;

                // Get properties
                SerializedProperty useConstant = property.FindPropertyRelative("UseConstant");
                SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
                SerializedProperty variable = property.FindPropertyRelative("Variable");

                Rect propRect = new Rect(position) { height = height };

                // Calculate rect for configuration button
                Rect buttonRect = new Rect(position);
                buttonRect.yMin += popupStyle.margin.top;
                buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
                buttonRect.x -= 20;
                buttonRect.height = height;

                position.xMin = buttonRect.xMax;


                var AddButtonRect = new Rect(propRect) { x = propRect.width + propRect.x - 18, width = 20 };
                var ValueRect = new Rect(AddButtonRect);

                // Store old indent level and set it to 0, the PrefixLabel takes care of it
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);
                useConstant.boolValue = (result == 0);

                bool varIsEmpty = variable.objectReferenceValue == null;

                if (!useConstant.boolValue)
                {
                    if (varIsEmpty)
                    { 
                        propRect.width -= 20;
                    }
                    else
                    {
                        if (ValidObject(variable.objectReferenceValue))   //Do not Paint other than Int float and Strings
                        {
                            //propRect.width -= 30;
                            ValueRect.width = (propRect.width / 2 * 0.25f) + 9;
                            propRect.width = (propRect.width/2 *1.75f)-13;
                          //  ValueRect.x -= 8;
                            ValueRect.x =position.x + propRect.width + 8;
                        }
                    }
                }


#if UNITY_2020_1_OR_NEWER
                EditorGUIUtility.labelWidth = 0.1f;
#endif
                EditorGUI.PropertyField(propRect, useConstant.boolValue ? constantValue : variable, GUIContent.none, false);
                EditorGUIUtility.labelWidth = 0;

                if (!useConstant.boolValue)
                {
                    if (varIsEmpty)
                    {
                        if (GUI.Button(AddButtonRect, plus, UnityEditor.EditorStyles.helpBox))
                        {
                            MTools.CreateScriptableAsset(variable, MTools.GetPropertyType(variable), MTools.GetSelectedPathOrFallback());
#if UNITY_2020_1_OR_NEWER
                            GUIUtility.ExitGUI(); //Unity Bug!
#endif
                        }
                    }
                    else
                    {
                        ShowScriptVar(ValueRect, variable);
                    }
                }
                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }

        private static void ShowScriptVar(Rect variableRect, SerializedProperty variable)
        {
            if (variable.objectReferenceValue != null)
            {
                if (!ValidObject(variable.objectReferenceValue)) return; //Do not Paint vectors

                SerializedObject objs = new SerializedObject(variable.objectReferenceValue);
                var Var = objs.FindProperty("value");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(variableRect, Var, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    objs.ApplyModifiedProperties();
                    EditorUtility.SetDirty(variable.objectReferenceValue);
                }
            }
        }


        private static bool ValidObject(Object val) => (val is IntVar) || (val is FloatVar &&  !(val is FloatRangeVar) ) || (val is BoolVar) || (val is StringVar);

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{

        //    float height = base.GetPropertyHeight(property, label);

        //    SerializedProperty useConstant = property.FindPropertyRelative("UseConstant");
        //    if (!useConstant.boolValue)
        //    {
        //        SerializedProperty variable = property.FindPropertyRelative("Variable");
        //        if (variable.objectReferenceValue != null)
        //        height = height * 2 + 8;
        //    }

        //    return height;
        //}
    }
}
#endif