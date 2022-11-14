using UnityEngine; 
using System.Collections.Generic;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using UnityEngine.Events;
using MalbersAnimations.Scriptables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Weapons
{
    [AddComponentMenu("Malbers/Weapons/Melee Weapon")]
    public class MMelee : MWeapon
    {
        [RequiredField] public Collider meleeTrigger;
        [Tooltip("Do not interact with Static Objects")]
        public bool ignoreStaticObjects = true;
        public BoolEvent OnCauseDamage = new BoolEvent();
        public Color DebugColor = new Color(1, 0.25f, 0, 0.5f);

        public bool UseCameraSide;
        public bool InvertCameraSide;



        [Tooltip("What Abilities to apply to the meleee weapons if they are not using any Combo")]
        public int[] GroundAttackAbilities;

        [Tooltip("What Abilities to apply to the meleee weapons if they are not using any Combo")]
        public int[] RidingAttackAbilities;

        protected bool canCauseDamage;                      //The moment in the Animation the weapon can cause Damage 
        public bool CanCauseDamage
        {
            get => canCauseDamage;
            set
            {
                Debugging($"Can cause Damage [{value}]", this);
                canCauseDamage = value;
                if (proxy) proxy.Active = value;
                meleeTrigger.enabled = value;         //Enable/Disable the Trigger
            }
        }

        protected TriggerProxy proxy { get; private set; }


        /// <summary>Damager from the Attack Triger Behaviour</summary>
        public override void ActivateDamager(int value, float multiplier)
        {
            base.ActivateDamager(value, multiplier);

            if (value == 0)
            {
                CanCauseDamage = false;
                OnCauseDamage.Invoke(CanCauseDamage);
            }
            else if (value == -1 || value == Index)
            {
                CanCauseDamage = true;
                OnCauseDamage.Invoke(CanCauseDamage);
            }
        }

        private void Awake()
        {
            if (animator)
                defaultAnimatorSpeed = animator.speed;

            Initialize();
        }


        public override void Initialize()
        {
            base.Initialize();
            FindTrigger();
        }

        void OnEnable()
        {
            if (proxy)
            {
                proxy.EnterTriggerInteraction += AttackTriggerEnter;
                // proxy.ExitTriggerInteraction += AttackTriggerExit;
            }
            CanCauseDamage = false;
        }

        /// <summary>Disable Listeners </summary>
        void OnDisable()
        {
            if (proxy)
            {
                proxy.EnterTriggerInteraction -= AttackTriggerEnter;
                //proxy.ExitTriggerInteraction -= AttackTriggerExit;
            }
        }

        //void AttackTriggerExit(GameObject root, Collider other)
        //{
        //    //???
        //}


        #region Main Attack 
        internal override void MainAttack_Start(IMWeaponOwner RC)
        {
            base.MainAttack_Start(RC);

            if (CanAttack)
            {
                WeaponAction.Invoke((int)Weapon_Action.Attack);
            }
        }

        //Super Uggly!!
        //internal override void MainAttack_Released(IMWeaponOwner RC)
        //{
        //    if (RC.IsRiding && RidingCombo != -1)
        //    {
        //        WeaponAction.Invoke((int)Weapon_Action.Idle);
        //        CanAttack = false;
        //    }
        //}
        #endregion


        void AttackTriggerEnter(GameObject root, Collider other)
        {
            if (IsInvalid(other)) return;                                               //Check Layers and Don't hit yourself
            if (other.transform.root == IgnoreTransform) return;                        //Check an Extra transform that you cannot hit...e.g your mount
            if (ignoreStaticObjects && other.transform.gameObject.isStatic) return;     //Ignore Static Objects

            var damagee = other.GetComponentInParent<IMDamage>();                      //Get the Animal on the Other collider

            var center = meleeTrigger.bounds.center;

            Direction = (other.bounds.center - center).normalized;                      //Calculate the direction of the attack

            Debugging($"Hit [{other.name}]", this);

            TryInteract(other.gameObject);                                              //Get the interactable on the Other collider
            TryPhysics(other.attachedRigidbody, other, center, Direction, Force);       //If the other has a riggid body and it can be pushed
            TryStopAnimator();
            TryHit(other, meleeTrigger.bounds.center);

            var Damage = new StatModifier(statModifier)
            { Value = Mathf.Lerp(MinDamage, MaxDamage, ChargedNormalized) };            //Do the Damage depending the charge

            TryDamage(damagee, Damage); //if the other does'nt have the Damagable Interface dont send the Damagable stuff 
        }



        public override void ResetWeapon()
        {
            meleeTrigger.enabled = false;
            proxy.Active = false;
            base.ResetWeapon();
        }

        private void FindTrigger()
        {
            if (meleeTrigger == null) meleeTrigger = GetComponent<Collider>();

            if (meleeTrigger)
            {
                proxy = TriggerProxy.CheckTriggerProxy(meleeTrigger, Layer, TriggerInteraction, Owner.transform);

                meleeTrigger.enabled = false;
                proxy.Active = meleeTrigger.enabled;
            }
            else
            {
                Debug.LogError($"Weapon [{name}] needs a collider. Please add one. Disabling Weapon", this);
                enabled = false;
            }
        }

        #region Gizmos

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (meleeTrigger != null)
                DrawTriggers(meleeTrigger.transform, meleeTrigger, DebugColor, false);
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                if (meleeTrigger != null)
                    DrawTriggers(meleeTrigger.transform, meleeTrigger, DebugColor, true);
        }


        protected override void Reset()
        {
            base.Reset();

            weaponType = MTools.GetInstance<WeaponID>("Melee");
            m_rate.Value = 0.5f;
            m_Automatic.Value = true;
        }
#endif
        #endregion
    }
    #region Inspector


