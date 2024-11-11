using MalbersAnimations.Events;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace MalbersAnimations.Scriptables
{
    [AddComponentMenu("Malbers/Variables/String Comparer")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class StringComparer : StringVarListener
    {
        public List<AdvancedStringEvent> compare = new List<AdvancedStringEvent>();
      
        /// <summary>Set the first value on the comparer </summary>
        public string CompareFirst { get => compare[0].Value.Value; set => compare[0].Value.Value = value; }

        private AdvancedStringEvent Pin;

        public override string Value
        {
            set
            {
                base.Value = value; 
                if (Auto) Compare();
            }
        }


        public string this[int index]
        {
            get => compare[index].Value.Value;
            set => compare[index].Value.Value = value;
        }


        public void Pin_Comparer(int index) => Pin = compare[index];

        public void Pin_Comparer_SetValue(string value)
        {
            if (Pin != null) Pin.Value.Value = value;
        }

        public void Pin_Comparer_SetValue(StringVar value)
        {
            if (Pin != null) Pin.Value.Value = value;
        }


        void OnEnable()
        {
            if (value.Variable && Auto)
            {
                value.Variable.OnValueChanged += Compare;
            } 
        }

        void OnDisable()
        {
            if (value.Variable && Auto)
            {
                value.Variable.OnValueChanged -= Compare;
            }
        }


        /// <summary>Compares the Int parameter on this Component and if the condition is made then the event will be invoked</summary>
        public virtual void Compare()
        {
            foreach (var item in compare)
                item.ExecuteAdvanceStringEvent(value);
        }


        /// <summary>Compares an given int Value and if the condition is made then the event will be invoked</summary>
        public virtual void Compare(string value)
        {
            foreach (var item in compare)
                item.ExecuteAdvanceStringEvent(value);
        }

        /// <summary>Compares an given intVar Value and if the condition is made then the event will be invoked</summary>
        public virtual void Compare(StringReference value)
        {
            foreach (var item in compare)
                item.ExecuteAdvanceStringEvent(value.Value);
        }


        public void Index_Disable(int index) => compare[index].active = false;
        public void Index_Enable(int index) => compare[index].active = true;
    }


    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(StringComparer))]
    public class StringComparerEditor : IntCompareEditor { }
    
#endif
}