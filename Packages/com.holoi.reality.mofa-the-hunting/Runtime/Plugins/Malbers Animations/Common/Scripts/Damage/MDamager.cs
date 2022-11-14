using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using MalbersAnimations.Utilities;
using System.Collections;
using MalbersAnimations.Controller.Reactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    /// <summary> Core Class to cause damage to the stats</summary>
   // [AddComponentMenu("Malbers/Damage/Damager")]

    public abstract class MDamager : MonoBehaviour, IMDamager, IInteractor
    {
        #region Public Variables
        [SerializeField, Tooltip("Index of the Damager, You can have multiple swords ... this identifies if a sword is different from another")]
        protected int index = 1;

        /// <summary>Enable/Disable the Damager</summary>
        [SerializeField, Tooltip("Enable/Disable the Damager")]
        protected BoolReference m_Active = new BoolReference(true);

   
        [SerializeField, Tooltip("Hit Layer to interact with Objects"), ContextMenuItem("Get Layer from Root", "GetLayerFromRoot")]
        protected LayerReference m_hitLayer = new LayerReference(-1);

     
        [SerializeField, Tooltip("What to do with Triggers")]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

       
        [SerializeField, Tooltip("Owner. usually the Character Owns the Damager")]
        [ContextMenuItem("Find Owner", "Find_Owner")]
        protected GameObject owner;

        [SerializeField, Tooltip("This Gameobject will be enabled on Impact, if its a Prefab it will be instantiated")]
        internal GameObjectReference m_HitEffect;

        public GameObject HitEffect { get => m_HitEffect.Value; set => m_HitEffect.Value = value; }

        [Tooltip("The HitEffect will be destroyed after this time has elapsed, if it is a prefab. if = to zero, will be ignored")]
        [Min(0)] public float DestroyHitEffect;

        // protected IInteractor interactor;
        public IntReference interactorID = new IntReference(0);

        protected float damage_Multiplier = 1f;
        
        [Tooltip("Dont Hit any objects on the Owner's hierarchy")]
        public BoolReference dontHitOwner = new BoolReference( true);

        [Tooltip("Don't use the Default Reaction of the Damageable Component")]
        [ExposeScriptableAsset] public MReaction CustomReaction;


        /// <summary> Extra Transform to Ignore Damage. E.g. The Mount Animal</summary>
        public virtual Transform IgnoreTransform { get; set; }

        /// <summary>Damager can activate interactables</summary>
        [Tooltip("Damager can activate interactables")]
        public BoolReference interact = new BoolReference(true);

        /// <summary>Damager allows the Damagee to apply a reaction</summary>
        [Tooltip("Damager allows the Damagee to apply an animal reaction")]
        public BoolReference react = new BoolReference(true);

        /// <summary>Ignores Damagee Multiplier</summary>
        [Tooltip("If true the Damage Receiver will not apply its Default Multiplier")]
        public BoolReference pureDamage = new BoolReference(false);

        /// <summary>Stat to modify on the Damagee</summary>
        [Tooltip("Stat to modify on the Damagee")]
        [ContextMenuItem("Set Default Damage", "Set_DefaultDamage")]
        public StatModifier statModifier = new StatModifier();

        /// <summary>Critical Change (0 - 1)</summary>
        [SerializeField, Tooltip("Critical Change (0 - 1)\n1 means it will be always critical")]
        protected FloatReference m_cChance = new FloatReference(0);

        /// <summary>If the Damage is critical, the Stat modifier value will be multiplied by the Critical Multiplier</summary>
        [SerializeField, Tooltip("If the Damage is critical, the Stat modifier value will be multiplied by the Critical Multiplier")]
        protected FloatReference cMultiplier = new FloatReference(2);

        [SerializeField, Tooltip("Force to Apply to RigidBodies when the Damager hit them")]
        protected FloatReference m_Force = new FloatReference(50f);

        [Tooltip("Force mode to apply to the Object that the Damager Hits")]
        public ForceMode forceMode = ForceMode.Force;
        [Tooltip("Direction of the Attack. Used to apply the Force and to know the Direction of the Hit from the Damager")]
        protected Vector3 Direction = Vector3.forward;

        public TransformEvent OnHit = new TransformEvent();
        public Vector3Event OnHitPosition = new Vector3Event();
        public IntEvent OnHitInteractable = new IntEvent();


        //[Tooltip("When the Attack Trigger Touches a valid collider, it will stop the animator to give an extra effect")]
        //public BoolReference StopAnimator = new BoolReference(false);
        [Tooltip("If there's an Animator Controller it will be stopped")]
        [ContextMenuItem("Find Animator", "Find_Animator")]
        [ContextMenuItem("Clear Animator", "Clear_Animator")]
        public Animator animator;
        [Tooltip("Value of the Animator Speed when its stopped")]
        public FloatReference AnimatorSpeed = new FloatReference(0.05f);
        [Tooltip("Time the Animator will be stopped. If its zero, stopping the animator is ignored")]
        public FloatReference AnimatorStopTime = new FloatReference(0.1f);

        #endregion

        #region Properties
        /// <summary>Owner of the Damager</summary>
        public virtual GameObject Owner { get => owner; set => owner = value; }

        /// <summary>Force of the Damager</summary>
        public float Force { get => m_Force; set => m_Force = value; }

      
        public LayerMask Layer { get => m_hitLayer.Value; set => m_hitLayer.Value = value; }
        public QueryTriggerInteraction TriggerInteraction  { get => triggerInteraction; set => triggerInteraction = value; }


        /// <summary>Does the hit was Critical</summary>
        public bool IsCritical { get; set; }
        public bool debug;


        /// <summary>If the Damage is critical, the Stat modifier value will be multiplied by the Critical Multiplier</summary>
        public float CriticalMultiplier { get => cMultiplier.Value; set => cMultiplier.Value = value; }

        /// <summary>>Critical Change (0 - 1)</summary>
        public float CriticalChance { get => m_cChance.Value; set => m_cChance.Value = value; }

        /// <summary>>Index of the Damager</summary>
        public virtual int Index => index;
        public virtual int ID => interactorID.Value;

        /// <summary>  Set/Get the Damager Active  </summary>
        public virtual bool Enabled 
        { 
            get => m_Active.Value;
            set => m_Active.Value = enabled = value; 
        }


        /// <summary>Point of of Contact</summary>
        public Vector3 HitPosition { get; private set; }

        /// <summary>Rotation of the Point of contact (Normal)</summary>
        public Quaternion HitRotation { get; private set; }
        #endregion

        /// <summary>  The Damagee does not have all the conditions to apply the Damage  </summary>
        public virtual bool IsInvalid(Collider damagee)
        {
            if (damagee.isTrigger && TriggerInteraction == QueryTriggerInteraction.Ignore) return true;    //just collapse when is a collider what we are hitting
            if (!MTools.Layer_in_LayerMask(damagee.gameObject.layer, Layer)) { return true; }        //Just hit what is on the HitMask Layer
            if (dontHitOwner && Owner != null && damagee.transform.IsChildOf(Owner.transform)) { return true; }   //Dont hit yourself!
           // if (damagee.gameObject.isStatic) return true;
            return false;
        }


      
        /// <summary>  Applies the Damage to the Game object  </summary>
        /// <returns>is False if the other gameobject didn't had a IMDamage component attached</returns>
        protected virtual bool TryDamage(IMDamage damagee, StatModifier stat)
        {
            if (damagee != null && !stat.IsNull)
            {
                var criticalStat = CheckCriticalCheckMultiplier(stat);
                damagee.ReceiveDamage(Direction, Owner, criticalStat, IsCritical, react.Value, CustomReaction , pureDamage.Value);

                Debugging($"Do Damage to [{damagee.Damagee.name}]", damagee.Damagee);
                return true;
            }
            return false;
        }

        protected void TryHit(Collider col, Vector3 DamageCenter)
        {
            if (col is MeshCollider && !(col as MeshCollider).convex) return; //Do not hit NonConvex Collider
            if (col is TerrainCollider) return; //Do not hit  a Terrain Collider

            HitPosition = col.ClosestPoint(DamageCenter); //Find the closest point on the Collider hitted 
            HitRotation = Quaternion.FromToRotation(Vector3.up, col.bounds.center - DamageCenter);
            OnHitPosition.Invoke(HitPosition);

            MTools.DrawWireSphere(HitPosition, Color.red, 0.2f, 1);

            if (HitEffect != null)
            {
                if (HitEffect.IsPrefab())
                {
                    var instance = Instantiate(HitEffect, HitPosition, HitRotation);
                   // instance.transform.parent = col.transform;
                    //Reset the gameobject visibility 

                    CheckHitEffect(instance);
                    if (DestroyHitEffect > 0) Destroy(instance, DestroyHitEffect);
                }
                else
                {
                    HitEffect.transform.parent = null;
                    HitEffect.transform.SetPositionAndRotation(HitPosition, HitRotation);
                    CheckHitEffect(HitEffect);
                }
            }

            OnHit.Invoke(col.transform);
        }

        protected void CheckHitEffect(GameObject hit)
        {
            //Check if the Hit Effect has a MDamager so pass the Layer and Owner (E.g. Explosions)
            var isDamager = hit.GetComponent<MDamager>();
            if (isDamager)
            {
                isDamager.Owner = Owner;
                isDamager.Layer = Layer;
                isDamager.TriggerInteraction = TriggerInteraction;
            }

            //Next Frame Reset the GameObject visibility
            this.Delay_Action(() =>
            {
                hit.SetActive(false);
                hit.SetActive(true);
            }
            );
        }

        protected virtual bool TryDamage(GameObject other, StatModifier stat) => TryDamage(other.FindInterface<IMDamage>(), stat);


        /// <summary>Activates the Damager in case the Damager uses a Trigger</summary>
        public virtual void DoDamage(bool value, float multiplier)  { damage_Multiplier = multiplier; }


        protected void TryStopAnimator()
        {
            if (animator && C_StopAnim == null)
            {
                C_StopAnim = C_StopAnimator();
                StartCoroutine(C_StopAnim);
            }
        }

        protected IEnumerator C_StopAnim;
        protected float defaultAnimatorSpeed = 1;

        protected IEnumerator C_StopAnimator()
        {
            animator.speed = AnimatorSpeed;
            yield return new WaitForSeconds(AnimatorStopTime.Value);
            animator.speed = defaultAnimatorSpeed;

            C_StopAnim = null;
        }

        /// <summary>Damager can Activate Interactables </summary>
        protected bool TryInteract(GameObject damagee)
        {
            if (interact)
            {
                var interactable = damagee.FindInterface<IInteractable>();
                if (interactable != null && interactable.Active)
                {
                    return Interact(interactable);              //if we have an Local Interactor then use it instead of this Damager
                }
            }
            return false;
        }


        public void Focus(IInteractable item)
        {
            if (item.Active) //Ignore One Disable Interactors
            {
                item.CurrentInteractor = this;
                item.Focused = true;
                if (item.Auto) Interact(item); //Interact if the interacter is on Auto
            }
        }

        public void UnFocus(IInteractable item)
        {
            if (item != null)
            {
                item.CurrentInteractor = this;
                item.Focused = false;
                item.CurrentInteractor = null;
            }
        }  

        /// <summary> Interact locally  </summary>
        public virtual bool Interact(IInteractable interactable)
        {
            if (interactable != null)
            {
                Debugging($"Interact with <B>[{interactable.Owner.name}]</B>", interactable.Owner);
                if (interactable.Interact(this))
                {
                    OnHitInteractable.Invoke(interactable.Index);
                    return true;
                }
                return false;
            }
            return false;
        }


        /// <summary> Restart method from Interactor </summary>
        public virtual void Restart() { } 
            
        /// <summary>Apply Physics to the Damageee </summary>
        protected virtual bool TryPhysics(Rigidbody rb, Collider col,Vector3 Origin ,Vector3 Direction, float force)
        {
            if (rb && force > 0)
            {
                MTools.Draw_Arrow(Origin, Direction, Color.red, 1);


                if (col) //When using collider
                {
                    var HitPoint = col.ClosestPoint(Origin);
                    rb.AddForceAtPosition(Direction * force, HitPoint, forceMode); 

                    MTools.DrawWireSphere(HitPoint, Color.red, 0.1f, 2f);
                    MTools.Draw_Arrow(HitPoint, Direction * force, Color.red, 2f);

                }
                else
                    rb.AddForce(Direction * force, forceMode);

                Debugging($"Apply Force to [{rb.name}]", this);

                return true;
            }
            return false;
        }

        public virtual void SetOwner(GameObject owner) => Owner = owner;
        public virtual void SetOwner(Transform owner) => Owner = owner.gameObject;

        /// <summary>  Prepare the modifier value to change it if is critical  </summary>
        protected virtual StatModifier CheckCriticalCheckMultiplier(StatModifier mod)
        {
            IsCritical = m_cChance > Random.value;  //Calculate if is critical

            var modifier = new StatModifier(mod);

            if (IsCritical && CriticalChance > 0)
            {
                modifier.Value = mod.Value * CriticalMultiplier;        //apply the Critical Damage and Animation Multiplier
            }
            
            modifier.Value.Value *= damage_Multiplier;

            return modifier;
        }


        //protected void GetLayerFromRoot()
        //{
        //    var HitMaskOwner = m_Owner.GetComponentInParent<IMLayer>();

        //    if (HitMaskOwner != null)
        //    {
        //        Layer = HitMaskOwner.Layer;
        //        Debugging($"{name} Layer set by its Root: {transform.root}",null);
              
        //    }
        //}

        protected void Find_Owner()
        {
            if (Owner == null) Owner = transform.root.gameObject;
            MTools.SetDirty(this);
        }

        protected void Find_Animator()
        {
            if (animator == null) animator = gameObject.FindComponent<Animator>();
            MTools.SetDirty(this);
        }

        protected void Clear_Animator()
        {
            animator = null;
            MTools.SetDirty(this);
        }


#if UNITY_EDITOR
        protected virtual void Reset()
        {
            statModifier = new StatModifier()
            {
                ID = MTools.GetInstance<StatID>("Health"),
                modify = StatOption.SubstractValue,
                Value = new FloatReference(10)
            };

            m_hitLayer.Variable = MTools.GetInstance<LayerVar>("Hit Layer");
            m_hitLayer.UseConstant = false;

            var core = transform.GetComponentInParent<IObjectCore>();
            if (core != null) owner = core.transform.gameObject;
            owner = transform.root.gameObject;
        }

        public static void DrawTriggers(Transform transform, Collider Trigger, Color DebugColor, bool selected = false)
        {
            if (Trigger == null) return;

            Gizmos.color = DebugColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            var DColorFlat = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 1f);

            if (selected) DColorFlat = Color.yellow;

            if (Trigger is BoxCollider)
            {
                BoxCollider _C = Trigger as BoxCollider;

                var pos = _C.center;
                var sca = _C.size;


                if (!Application.isPlaying || Application.isPlaying && Trigger.enabled)
                {
                    Gizmos.DrawCube(pos, sca);


                    Gizmos.color = DColorFlat;
                    Gizmos.DrawWireCube(pos, sca);
                }

            }
            else if (Trigger is SphereCollider)
            {
                SphereCollider _C = Trigger as SphereCollider;

                if (!Application.isPlaying || Application.isPlaying && Trigger.enabled)
                {
                    Gizmos.DrawSphere(_C.center, _C.radius);
                    Gizmos.color = DColorFlat;
                    Gizmos.DrawWireSphere(_C.center, _C.radius);
                }
            }

            //Trigger.enabled = isen;

        }



