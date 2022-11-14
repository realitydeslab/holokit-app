using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using MalbersAnimations.Animals;

namespace MalbersAnimations.Controller
{
    [CustomPropertyDrawer(typeof(JumpProfile))]
    public class JumpProfileDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var name = property.FindPropertyRelative("name");
            var VerticalSpeed = property.FindPropertyRelative("VerticalSpeed");
            var JumpLandDistance = property.FindPropertyRelative("JumpLandDistance");
            var ExitTime = property.FindPropertyRelative("ExitTime");
            var fallingTime = property.FindPropertyRelative("fallingTime");
            var CliffLandDistance = property.FindPropertyRelative("CliffLandDistance");
            var HeightMultiplier = property.FindPropertyRelative("HeightMultiplier");
            var ForwardMultiplier = property.FindPropertyRelative("ForwardMultiplier");
            var ForwardPressed = property.FindPropertyRelative("ForwardPressed");
            var LastState = property.FindPropertyRelative("LastState");
            var CliffTime = property.FindPropertyRelative("CliffTime");

            position.y += 2;

            EditorGUI.BeginProperty(position, label, property);

            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                var height = EditorGUIUtility.singleLineHeight;


                var line = new Rect(position);
                line.x += 4;
                line.width -= 8;
                line.height = height;
                var lineParameter = new Rect(line);
                var foldout = new Rect(lineParameter);
                foldout.width = 10;
                foldout.y -= 2.5f;
                //lineParameter.y -= 2;
                lineParameter.height = height + 2;
                // lineParameter.x -= 10;
                // lineParameter.width += 00;
                lineParameter.height += 5;
                //GUI.Label(line, label, EditorStyles.boldLabel);

                //line.y += height + 2;

                var styll = new GUIStyle(EditorStyles.toolbarTextField);
                styll.fontStyle = FontStyle.Bold;


                name.stringValue = GUI.TextField(lineParameter, name.stringValue, styll);

                if (property.isExpanded)
                {
                    line.y += height;
                    float Division = line.width / 2;
                    var lineSplitted = line;

                    EditorGUI.PropertyField(lineSplitted, LastState, new GUIContent("Last State", "Last State for the Jump"));
                    line.y += height + 2;
                    lineSplitted = line;

                    lineSplitted.width = Division + 20;


                    EditorGUI.PropertyField(lineSplitted, VerticalSpeed, new GUIContent("Min Vertical", "Minimal Vertical speed on the Animator to activate this profile"));

                    lineSplitted.x += Division + 42;
                    lineSplitted.width -= 62;

                    EditorGUIUtility.labelWidth = 65;
                    EditorGUI.PropertyField(lineSplitted, JumpLandDistance, new GUIContent("Jump Ray", "Ray Length to check if the ground is at the same level of the beginning of the jump and it allows to complete the Jump End Animation"));
                    EditorGUIUtility.labelWidth = 0;



                    ///NEW LINE
                    line.y += height + 2;
                    lineSplitted = line;

                    EditorGUI.PropertyField(lineSplitted, fallingTime,
                        new GUIContent("Fall Time", "Animation normalized time to change to fall animation if the ray checks if the animal is falling"));

                    ///NEW LINE
                    line.y += height + 2;
                    lineSplitted = line;

                    EditorGUI.PropertyField(lineSplitted, ExitTime);

                    if (ExitTime.floatValue < fallingTime.floatValue) ExitTime.floatValue = fallingTime.floatValue;

                    line.y += height + 2;
                    EditorGUI.PropertyField(line, CliffLandDistance);

                    if (CliffLandDistance.floatValue > 0)
                    {
                        line.y += height + 2;
                        EditorGUI.PropertyField(line, CliffTime);
                    }

                    line.y += height + 8;
                    EditorGUI.LabelField(line, "Multipliers", EditorStyles.boldLabel);
                    line.y += height + 2;
                    lineSplitted = line;


                    EditorGUI.PropertyField(lineSplitted, ForwardPressed);

                    ///NEW LINE
                    line.y += height + 2;
                    lineSplitted = line;

                    lineSplitted.width = Division + 30;

                    EditorGUI.PropertyField(lineSplitted, HeightMultiplier, new GUIContent("Height", "Height multiplier for the Jump. Default:1"));


                    lineSplitted.x += Division + 35;
                    lineSplitted.width -= 65;

                    EditorGUIUtility.labelWidth = 55;
                    EditorGUI.PropertyField(lineSplitted, ForwardMultiplier, new GUIContent("Forward", "Forward multiplier for the Jump. Default:1"));
                    EditorGUIUtility.labelWidth = 0;

                    lineSplitted.x += Division + 35;
                    lineSplitted.width -= 65;
                }

                EditorGUIUtility.labelWidth = 16;
                property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, GUIContent.none);
                EditorGUIUtility.labelWidth = 0;
                if (name.stringValue == string.Empty) name.stringValue = "NameHere";

                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return base.GetPropertyHeight(property, label);

            float lines = 11;

            var CliffLandDistance = property.FindPropertyRelative("CliffLandDistance");
            if (CliffLandDistance.floatValue == 0) lines--;

            return base.GetPropertyHeight(property, label) * lines + (2 * lines) - 8;
        }
    }
}