using MalbersAnimations.Controller;
using MalbersAnimations.Controller.Reactions;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    [DisallowMultipleComponent]
    /// <summary> Damager Receiver</summary>
    [AddComponentMenu("Malbers/Damage/MDamageable")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/mdamageable")]
    public class MDamageable : MonoBehaviour, IMDamage
    {
        [Tooltip("Animal Reaction to apply when the damage is done")]
        public Component character;

        [Tooltip("Animal Reaction to apply when the damage is done"),ExposeScriptableAsset]
        public MReaction reaction;

        [Tooltip("Stats component to apply the Damage")]
        public Stats stats;

        [Tooltip("Multiplier for the Stat modifier Value")]
        public FloatReference multiplier = new FloatReference(1);

        [Tooltip("When Enabled the animal will rotate towards the Damage direction"),UnityEngine.Serialization.FormerlySerializedAs("AlingToDamage")]
        public BoolReference AlignToDamage = new BoolReference(false);

        [Tooltip("Time to align to the damage direction")]
        public FloatReference AlignTime = new FloatReference(0.25f);
        [Tooltip("Aligmend curve")]
        public AnimationCurve AlignCurve = new AnimationCurve(MTools.DefaultCurve);

        public MDamageable Root; 
        public damagerEvents events;

        public Vector3 HitDirection { get; set; }
        public GameObject Damager { get; set; }
        public GameObject Damagee => gameObject;

        public DamageData LastDamage;


        [HideInInspector] public int Editor_Tabs1;

        private void Start()
        {
            if (character == null) character = stats.GetComponent(reaction.ReactionType()); //Find the character where the Stats are
        }

        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier modifier,
            bool isCritical, bool react, MReaction customReaction, bool pureDamage)
        {
            if (!enabled) return;       //This makes the Animal Immortal.
            HitDirection = Direction;   //IMPORTANT!!! to React

            if (customReaction) customReaction.React(character);        //Custom reaction
            else if (react && reaction)
            {
                reaction.React(character);     //Lets React 
            }


            var stat = stats.Stat_Get(modifier.ID);
            if (stat == null || !stat.Active ||  stat.IsEmpty) return; //Do nothing if the stat is empty

            SetDamageable(Direction, Damager);
            Root?.SetDamageable(Direction, Damager);                     //Send the Direction and Damager to the Root 

            if (isCritical)
            {
                events.OnCriticalDamage.Invoke();
                Root?.events.OnCriticalDamage.Invoke();
            }

            if (!pureDamage) modifier.Value *= multiplier;               //Apply to the Stat modifier a new Modification

            events.OnReceivingDamage.Invoke(modifier.Value);
            events.OnDamager.Invoke(Damager);

           

            if (Root != this)
            {
                Root?.events.OnReceivingDamage.Invoke(modifier.Value);
                Root?.events.OnDamager.Invoke(Damager);
            }

            LastDamage = new DamageData(Damager,gameObject ,modifier);
            if (Root) Root.LastDamage = LastDamage;


            modifier.ModifyStat(stats.Stat_Get(modifier.ID));

            if (AlignToDamage.Value)
            {
                AlignToDamageDirection(Damager);
            }
        }

        private void AlignToDamageDirection(GameObject Direction)
        {
            StartCoroutine(MTools.AlignLookAtTransform(character.transform, Direction.transform.position, AlignTime.Value, AlignCurve));
        }

        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount)
        {
            var modifier = new StatModifier(){ ID = stat, modify = StatOption.SubstractValue, Value  = amount};
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false);
        }



        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false);
        }



        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="modifyStat">Type of modification applied to the stat</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue,
             bool isCritical = false, bool react = true, MReaction customReaction = null, bool pureDamage = false)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage);
        }


        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat, 
            float amount, bool isCritical = false, bool react = true, MReaction customReaction = null, bool pureDamage = false)
        {
            var modifier = new StatModifier() { ID = stat, modify = StatOption.SubstractValue, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage);
        } 

        internal void SetDamageable(Vector3 Direction, GameObject Damager)
        {
            HitDirection = Direction;
            this.Damager = Damager;
        }

        [System.Serializable]
        public class damagerEvents
        {
            public FloatEvent OnReceivingDamage = new FloatEvent();
            public UnityEvent OnCriticalDamage = new UnityEvent();
            public GameObjectEvent OnDamager = new GameObjectEvent();
        }

        public struct DamageData
        {
            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damager;

            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damagee;
            /// <summary>  Final Stat Modifier ? </summary>
            public StatModifier stat;
            /// <summary> Final value who modified the Stat</summary>
            public float Damage => stat.modify != StatOption.None ? stat.Value.Value : 0f;

            public DamageData(GameObject damager, GameObject damagee, StatModifier stat)
            {
                Damager = damager;
                Damagee = damagee;
                this.stat = new StatModifier(stat);
            }
        }


#if UNITY_EDITOR
        private void Reset()
        {
            reaction = MTools.GetInstance<ModeReaction>("Damaged");
            stats = this.FindComponent<Stats>();
            Root = transform.root.GetComponent<MDamageable>();     //Check if there's a Damageable on the Root
            if (Root == this) Root = null;

            if (stats == null)
            {
                stats = gameObject.AddComponent<Stats>();  
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MDamageable))]
    public class MDamageableEditor : Editor
    {
        SerializedProperty reaction, stats, multiplier, events, Root, AlignTime, AlignCurve, AlignToDamage, Editor_Tabs1;
        MDamageable M;

        protected string[] Tabs1 = new string[] { "General", "Events" };


        private void OnEnable()
        {
            M = (MDamageable)target;

            reaction = serializedObject.FindProperty("reaction");
            stats = serializedObject.FindProperty("stats");
            multiplier = serializedObject.FindProperty("multiplier");
            events = serializedObject.FindProperty("events");
            Root = serializedObject.FindProperty("Root");
            AlignToDamage = serializedObject.FindProperty("AlignToDamage");
            AlignCurve = serializedObject.FindProperty("AlignCurve");
            AlignTime = serializedObject.FindProperty("AlignTime");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Allows the Animal React and Receive damage from external sources");


            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

          //  using (new GUILayout.VerticalScope(MalbersEditor.StyleGray))
            {
                if (Editor_Tabs1.intValue == 0) DrawGeneral();
                else DrawEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneral()
        {
            if (M.transform.parent != null)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(Root);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
               // EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(reaction);
               // EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(stats);
                EditorGUILayout.PropertyField(multiplier);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(AlignToDamage);

                if (M.AlignToDamage.Value)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(AlignTime);
                        EditorGUILayout.PropertyField(AlignCurve, GUIContent.none, GUILayout.MaxWidth(75));
                    }
                }
            }
        }


        private void DrawEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(events, true);
                EditorGUI.indentLevel--;
            }
        }

    
    }
#endif
}