#endif

        public void Debugging(string value, Object obj)
        {
#if UNITY_EDITOR
            if (debug)
                Debug.Log($"<color=yellow><B>[{name}]</B> → {value} </color>", obj);
#endif
        }
    }


    ///--------------------------------INSPECTOR-------------------
    ///
#if UNITY_EDITOR
    [CustomEditor(typeof(MDamager)),CanEditMultipleObjects]
    public class MDamagerEd : Editor
    {
        protected MDamager MD;
        protected SerializedProperty Force, forceMode, index, statModifier, onhit, OnHitPosition, OnHitInteractable, dontHitOwner, owner, m_Active, debug,
            hitLayer, triggerInteraction, m_cChance, cMultiplier, pureDamage, react, CustomReaction, interact , m_HitEffect,  interactorID, DestroyHitEffect,
            StopAnimator, AnimatorSpeed, AnimatorStopTime, animator;


        private void OnEnable() => FindBaseProperties();

        protected virtual void FindBaseProperties()
        {
            MD = (MDamager)target;
            index = serializedObject.FindProperty("index");
            m_HitEffect = serializedObject.FindProperty("m_HitEffect");
            OnHitPosition = serializedObject.FindProperty("OnHitPosition");
            m_Active = serializedObject.FindProperty("m_Active");
            hitLayer = serializedObject.FindProperty("m_hitLayer");
            triggerInteraction = serializedObject.FindProperty("triggerInteraction");
            dontHitOwner = serializedObject.FindProperty("dontHitOwner");
            owner = serializedObject.FindProperty("owner");
            interactorID = serializedObject.FindProperty("interactorID");
            DestroyHitEffect = serializedObject.FindProperty("DestroyHitEffect");


            react = serializedObject.FindProperty("react");
            CustomReaction = serializedObject.FindProperty("CustomReaction");

            interact = serializedObject.FindProperty("interact");
            pureDamage = serializedObject.FindProperty("pureDamage");

            m_cChance = serializedObject.FindProperty("m_cChance");
            cMultiplier = serializedObject.FindProperty("cMultiplier");

            Force = serializedObject.FindProperty("m_Force");
            forceMode = serializedObject.FindProperty("forceMode");

            statModifier = serializedObject.FindProperty("statModifier");

            onhit = serializedObject.FindProperty("OnHit");
            OnHitInteractable = serializedObject.FindProperty("OnHitInteractable");
            debug = serializedObject.FindProperty("debug");


            StopAnimator = serializedObject.FindProperty("StopAnimator");
            animator = serializedObject.FindProperty("animator");
            AnimatorSpeed = serializedObject.FindProperty("AnimatorSpeed");
            AnimatorStopTime = serializedObject.FindProperty("AnimatorStopTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDescription("Damager Core Logic");
           // DrawScript();
            DrawGeneral();
            DrawPhysics();
            DrawCriticalDamage();
            DrawStatModifier();
            DrawMisc();
            DrawEvents();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))   
            {
                EditorGUILayout.PropertyField(onhit);
                EditorGUILayout.PropertyField(OnHitPosition);
                EditorGUILayout.PropertyField(OnHitInteractable);
                DrawCustomEvents();
            }
        }

        protected virtual void DrawCustomEvents()  { }
       

        protected virtual void DrawMisc(bool drawbox = true)
        {
            if (drawbox) EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            react.isExpanded = MalbersEditor.Foldout(react.isExpanded, "Interactions");

            if (react.isExpanded)
            {

                var p = " [Prefab]";
                if (MD.HitEffect == null
                    || !MD.HitEffect.IsPrefab()) p = "";

                EditorGUILayout.PropertyField(m_HitEffect, new GUIContent(m_HitEffect.displayName + p));

                if (MD.HitEffect != null)
                {
                    if (MD.HitEffect.IsPrefab())
                        EditorGUILayout.PropertyField(DestroyHitEffect);

                    EditorGUILayout.HelpBox(
                        MD.HitEffect.IsPrefab() ?
                        "The Hit Effect its a Prefab. The Effect will be instantiated as a child of the hitted collider, positioned and oriented using the hit position" :
                        "The Hit Effect its a NOT a Prefab. The Effect will be positioned and oriented using the hit position. It will be enabled and disabled",
                        MessageType.Info);
                }


                EditorGUILayout.PropertyField(react);
                EditorGUILayout.PropertyField(CustomReaction);
                EditorGUILayout.PropertyField(interact);

                if (MD.interact.Value)
                    EditorGUILayout.PropertyField(interactorID);
            }
            //  EditorGUILayout.Space();

            AnimatorStopTime.isExpanded = MalbersEditor.Foldout(AnimatorStopTime.isExpanded, "Stop Animator");

            if (AnimatorStopTime.isExpanded)
            {
                EditorGUILayout.PropertyField(AnimatorStopTime);

                if (MD.AnimatorStopTime.Value > 0)
                {
                    EditorGUILayout.PropertyField(AnimatorSpeed);
                    EditorGUILayout.PropertyField(animator);
                }
            }


            if (drawbox) EditorGUILayout.EndVertical();
        }

        protected virtual void DrawGeneral(bool drawbox = true)
        {
            if (drawbox) EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_Active);
            MalbersEditor.DrawDebugIcon(debug);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(index);
            EditorGUILayout.PropertyField(hitLayer);
            EditorGUILayout.PropertyField(triggerInteraction);

            EditorGUILayout.PropertyField(dontHitOwner, new GUIContent("Don't hit Owner"));
            if (MD.dontHitOwner.Value)
            {
                EditorGUILayout.PropertyField(owner);
               // Debug.Log("MD = " + MD.Owner);
            }

            if (drawbox) EditorGUILayout.EndVertical();
        }

        protected virtual void DrawPhysics(bool drawbox = true)
        {
            if (drawbox) EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            Force.isExpanded = MalbersEditor.Foldout(Force.isExpanded, "Physics");

            if (Force.isExpanded)
            {
                //EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(Force);
                EditorGUILayout.PropertyField(forceMode, GUIContent.none, GUILayout.MaxWidth(90), GUILayout.MinWidth(20));
                EditorGUILayout.EndHorizontal();
            }
            if (drawbox) EditorGUILayout.EndVertical();
        }


        protected virtual void DrawCriticalDamage(bool drawbox = true)
        {
            if (drawbox) EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Critical Damage", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_cChance, new GUIContent("Chance [0-1]"), GUILayout.MinWidth(50));
            EditorGUIUtility.labelWidth = 47;
            EditorGUILayout.PropertyField(cMultiplier, new GUIContent("Mult"), GUILayout.MinWidth(50));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();
            if (drawbox) EditorGUILayout.EndVertical();
        }


        protected virtual void DrawStatModifier(bool drawbox = true)
        {
            if (drawbox) EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(statModifier, new GUIContent("Stat Modifier","Which Stat will be affected on the Object to hit after Impact"), true);
            EditorGUILayout.PropertyField(pureDamage);
            if (drawbox) EditorGUILayout.EndVertical();
        } 
        

        protected void DrawDescription(string desc) => MalbersEditor.DrawDescription(desc);


        public bool Foldout(bool prop, string name)
        {
            EditorGUI.indentLevel++;
            prop = GUILayout.Toggle(prop, name, EditorStyles.foldoutHeader);
            EditorGUI.indentLevel--;
            return prop;
        }
    }
#endif
}