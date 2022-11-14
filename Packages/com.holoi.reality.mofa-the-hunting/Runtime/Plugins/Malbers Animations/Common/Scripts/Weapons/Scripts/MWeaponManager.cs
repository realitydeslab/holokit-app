using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Weapons;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using MalbersAnimations.Controller;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations
{

    /// <summary> Rider Combat Mode</summary>

    [AddComponentMenu("Malbers/Weapons/Weapon Manager [AC]")]

    public partial class MWeaponManager : MonoBehaviour, IAnimatorListener, IMAnimator, IMWeaponOwner, IMDamagerSet, IWeaponManager
    {
        [HideInInspector] public int Editor_Tabs1;
        [HideInInspector] public int Editor_Tabs2;


        public void Debugging(string value, string color = "white")
        {
#if UNITY_EDITOR
            if (debug)
                Debug.Log($"<B>[{name}] → [WeaponM] → <color={color}>{value}</color></B>", this);
#endif
        }


        ///This was left blank intentionally
        /// Callbacks: all the public functions and methods
        /// Logic: all Combat logic is there, Equip, Unequip, Aim Mode...
        /// Variables: All Variables and Properties
        #region RESET COMBAT VALUES WHEN THE SCRIPT IS CREATED ON THE EDITOR

#if UNITY_EDITOR

        private void Reset()
        {
            var m_Aim = GetComponent<Aim>();
            if (m_Aim == null) m_Aim = gameObject.AddComponent<Aim>();


            BoolVar WMCombatMode = MTools.GetInstance<BoolVar>("WM Combat Mode");
            MEvent WMEquipedWeapon = MTools.GetInstance<MEvent>("WM Equiped Weapon");
            MEvent SetCameraAimState = MTools.GetInstance<MEvent>("Set Camera AimState");

            if (WMCombatMode != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCombatMode, WMCombatMode.SetValue);
            if (WMEquipedWeapon != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(OnEquipWeapon, WMEquipedWeapon.Invoke);

            if (SetCameraAimState != null)
                UnityEditor.Events.UnityEventTools.AddPersistentListener(m_Aim.OnAimSide, SetCameraAimState.Invoke);


            //AC Weapons
            animal = this.FindComponent<MAnimal>();
            comboManager = this.FindComponent<ComboManager>();
            DrawWeaponModeID = MTools.GetInstance<ModeID>("Weapon Draw");
            StoreWeaponModeID = MTools.GetInstance<ModeID>("Weapon Store");
            UnarmedModeID = MTools.GetInstance<ModeID>("Attack1");

            var DefaultHolster = MTools.GetInstance<HolsterID>("Left Holster");

            holsters = new List<Holster>() { new Holster() { ID = DefaultHolster, Slots = new List<Transform>(1) { transform } } };



            Animator Anim = GetComponent<Animator>();

            if (Anim && Anim.avatar.isHuman)
            {
                LeftHandEquipPoint = Anim.GetBoneTransform(HumanBodyBones.LeftHand);
                RightHandEquipPoint = Anim.GetBoneTransform(HumanBodyBones.RightHand);
            }
        }


        [ContextMenu("Create Weapon Inputs")]
        void CreateInputs()
        {
            MInput input = GetComponent<MInput>();
            if (input == null) { input = gameObject.AddComponent<MInput>(); }


            Debug.Log("input = " + input.ActiveMap.name);

            var RCA1 = input.FindInput("MainAttack");
            if (RCA1 == null)
            {
                RCA1 = new InputRow("MainAttack", "MainAttack", KeyCode.Mouse0, InputButton.Press, InputType.Key);
                input.ActiveMap.inputs.Add(RCA1);
                Debug.Log("<B>WeaponAttack1</B> Input created");
            }

            RCA1 = input.FindInput("SecondAttack");
            if (RCA1 == null)
            {
                RCA1 = new InputRow("SecondAttack", "SecondAttack", KeyCode.Mouse1, InputButton.Press, InputType.Key);
                input.ActiveMap.inputs.Add(RCA1);
                Debug.Log("<B>SecondAttack</B> Input created");
            }

            var Reload = input.FindInput("Reload");
            if (Reload == null)
            {
                Reload = new InputRow("Reload", "Reload", KeyCode.R, InputButton.Down, InputType.Key);
                input.ActiveMap.inputs.Add(Reload);
                Debug.Log("<B>Reload</B> Input created");
            }

            var Aim = input.FindInput("Aim");
            if (Aim == null)
            {
                Aim = new InputRow("Aim", "Aim", KeyCode.Mouse1, InputButton.Press, InputType.Key);
                input.ActiveMap.inputs.Add(Aim);
                Debug.Log("<B>Aim</B> Input created");
            }

            EditorUtility.SetDirty(input);
        }

        [ContextMenu("Create Holsters Inputs")]

        void ConnectDefaultHolsters()
        {
            MInput input = GetComponent<MInput>();
            if (input == null) { input = gameObject.AddComponent<MInput>(); }

            #region Holster Left
            var Holster_ = input.FindInput("HolsterLeft");
            if (Holster_ == null)
            {
                Holster_ = new InputRow("HolsterLeft", "HolsterLeft", KeyCode.Alpha4, InputButton.Down, InputType.Key);
                input.inputs.Add(Holster_);

                Debug.Log("<B>Holster Left</B> Input created");

            }
            #endregion

            #region Holster Right
            Holster_ = input.FindInput("HolsterRight");
            if (Holster_ == null)
            {
                Holster_ = new InputRow("HolsterRight", "HolsterRight", KeyCode.Alpha5, InputButton.Down, InputType.Key);
                input.inputs.Add(Holster_);

                Debug.Log("<B>Holster Right</B> Input created");
            }
            #endregion

            #region Holster Back
            Holster_ = input.FindInput("HolsterBack");
            if (Holster_ == null)
            {
                Holster_ = new InputRow("HolsterBack", "HolsterBack", KeyCode.Alpha6, InputButton.Down, InputType.Key);
                input.inputs.Add(Holster_);

                Debug.Log("<B>Holster Back</B> Input created");
            }
            #endregion

            EditorUtility.SetDirty(input);

        }


        [ContextMenu("Create Event Listeners")]
        void CreateEventListeners()
        {
            MEvent RCSetAim = MTools.GetInstance<MEvent>("RC Set Aim");
            MEvent RCMainAttack = MTools.GetInstance<MEvent>("RC Main Attack");
            MEvent RCSecondaryAttack = MTools.GetInstance<MEvent>("RC Secondary Attack");

            MEventListener listener = GetComponent<MEventListener>();

            if (listener == null) listener = gameObject.AddComponent<MEventListener>();
            if (listener.Events == null) listener.Events = new List<MEventItemListener>();


            //*******************//
            if (listener.Events.Find(item => item.Event == RCSetAim) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = RCSetAim,
                    useVoid = false,
                    useBool = true,
                };

                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool, Aim_Set);
                listener.Events.Add(item);

                Debug.Log("<B>RC Set Aim</B> Added to the Event Listeners");
            }

            //*******************//
            if (listener.Events.Find(item => item.Event == RCMainAttack) == null)
            {
                var item = new MEventItemListener()
                {
                    Event = RCMainAttack,
                    useVoid = false,
                    useBool = true
                };

                UnityEditor.Events.UnityEventTools.AddPersistentListener(item.ResponseBool, MainAttack);
                listener.Events.Add(item);

                Debug.Log("<B>RC MainAttack</B> Added to the Event Listeners");
            }
        }

#endif
        #endregion
    }




    #region Inspector

