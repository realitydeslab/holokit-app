using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Scriptables
{
    public abstract class RuntimeCollection<T> : ScriptableObject where T : Object
    {
        public List<T> items = new List<T>();

       // [TextArea(4,5)]
        public string Description;

        public UnityEvent OnSetEmpty = new UnityEvent();

        /// <summary>Ammount of object on the list</summary>
        public int Count => items.Count;

        public List<T> Items { get => items; set => items = value; }


        public T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }


        /// <summary> Clears the list of objects </summary>
        public virtual void Clear()
        {
            items = new List<T>();
            OnSetEmpty.Invoke();
        }

        /// <summary>Gets an object on the list by an index </summary>
        public virtual T Item_Get(int index) => items[index % items.Count];

        /// <summary>Gets the first object of the list</summary>
        public virtual T Item_GetFirst() => items[0];

        public virtual T Item_Get(string name) => items.Find(x => x.name == name);

        /// <summary>Gets the Index on the list of an object</summary>
        public virtual int Item_Index(T obj) => items.IndexOf(obj);

       
        /// <summary>Gets a rando first object of the list</summary>
        public virtual T Item_GetRandom()
        {
            if (items != null && items.Count > 0)
            {  
                return items[Random.Range(0,items.Count)];
            }
            return default;
        }

        public virtual void Item_Add(T newItem)
        {
            if (newItem != null)
            {
                items.RemoveAll(x => x == null); //Remove all Assets that are Empty/ Type Mismatch error

                if (!items.Contains(newItem))
                {
                    items.Add(newItem);
                    OnAddEvent(newItem);
                }
            }
        }  
        
        public virtual void Item_Remove(T newItem)
        {
            if (newItem != null)
            {
                items.RemoveAll(x => x == null); //Remove all Assets that are Empty/ Type Mismatch error

                if (items.Contains(newItem))
                {
                    OnRemoveEvent(newItem);
                    items.Remove(newItem);
                }
            }

            if (items == null || items.Count == 0)
                Clear();
        }

        /// <summary> Override this method to add your OnAdd Event here </summary>
        protected virtual void OnAddEvent(T newItem) { }
        /// <summary> Override this method to add your OnRemove Event here </summary>
        protected virtual void OnRemoveEvent(T newItem) { }
    }




#if UNITY_EDITOR
    //[CustomEditor(typeof(RuntimeCollection<>),true)]
    public abstract class RuntimeCollectionEditor<T> : Editor where T:Object
    {
        RuntimeCollection<T> M;

        SerializedProperty Description, OnSetEmpty, OnItemAdded, OnItemRemoved;

        private void OnEnable()
        { 
            M = (RuntimeCollection<T>)target;

            Description = serializedObject.FindProperty("Description");
            OnSetEmpty = serializedObject.FindProperty("OnSetEmpty");
            OnItemAdded = serializedObject.FindProperty("OnItemAdded");
            OnItemRemoved = serializedObject.FindProperty("OnItemRemoved");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                MalbersEditor.DrawHeader(M.name + " - List");

                EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < M.Items.Count; i++)
                {
                    EditorGUILayout.ObjectField("Item " + i, M.Items[i], typeof(T), false);
                }
                EditorGUI.EndDisabledGroup();
            }

            Description.stringValue =  EditorGUILayout.TextArea(Description.stringValue, GUILayout.MinHeight(16*3));
            EditorGUILayout.PropertyField(OnSetEmpty);
            if (OnItemAdded != null)  EditorGUILayout.PropertyField(OnItemAdded);
            if (OnItemRemoved != null) EditorGUILayout.PropertyField(OnItemRemoved);

            if (!Application.isPlaying && M.Items != null && M.Items.Count > 0 && GUILayout.Button("Clear Set - " + M.Items.Count))
            {
                M.Clear();
                MTools.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}