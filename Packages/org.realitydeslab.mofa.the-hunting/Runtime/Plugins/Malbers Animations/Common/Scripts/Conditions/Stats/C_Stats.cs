using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using static MalbersAnimations.Conditions.C_Stats;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_Stats : MCondition
    {
        public enum StatCondition { HasStat, Enabled, Full, Empty, Regenerating, Degenerating, Inmune, Value, ValueNormalized, MaxValue, MinValue }

        public override string DisplayName => "General/Stats";

        [RequiredField] public Stats Target;
        public StatID ID;
        public StatCondition Condition;
        public ComparerInt Compare;
        public FloatReference Value;
        private Stat st;

        public void _SetTarget(Stats targ) => Target = targ;
        public void _SetID(StatID targ) => ID = targ;
        public void _SetValue(FloatVar targ) => Value.Value = targ;
        public void _SetValue(float targ) => Value.Value = targ;

        private void OnEnable()
        {
            if (Target)  st = Target.Stat_Get(ID);
        }

        public override void SetTarget(Object target)
        {
            if (target is Stats) this.Target = target as Stats;
        }

        public override bool _Evaluate()
        {
            if (Target)
            {
                if (st != null)
                {
                    switch (Condition)
                    {
                        case StatCondition.Enabled: return st.Active;
                        case StatCondition.Regenerating: return st.IsRegenerating;
                        case StatCondition.Degenerating: return st.IsDegenerating;
                        case StatCondition.Inmune: return st.IsInmune;
                        case StatCondition.Value: return st.Value.CompareFloat(Value.Value, Compare);
                        case StatCondition.ValueNormalized: return st.NormalizedValue.CompareFloat(Value.Value, Compare);
                        case StatCondition.Full:return st.IsFull;
                        case StatCondition.Empty: return st.IsEmpty;
                        case StatCondition.MaxValue: return st.MaxValue.CompareFloat(Value.Value, Compare);
                        case StatCondition.MinValue:   return st.MinValue.CompareFloat(Value.Value, Compare);
                    }
                }
                else
                {
                    if (Condition == StatCondition.HasStat) return false;
                }

            }
            return false;
        }

        private void Reset() => Name = "New Stat Comparer";
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(C_Stats))]
    public class C_StatsEditor : MConditionEditor
    {
        SerializedProperty ID,Compare;

        protected override void OnEnable()
        {
            base.OnEnable();
            ID = so.FindProperty("ID");
            Compare = so.FindProperty("Compare");
        }

        public override void CustomInspector()
        {
            EditorGUILayout.PropertyField(ID);

            var c =  Condition.intValue;

            if (c >= 7)
            {
                EditorGUILayout.PropertyField(Compare);
                EditorGUILayout.PropertyField(Value);
            }    
        }
    }
#endif
}
