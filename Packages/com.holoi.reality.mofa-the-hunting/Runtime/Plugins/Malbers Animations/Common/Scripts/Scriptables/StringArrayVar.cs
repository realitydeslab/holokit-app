using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Scriptables
{
    ///<summary>String Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple</summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Variables/String Array", order = 1000)]
    public class StringArrayVar : StringVar
    {
        [SerializeField]  private IntReference index = new IntReference(-1);
        [SerializeField]  private List<string> array = new List<string>();

        /// <summary>Value of the String Scriptable variable</summary>
        public override string Value
        {
            get
            {
                if (array != null && array.Count > 0)
                {
                    if (index == -1) //means its a random value
                    {
                        return array[UnityEngine.Random.Range(0, array.Count)];
                    }
                    else
                    {

                        return array[index % array.Count];
                    }
                }
                return string.Empty;
            }

            set
            {
                if (array != null && array.Count > 0 && index != -1)
                {
                    array[Index] = value;
                }
            }
        }

        public int Index { get => index.Value; set => index.Value = value; }

    }

#if UNITY_EDITOR
    [CanEditMultipleObjects, CustomEditor(typeof(StringArrayVar))]
    public class StringArrayVarEditor : Editor
    {
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        protected SerializedProperty value, Description, debug, index, array;

        private void OnEnable()
        {
            value = serializedObject.FindProperty("value");
            Description = serializedObject.FindProperty("Description");
            debug = serializedObject.FindProperty("debug");
            array = serializedObject.FindProperty("array");
            index = serializedObject.FindProperty("index");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Scriptable String Array");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //    EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(Description);
         //   MalbersEditor.DrawDebugIcon(debug);
         //   EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(index);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(array,true);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
