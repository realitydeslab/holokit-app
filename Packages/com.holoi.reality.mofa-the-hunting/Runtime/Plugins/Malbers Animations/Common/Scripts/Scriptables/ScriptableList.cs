using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    public abstract class ScriptableList<T> : ScriptableObject where T : Object
    {
        [TextArea(3, 20)]
        public string Description = "Store a List of Objects";
        
        [SerializeField] 
        private List<T> items = new List<T>();

        /// <summary>Ammount of object on the list</summary>
        public int Count => items.Count;

        public List<T> Items { get => items; set => items = value; }

        /// <summary>Gets a rando first object of the list</summary>
        public virtual T Item_GetRandom()
        {
            if (items != null && items.Count > 0)
            {
                return items[Random.Range(0, items.Count)];
            }
            return default;
        }

        /// <summary>Gets an object on the list by an index </summary>
        public virtual T Item_Get(int index) => items[index % items.Count];

        /// <summary>Gets the first object of the list</summary>
        public virtual T Item_GetFirst() => items[0];

        public virtual T Item_Get(string name) => items.Find(x => x.name == name);
    }
}