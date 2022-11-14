using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Conditions
{
    public enum GOCondition { ActiveInHierarchy, ActiveSelf, Null, Equal, Prefab, Name,  Layer, Tag, MalbersTag}

    [System.Serializable]
    public class C_GameObject : MCondition
    {
        public override string DisplayName => "Unity/GameObject";

        public GameObjectReference Target;
        public GOCondition Condition;
        public GameObjectReference Value;
        public StringReference checkName;
        public LayerReference Layer;
        public Tag[] tags;

        public override bool _Evaluate()
        {
            if (Condition == GOCondition.Null) return Target.Value == null;

            if (Target.Value)
            {
                switch (Condition)
                {
                    case GOCondition.Name:              return Target.Value.name.Contains(checkName);
                    case GOCondition.Prefab:            return Value.Value.IsPrefab();
                    case GOCondition.ActiveInHierarchy: return Value.Value.activeInHierarchy;
                    case GOCondition.ActiveSelf:        return Value.Value.activeSelf;
                    case GOCondition.Equal:             return Value.Value == Target.Value;
                    case GOCondition.Layer:             return MTools.Layer_in_LayerMask(Value.Value.layer, Layer.Value);
                    case GOCondition.Tag:               return Value.Value.CompareTag(checkName);
                    case GOCondition.MalbersTag:        return Value.Value.HasMalbersTag(tags);
                    default: return false;
                }
            }
            return false;
        }

        public override void SetTarget(Object target)
        {
            if (target is GameObject) this.Target.Value = target as GameObject;
        }

        private void Reset() => Name = "New GameObject Condition";

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(C_GameObject))]
    public class C_GameObjectEditor : MConditionEditor
    {
        SerializedProperty checkName, Layer, tags;

        protected override void OnEnable()
        {
            base.OnEnable();
            checkName = so.FindProperty("checkName");
            Layer = so.FindProperty("Layer");
            tags = so.FindProperty("tags");
        }

        public override void CustomInspector()
        {
            var c = (GOCondition)Condition.intValue;


            if (c == GOCondition.Equal || c == GOCondition.Prefab)

                EditorGUILayout.PropertyField(Value);

            else if (c == GOCondition.Name || c == GOCondition.Tag)

                EditorGUILayout.PropertyField(checkName, new GUIContent(c.ToString()));
            else if (c == GOCondition.Layer)
                EditorGUILayout.PropertyField(Layer);
            else if (c == GOCondition.MalbersTag)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(tags, true);
                EditorGUI.indentLevel--;
            }
        }
    }
#endif
}
