using UnityEngine;
using System;
using UnityEngine.Events;
 

namespace MalbersAnimations.Scriptables
{
    [Serializable] public class StatsEvent : UnityEvent<Stats> { }

    [CreateAssetMenu(menuName = "Malbers Animations/Collections/Runtime Stats Set", order = 1000, fileName = "New Runtime Stats Collection")]
    public class RuntimeStats : RuntimeCollection<Stats>
    {
        public StatsEvent OnItemAdded = new StatsEvent();
        public StatsEvent OnItemRemoved = new StatsEvent();

        //public Action<Stats,Stat> OnStatValueChange = delegate { };

        /// <summary>Return the Closest game object from an origin</summary>

        public Stats Item_GetClosest(Stats origin)
        {
            items.RemoveAll(x => x == null); //Remove all Assets that are Empty/ Type Mismatch error


            Stats closest = null;

            float minDistance = float.MaxValue;

            foreach (var item in items)
            {
                var Distance = Vector3.Distance(item.transform.position, origin.transform.position);

                if (Distance < minDistance)
                {
                    closest = item;
                    minDistance = Distance;
                }
            }
            return closest;
        }


        public void ItemAdd(Component newItem)
        {
            var s = newItem.FindComponent<Stats>();
            if (s) Item_Add(s);
        }

        public void Item_Add(GameObject newItem)
        {

            var s = newItem.FindComponent<Stats>();
            if (s)
                Item_Add(s);
        }
 

        protected override void OnAddEvent(Stats newItem) => OnItemAdded.Invoke(newItem);
        protected override void OnRemoveEvent(Stats newItem) => OnItemRemoved.Invoke(newItem);
         

        public void ItemRemove(Component newItem)
        {
            var s = newItem.FindComponent<Stats>();
            if (s) Item_Remove((Stats)s);
        }

        public void Item_Remove(GameObject newItem)
        {
            if (newItem)
            {
                var s = newItem.FindComponent<Stats>();
                if (s) Item_Remove(s);
            }
        }
    }



#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RuntimeStats))]
    public class RuntimeStatsEditor : RuntimeCollectionEditor<Stats> { }
#endif


    //#if UNITY_EDITOR
    //    [CustomEditor(typeof(RuntimeStats))]
    //    public class RuntimeStatsEditor : Editor
    //    {
    //        public override void OnInspectorGUI()
    //        {
    //            serializedObject.Update();
    //            var M = (RuntimeStats)target;

    //            if (Application.isPlaying)
    //            {
    //                MalbersEditor.DrawHeader(M.name + " - List");

    //                EditorGUI.BeginDisabledGroup(true);
    //                for (int i = 0; i < M.Items.Count; i++)
    //                {
    //                    EditorGUILayout.ObjectField("Item " + i, M.Items[i], typeof(Stats), false);
    //                }
    //                EditorGUI.EndDisabledGroup();
    //            }


    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnSetEmpty"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemAdded"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemRemoved"));

    //            if (!Application.isPlaying && M.Items != null &&  M.Items.Count > 0 && GUILayout.Button("Clear Set - " + M.Items.Count))
    //            {
    //                M.Clear();
    //                MTools.SetDirty(target);
    //                serializedObject.ApplyModifiedProperties();
    //            }

    //            serializedObject.ApplyModifiedProperties();
    //        }
    //    }
    //#endif
}