#if UNITY_EDITOR
    [CustomEditor(typeof(MWeaponManager))]
    public class MWeaponManagerEditor : Editor
    {
        GUIStyle StyleGreen => MTools.Style(new Color(0f, 1f, 0.5f, 0.3f));

        MWeaponManager M;

        private SerializedProperty
            debug, LeftHandEquipPoint, RightHandEquipPoint,/* UseDefaultIK,*/
            Anim,
            OnEquipWeapon, OnUnequipWeapon, OnCombatMode, OnCanAim, OnWeaponAction,
            m_CombatLayerPath, m_CombatLayerName,
           // OnMainAttackStart,

            DisableModes, ExitOnModes, ExitOnState, ExitFast,// IKLerp,

            holsters, UseExternal, UseHolsters,
            HolsterTime, DestroyOnUnequip, InstantiateOnEquip,
            StoreAfter, start_weapon, m_IgnoreDraw, aim, m_IgnoreStore,

            animal, comboManager, DrawWeapon, StoreWeapon, UnarmedMode, IgnoreHandOffset,

            m_AimInput, m_ReloadInput, m_MainAttack, m_SecondAttack,  // m_SpecialAttack,

          Editor_Tabs1, Editor_Tabs2, m_WeaponType, m_LeftHand, m_IKFreeHand, m_IKAim,  //m_WeaponPower, ExitCombatOnDismount;

            m_ModeOn, m_Mode, m_WeaponPower
        ;

        private ReorderableList holsterReordable;

        private void OnEnable()
        {
            M = (MWeaponManager)target;

            // StyleGreen = 

            FindProperties();

            holsterReordable = new ReorderableList(serializedObject, holsters, true, true, true, true)
            {
                drawElementCallback = DrawHolsterElement,
                drawHeaderCallback = DrawHolsterHeader
            };

        }
        private void FindProperties()
        {
            animal = serializedObject.FindProperty("animal");


            m_AimInput = serializedObject.FindProperty("m_AimInput");
            m_ReloadInput = serializedObject.FindProperty("m_ReloadInput");
            m_MainAttack = serializedObject.FindProperty("m_MainAttack");
            m_SecondAttack = serializedObject.FindProperty("m_SecondAttack");
            //m_SpecialAttack = serializedObject.FindProperty("m_SpecialAttack");

             

            DisableModes = serializedObject.FindProperty("DisableModes");
            ExitOnState = serializedObject.FindProperty("ExitOnState");
            ExitOnModes = serializedObject.FindProperty("ExitOnModes");
            ExitFast = serializedObject.FindProperty("ExitFast");

            Anim = serializedObject.FindProperty("anim");

            start_weapon = serializedObject.FindProperty("startWeapon");
            aim = serializedObject.FindProperty("aim");

            IgnoreHandOffset = serializedObject.FindProperty("IgnoreHandOffset");
            comboManager = serializedObject.FindProperty("comboManager");
            DrawWeapon = serializedObject.FindProperty("DrawWeaponModeID");
            StoreWeapon = serializedObject.FindProperty("StoreWeaponModeID");
            UnarmedMode = serializedObject.FindProperty("UnarmedModeID");

            // UseDefaultIK = serializedObject.FindProperty("UseDefaultIK");
            StoreAfter = serializedObject.FindProperty("StoreAfter");
            m_IgnoreDraw = serializedObject.FindProperty("m_IgnoreDraw");
            m_IgnoreStore = serializedObject.FindProperty("m_IgnoreStore");
           // DisableAim = serializedObject.FindProperty("DisableAim");


            #region Animator Parameters
            m_WeaponType = serializedObject.FindProperty("m_WeaponType");
            m_LeftHand = serializedObject.FindProperty("m_LeftHand");
            m_IKFreeHand = serializedObject.FindProperty("m_IKFreeHand");
            m_IKAim = serializedObject.FindProperty("m_IKAim");
            m_ModeOn = serializedObject.FindProperty("m_ModeOn");
            m_Mode = serializedObject.FindProperty("m_Mode");
            m_WeaponPower = serializedObject.FindProperty("m_WeaponPower");
            #endregion


            holsters = serializedObject.FindProperty("holsters");
            //  DefaultHolster = serializedObject.FindProperty("DefaultHolster");
            HolsterTime = serializedObject.FindProperty("HolsterTime");



            // ExitCombatOnDismount = serializedObject.FindProperty("ExitCombatOnDismount");

            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            Editor_Tabs2 = serializedObject.FindProperty("Editor_Tabs2");
            //HitMask = serializedObject.FindProperty("HitMask"); 


            LeftHandEquipPoint = serializedObject.FindProperty("LeftHandEquipPoint");
            RightHandEquipPoint = serializedObject.FindProperty("RightHandEquipPoint");

            OnCombatMode = serializedObject.FindProperty("OnCombatMode");
            OnCanAim = serializedObject.FindProperty("OnCanAim");
            OnEquipWeapon = serializedObject.FindProperty("OnEquipWeapon");
            OnUnequipWeapon = serializedObject.FindProperty("OnUnequipWeapon");
            OnWeaponAction = serializedObject.FindProperty("OnWeaponAction");
            //   OnMainAttackStart = serializedObject.FindProperty("OnMainAttackStart");


            m_CombatLayerName = serializedObject.FindProperty("m_CombatLayerName");
            m_CombatLayerPath = serializedObject.FindProperty("m_CombatLayerPath");




            UseExternal = serializedObject.FindProperty("UseExternal");
            UseHolsters = serializedObject.FindProperty("UseHolsters");


            DestroyOnUnequip = serializedObject.FindProperty("DestroyOnUnequip");
            InstantiateOnEquip = serializedObject.FindProperty("InstantiateOnEquip");


            debug = serializedObject.FindProperty("debug");
        }

        private void DrawHolsterHeader(Rect rect)
        {
            var IDRect = new Rect(rect);
            IDRect.height = EditorGUIUtility.singleLineHeight;
            IDRect.width *= 0.5f;
            IDRect.x += 18;
            EditorGUI.LabelField(IDRect, "   Holster ID");
            IDRect.x += IDRect.width - 10;
            IDRect.width -= 18;
            EditorGUI.LabelField(IDRect, "   Weapon ");

            //var buttonRect = new Rect(rect) { x = rect.width - 30, width = 63, y = rect.y - 1 };
            //var oldColor = GUI.backgroundColor;
            //GUI.backgroundColor = new Color(.8f, .8f, 1f, 1f);

            //if (GUI.Button(buttonRect, new GUIContent("Weapon", "Check for Weapons on the Holsters.\nIf the weapons are prefab it will instantiate them on the scene"), EditorStyles.miniButton))
            //{
            //    M.ValidateWeaponsChilds();
            //    EditorUtility.SetDirty(target);
            //}


            //GUI.backgroundColor = oldColor;
        }

        private void DrawHolsterElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;

            var holster = holsters.GetArrayElementAtIndex(index);

            var ID = holster.FindPropertyRelative("ID");
            //  var t = holster.FindPropertyRelative("Transform");
            var Weapon = holster.FindPropertyRelative("Weapon");



            var IDRect = new Rect(rect)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            IDRect.width *= 0.5f;
            IDRect.x += 18;
            IDRect.width -= 10;
            EditorGUI.PropertyField(IDRect, ID, GUIContent.none);





            var pre = "";
            var tooltip = "";
            var oldColor = GUI.backgroundColor;
            var newColor = oldColor;

            var weaponObj = Weapon.objectReferenceValue as Component;
            if (weaponObj && weaponObj.gameObject != null)
            {
                if (weaponObj.gameObject.IsPrefab())
                {
                    newColor = Color.green;
                    pre = "[P]";
                    tooltip = "The Weapon is a Prefab. It will be instantiated on start";
                }
                else
                {
                    pre = "[S]";
                    tooltip = "The Weapon is in the scene";
                }
            }


            IDRect.x += IDRect.width + 10;
            IDRect.width -= 25;


            GUI.backgroundColor = newColor;
            EditorGUI.PropertyField(IDRect, Weapon, GUIContent.none);
            GUI.backgroundColor = oldColor;

            var lbRect = new Rect(rect)
            {
                height = EditorGUIUtility.singleLineHeight,
                width = 20,
                x = rect.width + 25,
            };

            EditorGUI.LabelField(lbRect, new GUIContent(pre, tooltip));
        }


        /// <summary> Draws all of the fields for the selected ability. </summary>

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("System for using weapons while a character is grounded & riding");

            //if (!Application.isPlaying)
            //    AddLayer();

            if (helpboxStyle == null)
            {
                helpboxStyle = new GUIStyle(MTools.StyleGray)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true
                };
                helpboxStyle.normal.textColor = EditorStyles.boldLabel.normal.textColor;
            }


            var Hols = "Holsters";

            if (!M.UseHolsters) Hols = "External";

            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, new string[] { "General", Hols, "AC", "Events" });

            if (Editor_Tabs1.intValue != 5) Editor_Tabs2.intValue = 5;

            Editor_Tabs2.intValue = GUILayout.Toolbar(Editor_Tabs2.intValue, new string[] { "Inputs", "Advanced", "Animator", "Debug" });

            if (Editor_Tabs2.intValue != 5) Editor_Tabs1.intValue = 5;

            //First Tabs
            int Selection = Editor_Tabs1.intValue;

            if (Selection == 0) DrawGeneral();
            else if (Selection == 1) ShowHolsters();
            else if (Selection == 2) DrawAC();
            else if (Selection == 3) DrawEvents();


            //2nd Tabs
            Selection = Editor_Tabs2.intValue;

            if (Selection == 0) DrawInputs();
            else if (Selection == 1) DrawAdvanced();
            else if (Selection == 2) DrawAnimator();
            else if (Selection == 3) DrawDebug();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInputs()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(m_AimInput);
                EditorGUILayout.PropertyField(m_ReloadInput);
                EditorGUILayout.PropertyField(m_MainAttack);
                EditorGUILayout.PropertyField(m_SecondAttack);
            }

            if (M.UseHolsters)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    for (int i = 0; i < holsters.arraySize; i++)
                    {
                        var element = holsters.GetArrayElementAtIndex(i);
                        var input = element.FindPropertyRelative("Input");
                        EditorGUILayout.PropertyField(input, new GUIContent($"Holster [{i}] Input"));
                    }
                }
            }

        }

        private void DrawAC()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(animal);

                if (animal.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(DrawWeapon, new GUIContent("Mode [Draw Weapon]"));
                    EditorGUILayout.PropertyField(StoreWeapon, new GUIContent("Mode [Store Weapon]"));
                    EditorGUILayout.PropertyField(UnarmedMode, new GUIContent("Mode [Unarmed]"));

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(comboManager);
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(DisableModes);
                    EditorGUI.indentLevel--;

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(ExitOnState);
                    EditorGUI.indentLevel--; 
                    
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(ExitOnModes);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(ExitFast);
                }
            }
        }

        private void DrawAnimator()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(Anim);
                EditorGUILayout.LabelField("Animator Parameters", EditorStyles.boldLabel);
                //     DisplayParam(m_WeaponAction, UnityEngine.AnimatorControllerParameterType.Int);
                MalbersEditor.DisplayParam(M.Anim, m_WeaponType, UnityEngine.AnimatorControllerParameterType.Int);
                MalbersEditor.DisplayParam(M.Anim, m_LeftHand, UnityEngine.AnimatorControllerParameterType.Bool);
                MalbersEditor.DisplayParam(M.Anim, m_IKFreeHand, UnityEngine.AnimatorControllerParameterType.Float);
                MalbersEditor.DisplayParam(M.Anim, m_IKAim, UnityEngine.AnimatorControllerParameterType.Float);

                if (M.animal == null)
                {
                    MalbersEditor.DisplayParam(M.Anim, m_ModeOn, UnityEngine.AnimatorControllerParameterType.Trigger);
                    MalbersEditor.DisplayParam(M.Anim, m_Mode, UnityEngine.AnimatorControllerParameterType.Int);
                    MalbersEditor.DisplayParam(M.Anim, m_WeaponPower, UnityEngine.AnimatorControllerParameterType.Float);
                }
            }


            EditorGUILayout.LabelField("Weapon Action Values:" +
              "\nNone = 0" +
              "\nIdle = 100" +
              "\nAim = 97" +
              "\nAttack = 101" +
              "\nReload = 96" +
              //"\nCharge_Hold = 95" +
              "\nDraw = 99" +
              "\nStore = 98", helpboxStyle);
        }
        private GUIStyle helpboxStyle;

        private void DrawDebug()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(debug, new GUIContent("Debug", ""));

                if (Application.isPlaying)
                {
                    Repaint();
                    EditorGUI.BeginDisabledGroup(true);

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        if (M.HasAnimal) EditorGUILayout.Toggle("Preparing Mode", M.animal.IsPreparingMode);
                        EditorGUILayout.Toggle("Is In Combat mode", M.CombatMode);
                        EditorGUILayout.Toggle("Is Riding: ", M.IsRiding);
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Toggle("Is Aiming", M.Aim);
                        if (M.Aimer != null) EditorGUILayout.Toggle("Aiming Side", M.AimingSide);
                    }
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.FloatField("IK Aim", M.IKAimWeight);
                        EditorGUILayout.FloatField("IK 2Hands", M.IK2HandsWeight);
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.EnumPopup("Weapon Action: ", M.WeaponAction);
                        EditorGUILayout.IntField("Anim Action: ", M.WeaponAnimAction);
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.ObjectField("Active Weapon:  ", M.Weapon, typeof(MWeapon), false);

                        if (M.Weapon)
                        {
                            EditorGUILayout.ObjectField("Active Holster:  ", M.ActiveHolster?.ID, typeof(HolsterID), false);
                            EditorGUILayout.ObjectField("Weapon.Type:  ", M.Weapon?.WeaponType, typeof(WeaponID), false);
                            EditorGUILayout.Toggle("Weapon.Active: ", M.Weapon.Enabled);
                            EditorGUILayout.Toggle("Weapon.Input: ", M.Weapon.Input);
                            EditorGUILayout.Toggle("Weapon.IsAiming: ", M.Weapon.IsAiming);
                            EditorGUILayout.Toggle("Weapon.RightHand: ", M.Weapon.IsRightHanded);
                            EditorGUILayout.Toggle("Weapon.Ready: ", M.Weapon.IsReady);
                            EditorGUILayout.Toggle("Weapon.CanAttack: ", M.Weapon.CanAttack);
                            EditorGUILayout.Toggle("Weapon.IsAttacking: ", M.Weapon.IsAttacking);
                            EditorGUILayout.Toggle("Weapon.IsReloading: ", M.Weapon.IsReloading);
                            EditorGUILayout.Toggle("Weapon.CanCharge: ", M.Weapon.CanCharge);
                            EditorGUILayout.Toggle("Weapon.HasAmmo: ", M.Weapon.HasAmmo);

                            if (M.Weapon.CanCharge)
                            {
                                EditorGUILayout.Toggle("Weapon.IsCharging: ", M.Weapon.IsCharging);
                                EditorGUILayout.FloatField("Weapon.Power: ", M.Weapon.Power);
                                EditorGUILayout.FloatField("Weapon.ChargeNorm: ", M.Weapon.ChargedNormalized);
                            }
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void ShowHolsters()
        {
            EditorGUILayout.Space();
            var selection = UseHolsters.boolValue ? 0 : 1;
            selection = GUILayout.Toolbar(selection, new string[] { "Use Holsters", "Use External" });
            UseExternal.boolValue = selection != 0;
            UseHolsters.boolValue = selection == 0;


            if (styleDesc == null)
            {
                styleDesc = new GUIStyle(StyleGreen)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true
                };

                styleDesc.normal.textColor = EditorStyles.label.normal.textColor;
            }


            if (UseExternal.boolValue)
            {
                EditorGUILayout.LabelField("Use the Method <Equip_External(GameObject)> to equip weapons", styleDesc);
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(InstantiateOnEquip);
                    EditorGUILayout.PropertyField(DestroyOnUnequip);
                }
            }

            //Holder Stufss
            if (M.UseHolsters)
            {
                EditorGUILayout.LabelField("The weapons are child of the Holsters", styleDesc);

                // EditorGUILayout.PropertyField(DefaultHolster, new GUIContent("Default Holster", "Default  Holster used when no Holster is selected"));
                EditorGUILayout.PropertyField(HolsterTime, new GUIContent("Holster Time", "Time to smooth parent the weapon to the Hand and Holster"));

                holsterReordable.DoLayoutList();

                if (holsterReordable.index != -1)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        var element = holsters.GetArrayElementAtIndex(holsterReordable.index);
                        var Input = element.FindPropertyRelative("Input");
                        var Slots = element.FindPropertyRelative("Slots");
                        EditorGUILayout.PropertyField(Input);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(Slots, true);
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }

        private void DrawGeneral()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var dC = start_weapon.displayName;
                if (M.StartWeapon && M.StartWeapon.IsPrefab())
                {
                    dC += " [Prefab]";
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(start_weapon, new GUIContent(dC, start_weapon.tooltip));
                    MalbersEditor.DrawDebugIcon(debug);
                }

                //using (var cc = new EditorGUI.ChangeCheckScope())
                //{
                //    EditorGUILayout.PropertyField(aim);
                //    if (cc.changed && Application.isPlaying)
                //    {
                //        serializedObject.ApplyModifiedProperties();
                //        M.SetAimLogic(M.Aim);
                //        //Debug.Log("M.Aim = " + M.Aim);
                //    }
                //}


                EditorGUILayout.PropertyField(m_IgnoreDraw);
                EditorGUILayout.PropertyField(m_IgnoreStore);
                EditorGUILayout.PropertyField(StoreAfter);
            }

            EquipWeaponPoints();
        }

        private void EquipWeaponPoints()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                LeftHandEquipPoint.isExpanded = MalbersEditor.Foldout(LeftHandEquipPoint.isExpanded, "Weapon Equip Points");

                if (LeftHandEquipPoint.isExpanded)
                {
                    EditorGUILayout.PropertyField(LeftHandEquipPoint, new GUIContent("Left Hand"));
                    EditorGUILayout.PropertyField(RightHandEquipPoint, new GUIContent("Right Hand"));
                    EditorGUILayout.PropertyField(IgnoreHandOffset);
                }


                //Animator Anim = M.GetComponent<Animator>();
                //if (Anim)
                //{
                //    if (LeftHandEquipPoint.objectReferenceValue == null)
                //    {
                //        M.LeftHandEquipPoint = Anim.GetBoneTransform(HumanBodyBones.LeftHand);
                //    }

                //    if (RightHandEquipPoint.objectReferenceValue == null)
                //    {
                //        M.RightHandEquipPoint = Anim.GetBoneTransform(HumanBodyBones.RightHand);
                //    }
                //}
            }

        }
        private void DrawAdvanced()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
              if (M.animal == null)
                    AddLayers();


                EditorGUILayout.LabelField(new GUIContent("Combat Animator", "Location and Name of the Combat while Riding Layer, on the Resource folder"), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_CombatLayerName, new GUIContent("Layer Name", "Name of the Riding Combat Layer"));
                EditorGUILayout.PropertyField(m_CombatLayerPath, new GUIContent("Animator Path", "Path of the Combat Layer on the Resource Folder"));
            
            }
        }


        private GUIStyle styleDesc;

        void DrawEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(OnCombatMode);
                EditorGUILayout.PropertyField(OnCanAim);
                EditorGUILayout.PropertyField(OnEquipWeapon);
                EditorGUILayout.PropertyField(OnUnequipWeapon);
              //  EditorGUILayout.PropertyField(OnMainAttackStart);
                EditorGUILayout.PropertyField(OnWeaponAction);
            }

            if (M.UseHolsters)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    for (int i = 0; i < holsters.arraySize; i++)
                    {
                        var element = holsters.GetArrayElementAtIndex(i);
                        var input = element.FindPropertyRelative("OnWeaponInHolster");
                        EditorGUILayout.PropertyField(input, new GUIContent($" On Weapon In Holster [{i}]"));
                    }
                }
            }
        }

        void AddLayerCombat(UnityEditor.Animations.AnimatorController CurrentAnimator)
        {
            var m_CombatLayerPath = serializedObject.FindProperty("m_CombatLayerPath");

            UnityEditor.Animations.AnimatorController MountAnimator = 
                Resources.Load<UnityEditor.Animations.AnimatorController>(m_CombatLayerPath.stringValue);

            MTools.AddParametersOnAnimator(CurrentAnimator, MountAnimator);

            foreach (var item in MountAnimator.layers)
                CurrentAnimator.AddLayer(item);
        }

        private void AddLayers()
        {
            Animator anim = M.GetComponent<Animator>();

            if (anim)
            {
                var controller = (UnityEditor.Animations.AnimatorController)anim.runtimeAnimatorController;

                if (controller)
                {
                    var layers = controller.layers.ToList();

                    var defaultColor = GUI.color;
                    GUI.color = Color.green;

                    var ST = new GUIStyle(EditorStyles.miniButtonMid) { fontStyle = FontStyle.Bold };

                  

                    if (layers.Find(layer => layer.name == m_CombatLayerName.stringValue) == null)
                    {
                        if (GUILayout.Button(new GUIContent("Add Combat Layers",
                            "There's no [Combat] layers on the current Animator. " +
                            "This will add all the Animator Parameters and States needed for the using weapons while Riding"), ST))
                        {
                            AddLayerCombat(controller);
                        }
                    }
                    GUI.color = defaultColor;

                }
            }
        }
    }
#endif
    #endregion
}