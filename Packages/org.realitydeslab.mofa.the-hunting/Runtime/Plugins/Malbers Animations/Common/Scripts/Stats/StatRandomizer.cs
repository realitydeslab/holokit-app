using System.Collections;
using UnityEngine;
using static MalbersAnimations.Controller.Reactions.StatRandomizer;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif


namespace MalbersAnimations.Controller.Reactions
{
    /// <summary> Reaction Script for Making the Animal do something </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Modifier/Stat Randomizer", fileName = "New Stat Randomizer", order = -100)]
    public class StatRandomizer : ScriptableObject
    {
        public enum StatValues
        {
            Value = 1,
            Multiplier = 2,
            MinValue = 4,
            MaxValue = 8,
            RegenerationRate = 16,
            RegenerationWaitTime = 32,
            DegenerationRate = 64,
            DegenerationWaitTime = 128,
            InmuneTime = 256
        };

        public StatID statID;
        [Utilities.Flag]
        public StatValues modify;

        [Tooltip("Current Value of the Stat")]
        public RangedFloat Value = new RangedFloat(80,120);
        [Tooltip("Multipler that is applied to the Stat Value")]
        public RangedFloat Multiplier = new RangedFloat(0.5f, 1.5f);
        [Tooltip("Minimum Stat Value")]
        public RangedFloat MinValue;
        [Tooltip("Maximum Stat Value")]
        public RangedFloat MaxValue = new RangedFloat(100, 200);
        [Tooltip("Regeneration Rate")]
        public RangedFloat RegenRate = new RangedFloat(0, 10);
        [Tooltip("Regeneration Rate wait time")]
        public RangedFloat RegenWaitTime = new RangedFloat(0, 10);
        [Tooltip("Degeneration Rate")]
        public RangedFloat DegenRate = new RangedFloat(0, 10);
        [Tooltip("Degeneration Rate wait time")]
        public RangedFloat DegenWaitTime = new RangedFloat(0, 10);
        [Tooltip("Inmune time, uses to avoid fast changes to the Stat value")]
        public RangedFloat InmuneTime = new RangedFloat(0, 5);

        public void Randomize(Stats stats)
        {
            Stat s = stats.Stat_Get(this.statID);

            if (s != null)
            {
                if (Check(StatValues.Value)) s.SetValue(Value.RandomValue);
                if (Check(StatValues.Multiplier)) s.Multiplier = (Multiplier.RandomValue);
                if (Check(StatValues.MinValue)) s.MinValue = (MinValue.RandomValue);
                if (Check(StatValues.MaxValue)) s.MaxValue = (MaxValue.RandomValue);
                if (Check(StatValues.RegenerationRate)) s.RegenRate = (RegenRate.RandomValue);
                if (Check(StatValues.RegenerationWaitTime)) s.RegenWaitTime = (RegenWaitTime.RandomValue);
                if (Check(StatValues.DegenerationRate)) s.DegenRate = (DegenRate.RandomValue);
                if (Check(StatValues.DegenerationWaitTime)) s.DegenWaitTime = (DegenWaitTime.RandomValue);
                if (Check(StatValues.InmuneTime)) s.InmuneTime = (InmuneTime.RandomValue);
            }
        }

        private bool Check(StatValues modifier) => ((modify & modifier) == modifier);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(StatRandomizer))]
    public class StatRandomizerEditor : Editor
    {
        SerializedProperty statID, modify, Value, Multiplier, MinValue, MaxValue, RegenRate, RegenWaitTime, DegenRate, DegenWaitTime, InmuneTime;

        private void OnEnable()
        {
            statID = serializedObject.FindProperty("statID");
            modify = serializedObject.FindProperty("modify");
            Value = serializedObject.FindProperty("Value");
            Multiplier = serializedObject.FindProperty("Multiplier");
            MinValue = serializedObject.FindProperty("MinValue");
            MaxValue = serializedObject.FindProperty("MaxValue");
            RegenRate = serializedObject.FindProperty("RegenRate");
            RegenWaitTime = serializedObject.FindProperty("RegenWaitTime");
            DegenRate = serializedObject.FindProperty("DegenRate");
            DegenWaitTime = serializedObject.FindProperty("DegenWaitTime");
            InmuneTime = serializedObject.FindProperty("InmuneTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(statID);
            EditorGUILayout.PropertyField(modify);

            var mod = modify.intValue;

            if (Check(mod,StatValues.Value)) EditorGUILayout.PropertyField(Value);
            if (Check(mod, StatValues.Multiplier)) EditorGUILayout.PropertyField(Multiplier);
            if (Check(mod, StatValues.MinValue)) EditorGUILayout.PropertyField(MinValue);
            if (Check(mod, StatValues.MaxValue)) EditorGUILayout.PropertyField(MaxValue);
            if (Check(mod, StatValues.RegenerationRate)) EditorGUILayout.PropertyField(RegenRate);
            if (Check(mod, StatValues.RegenerationWaitTime)) EditorGUILayout.PropertyField(RegenWaitTime);
            if (Check(mod, StatValues.DegenerationRate)) EditorGUILayout.PropertyField(DegenRate);
            if (Check(mod, StatValues.DegenerationWaitTime)) EditorGUILayout.PropertyField(DegenWaitTime);
            if (Check(mod, StatValues.InmuneTime)) EditorGUILayout.PropertyField(InmuneTime);

            serializedObject.ApplyModifiedProperties();
        }

        private bool Check(int modify, StatValues modifier) => ((modify & (int)modifier) == (int)modifier);
    }
#endif
}