#if UNITY_EDITOR
    [CanEditMultipleObjects, CustomEditor(typeof(MMelee))]
    public class MMeleeEditor : MWeaponEditor
    {
        SerializedProperty meleeCollider, OnCauseDamage, UseCameraSide, InvertCameraSide,
            GroundCombo, GroundAttackAbilities, RidingCombo,
            RidingAttackAbilities, ignoreStaticObjects;

        void OnEnable()
        {
            WeaponTab = "Melee";
            SetOnEnable();
            meleeCollider = serializedObject.FindProperty("meleeTrigger");
            ignoreStaticObjects = serializedObject.FindProperty("ignoreStaticObjects");
            OnCauseDamage = serializedObject.FindProperty("OnCauseDamage");
            InvertCameraSide = serializedObject.FindProperty("InvertCameraSide");
            UseCameraSide = serializedObject.FindProperty("UseCameraSide");
            //  Attacks = serializedObject.FindProperty("Attacks");
            RidingAttackAbilities = serializedObject.FindProperty("RidingAttackAbilities");
            GroundAttackAbilities = serializedObject.FindProperty("GroundAttackAbilities");
            GroundCombo = serializedObject.FindProperty("GroundCombo");
            RidingCombo = serializedObject.FindProperty("RidingCombo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDescription("Melee Weapon Properties");
            if (meleeCollider.objectReferenceValue == null)
                EditorGUILayout.HelpBox("Weapon needs a collider. Check [Melee] Tab", MessageType.Error);

            WeaponInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void UpdateSoundHelp()
        {
            SoundHelp = "0:Draw   1:Store   2:Swing   3:Hit \n (Leave 3 Empty, add SoundByMaterial and Invoke 'PlayMaterialSound' for custom Hit sounds)";
        }

        protected override void ChildWeaponEvents()
        {
            EditorGUILayout.PropertyField(OnCauseDamage, new GUIContent("On AttackTrigger Active"));
        }

        protected override void DrawAdvancedWeapon()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(ignoreStaticObjects);
                EditorGUILayout.PropertyField(meleeCollider,
                    new GUIContent("Melee Trigger", "Gets the reference of where is the Melee Collider of this weapon (Not Always is in the same gameobject level)"));
            }
            if (DescSTyle == null) DescSTyle = MalbersEditor.DescriptionStyle;
            EditorGUILayout.LabelField("Set Combos Values to -1 to ignore doing combos", DescSTyle);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Ground Attacks", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(GroundCombo);

                if (GroundCombo.intValue == -1)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(GroundAttackAbilities);
                    EditorGUI.indentLevel--;
                }
            }


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Riding Attacks", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(RidingCombo);

                if (RidingCombo.intValue == -1)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(RidingAttackAbilities);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(UseCameraSide, new GUIContent("Use Camera Side", "The Attacks are Activated by the Main Attack and It uses the Side of the Camera to Attack on the Right or the Left side of the Mount"));

                    if (UseCameraSide.boolValue)
                        EditorGUILayout.PropertyField(InvertCameraSide, new GUIContent("Invert Camera Side", "Inverts the camera side value"));
                }
            }
        }
    }
#endif
    #endregion
}