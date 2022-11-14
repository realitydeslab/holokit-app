using UnityEngine;
using MalbersAnimations.Events; 

namespace MalbersAnimations.Scriptables
{
    [CreateAssetMenu(menuName = "Malbers Animations/Collections/Runtime GameObject Set", order = 1000, fileName = "New Runtime Gameobject Collection")]
    public class RuntimeGameObjects : RuntimeCollection<GameObject>
    {
        public GameObjectEvent OnItemAdded = new GameObjectEvent();
        public GameObjectEvent OnItemRemoved = new GameObjectEvent();

        /// <summary>Return the Closest game object from an origin</summary>
        public GameObject Item_GetClosest(GameObject origin)
        {
            GameObject closest = null;

            items.RemoveAll(x => x == null); //Remove all Assets that are Empty/Type Mismatch error

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

        protected override void OnAddEvent(GameObject newItem) => OnItemAdded.Invoke(newItem);
        protected override void OnRemoveEvent(GameObject newItem) => OnItemRemoved.Invoke(newItem); 
        
        public  void Item_Add(Component newItem) => Item_Add(newItem.gameObject);
        public  void Item_Remove(Component newItem) => Item_Remove(newItem.gameObject);
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RuntimeGameObjects))]
    public class RuntimeGameObjectsEditor : RuntimeCollectionEditor<GameObject> { }
#endif



    //#if UNITY_EDITOR
    //    [CustomEditor(typeof(RuntimeGameObjects))]
    //    public class RuntimeGameObjectsEditor : Editor
    //    {
    //        public override void OnInspectorGUI()
    //        {
    //            serializedObject.Update();
    //            var M = (RuntimeGameObjects)target;

    //            if (Application.isPlaying)
    //            {
    //                MalbersEditor.DrawHeader(M.name + " - List");

    //                EditorGUI.BeginDisabledGroup(true);
    //                for (int i = 0; i < M.Items.Count; i++)
    //                {
    //                    EditorGUILayout.ObjectField("Item " + i, M.Items[i], typeof(GameObject), false);
    //                }
    //                EditorGUI.EndDisabledGroup();
    //            }


    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnSetEmpty"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemAdded"));
    //            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemRemoved"));

    //            if (!Application.isPlaying && M.Items != null &&  M.Items.Count > 0 && GUILayout.Button("Clear Set - " + M.Items.Count))
    //            {
    //                M.Items = new System.Collections.Generic.List<GameObject>();
    //                MTools.SetDirty(target);
    //                serializedObject.ApplyModifiedProperties();
    //            }

    //            serializedObject.ApplyModifiedProperties();
    //        }
    //    }
    //#endif
}

