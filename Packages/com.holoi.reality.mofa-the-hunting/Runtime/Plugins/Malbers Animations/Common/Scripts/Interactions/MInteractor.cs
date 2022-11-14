using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Interaction/Interactor")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/global-components/interactor")]
    public class MInteractor : UnityUtils, IInteractor
    {
        [Tooltip("Layer for the Interact with colliders")]
        [SerializeField] private LayerReference Layer = new LayerReference(-1);
        [SerializeField] private QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.Ignore;


        [Tooltip("ID for the Interactor")]
        public IntReference m_ID = new IntReference(0);

        [Tooltip("Collider set as Trigger to Find Interactables OnTrigger Enter")]
        //[RequiredField] 
        public Collider InteractionArea;

        [Tooltip("When an Interaction is executed these events will be invoked." +
         "\n\nOnInteractWithGO(GameObject) -> will have the *INTERACTABLE* gameObject as parameter" +
         "\n\nOnInteractWith(Int) -> will have the *INTERACTABLE* ID as parameter")]
        public InteractionEvents events = new InteractionEvents();
        public GameObjectEvent OnFocused = new GameObjectEvent();
        public GameObjectEvent OnUnfocused = new GameObjectEvent();

        public int ID => m_ID.Value;

        public bool Enabled { get => !enabled; set => enabled = !value; }

        public GameObject Owner => gameObject;

        /// <summary>Current Interactable this interactor has on its Interaction Area </summary>
        public List<IInteractable> FocusedInteractables;


        /// <summary>Interaction Trigger Proxy to Subsribe to OnEnter OnExit Trigger</summary>
        public TriggerProxy Proxy { get; set; }

        

        private void OnEnable()
        {
            FocusedInteractables = new List<IInteractable>();

            Proxy = TriggerProxy.CheckTriggerProxy(InteractionArea, Layer, TriggerInteraction, transform.root);

            if (Proxy)
            {
                Proxy.OnTrigger_Enter.AddListener(TriggerEnter);
                Proxy.OnTrigger_Exit.AddListener(TriggerExit);
            }
        }

        private void OnDisable()
        {
            var focusCache = FocusedInteractables.ToArray(); //Cache in case the List changes (Crazy Error)

            foreach (var item in focusCache) UnFocus(item);
            

            FocusedInteractables = null;

            if (Proxy)
            {
                Proxy.OnTrigger_Enter.RemoveListener(TriggerEnter);
                Proxy.OnTrigger_Exit.RemoveListener(TriggerExit);
            }
        }

        private void TriggerEnter(Collider collider)
        {
            if (collider.isTrigger && TriggerInteraction == QueryTriggerInteraction.Ignore) return;    //Skip colliders

            var NewInteractables = collider.FindInterfaces<IInteractable>(); //Find all Interactables
 
            if (NewInteractables != null)
            foreach (var item in NewInteractables)
            {
                if (FocusedInteractables.Contains(item)) continue; //The new interactable its already there
                Focus(item);
            }
        }

        private void TriggerExit(Collider collider)
        {
            if (collider != null)
            {
                var NewInteractabless = collider.FindInterfaces<IInteractable>();

                if (NewInteractabless != null)
                    foreach (var item in NewInteractabless)
                    {
                        if (item != null && FocusedInteractables.Contains(item)) //means the interactor is exiting
                            UnFocus(item);
                    }
            }
        }


        public virtual void Focus(IInteractable item )
        {
            if (item != null && item.Active) //Ignore One Disable Interactors
            {
                item.CurrentInteractor = this;
                OnFocused.Invoke(item.Owner);
                item.Focused = true;
                FocusedInteractables.Add(item);
                if (item.Auto) Interact(item); //Interact if the interacter is on Auto
            }
        }

        public virtual void Focus(Component item)
        {
            if (item is IInteractable) Focus(item as IInteractable);
        }

        public virtual void Focus(GameObject item)
        {
            if (item != null) Focus(item.FindInterface<IInteractable>());
        }

        public void UnFocus(IInteractable item)
        {
            if (item != null)
            {
                OnUnfocused.Invoke(item.Owner);
                item.Focused = false;
                item.CurrentInteractor = null;
                FocusedInteractables.Remove(item);
            }
        }

       
        /// <summary> Receive an Interaction from the Interacter </summary>
        public bool Interact(IInteractable inter)
        {
            if (inter.Interact(this))
            {
                events.OnInteractWithGO.Invoke(inter.Owner);
                events.OnInteractWith.Invoke(inter.Index);
                return false;
            }

            return false;
        }


        public void Interact()
        {
            var focusCache = FocusedInteractables.ToArray(); //Cache in case the List changes (Crazy Error)
            foreach (var item in focusCache)
                Interact(item);
        }

        public void Restart()
        {
            FocusedInteractables = new List<IInteractable>();
            OnUnfocused.Invoke(null);
            OnFocused.Invoke(null);
        }

        public void Interact(GameObject interactable)
        {
            if (interactable)
            Interact(interactable.FindInterface<IInteractable>());
        }

        public void Interact(Component interactable)
        {
            if (interactable)
                Interact(interactable.FindInterface<IInteractable>());
        }

        [SerializeField] private int Editor_Tabs1;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MInteractor))]
    public class MInteractorEditor : UnityEditor.Editor
    {
        SerializedProperty m_ID, InteractionArea, events,  Editor_Tabs1, OnFocusedInteractable, OnUnfocusedInteractable,
            triggerInteraction, Layer;
        protected string[] Tabs1 = new string[] { "General", "Events" };

        MInteractor M;

        private void OnEnable()
        {
            M = (MInteractor)target;
            m_ID = serializedObject.FindProperty("m_ID");
            InteractionArea = serializedObject.FindProperty("InteractionArea");
            events = serializedObject.FindProperty("events");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            OnFocusedInteractable = serializedObject.FindProperty("OnFocused");
            OnUnfocusedInteractable = serializedObject.FindProperty("OnUnfocused");
            Layer = serializedObject.FindProperty("Layer");
            triggerInteraction = serializedObject.FindProperty("TriggerInteraction");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Interactor element that invoke events when interacts with an Interactable");
            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);
            if (Editor_Tabs1.intValue == 0) DrawGeneral();
            else DrawEvents();

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                if (M.FocusedInteractables != null)
                {
                    foreach (var item in M.FocusedInteractables)
                    {
                        EditorGUILayout.ObjectField($"Focused Item [ID:{item.Index }]", item.Owner, typeof(GameObject), false);
                    }
                }
                EditorGUI.EndDisabledGroup();

                Repaint();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneral()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(Layer);
            EditorGUILayout.PropertyField(triggerInteraction);
            EditorGUILayout.PropertyField(m_ID);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(InteractionArea);
            EditorGUILayout.EndVertical();
        }

        private void DrawEvents()
        {
            EditorGUILayout.PropertyField(events);
            
            if (events.isExpanded)
            {
                EditorGUILayout.PropertyField(OnFocusedInteractable);
                EditorGUILayout.PropertyField(OnUnfocusedInteractable);
            }
        }
    }
#endif
}