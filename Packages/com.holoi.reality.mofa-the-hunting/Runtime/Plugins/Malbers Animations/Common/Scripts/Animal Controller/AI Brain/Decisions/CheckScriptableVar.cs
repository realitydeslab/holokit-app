using MalbersAnimations.Scriptables;
using UnityEngine;


namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Check Scriptable Variable", order = 6)]
    public class CheckScriptableVar : MAIDecision
    {
        public override string DisplayName => "Variables/Check Scriptable Variable";

        [Tooltip("Check on the Target or Self if it has a Listener Variable Component <Int><Bool><Float> and compares it with the local variable)")]
        public VarType varType = VarType.Bool;


        [CreateScriptableAsset] public BoolVar Bool;
        [CreateScriptableAsset] public IntVar Int;
        [CreateScriptableAsset] public FloatVar Float;

        public ComparerInt compare;

        public bool boolValue = true;
        public int intValue = 0;
        public float floatValue = 0f; 

        public override bool Decide(MAnimalBrain brain, int Index)
        {
            switch (varType)
            {
                case VarType.Bool:
                    return Bool != null && Bool.Value == boolValue;
                case VarType.Int:
                    return Int != null && CompareInteger(Int.Value);
                case VarType.Float:
                    return Float != null && CompareFloat(Float.Value);
                default:
                    return false;
            }
        }


        public enum VarType { Bool, Int, Float }
        public enum BoolType { True, False }

        public bool CompareInteger(int IntValue)
        {
            switch (compare)
            {
                case ComparerInt.Equal:
                    return (IntValue == intValue);
                case ComparerInt.Greater:
                    return (IntValue > intValue);
                case ComparerInt.Less:
                    return (IntValue < intValue);
                case ComparerInt.NotEqual:
                    return (IntValue != intValue);
                default:
                    return false;
            }
        }
        public bool CompareFloat(float IntValue)
        {
            switch (compare)
            {
                case ComparerInt.Equal:
                    return (IntValue == floatValue);
                case ComparerInt.Greater:
                    return (IntValue > floatValue);
                case ComparerInt.Less:
                    return (IntValue < floatValue);
                case ComparerInt.NotEqual:
                    return (IntValue != floatValue);
                default:
                    return false;
            }
        }

#if UNITY_EDITOR

        [UnityEditor.CustomEditor(typeof(CheckScriptableVar)), UnityEditor.CanEditMultipleObjects]
        public class CheckScriptableVarEditor : MAIDecisionEditor
        {
            protected UnityEditor.SerializedProperty varType, Bool, Float, Int, boolValue, intValue, floatValue, compare;
            protected override void OnEnable()
            {
                base.OnEnable();

                varType = serializedObject.FindProperty("varType");
                Bool = serializedObject.FindProperty("Bool");
                Float = serializedObject.FindProperty("Float");
                Int = serializedObject.FindProperty("Int");

                intValue = serializedObject.FindProperty("intValue");
                floatValue = serializedObject.FindProperty("floatValue");
                boolValue = serializedObject.FindProperty("boolValue");
                compare = serializedObject.FindProperty("compare");
            }


            public override void DecisionParameters()
            {
                //UnityEditor.EditorGUILayout.LabelField("Check Variable", UnityEditor.EditorStyles.boldLabel);


                UnityEditor.EditorGUILayout.PropertyField(varType, new GUIContent("Check Variable"));
                UnityEditor.EditorGUILayout.BeginHorizontal();

                var LBW = UnityEditor.EditorGUIUtility.labelWidth;

                switch ((VarType)varType.intValue)
                {
                    case VarType.Bool:
                        UnityEditor.EditorGUILayout.PropertyField(Bool, GUIContent.none, GUILayout.Width(LBW));
                      
                        var Ct = new GUIContent(boolValue.boolValue ? "Is True" : "Is False");
                        // UnityEditor.EditorGUILayout.LabelField(Ct, UnityEditor.EditorStyles.miniButton, GUILayout.MinWidth(50));
                        boolValue.boolValue =  GUILayout.Toggle(boolValue.boolValue, Ct, UnityEditor.EditorStyles.miniButton);
                        break;  
                    case VarType.Int:
                        UnityEditor.EditorGUILayout.PropertyField(Int, GUIContent.none, GUILayout.Width(LBW));
                        UnityEditor.EditorGUILayout.PropertyField(compare, GUIContent.none, GUILayout.MinWidth(70));
                        UnityEditor.EditorGUILayout.PropertyField(intValue, GUIContent.none, GUILayout.MinWidth(20));

                        break;
                    case VarType.Float:
                        UnityEditor.EditorGUILayout.PropertyField(Float, GUIContent.none, GUILayout.Width(LBW));
                        UnityEditor.EditorGUILayout.PropertyField(compare, GUIContent.none, GUILayout.MinWidth(70));
                        UnityEditor.EditorGUILayout.PropertyField(floatValue, GUIContent.none, GUILayout.MinWidth(20));
                        break;
                    default:
                        break;
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
        }


#endif
    }
}
