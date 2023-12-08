using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;
using MalbersAnimations.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    /// <summary>Simple Script to make damage anything with a stat</summary>
    [AddComponentMenu("Malbers/Damage/Attack Trigger")]
    [SelectionBase]
    public class MAttackTrigger : MDamager
    {
        [RequiredField, Tooltip("Collider used for the Interaction")]
        public Collider Trigger;

        protected TriggerProxy Proxy { get; private set; }

        /// <summary>When the Attack Trigger is Enabled, Affect your stat</summary>
        [Tooltip("When the Attack Trigger is Enabled, Affect your stat")]
        public StatModifier SelfStatEnter;

        /// <summary>When the Attack Trigger is Disabled, Affect your stat</summary>
        [Tooltip("When the Attack Trigger is Disabled, Affect your stat")]
        public StatModifier SelfStatExit;

        /// <summary>When the Attack Trigger Exits an enemy, Affect his stats</summary>
        [Tooltip("When the Attack Trigger Exits an enemy, Affect his stats")]
        public StatModifier EnemyStatExit;

        public UnityEvent OnAttackBegin = new UnityEvent();
        public UnityEvent OnAttackEnd = new UnityEvent();

        public Color DebugColor = new Color(1, 0.25f, 0, 0.15f);

        /// <summary>Damageee that can be Damaged</summary>
        private IMDamage damagee;

        ///// <summary>All the Gameobjects using the Trigger</summary>
        //internal List<GameObject> EnteringGameObjects = new List<GameObject>();

        ///// <summary>All the colliders that are entering the Trigger</summary>
        //protected List<Collider> m_Colliders = new List<Collider>(); 
          
        [HideInInspector] public int Editor_Tabs1;

        private void Awake()
        {
            this.Delay_Action(1, () => { if (animator) defaultAnimatorSpeed = animator.speed; });
            //Delay this so the Animal can change the Animator Speed the first time
            FindTrigger();

            if (!m_Active.Value) enabled = false;
        }


        private void FindTrigger()
        {
            if (Owner == null)
            {
                Owner = transform.root.gameObject;                         //Set which is the owner of this AttackTrigger
                var core = transform.GetComponentInParent<IObjectCore>();
                if (core != null) owner = core.transform.gameObject;
            }
            if (Trigger)
            {
                Proxy = TriggerProxy.CheckTriggerProxy(Trigger, Layer, TriggerInteraction, Owner.transform); 
            }
            else
            {
                Debug.LogWarning($"Attack trigger {name} need a Collider", this);
            }
        }

        void OnEnable()
        { 
            if (Trigger)
            {
                Trigger.enabled = Trigger.isTrigger = Proxy.Active = true;
            }

            Proxy.EnterTriggerInteraction += AttackTriggerEnter;
            Proxy.ExitTriggerInteraction += AttackTriggerExit;

            damagee = null; 
          
            OnAttackBegin.Invoke();
        }

        void OnDisable()
        {
            if (Trigger)
            {
                Trigger.enabled = Proxy.Active = false;
            }

            Proxy.EnterTriggerInteraction -= AttackTriggerEnter;
            Proxy.ExitTriggerInteraction -= AttackTriggerExit;

            TryDamage(damagee, EnemyStatExit); //Means the Colliders was disable before Exit Trigger

            OnAttackEnd.Invoke();

            if(animator) animator.speed = defaultAnimatorSpeed;

            damagee = null;
        } 

        private void AttackTriggerEnter(GameObject newGo, Collider other)
        {
            if (dontHitOwner && Owner != null && other.transform.IsChildOf(Owner.transform))   return;  

            damagee = other.GetComponentInParent<IMDamage>();                      //Get the Animal on the Other collider
            var center = Trigger.bounds.center;
            Direction = (other.bounds.center- center ).normalized;                      //Calculate the direction of the attack

            TryInteract(other.gameObject);                                              //Get the interactable on the Other collider
            TryPhysics(other.attachedRigidbody, other, center, Direction, Force);       //If the other has a riggid body and it can be pushed
            TryStopAnimator();
            TryHit(other, Trigger.bounds.center);
            TryDamage(damagee, statModifier); //if the other does'nt have the Damagable Interface dont send the Damagable stuff  
        }

        private void AttackTriggerExit(GameObject newGo, Collider other)
        {
            if (dontHitOwner && Owner != null && other.transform.IsChildOf(Owner.transform))  return;  

            TryDamage(other.GetComponentInParent<IMDamage>(), EnemyStatExit); //if the other does'nt have the Damagable Interface dont send the Damagable stuff
        }

        public override void DoDamage(bool value,float multiplier)
        {
            base.DoDamage(value,multiplier);
            enabled = value;
        }




#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Trigger = this.FindComponent<Collider>();
            if (!Trigger) Trigger = gameObject.AddComponent<BoxCollider>();
            Trigger.isTrigger = true;
            Trigger.enabled = false;
            enabled = false;
            m_Active.Value = false;
            gameObject.SetLayer(2); //Force ignore Raycast
        }


        void OnDrawGizmos()
        {
            DrawTriggers(transform, Trigger, DebugColor, false);
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                DrawTriggers(transform, Trigger, DebugColor, true);
        }


     

        //[ContextMenu("Create Cinemachine Impulse")]
        //void CreateCinemachinePulse()
        //{
        //    var cinemachinePulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        //    if (cinemachinePulse == null)
        //    {
        //        cinemachinePulse = gameObject.AddComponent<Cinemachine.CinemachineImpulseSource>();
        //        MTools.SetDirty(gameObject);
        //    }

        //    cinemachinePulse.m_DefaultVelocity = Vector3.up * 0.1f;
        //    cinemachinePulse.m_ImpulseDefinition.m_ImpulseType = Cinemachine.CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        //    cinemachinePulse.m_ImpulseDefinition.m_ImpulseShape = Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Bump;
        //    cinemachinePulse.m_ImpulseDefinition.m_ImpulseDuration = 0.2f;
        //    UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(OnHit, cinemachinePulse.GenerateImpulse);
        //    MTools.SetDirty(cinemachinePulse);
        //}
