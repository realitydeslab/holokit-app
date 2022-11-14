using UnityEngine; 
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MalbersAnimations
{
    public abstract class VarListener : MonoBehaviour
    {
        [HideInInspector] public bool ShowEvents = false;
        
        [Tooltip("ID value is used on the AI Brain to know which Var Listener is picked, in case there more than one on one Game Object")]
        public IntReference ID;

        public bool Enable => gameObject.activeInHierarchy && enabled;

        [Tooltip("The Events will be invoked when the Listener Value changes.\nIf is set to false, call Invoke() to invoke the events manually")]
        public bool Auto = true;
        [Tooltip("Invokes the current value on Enable")]
        public bool InvokeOnEnable = true;

        public string Description = "";
        [HideInInspector] public bool ShowDescription = false;
        [ContextMenu("Show Description")]
        internal void EditDescription() => ShowDescription ^= true;

        public bool debug = false;
    }

    [System.Serializable]
    public class AdvancedIntegerEvent
    {
        public bool active = true;
        public string name;
        public string description;
        public ComparerInt comparer = ComparerInt.Equal;
        public IntReference Value = new IntReference();
        public IntEvent Response = new IntEvent();

        /// <summary>Use the comparer to execute a response using the Int Event and the Value</summary>
        /// <param name="IntValue">Value that comes from the IntEvent</param>
        public void ExecuteAdvanceIntegerEvent(int IntValue)
        {
            if (active)
            {
                switch (comparer)
                {
                    case ComparerInt.Equal:
                        if (IntValue == Value) Response.Invoke(IntValue);
                        break;
                    case ComparerInt.Greater:
                        if (IntValue > Value) Response.Invoke(IntValue);
                        break;
                    case ComparerInt.Less:
                        if (IntValue < Value) Response.Invoke(IntValue);
                        break;
                    case ComparerInt.NotEqual:
                        if (IntValue != Value) Response.Invoke(IntValue);
                        break;
                    default:
                        break;
                }
            }
        }

        public AdvancedIntegerEvent()
        {
            active = true;
            name = "NameHere";
            description = "";
            comparer = ComparerInt.Equal;
            Value = new IntReference();
            Response = new IntEvent();
        }
    }

    [System.Serializable]
    public class AdvancedFloatEvent
    {
        public bool active = true;
        public string name;
        public string description;
        public ComparerInt comparer = ComparerInt.Equal;
        public FloatReference Value = new FloatReference();
        public FloatEvent Response = new FloatEvent();

        /// <summary>Use the comparer to execute a response using the Int Event and the Value</summary>
        /// <param name="v">Value that comes from the IntEvent</param>
        public void ExecuteAdvanceFloatEvent(float v)
        {
            if (active)
            {
                switch (comparer)
                {
                    case ComparerInt.Equal:
                        if (v == Value) Response.Invoke(v);
                        break;
                    case ComparerInt.Greater:
                        if (v > Value) Response.Invoke(v);
                        break;
                    case ComparerInt.Less:
                        if (v < Value) Response.Invoke(v);
                        break;
                    case ComparerInt.NotEqual:
                        if (v != Value) Response.Invoke(v);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class AdvancedBoolEvent
    {
        public bool active = true;
        public string name;
        public ComparerBool comparer = ComparerBool.Equal;
        public BoolReference Value = new BoolReference();
        public UnityEvent Response = new UnityEvent();

        /// <summary>Use the comparer to execute a response using the Int Event and the Value</summary>
        /// <param name="boolValue">Value that comes from the IntEvent</param>
        public void ExecuteAdvanceBoolEvent(bool boolValue)
        {
            if (active)
            {
                switch (comparer)
                {
                    case ComparerBool.Equal:
                        if (boolValue == Value) Response.Invoke();
                        break;
                    case ComparerBool.NotEqual:
                        if (boolValue != Value) Response.Invoke();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class AdvancedStringEvent
    {
        public bool active = true;
        public string name;
        public string description;
        public ComparerString comparer = ComparerString.Equal;
        public StringReference Value = new StringReference();
        public StringEvent Response = new StringEvent();

        /// <summary>Use the comparer to execute a response using the Int Event and the Value</summary>
        /// <param name="val">Value that comes from the string event</param>
        public void ExecuteAdvanceStringEvent(string val)
        {
            if (active)
            {
                switch (comparer)
                {
                    case ComparerString.Equal:
                        if (val == Value.Value) Response.Invoke(val);
                        break;
                    case ComparerString.NotEqual:
                        if (val != Value.Value) Response.Invoke(val);
                        break;
                    case ComparerString.Empty:
                        if (string.IsNullOrEmpty(val)) Response.Invoke(val);
                        break;
                    default:
                        break;
                }
            }
        }
    }


    //INSPECTOR
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AdvancedIntegerEvent))]
    [CustomPropertyDrawer(typeof(AdvancedFloatEvent))]
    public class AdvIntegerComparerDrawer : PropertyDrawer
    {
        //const float labelwith = 27f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += 2;

            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var height = EditorGUIUtility.singleLineHeight;
            var name = property.FindPropertyRelative("name");
            var comparer = property.FindPropertyRelative("comparer");
            var Value = property.FindPropertyRelative("Value");
            var Response = property.FindPropertyRelative("Response");

            //bool isExpanded = property.isExpanded;


            if (name.stringValue == string.Empty) name.stringValue = "NameHere";

            var line = position;
            line.height = height;

            line.x += 4;
            line.width -= 8;

            var foldout = line;
            foldout.width = 10;
            foldout.x += 10;

            EditorGUIUtility.labelWidth = 16;
            property.isExpanded = EditorGUI.Foldout(foldout, property.isExpanded, GUIContent.none);
            EditorGUIUtility.labelWidth = 0;

            var rectName = line;

            rectName.x += 10;
            rectName.width -= 10;

            name.stringValue = GUI.TextField(rectName, name.stringValue, EditorStyles.boldLabel);

            line.y += height + 2;

            if (property.isExpanded)
            {
                var ComparerRect = new Rect(line.x, line.y, line.width / 2 - 10, height);
                var ValueRect = new Rect(line.x + line.width / 2 + 15, line.y, line.width / 2 - 10, height);

                EditorGUI.PropertyField(ComparerRect, comparer, GUIContent.none);
                EditorGUI.PropertyField(ValueRect, Value, GUIContent.none);
                line.y += height + 2;
                EditorGUI.PropertyField(line, Response);
                position.height = line.height;
            }
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return base.GetPropertyHeight(property, label);

            var Response = property.FindPropertyRelative("Response");
            float ResponseHeight = EditorGUI.GetPropertyHeight(Response);

            return 16 * 2 + ResponseHeight + 10;
        }
    }
     

    [CustomEditor(typeof(VarListener))]
    public class VarListenerEditor : UnityEditor.Editor
    {
        protected UnityEditor.SerializedProperty value, Description, Index, ShowEvents, ShowDescription, Debug, InvokeOnEnable, Auto;
        protected GUIStyle style, styleDesc;

        void OnEnable()    { SetEnable(); }

        protected void SetEnable()
        {
            value = serializedObject.FindProperty("value");
            Description = serializedObject.FindProperty("Description");
            ShowDescription = serializedObject.FindProperty("ShowDescription");
            Index = serializedObject.FindProperty("ID");
            ShowEvents = serializedObject.FindProperty("ShowEvents");
            Debug = serializedObject.FindProperty("debug");
            Auto = serializedObject.FindProperty("Auto");
            InvokeOnEnable = serializedObject.FindProperty("InvokeOnEnable");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (ShowDescription.boolValue)
            {
                if (ShowDescription.boolValue)
                {
                    if (style == null)
                        style = new GUIStyle(MTools.StyleBlue)
                        {
                            fontSize = 12,
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleLeft,
                            stretchWidth = true
                        };

                    style.normal.textColor = UnityEditor.EditorStyles.boldLabel.normal.textColor;

                    Description.stringValue = UnityEditor.EditorGUILayout.TextArea(Description.stringValue, style);
                }
            }


            UnityEditor.EditorGUILayout.BeginHorizontal(UnityEditor.EditorStyles.helpBox);
            UnityEditor.EditorGUIUtility.labelWidth = 55;
            UnityEditor.EditorGUILayout.PropertyField(value, GUILayout.MinWidth(25));
            UnityEditor.EditorGUIUtility.labelWidth = 40;
            UnityEditor.EditorGUILayout.PropertyField(Index, new GUIContent("    ID"),  GUILayout.MinWidth(15));
            UnityEditor.EditorGUIUtility.labelWidth = 0;
            ShowEvents.boolValue = GUILayout.Toggle(ShowEvents.boolValue, new GUIContent("E", "Show Events"), UnityEditor.EditorStyles.miniButton, GUILayout.Width(22));
            UnityEditor.EditorGUILayout.EndHorizontal();

            if (ShowEvents.boolValue)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUIUtility.labelWidth = 55;
                UnityEditor.EditorGUILayout.PropertyField(Auto);
                 UnityEditor.EditorGUIUtility.labelWidth = 65;
                UnityEditor.EditorGUILayout.PropertyField(InvokeOnEnable,new GUIContent("On Enable"));
                UnityEditor.EditorGUIUtility.labelWidth = 0;
                MalbersEditor.DrawDebugIcon(Debug);
                //Debug.boolValue = GUILayout.Toggle(Debug.boolValue, new GUIContent("D"), UnityEditor.EditorStyles.miniButton, GUILayout.Width(22));
                UnityEditor.EditorGUILayout.EndHorizontal();

                DrawEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void DrawEvents()  {
          
        }
    }
#endif
}