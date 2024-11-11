using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/Object Comparer")]
    public class ObjectComparer : MonoBehaviour
    {
        [Tooltip("The Events will be invoked when the Listener Value changes.\nIf is set to false, call Invoke() to invoke the events manually")]
        public bool Auto = true;
        public Object value;
        public List<ObjectsComp> compare = new List<ObjectsComp> ();
        public Object Value
        {
            set
            {
                this.value = value;

                if (Auto) Invoke();
            }
            get => value;
        }

        public Object this[int index]
        {
            get => compare[index].CompareTo;
            set => compare[index].CompareTo = value;
        }

        private void OnEnable()
        {
            if (Auto) Invoke ();    
        }

        /// <summary>Check if an Object is equal to another </summary>
        public virtual void Invoke()
        {
            foreach (var o in compare)
            {
                o.Invoke(Value);
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < compare.Count; i++)
            {
                var O = compare[i];

                O.elementName = O.CompareTo == null ? $" [{i}] Is Object [Null] ?" : $" [{i}] Is Object equal to [{O.CompareTo.name}] ?";
            }

             
        }
    }



    [System.Serializable]
    public class ObjectsComp
    {
        [HideInInspector] public string elementName;
        public Object CompareTo;
        public UnityEvent Then = new UnityEvent();
        public UnityEvent Else = new UnityEvent();


        /// <summary> Used to use turn Objects to True or false </summary>
        public virtual void Invoke(Object value)
        { 
            Response(value == CompareTo);
        }

        private void Response(bool value)
        {
            if (value) Then.Invoke(); else Else.Invoke();
        }
    }
}