#endif
    } 

#if UNITY_EDITOR


    [CustomEditor(typeof(MAttackTrigger)),CanEditMultipleObjects]
    public class MAttackTriggerEd : MDamagerEd
    {
        SerializedProperty Trigger, EnemyStatExit, DebugColor, OnAttackBegin, OnAttackEnd, Editor_Tabs1;
        protected string[] Tabs1 = new string[] { "General", "Damage", "Extras", "Events" };


        private void OnEnable()
        {
            FindBaseProperties();

            Trigger = serializedObject.FindProperty("Trigger");

            EnemyStatExit = serializedObject.FindProperty("EnemyStatExit");
           
            DebugColor = serializedObject.FindProperty("DebugColor");

            OnAttackBegin = serializedObject.FindProperty("OnAttackBegin");
            OnAttackEnd = serializedObject.FindProperty("OnAttackEnd");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1"); 
        }


        protected override void DrawCustomEvents()
        {
            EditorGUILayout.PropertyField(OnAttackBegin);
            EditorGUILayout.PropertyField(OnAttackEnd);
        }

        protected override void DrawStatModifier(bool drawbox =true)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Modify Stat", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(statModifier, new GUIContent("Enemy Stat Enter"), true);
            EditorGUILayout.PropertyField(EnemyStatExit, true);
            EditorGUILayout.PropertyField(pureDamage);
            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDescription("Attack Trigger Logic. Creates damage to the stats of any collider entering the trigger");

            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

            int Selection = Editor_Tabs1.intValue;

            if (Selection == 0) DrawGeneral();
            else if (Selection == 1) DrawDamage();
            else if (Selection == 2) DrawExtras();
            else if (Selection == 3) DrawEvents();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawGeneral(bool drawbox = true)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(Trigger);
                    EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.Width(55));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            base.DrawGeneral(true);
        }

        private void DrawDamage()
        {
            DrawStatModifier();
            DrawCriticalDamage();
        }

        private void DrawExtras()
        {
            DrawPhysics();
            DrawMisc();
        }
    }
#endif
}

