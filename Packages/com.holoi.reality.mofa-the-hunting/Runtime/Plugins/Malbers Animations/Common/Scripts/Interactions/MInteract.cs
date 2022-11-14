using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    [DefaultExecutionOrder(15)]
    [SelectionBase]
    [AddComponentMenu("Malbers/Interaction/Interactable")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/global-components/interactable")]
    public class MInteract : UnityUtils, IInteractable
    {
        [Tooltip("Own Index. This is used to Identify each Interactable. (Used on UnityEvents)")]
       //[UnityEngine.Serialization.FormerlySerializedAs("m_ID")]
        public IntReference m_ID = new IntReference(0);
        [Tooltip("ID for the Interactor. Makes this Interactable to interact only with Interactors with this ID Value\n" +
            "By default its -1, which means that can be activated by anyone")]
        [UnityEngine.Serialization.FormerlySerializedAs("m_InteracterID")]
        public IntReference m_InteractorID = new IntReference(-1);

        [Tooltip("If the Interactor has this Interactable focused, it will interact with it automatically.\n" +
            "It also is used by the AI Animals. If the Animal Reaches this gameobject to Interact with it this needs to be set to true")]
        [SerializeField] private BoolReference m_Auto = new BoolReference(false);

        [Tooltip("Interact Once, after that it cannot longer work, unlest the Interactable is Restarted. Disable the component")]
        [SerializeField] private BoolReference m_singleInteraction = new BoolReference(false);

        [Tooltip("Delay time to activate the events on the Interactable")]
        public FloatReference m_Delay = new FloatReference(0);

        [Tooltip("CoolDown between Interactions when the Interactable is NOT a Single/One time interaction")]
        public FloatReference m_CoolDown = new FloatReference(0); 

        [Tooltip("When an Interaction is executed these events will be invoked." +
            "\n\nOnInteractWithGO(GameObject) -> will have the *INTERACTER* gameObject as parameter" +
            "\n\nOnInteractWith(Int) -> will have the *INTERACTER* ID as parameter")]
        public InteractionEvents events = new InteractionEvents();

        public GameObjectEvent OnFocused = new GameObjectEvent();
        public GameObjectEvent OnUnfocused = new GameObjectEvent();

        public int Index => m_ID;

        public bool Active { get => enabled && !InCooldown; set => enabled = value; }
        public bool SingleInteraction { get => m_singleInteraction.Value; set => m_singleInteraction.Value = value; }
        public bool Auto { get => m_Auto.Value; set => m_Auto.Value = value; }
       // public bool Active { get =>enabled; set => enabled = value; }

        /// <summary>Delay time to Activate the Interaction on the Interactable</summary>
        public float Delay { get => m_Delay.Value; set => m_Delay.Value = value; }

        /// <summary>CoolDown Between Interactions</summary>
        public float Cooldown { get => m_CoolDown.Value; set => m_CoolDown.Value = value; }

        /// <summary>Is the Interactable in CoolDown?</summary>
        public bool InCooldown => !MTools.ElapsedTime(CurrentActivationTime, Cooldown);

        public IInteractor CurrentInteractor { get; set; }

        private bool focused;
        public bool Focused
        {
            get => focused;
            set
            {
                if (focused != value)
                {
                    focused = value;

                    if (focused)
                    {
                        OnFocused.Invoke(CurrentInteractor != null ? CurrentInteractor.Owner : null);
                    }
                    else
                    {
                        OnUnfocused.Invoke(CurrentInteractor != null ? CurrentInteractor.Owner : null);
                    }
                }
            }
        }
      
        public GameObject Owner => gameObject;

        private float CurrentActivationTime;
       
        public string Description = "Invoke events when an Interactor interacts with it";
        [HideInInspector] public bool ShowDescription = true;
        [ContextMenu("Show Description")]
        internal void EditDescription() => ShowDescription ^= true;

        private void OnEnable()
        {
            CurrentActivationTime = -Cooldown;
        }

        private void OnDisable()
        {
            focused = false;
            CurrentInteractor?.UnFocus(this);    //Clean the Current Focused item  
        }

        /// <summary> Receive an Interaction from the Interacter </summary>
        /// <param name="InteracterID">ID of the Interacter</param>
        /// <param name="interacter">Interacter's GameObject</param>
        public bool Interact(int InteracterID, GameObject interacter)
        {
            if (Active)
            {
                if (m_InteractorID <= 0 || m_InteractorID == InteracterID) //Check for Interactor ID
                {
                    CurrentActivationTime = Time.time;

                    this.Delay_Action(Delay, () =>
                     {
                         events.OnInteractWithGO.Invoke(interacter);
                         events.OnInteractWith.Invoke(InteracterID);
                     }
                    );

                    if (SingleInteraction)
                    {
                        Focused = false;
                        Active = false;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }  

        /// <summary>  Receive an Interaction from an gameObject </summary>
        /// <param name="InteracterID">ID of the Interacter</param>
        /// <param name="interacter">Interacter's GameObject</param>
        public bool Interact(IInteractor interacter)
        { 
            if (interacter != null)
              return Interact(interacter.ID, interacter.Owner.gameObject);

            return false;
        }

        public void Interact() => Interact(-1, null);

        public virtual void Restart()
        {
            Focused = false;
            Active = true;
            CurrentActivationTime = -Cooldown;
        }

        [SerializeField] private int Editor_Tabs1;

    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MInteract)),CanEditMultipleObjects]
    public class MInteractEditor : UnityEditor.Editor
    {
        SerializedProperty m_ID, m_InteractorID, m_Auto, m_singleInteraction, m_Delay,
            m_CoolDown, events, OnFocused, OnUnfocused, Editor_Tabs1, Description, ShowDescription;
        protected string[] Tabs1 = new string[] { "General", "Events" };
        MInteract M;

        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));
        private GUIStyle style;
        private void OnEnable()
        {
            M = (MInteract)target;
            m_ID = serializedObject.FindProperty("m_ID");
            m_InteractorID = serializedObject.FindProperty("m_InteractorID");
            m_Auto = serializedObject.FindProperty("m_Auto");
            m_singleInteraction = serializedObject.FindProperty("m_singleInteraction");
            m_Delay = serializedObject.FindProperty("m_Delay");
            m_CoolDown = serializedObject.FindProperty("m_CoolDown");
            events = serializedObject.FindProperty("events");
            OnFocused = serializedObject.FindProperty("OnFocused");
            OnUnfocused = serializedObject.FindProperty("OnUnfocused");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            ShowDescription = serializedObject.FindProperty("ShowDescription");
            Description = serializedObject.FindProperty("Description");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (ShowDescription.boolValue)
            {
                if (style == null)
                {
                    style = new GUIStyle(StyleBlue)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true
                    };
                }

                style.normal.textColor = EditorStyles.label.normal.textColor;
                Description.stringValue = UnityEditor.EditorGUILayout.TextArea(Description.stringValue, style);
            }


            //MalbersEditor.DrawDescription("Interactable Element that invoke events when an Interactor interact with it");
            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

            if (Editor_Tabs1.intValue == 0) 
                DrawGeneral();
            else DrawEvents();

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Interactor",
                    M.CurrentInteractor != null ? M.CurrentInteractor.Owner : null, typeof(GameObject), false);

                EditorGUI.EndDisabledGroup();
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneral()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(m_InteractorID, new GUIContent("Interactor ID"));
                EditorGUILayout.PropertyField(m_ID, new GUIContent("Index"));

                    EditorGUILayout.PropertyField(m_Auto,new GUIContent("Auto Interact"));
                    EditorGUILayout.PropertyField(m_singleInteraction, new GUIContent("Single Interaction"));
                    EditorGUILayout.PropertyField(m_Delay);
                    if (!M.SingleInteraction) EditorGUILayout.PropertyField(m_CoolDown, new GUIContent("Cooldown"));
            }
            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = 0;
        }

        private void DrawEvents()
        {
            EditorGUILayout.PropertyField(events, true);
            if (events.isExpanded)
            {
                EditorGUILayout.PropertyField(OnFocused);
                EditorGUILayout.PropertyField(OnUnfocused);
            }
        }
    }
#endif
}