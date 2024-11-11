using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [CustomEditor(typeof(MAnimal))]
    public class MAnimalEditor : Editor
    {
        public readonly string version = "Animal Controller [v1.4.0a]";

        public static GUIStyle StyleGray => MTools.Style(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        //  private GUIContent plus, minus;


        private List<Type> StatesType = new List<Type>();
        private ReorderableList Reo_List_States;
        private ReorderableList Reo_List_Modes;
        private ReorderableList Reo_List_Stances;
        private ReorderableList Reo_List_Speeds;
        private Dictionary<string, ReorderableList> innerListDict = new Dictionary<string, ReorderableList>();
        private Dictionary<string, Editor> State_Editor = new Dictionary<string, Editor>();


        private int SpeedTabs = 0;
        private int SelectedSpeed = -1;

        SerializedProperty
            S_StateList, S_PivotsList, Height, S_Mode_List, Editor_Tabs1, Editor_Tabs2, StartWithMode, 
            OnEnterExitStances, OnEnterExitStates, OnEnterExitSpeeds,
            RB, Anim, NoParent,
            m_Vertical, m_Horizontal, m_StateFloat, m_ModeStatus, m_State, m_StateStatus, m_StateExitStatus, m_LastState, m_Mode, m_Grounded, m_Movement, m_Random, m_ModePower,
            m_SpeedMultiplier, m_UpDown, m_DeltaUpDown, m_StateOn,  m_Sprint, m_ModeOn,// m_StanceOn,
            currentStance, defaultStance, Stances_List,
            m_Stance, m_LastStance, m_Slope, deepSlope, m_Type, m_StateTime, m_TargetAngle, m_StrafeAnim,
            lockInput, lockMovement, Rotator, AlignLoop, animalType, RayCastRadius, MainCamera, sleep, m_gravityTime, m_gravityTimeLimit, RootBone,

             m_CanStrafe, Aimer, m_strafe, OnStrafe, m_StrafeNormalize,  /*FallForward, */m_StrafeLerp, OrientToGround,


            alwaysForward, AnimatorSpeed, m_TimeMultiplier,
            OnMovementLocked, OnMovementDetected, OnMaxSlopeReached,
            OnInputLocked, OnSprintEnabled, OnGrounded, OnStanceChange, OnStateChange, OnModeStart, OnModeEnd, OnTeleport,
            OnSpeedChange, OnAnimationChange, GroundLayer, maxAngleSlope, AlignPosLerp, AlignPosDelta, AlignRotDelta,
            AlignRotLerp, m_gravity, m_gravityPower, useCameraUp, ground_Changes_Gravity,
             useSprintGlobal, SmoothVertical, 
            TurnMultiplier, TurnLimit, InPlaceDamp,
            Player, OverrideStartState, CloneStates, S_Speed_List, UseCameraInput,
            LockUpDownMovement, LockHorizontalMovement, LockForwardMovement, DebreeTag;



        //EditorStuff
        SerializedProperty
             ShowStateInInspector, Ability_Tabs, Mode_Tabs1,
              SelectedMode, SelectedStance, SelectedState, showPivots,//   m_TargetHorizontal,
                Editor_EventTabs/*, showExposedVariables, InteractionArea, m_InteracterID,*/;

        MAnimal m;
        // private MonoScript script;
        private GenericMenu addMenu;
        private GUIStyle DescriptionStyle;

        private void FindSerializedProperties()
        {
            S_PivotsList = serializedObject.FindProperty("pivots");
            sleep = serializedObject.FindProperty("sleep");
            S_Mode_List = serializedObject.FindProperty("modes");
            Ability_Tabs = serializedObject.FindProperty("Ability_Tabs");
            Mode_Tabs1 = serializedObject.FindProperty("Mode_Tabs1");

            ground_Changes_Gravity = serializedObject.FindProperty("ground_Changes_Gravity");
            Stances_List = serializedObject.FindProperty("Stances");


            SelectedMode = serializedObject.FindProperty("SelectedMode");
            NoParent = serializedObject.FindProperty("NoParent");


            DebreeTag = serializedObject.FindProperty("DebrisTag");
            SelectedState = serializedObject.FindProperty("SelectedState");
            SelectedStance = serializedObject.FindProperty("SelectedStance");
            ShowStateInInspector = serializedObject.FindProperty("ShowStateInInspector");
          
            
       

           

            m_CanStrafe = serializedObject.FindProperty("m_CanStrafe");
            m_StrafeNormalize = serializedObject.FindProperty("m_StrafeNormalize");
            m_strafe = serializedObject.FindProperty("m_strafe");
            Aimer = serializedObject.FindProperty("Aimer");
            OnStrafe = serializedObject.FindProperty("OnStrafe");
            m_StrafeLerp = serializedObject.FindProperty("m_StrafeLerp");

            alwaysForward = serializedObject.FindProperty("alwaysForward");

            MainCamera = serializedObject.FindProperty("m_MainCamera");
          

            S_Speed_List = serializedObject.FindProperty("speedSets");

            currentStance = serializedObject.FindProperty("currentStance");
            defaultStance = serializedObject.FindProperty("defaultStance");


            RB = serializedObject.FindProperty("RB");
            Anim = serializedObject.FindProperty("Anim");

            UseCameraInput = serializedObject.FindProperty("useCameraInput");
            useCameraUp = serializedObject.FindProperty("useCameraUp");
            StartWithMode = serializedObject.FindProperty("StartWithMode");

            OnEnterExitStates = serializedObject.FindProperty("OnEnterExitStates");
            OnEnterExitStances = serializedObject.FindProperty("OnEnterExitStances");
            OnEnterExitSpeeds = serializedObject.FindProperty("OnEnterExitSpeeds");

            Height = serializedObject.FindProperty("height");
            // ModeIndexSelected = serializedObject.FindProperty("ModeIndexSelected");

            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            Editor_Tabs2 = serializedObject.FindProperty("Editor_Tabs2");

            m_Vertical = serializedObject.FindProperty("m_Vertical");
            // Center = serializedObject.FindProperty("Center");
            m_Horizontal = serializedObject.FindProperty("m_Horizontal");
            m_StateFloat = serializedObject.FindProperty("m_StateFloat");
            m_ModeStatus = serializedObject.FindProperty("m_ModeStatus");
            m_State = serializedObject.FindProperty("m_State");
            m_StateStatus = serializedObject.FindProperty("m_StateStatus");
            m_StateExitStatus = serializedObject.FindProperty("m_StateExitStatus");
            m_LastState = serializedObject.FindProperty("m_LastState");
            m_Mode = serializedObject.FindProperty("m_Mode");
            m_Grounded = serializedObject.FindProperty("m_Grounded");
            m_Movement = serializedObject.FindProperty("m_Movement");
            m_Random = serializedObject.FindProperty("m_Random");
            m_ModePower = serializedObject.FindProperty("m_ModePower");
            m_StrafeAnim = serializedObject.FindProperty("m_Strafe");
            m_SpeedMultiplier = serializedObject.FindProperty("m_SpeedMultiplier");

            m_UpDown = serializedObject.FindProperty("m_UpDown");
            m_DeltaUpDown = serializedObject.FindProperty("m_DeltaUpDown");

            m_StateOn = serializedObject.FindProperty("m_StateOn");
            //m_StanceOn = serializedObject.FindProperty("m_StanceOn");
            m_ModeOn = serializedObject.FindProperty("m_ModeOn");
            m_Sprint = serializedObject.FindProperty("m_Sprint");

            m_Stance = serializedObject.FindProperty("m_Stance");
            m_LastStance = serializedObject.FindProperty("m_LastStance");

            m_Slope = serializedObject.FindProperty("m_Slope");
            m_Type = serializedObject.FindProperty("m_Type");
            m_StateTime = serializedObject.FindProperty("m_StateTime");
            m_TargetAngle = serializedObject.FindProperty("m_DeltaAngle");
            lockInput = serializedObject.FindProperty("lockInput");
            lockMovement = serializedObject.FindProperty("lockMovement");
            Rotator = serializedObject.FindProperty("Rotator");
            animalType = serializedObject.FindProperty("animalType");
            RayCastRadius = serializedObject.FindProperty("rayCastRadius");
            AlignLoop = serializedObject.FindProperty("AlignLoop");
            AnimatorSpeed = serializedObject.FindProperty("AnimatorSpeed");
            m_TimeMultiplier = serializedObject.FindProperty("m_TimeMultiplier");
           // m_TargetHorizontal = serializedObject.FindProperty("m_TargetHorizontal");

            LockForwardMovement = serializedObject.FindProperty("lockForwardMovement");
            LockHorizontalMovement = serializedObject.FindProperty("lockHorizontalMovement");
            LockUpDownMovement = serializedObject.FindProperty("lockUpDownMovement");



            OnMaxSlopeReached = serializedObject.FindProperty("OnMaxSlopeReached");
            OnMovementLocked = serializedObject.FindProperty("OnMovementLocked");
            OnMovementDetected = serializedObject.FindProperty("OnMovementDetected");
            OnInputLocked = serializedObject.FindProperty("OnInputLocked");
            OnSprintEnabled = serializedObject.FindProperty("OnSprintEnabled");
            OnGrounded = serializedObject.FindProperty("OnGrounded");
            OnStanceChange = serializedObject.FindProperty("OnStanceChange");
            OnStateChange = serializedObject.FindProperty("OnStateChange");
            OnModeStart = serializedObject.FindProperty("OnModeStart");

            OnModeEnd = serializedObject.FindProperty("OnModeEnd");
            OnSpeedChange = serializedObject.FindProperty("OnSpeedChange");
            OnTeleport = serializedObject.FindProperty("OnTeleport");
            OnAnimationChange = serializedObject.FindProperty("OnAnimationChange");


            showPivots = serializedObject.FindProperty("showPivots");
            // ShowpivotColor = serializedObject.FindProperty("ShowpivotColor");
            GroundLayer = serializedObject.FindProperty("groundLayer");
            
            maxAngleSlope = serializedObject.FindProperty("maxAngleSlope");
            deepSlope = serializedObject.FindProperty("m_deepSlope");

            AlignPosLerp = serializedObject.FindProperty("AlignPosLerp");
            OrientToGround = serializedObject.FindProperty("m_OrientToGround");
            AlignPosDelta = serializedObject.FindProperty("AlignPosDelta");
            AlignRotDelta = serializedObject.FindProperty("AlignRotDelta");
            AlignRotLerp = serializedObject.FindProperty("AlignRotLerp");


            m_gravity = serializedObject.FindProperty("m_gravityDir");
            m_gravityPower = serializedObject.FindProperty("m_gravityPower");
            m_gravityTime = serializedObject.FindProperty("m_gravityTime");
            m_gravityTimeLimit = serializedObject.FindProperty("m_gravityTimeLimit");

            useSprintGlobal = serializedObject.FindProperty("useSprintGlobal");
            SmoothVertical = serializedObject.FindProperty("SmoothVertical");
            TurnMultiplier = serializedObject.FindProperty("TurnMultiplier");
            TurnLimit = serializedObject.FindProperty("TurnLimit");
            InPlaceDamp = serializedObject.FindProperty("inPlaceDamp");


            Player = serializedObject.FindProperty("isPlayer");
            OverrideStartState = serializedObject.FindProperty("OverrideStartState");
            CloneStates = serializedObject.FindProperty("CloneStates");
            RootBone = serializedObject.FindProperty("RootBone");
            Editor_EventTabs = serializedObject.FindProperty("Editor_EventTabs");
             
        }

        private void OnEnable()
        {
            m = (MAnimal)target;
            //  script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            FindSerializedProperties();


            StatesType.Clear();
            StatesType = MTools.GetAllTypes<State>();

            S_StateList = serializedObject.FindProperty("states");

            Reo_List_States = new ReorderableList(serializedObject, S_StateList, true, true, true, true)
            {
                drawHeaderCallback = Draw_Header_State,
                drawElementCallback = Draw_Element_State,
                // onReorderCallback = OnReorderCallback_States,
                onReorderCallbackWithDetails = OnReorderCallback_States_Details,
                onAddCallback = OnAddCallback_State,
                onRemoveCallback = OnRemove_State,
                onSelectCallback = Selected_State,
            };

            Reo_List_Modes = new ReorderableList(serializedObject, S_Mode_List, true, true, true, true)
            {
                drawElementCallback = Draw_Element_Modes,
                drawHeaderCallback = Draw_Header_Modes,
                onAddCallback = OnAdd_Modes,
                onRemoveCallback = OnRemoveCallback_Mode,
                onSelectCallback = Selected_Mode,
            };

            Reo_List_Speeds = new ReorderableList(serializedObject, S_Speed_List, true, true, true, true)
            {
                drawElementCallback = Draw_Element_Speed,
                drawHeaderCallback = Draw_Header_Speed,
                onAddCallback = OnAddCallback_Speeds,
                onRemoveCallback = OnRemoveCallback_Speeds,

                onSelectCallback = (list) =>
                {
                    SelectedSpeed = list.index;
                }

            };

            Reordable_Stances();


            if (m.states != null && m.states.Count > 0 && m.states[0] != null && m.states[0].Priority == 0) //Means that the priorities are not set so check once just in case
                OnReorderCallback_States(null);
           
        }

        private void CheckHelpBox()
        {
            if (DescriptionStyle == null)
            {
                DescriptionStyle = new GUIStyle(MTools.StyleGray)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true
                };
                DescriptionStyle.normal.textColor = EditorStyles.boldLabel.normal.textColor;
            }
        }


        private void Selected_Mode(ReorderableList list) => SelectedMode.intValue = list.index;
        private void Selected_State(ReorderableList list) => SelectedState.intValue = list.index;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CheckHelpBox();
            var descri = version + ((Application.isPlaying && m.Sleep) ? " ****[SLEEP]****" : string.Empty);

            MalbersEditor.DrawDescription(descri);

           //using (new GUILayout.VerticalScope(MalbersEditor.StyleGray))
            {
                Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, new string[] { "General", "Speeds", "States", "Modes" });
                if (Editor_Tabs1.intValue != 4) Editor_Tabs2.intValue = 4;

                Editor_Tabs2.intValue = GUILayout.Toolbar(Editor_Tabs2.intValue, new string[] { "Advanced", "Stances", "Events", "Debug" });
                if (Editor_Tabs2.intValue != 4) Editor_Tabs1.intValue = 4;

                //First Tabs
                int Selection = Editor_Tabs1.intValue;
                if (Selection == 0) ShowGeneral();
                else if (Selection == 1) ShowSpeeds();
                else if (Selection == 2) ShowStates();
                else if (Selection == 3) ShowModes();


                //2nd Tabs
                Selection = Editor_Tabs2.intValue;
                if (Selection == 0) ShowAdvanced();
                else if (Selection == 1) ShowStances();
                else if (Selection == 2) ShowEvents();
                else if (Selection == 3) ShowDebug();

            }
            serializedObject.ApplyModifiedProperties();
        }


        private void Reordable_Stances()
        {
            Reo_List_Stances = new ReorderableList(serializedObject, Stances_List, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    var r = new Rect(rect);
                    r.x += 40;
                    r.width = 90;
                    EditorGUI.LabelField(r, new GUIContent("Stances", "Stances allowed in this Controller"));

                    var activeRect = rect;
                    activeRect.width -= 20;
                    activeRect.x += 20;
                    var IDRect = new Rect(activeRect.width + 35, activeRect.y, 35, activeRect.height);

                    EditorGUI.LabelField(IDRect, new GUIContent("ID", "Mode ID:\n Numerical ID value for the Mode"));
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    if (Stances_List.arraySize <= index) return;

                    EditorGUI.BeginChangeCheck();
                    {
                        var ModeProperty = Stances_List.GetArrayElementAtIndex(index);
                        var ID = ModeProperty.FindPropertyRelative("ID");

                        var Active = ModeProperty.FindPropertyRelative("enabled");
                        var ConstValue = Active.FindPropertyRelative("ConstantValue");
                        var VarValue = Active.FindPropertyRelative("Variable");
                        var useConstant = Active.FindPropertyRelative("UseConstant").boolValue;
                        BoolVar variable = VarValue.objectReferenceValue as BoolVar;

                        var rectan = new Rect(rect);
                        rectan.width -= 20;
                        rectan.x += 20;
                        rectan.y -= 2;

                        var ActiveRect = new Rect(rect.x, rect.y - 2, 20, rect.height);
                        var IDRect = new Rect(rect.x + 40, rect.y, rect.width - 70, EditorGUIUtility.singleLineHeight);
                        var Rect_Label = new Rect(rect.width - 40, rect.y, 60, EditorGUIUtility.singleLineHeight);
                        if (Application.isPlaying) IDRect.width -= 60f;

                        if (useConstant)
                        {
                            ConstValue.boolValue = EditorGUI.Toggle(ActiveRect, GUIContent.none, ConstValue.boolValue);

                            if (variable != null)
                            {
                                variable.Value = ConstValue.boolValue;
                                EditorUtility.SetDirty(variable);
                            }
                        }
                        else
                        {
                            if (variable != null)
                            {
                                variable.Value = EditorGUI.Toggle(ActiveRect, GUIContent.none, variable.Value);
                                ConstValue.boolValue = variable.Value;
                            }
                            else
                            {
                                ConstValue.boolValue = EditorGUI.Toggle(ActiveRect, GUIContent.none, ConstValue.boolValue);
                            }
                        }

                        var oldColor = GUI.contentColor;

                        if (Application.isPlaying)
                        {
                            if (m.Stances[index].Active) GUI.contentColor = Color.yellow;
                            else if (m.Stances[index].Persistent) GUI.contentColor = Color.red+Color.white;
                            else  if (m.Stances[index].DisableTemp) GUI.contentColor = Color.white;
                        }

                        var st_label = "";

                        var stanceElement = m.Stances[index];

                        if (Application.isPlaying)
                        {
                          if (stanceElement.Active)    st_label = "[Active]";
                          if (stanceElement.Persistent)    st_label = "[Persis]";
                          if (stanceElement.Queued)    st_label = "[Queued]";
                          if (stanceElement.DisableTemp)    st_label = "[Disabled]";
                        }

                        EditorGUI.PropertyField(IDRect, ID, GUIContent.none);
                        EditorGUI.LabelField(Rect_Label, st_label);
                        GUI.contentColor = oldColor;

                        var style = new GUIStyle(EditorStyles.boldLabel)
                        { alignment = TextAnchor.UpperRight };

                        if (stanceElement.ID != null)
                        {
                            var IDVal = new Rect(rectan.width + 25, rectan.y + 3, 35, rectan.height);
                            EditorGUI.LabelField(IDVal, stanceElement.ID.ID.ToString(), style);
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Inspector");
                        EditorUtility.SetDirty(target);
                    }
                },

                onAddCallback = (list) =>
                {
                    if (m.Stances == null) m.Stances = new List<Stance>();

                    var newStance = new Stance();
                    m.Stances.Add(newStance);
                    EditorUtility.SetDirty(m);

                },

                onSelectCallback = (list) =>
                { SelectedStance.intValue = list.index; }
            };
        }


        private void ShowStances()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Stances", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(defaultStance, new GUIContent("Default Stance", "Default Stance ID to reset to when the animal exit an Stance"));
                EditorGUILayout.PropertyField(currentStance, new GUIContent("Current Stance", "Current Stance ID the animal is On"));

                Reo_List_Stances.index = SelectedStance.intValue;

                Reo_List_Stances.DoLayoutList();
                  
                var SnceIndex = Reo_List_Stances.index; 

                if (SnceIndex != -1 && Stances_List.arraySize > 0 && SnceIndex < Stances_List.arraySize)
                { 
                    EditorGUILayout.Space(-16);
                    var SelectedStance = Stances_List.GetArrayElementAtIndex(SnceIndex);

                    if (SelectedStance != null)
                    {
                        var Active = SelectedStance.FindPropertyRelative("enabled");
                        var Input = SelectedStance.FindPropertyRelative("Input");
                        var persistent = SelectedStance.FindPropertyRelative("persistent");
                        var CoolDown = SelectedStance.FindPropertyRelative("CoolDown");
                        var ExitAfter = SelectedStance.FindPropertyRelative("ExitAfter");
                        var CanStrafe = SelectedStance.FindPropertyRelative("CanStrafe");
                        var states = SelectedStance.FindPropertyRelative("states");
                        var StateQueue = SelectedStance.FindPropertyRelative("StateQueue");
                        var Include = SelectedStance.FindPropertyRelative("Include");
                        var DisableStances = SelectedStance.FindPropertyRelative("DisableStances");


                        EditorGUILayout.PropertyField(Active);
                        EditorGUILayout.PropertyField(Input);
                        EditorGUILayout.PropertyField(CoolDown);
                        EditorGUILayout.PropertyField(ExitAfter);
                        EditorGUILayout.PropertyField(persistent);


                        var stance = m.Stances[SnceIndex];
                        var StanceName = stance.ID != null ? stance.ID.name : "-EMPTY-";

                        var inc = $"States the '{StanceName}' stance " + (Include.boolValue ? $"[can]" : "[cannot]") + " be activated.";
                        var btn =   Include.boolValue ? "Include" : "Exclude";
                        if (!stance.HasStates) inc += " [All]";


                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(CanStrafe);

                            if (states.arraySize > 0)
                            {
                                var dC = GUI.color;
                                GUI.color = !Include.boolValue ? Color.red + Color.white : Color.white + Color.green;
                                Include.boolValue = GUILayout.Toggle(Include.boolValue, 
                                    new GUIContent(btn, "Includes/Excludes the States List for the stance activation"),
                                   EditorStyles.miniButton, GUILayout.Width(60));
                                GUI.color = dC;
                            }
                        }

                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(states, new GUIContent(inc), true);
                        EditorGUI.indentLevel--; 
                        
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(StateQueue, true);
                        EditorGUI.indentLevel--;
                        
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(DisableStances, true);
                        EditorGUI.indentLevel--;


                        if (Application.isPlaying && m.HasStances)
                        {
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.IntField("Temp Disable", stance.DisableValue);
                            }
                        }
                    }
                }

            }
        }

        public void DropAreaGUI()
        {
            EditorGUILayout.Space(5);

            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0f, 20, GUILayout.ExpandWidth(true));

            //var GC = GUI.backgroundColor;
            //GUI.backgroundColor = GC * 0.5f + Color.green * 0.5f;
            // GUI.backgroundColor = Color.green; ;

            var st = new GUIStyle(EditorStyles.toolbarButton);
            st.alignment = TextAnchor.MiddleCenter;
            st.fontSize = 14;


            GUI.Box(drop_area, "> Drag created states here < ", st);
            //  GUI.backgroundColor = GC;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    // ... change whether or not the drag *can* be performed by changing the visual mode of the cursor based on the IsDragValid function.
                    DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Generic : DragAndDropVisualMode.Rejected;
                    break;
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (dragged_object is State)
                            {
                                State newState = dragged_object as State;

                                if (m.states.Contains(newState)) continue;

                                EditorUtility.SetDirty(m);

                                // Pull all the information from the target of the serializedObject.
                                S_StateList.serializedObject.Update();
                                // Add a null array element to the start of the array then populate it with the object parameter.
                                S_StateList.InsertArrayElementAtIndex(0);
                                S_StateList.GetArrayElementAtIndex(0).objectReferenceValue = newState;
                                // Push all the information on the serializedObject back to the target.
                                S_StateList.serializedObject.ApplyModifiedProperties();

                                Reo_List_States.index = -1;

                                EditorUtility.SetDirty(newState);
                            }
                        }
                    }
                    break;
            }
        }

        private bool IsDragValid()
        {
            // Go through all the objects being dragged...
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                // ... and if any of them are not script assets, return that the drag is invalid.
                if (DragAndDrop.objectReferences[i].GetType().BaseType != typeof(State))
                    return false;
            }

            // If none of the dragging objects returned that the drag was invalid, return that it is valid.
            return true;
        }

        private void ShowAdvanced()
        {

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (Anim.isExpanded = MalbersEditor.Foldout(Anim.isExpanded, "References"))
                {
                    EditorGUILayout.PropertyField(Anim, new GUIContent("Animator"));
                    EditorGUILayout.PropertyField(RB, new GUIContent("RigidBody"));
                    EditorGUILayout.PropertyField(MainCamera, new GUIContent("Main Camera"));
                    EditorGUILayout.PropertyField(Aimer);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (Rotator.isExpanded = MalbersEditor.Foldout(Rotator.isExpanded, "Free Movement"))
                {
                    EditorGUILayout.PropertyField(Rotator, G_Rotator);
                    EditorGUILayout.PropertyField(RootBone, G_RootBone);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (sleep.isExpanded = MalbersEditor.Foldout(sleep.isExpanded, "Lock Inputs"))
                {
                    EditorGUILayout.PropertyField(sleep, new GUIContent("Sleep", "Disable internally the Controller wihout disabling the component"));
                    EditorGUILayout.PropertyField(lockInput);
                    EditorGUILayout.PropertyField(lockMovement);
                    EditorGUILayout.PropertyField(LockForwardMovement, new GUIContent("Lock Forward"));
                    EditorGUILayout.PropertyField(LockHorizontalMovement, new GUIContent("Lock Horizontal"));
                    EditorGUILayout.PropertyField(LockUpDownMovement, new GUIContent("Lock UpDown"));
                }
            }
            ShowAnimParam();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                NoParent.isExpanded = MalbersEditor.Foldout(NoParent.isExpanded, "Extras");

                if (NoParent.isExpanded)
                {
                    EditorGUILayout.PropertyField(NoParent);
                    EditorGUILayout.PropertyField(animalType, G_animalType);
                }
            }
        }

        private void ShowAnimParam()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var v_float = UnityEngine.AnimatorControllerParameterType.Float;
                var v_int = UnityEngine.AnimatorControllerParameterType.Int;
                var v_bool = UnityEngine.AnimatorControllerParameterType.Bool;
                var v_trigger = UnityEngine.AnimatorControllerParameterType.Trigger;
                var anim = m.Anim;

                using (new GUILayout.HorizontalScope())
                {

                    m_Vertical.isExpanded = MalbersEditor.Foldout(m_Vertical.isExpanded, "Required Animator Parameters");

                    if (m_Vertical.isExpanded)
                    {
                        if (GUILayout.Button(new GUIContent("*", "Check Required Parameters"), GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            MalbersEditor.CheckAnimParameter(anim, m_StateOn.stringValue, v_trigger);
                            MalbersEditor.CheckAnimParameter(anim, m_ModeOn.stringValue, v_trigger);
                            MalbersEditor.CheckAnimParameter(anim, m_Vertical.stringValue, v_float);
                            MalbersEditor.CheckAnimParameter(anim, m_Horizontal.stringValue, v_float);
                            MalbersEditor.CheckAnimParameter(anim, m_State.stringValue, v_int);
                            MalbersEditor.CheckAnimParameter(anim, m_LastState.stringValue, v_int);
                            MalbersEditor.CheckAnimParameter(anim, m_StateStatus.stringValue, v_int);
                            MalbersEditor.CheckAnimParameter(anim, m_StateFloat.stringValue, v_float);
                            MalbersEditor.CheckAnimParameter(anim, m_Mode.stringValue, v_int);
                            MalbersEditor.CheckAnimParameter(anim, m_ModeStatus.stringValue, v_int);
                            MalbersEditor.CheckAnimParameter(anim, m_Grounded.stringValue, v_bool);
                            MalbersEditor.CheckAnimParameter(anim, m_Movement.stringValue, v_bool);
                            MalbersEditor.CheckAnimParameter(anim, m_SpeedMultiplier.stringValue, v_float);
                        }
                    }
                }

                if (m_Vertical.isExpanded)
                {
                    MalbersEditor.DisplayParam(anim, m_StateOn, v_trigger);
                    MalbersEditor.DisplayParam(anim, m_ModeOn, v_trigger);
                    EditorGUILayout.Space();
                    MalbersEditor.DisplayParam(anim, m_Vertical, v_float);
                    MalbersEditor.DisplayParam(anim, m_Horizontal, v_float);
                    EditorGUILayout.Space();
                    MalbersEditor.DisplayParam(anim, m_State, v_int);
                    MalbersEditor.DisplayParam(anim, m_LastState, v_int);
                    MalbersEditor.DisplayParam(anim, m_StateStatus, v_int);
                    MalbersEditor.DisplayParam(anim, m_StateFloat, v_float);
                    EditorGUILayout.Space();
                    MalbersEditor.DisplayParam(anim, m_Mode, v_int);
                    MalbersEditor.DisplayParam(anim, m_ModeStatus, v_int);
                    EditorGUILayout.Space();
                    MalbersEditor.DisplayParam(anim, m_Grounded, v_bool);
                    MalbersEditor.DisplayParam(anim, m_Movement, v_bool);
                    MalbersEditor.DisplayParam(anim, m_SpeedMultiplier, v_float);
                }

                m_UpDown.isExpanded = MalbersEditor.Foldout(m_UpDown.isExpanded, "Optional Animator Parameters");

                if (m_UpDown.isExpanded)
                {
                    MalbersEditor.DisplayParam(anim, m_UpDown, v_float);
                    MalbersEditor.DisplayParam(anim, m_DeltaUpDown, v_float);
                    MalbersEditor.DisplayParam(anim, m_TargetAngle, v_float);
                    MalbersEditor.DisplayParam(anim, m_Sprint, v_bool);
                    EditorGUILayout.Space();

                    MalbersEditor.DisplayParam(anim, m_StateExitStatus, v_int);
                    MalbersEditor.DisplayParam(anim, m_StateTime, v_float);
                    EditorGUILayout.Space();

                    MalbersEditor.DisplayParam(anim, m_Stance, v_int);
                    MalbersEditor.DisplayParam(anim, m_LastStance, v_int);

                    EditorGUILayout.Space();

                    MalbersEditor.DisplayParam(anim, m_ModePower, v_float);
                    EditorGUILayout.Space();

                    MalbersEditor.DisplayParam(anim, m_Slope, v_float);
                    MalbersEditor.DisplayParam(anim, m_Random, v_int);
                    MalbersEditor.DisplayParam(anim, m_StrafeAnim, v_bool);
                    //   DisplayParam(m_TargetHorizontal, v_float);
                    EditorGUILayout.Space();
                    MalbersEditor.DisplayParam(anim, m_Type, v_int);
                }
            }
        }

        private void ShowEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

                Editor_EventTabs.intValue = GUILayout.Toolbar(Editor_EventTabs.intValue,
                    new string[] { "Movement", "State", "Stance", "Modes", "Speeds", "Extras" }, EditorStyles.toolbarButton);

                switch (Editor_EventTabs.intValue)
                {
                    case 0:
                        EditorGUILayout.PropertyField(OnSprintEnabled, new GUIContent("On Sprint"));
                        EditorGUILayout.PropertyField(OnMovementDetected);
                        EditorGUILayout.Space();

                        if (m.CanStrafe)
                        {
                            EditorGUILayout.PropertyField(OnStrafe);
                        }
                        EditorGUILayout.PropertyField(OnGrounded);
                        EditorGUILayout.PropertyField(OnMaxSlopeReached);
                        EditorGUILayout.PropertyField(OnTeleport);
                        EditorGUILayout.Space();
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(OnStateChange);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(OnEnterExitStates);
                        EditorGUI.indentLevel--;
                        break;
                    case 2:
                        EditorGUILayout.PropertyField(OnStanceChange);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(OnEnterExitStances);
                        EditorGUI.indentLevel--;
                        break;
                    case 3:
                        EditorGUILayout.PropertyField(OnModeStart);
                        EditorGUILayout.PropertyField(OnModeEnd);
                        EditorGUILayout.Space();

                        for (int i = 0; i < S_Mode_List.arraySize; i++)
                        {
                            var SelectedMode = S_Mode_List.GetArrayElementAtIndex(i);

                            var ID = SelectedMode.FindPropertyRelative("ID").objectReferenceValue;
                            var ModeName = ID != null ? ID.name : "";


                            var OnAbilityIndex = SelectedMode.FindPropertyRelative("OnAbilityIndex");
                            OnAbilityIndex.isExpanded = MalbersEditor.Foldout(OnAbilityIndex.isExpanded, $"Mode [{ModeName}] Events");

                            if (OnAbilityIndex.isExpanded)
                            {
                                var OnEnterMode = SelectedMode.FindPropertyRelative("OnEnterMode");
                                var OnExitMode = SelectedMode.FindPropertyRelative("OnExitMode");
                                EditorGUILayout.PropertyField(OnEnterMode, new GUIContent($"On [{ModeName}] Enter "));
                                EditorGUILayout.PropertyField(OnExitMode, new GUIContent($"On [{ModeName}] Exit"));
                                EditorGUILayout.PropertyField(OnAbilityIndex, new GUIContent($"On [{ModeName}] Active Ability Index changed"));
                            }
                        }


                        break;
                    case 4:
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(OnEnterExitSpeeds);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(OnSpeedChange);
                        break;

                    case 5:
                        EditorGUILayout.PropertyField(OnMovementLocked);
                        EditorGUILayout.PropertyField(OnInputLocked);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(OnAnimationChange);
                        break;
                    default:
                        break;
                }
            }
        }


        public static void DrawDebugButton(SerializedProperty property, GUIContent name, Color Highlight)
        {
            var currentGUIColor = GUI.color;
            GUI.color = property.boolValue ? Highlight : currentGUIColor;
            property.boolValue = GUILayout.Toggle(property.boolValue, name,  EditorStyles.miniButton);
            GUI.color = currentGUIColor;
        }


        private void ShowDebug()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {

                var Deb = serializedObject.FindProperty("debugStates");
                var DebM = serializedObject.FindProperty("debugModes");
                var debugStances = serializedObject.FindProperty("debugStances");
                var DebG = serializedObject.FindProperty("debugGizmos");
                using (new GUILayout.HorizontalScope())
                {
                    var DebColor = Color.red + Color.white;

                    DrawDebugButton(Deb, new GUIContent(" States", "Activate debbuging on the States"), DebColor);
                    DrawDebugButton(DebM, new GUIContent(" Modes", "Activate debbuging on the Modes"), DebColor);
                    DrawDebugButton(debugStances, new GUIContent(" Stances", "Activate debbuging on the Stances"), DebColor);
                    DrawDebugButton(DebG, new GUIContent(" Gizmos", "Show States and Modes Gizmos"), DebColor);
                    DrawDebugButton(showPivots, new GUIContent(" Pivots", "Show Animal Pivos"), DebColor);
                }
            }

            if (Application.isPlaying)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    Repaint();
                    if (m.ActiveState)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.LabelField($"Active State: [{m.ActiveState.ID.name}] ({m.ActiveState.ID.ID})", EditorStyles.boldLabel);
                            if (m.CurrentSpeedSet != null)
                                EditorGUILayout.LabelField
                                    ($"Set: [{m.CurrentSpeedSet.name}] -  Speed: [{m.CurrentSpeedModifier.name}]. " +
                                    $"Index: [{m.CurrentSpeedIndex}]", EditorStyles.boldLabel);
                            DisplayActiveSpeed();
                            EditorGUILayout.LabelField($"RigidBody Horizontal Speed: [{m.HorizontalSpeed:F4}]", EditorStyles.boldLabel);
                        }

                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.ToggleLeft("Using Camera Input", m.UseCameraInput);
                        }


                        EditorGUIUtility.labelWidth = 70;
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                EditorGUILayout.ToggleLeft("Is Pending", m.ActiveState.IsPending);
                                EditorGUILayout.ToggleLeft("Can Exit", m.ActiveState.CanExit);
                                EditorGUILayout.ToggleLeft("++ Pos", m.UseAdditivePos);
                                EditorGUILayout.ToggleLeft("RootMotion", m.RootMotion);
                                EditorGUILayout.ToggleLeft("Orient To Ground", m.UseOrientToGround);
                                EditorGUILayout.ToggleLeft("Chest Ray", m.FrontRay);
                            }

                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                EditorGUILayout.ToggleLeft("Ignore Lower", m.ActiveState.IgnoreLowerStates);
                                EditorGUILayout.ToggleLeft("Is Persistent", m.ActiveState.IsPersistent);
                                EditorGUILayout.ToggleLeft("++ Rot]", m.UseAdditiveRot);
                                EditorGUILayout.ToggleLeft("Grounded", m.Grounded);
                                EditorGUILayout.ToggleLeft("Use Custom Rot", m.UseCustomAlign);
                                EditorGUILayout.ToggleLeft("Hip Ray", m.MainRay);
                            }
                        }
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.ToggleLeft("Move with Direction", m.UsingMoveWithDirection);
                            EditorGUILayout.ToggleLeft("Raw Input", m.UseRawInput);
                            EditorGUILayout.ToggleLeft("Use Sprint", m.UseSprint);
                            EditorGUILayout.ToggleLeft("Input Locked", m.LockInput);
                            EditorGUILayout.ToggleLeft("Free Move", m.FreeMovement);
                        }


                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.ToggleLeft("Rotate At Direction", m.Rotate_at_Direction);
                            EditorGUILayout.ToggleLeft("Movement Detected", m.MovementDetected);
                            EditorGUILayout.ToggleLeft("Sprint", m.Sprint);
                            EditorGUILayout.ToggleLeft("Movement Locked", m.LockMovement);
                            EditorGUILayout.ToggleLeft("Use Gravity", m.UseGravity);
                        }
                    }

                    EditorGUIUtility.labelWidth = 0;

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.ObjectField("Active Mode: ", m.IsPlayingMode ? m.ActiveMode.ID : null, typeof(ModeID), false);
                        EditorGUILayout.TextField("Ability: ", (m.ActiveMode != null && m.ActiveMode.ActiveAbility != null) ?
                            "[" + m.ActiveMode.ActiveAbility.Index.Value + "]" + m.ActiveMode.ActiveAbility.Name : "");
                        EditorGUILayout.ToggleLeft("Playing Mode", m.IsPlayingMode);
                        EditorGUILayout.ToggleLeft("Preparing Mode", m.IsPreparingMode);
                        EditorGUILayout.ToggleLeft("Mode In Transition", m.ActiveMode != null && m.ActiveMode.IsInTransition);
                        EditorGUILayout.IntField("Last Mode ID", m.LastModeID);
                        EditorGUILayout.IntField("Last Mode Ability", m.LastAbilityIndex);
                        EditorGUILayout.FloatField("Mode Time", m.ModeTime);
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("platform"));
                        EditorGUILayout.ObjectField("Is in Zone?", m.InZone, typeof(Zone), false);

                    } 

                    EditorGUIUtility.labelWidth = 80;

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.FloatField("Gravity Time", m.GravityTime, GUILayout.MinWidth(50));
                        EditorGUILayout.FloatField("Gravity Mult", m.GravityMultiplier, GUILayout.MinWidth(50));
                    }

                    EditorGUIUtility.labelWidth = 0;

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Vector3Field("External Force", m.CurrentExternalForce);
                        EditorGUILayout.Vector3Field("External Force Max", m.ExternalForce);
                        EditorGUILayout.FloatField("External Force Acel", m.ExternalForceAcel);
                        EditorGUILayout.ToggleLeft("Force Air Control ?", m.ExternalForceAirControl);
                    }


                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.FloatField("Delta Angle", m.DeltaAngle);
                        EditorGUILayout.FloatField("Pitch Angle", m.PitchAngle);
                        EditorGUILayout.FloatField("Bank", m.Bank);
                        //  EditorGUILayout.FloatField("Forward Multiplier", m.ForwardMultiplier);
                        EditorGUILayout.Vector2Field("Pitch[x]-Bank[y]", new Vector2(m.PitchAngle, m.Bank));
                    }


                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.FloatField("Terrain Slope", m.TerrainSlope);
                        EditorGUILayout.FloatField("Main Pivot Slope", m.MainPivotSlope);
                        EditorGUILayout.FloatField("Slope Normalized", m.SlopeNormalized);
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Vector3Field("Raw Input Axis", m.RawInputAxis.Round(3));
                        EditorGUILayout.Vector3Field("Movement Direction", m.Move_Direction.Round(3));
                        EditorGUILayout.Vector3Field("Movement Axis Raw", m.MovementAxisRaw.Round(3));
                        EditorGUILayout.Vector3Field("Movement Axis", m.MovementAxis.Round(3));
                        EditorGUILayout.Vector3Field("Movement Smooth", m.MovementAxisSmoothed.Round(3));
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Vector3Field("Inertia ", m.Inertia.Round(3));
                        EditorGUILayout.Vector3Field("Inertia Speed ", m.InertiaPositionSpeed.Round(3));
                        EditorGUILayout.Vector3Field("Pitch Direction", m.PitchDirection.Round(3));
                        EditorGUILayout.Space();
                        EditorGUILayout.IntField("Current Anim Tag", m.AnimStateTag);
                        EditorGUILayout.Space();
                        EditorGUILayout.IntField("Stance", m.Stance);
                    }
                }
            }
        }

        private void ShowModes()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(StartWithMode, G_StartWithMode);
                if (targets != null && targets.Length > 1)
                {
                    EditorGUILayout.EndVertical();
                    return; //Do not show Multiple Animals
                }

                Reo_List_Modes.DoLayoutList();        //Paint the Reordable List
            }
            
            Reo_List_Modes.index = SelectedMode.intValue;

            var index = Reo_List_Modes.index;


            if (index != -1 && S_Mode_List.arraySize > 0 && index < S_Mode_List.arraySize)
            {
                var SelectedMode = S_Mode_List.GetArrayElementAtIndex(index);

                if (SelectedMode != null)
                {
                    var ID = SelectedMode.FindPropertyRelative("ID").objectReferenceValue;
                    var ModeName = ID != null ? ID.name : "";


                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(SelectedMode, new GUIContent($"Mode [{ModeName}]"), false);
                        EditorGUI.indentLevel--;
                        if (SelectedMode.isExpanded)
                        {
                            Mode_Tabs1.intValue = GUILayout.Toolbar(Mode_Tabs1.intValue, new string[3] { "General", "Abilities", "Events" });

                            switch (Mode_Tabs1.intValue)
                            {
                                case 0:
                                    var Input = SelectedMode.FindPropertyRelative("Input");
                                    var hasCoolDown = SelectedMode.FindPropertyRelative("hasCoolDown");
                                    var CoolDown = SelectedMode.FindPropertyRelative("CoolDown");
                                    var allowRotation = SelectedMode.FindPropertyRelative("allowRotation");
                                    var m_Source = SelectedMode.FindPropertyRelative("m_Source");
                                    var allowMovement = SelectedMode.FindPropertyRelative("allowMovement");
                                    var modifier = SelectedMode.FindPropertyRelative("modifier");
                                    var ignoreLowerModes = SelectedMode.FindPropertyRelative("ignoreLowerModes");

                                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                                    {
                                        EditorGUILayout.PropertyField(Input);
                                        EditorGUILayout.PropertyField(ignoreLowerModes, new GUIContent("Ignore Lower", "It will play this mode even if another Lower Priority Mode is playing"));

                                        EditorGUILayout.PropertyField(hasCoolDown);
                                        if (hasCoolDown.boolValue)
                                            EditorGUILayout.PropertyField(CoolDown);
                                        EditorGUILayout.PropertyField(allowRotation, new GUIContent("Allow Rotation", "Allows rotate while is on the Mode"));
                                        EditorGUILayout.PropertyField(allowMovement, new GUIContent("Allow Movement", "Allows movement while is on the Mode"));

                                        EditorGUILayout.PropertyField(modifier, G_Modifier);
                                        EditorGUILayout.PropertyField(m_Source);
                                    }

                                    if (Application.isPlaying)
                                    {
                                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                                        {
                                            using (new EditorGUI.DisabledGroupScope(true))
                                            {
                                                EditorGUILayout.Toggle("Playing Mode", m.modes[index].PlayingMode);
                                                EditorGUILayout.Toggle("Input Value", m.modes[index].InputValue);
                                                EditorGUILayout.Toggle("In CoolDown", m.modes[index].InCoolDown);
                                            }
                                        }
                                    }


                                    break;
                                case 1:
                                    var AbilityIndex = SelectedMode.FindPropertyRelative("m_AbilityIndex");
                                    var DefaultIndex = SelectedMode.FindPropertyRelative("DefaultIndex");
                                    var ResetToDefault = SelectedMode.FindPropertyRelative("ResetToDefault");
                                    var Abilities = SelectedMode.FindPropertyRelative("Abilities");

                                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                                    {
                                        EditorGUIUtility.labelWidth = 70;
                                        EditorGUILayout.PropertyField(AbilityIndex, G_AbilityIndex, GUILayout.MinWidth(50));
                                        EditorGUILayout.PropertyField(DefaultIndex, G_DefaultIndex, GUILayout.MinWidth(50));
                                        EditorGUIUtility.labelWidth = 0;
                                        ResetToDefault.boolValue = GUILayout.Toggle(ResetToDefault.boolValue, G_ResetToDefault, EditorStyles.miniButton, GUILayout.Width(20));
                                    }

                                    EditorGUILayout.LabelField("[If Active Ability Index is -99, the mode will play a random ability]", DescriptionStyle);
                                    DrawAbilities(index, SelectedMode, Abilities);
                                    break;
                                case 2:
                                    var OnEnterMode = SelectedMode.FindPropertyRelative("OnEnterMode");
                                    var OnAbilityIndex = SelectedMode.FindPropertyRelative("OnAbilityIndex");
                                    var OnExitMode = SelectedMode.FindPropertyRelative("OnExitMode");
                                    EditorGUILayout.PropertyField(OnEnterMode, new GUIContent($"On [{ModeName}] Enter "));
                                    EditorGUILayout.PropertyField(OnExitMode, new GUIContent($"On [{ModeName}] Exit"));
                                    EditorGUILayout.PropertyField(OnAbilityIndex, new GUIContent($"On [{ModeName}] Active Ability Index changed "));
                                    break;
                            }

                        }
                    }
                }
            }
        }

        private void DrawAbilities(int ModeIndex, SerializedProperty SelectedMode, SerializedProperty Abilities)
        {
            ReorderableList Reo_AbilityList;
            string listKey = SelectedMode.propertyPath;

            if (innerListDict.ContainsKey(listKey))
            {
                // fetch the reorderable list in dict
                Reo_AbilityList = innerListDict[listKey];
            }
            else
            {
                Reo_AbilityList = new ReorderableList(SelectedMode.serializedObject, Abilities, true, true, true, true)
                {
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;

                        var element = Abilities.GetArrayElementAtIndex(index);

                        var IndexValue = element.FindPropertyRelative("Index");
                        var name = element.FindPropertyRelative("Name");

                        var Active = element.FindPropertyRelative("active");

                        var ConstValue = Active.FindPropertyRelative("ConstantValue");
                        var VarValue = Active.FindPropertyRelative("Variable");
                        var useConstant = Active.FindPropertyRelative("UseConstant").boolValue;
                        BoolVar variable = VarValue.objectReferenceValue as BoolVar;

                        var IDRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };

                        var ActiveRect = new Rect(IDRect);
                        var NameRect = new Rect(IDRect);

                        ActiveRect.width = 20;

                        IDRect.x = rect.width / 4 * 3 + 50;
                        IDRect.width = rect.width / 4 - 12;

                        NameRect.x += 24;
                        NameRect.width = rect.width / 4 * 3 - 50;

                        if (useConstant)
                        {
                            ConstValue.boolValue = EditorGUI.Toggle(ActiveRect, GUIContent.none, ConstValue.boolValue);

                            if (variable != null)
                            {
                                variable.Value = ConstValue.boolValue;
                                EditorUtility.SetDirty(variable);
                            }
                        }
                        else
                        {
                            if (variable != null)
                            {
                                variable.Value = EditorGUI.Toggle(ActiveRect, GUIContent.none, variable.Value);
                                ConstValue.boolValue = variable.Value;
                            }
                            else
                            {
                                ConstValue.boolValue = EditorGUI.Toggle(ActiveRect, GUIContent.none, ConstValue.boolValue);
                            }
                        }

                        EditorGUI.PropertyField(NameRect, name, GUIContent.none);

                        EditorGUIUtility.labelWidth = 56;
                        EditorGUI.PropertyField(IDRect, IndexValue, GUIContent.none);
                        EditorGUIUtility.labelWidth = 0;
                    },

                    drawHeaderCallback = rect =>
                    {
                        var IDRect = new Rect(rect)
                        {
                            height = EditorGUIUtility.singleLineHeight
                        };

                        var NameRect = new Rect(IDRect);

                        IDRect.x = rect.width / 4 * 3 + 40;
                        IDRect.width = 40;

                        NameRect.x += 24;
                        NameRect.width = rect.width / 4 * 3 - 50;


                        EditorGUI.LabelField(NameRect, "   Abilities");
                        EditorGUI.LabelField(IDRect, "Index");
                    },

                    onAddCallback = (list) =>
                    {
                        var index = list.count == 0 ? 0 : list.count - 1;
                        Abilities.InsertArrayElementAtIndex(list.count == 0 ? 0 : list.count - 1);
                        list.index = -1;
                    }
                };

                innerListDict.Add(listKey, Reo_AbilityList);  //Store it on the Editor
            }

            Reo_AbilityList.DoLayoutList();

            int SelectedAbility = Reo_AbilityList.index;


            if (SelectedAbility != -1 && SelectedAbility < Abilities.arraySize)
            {
                // Debug.Log("SelectedAbility = " + SelectedAbility);
                SerializedProperty ability = Abilities.GetArrayElementAtIndex(SelectedAbility);

                if (ability != null)
                {
                    var active = ability.FindPropertyRelative("active");
                    var Input = ability.FindPropertyRelative("Input");
                    var audioClip = ability.FindPropertyRelative("audioClip");
                    var audioSource = ability.FindPropertyRelative("audioSource");
                    var m_stopAudio = ability.FindPropertyRelative("m_stopAudio");
                    var ClipDelay = ability.FindPropertyRelative("ClipDelay");
                    var modifier = ability.FindPropertyRelative("modifier");
                    var Status = ability.FindPropertyRelative("Status");
                    var Release = ability.FindPropertyRelative("Release");
                    var abilityTime = ability.FindPropertyRelative("abilityTime");
                    var ChargeValue = ability.FindPropertyRelative("ChargeValue");
                    var ChargeCurve = ability.FindPropertyRelative("ChargeCurve");
                    var OnEnter = ability.FindPropertyRelative("OnEnter");
                    var OnExit = ability.FindPropertyRelative("OnExit");
                    var OnCharged = ability.FindPropertyRelative("OnCharged");
                    var Name = ability.FindPropertyRelative("Name");

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        var M = m.modes[ModeIndex];
                        if (M.ID != null && M.Abilities[SelectedAbility] != null)
                        {
                            var valu = m.modes[ModeIndex].Abilities[SelectedAbility].Index.Value;
                            var neg = valu > 0 ? 1 : -1;

                            EditorGUILayout.LabelField(new GUIContent($"Combined Index → " +
                            $"[{(m.modes[ModeIndex].ID.ID * 1000 + Mathf.Abs(valu)) * neg}]",
                             "The combined index is set using this formula: (Mode_ID * 1000 + Ability_Index)"), DescriptionStyle);
                        }
                        Ability_Tabs.intValue = GUILayout.Toolbar(Ability_Tabs.intValue, new string[5] { "General", "Status", "Limits", "Audio", "Events" });

                        switch (Ability_Tabs.intValue)
                        {
                            //General
                            case 0:
                                {
                                    EditorGUILayout.PropertyField(active);
                                    EditorGUILayout.PropertyField(Input);
                                    EditorGUILayout.PropertyField(modifier);
                                    var AdditivePosition = ability.FindPropertyRelative("AdditivePosition");
                                    EditorGUILayout.PropertyField(AdditivePosition);
                                    break;
                                }
                            //Status
                            case 1:
                                {
                                    var help = "";

                                    EditorGUILayout.PropertyField(Status);

                                    switch ((AbilityStatus)Status.intValue)
                                    {
                                        case AbilityStatus.Charged:
                                            EditorGUILayout.PropertyField(abilityTime, new GUIContent("Charge Time"));
                                            var ConstValue = abilityTime.FindPropertyRelative("ConstantValue");

                                            if (ConstValue.floatValue != 0)
                                            {
                                                EditorGUILayout.PropertyField(ChargeValue);
                                                EditorGUILayout.PropertyField(ChargeCurve);
                                                EditorGUILayout.PropertyField(Release);
                                                help = "The Ability can be charged, it will be active while the Input Value is [True]. It will be stopped when the Input is released.\n" +
                                               "The ModePower animator Parameter will store the charge value ";
                                            }
                                            else
                                            {
                                                help = "The Ability will be active while the input is press down";
                                            }
                                           
                                            break;
                                        case AbilityStatus.ActiveByTime:
                                            EditorGUILayout.PropertyField(abilityTime);
                                            help = "The Ability is active during the ability time, then it will stop";
                                            break;
                                        case AbilityStatus.PlayOneTime:
                                            help = "The Ability will play once";
                                            break;
                                        case AbilityStatus.Toggle:
                                            help = "The Ability will active when the Input Value is [True].\nIt will be stopped the next time the Input Value is [True]";
                                            break;
                                        case AbilityStatus.Forever:
                                            help = "The Ability will active forever. To stop it, call:\nMAnimal.Mode_Stop()";
                                            break;
                                        default: break;
                                    } 

                                    EditorGUILayout.LabelField(help, DescriptionStyle);
                                    break;
                                }
                            //Limits
                            case 2:
                                {
                                    var S_properties = ability.FindPropertyRelative("Limits");
                                   
                                    EditorGUILayout.PropertyField(S_properties, true);

                                    if (GUILayout.Button("Copy these limits to all other Abilities"))
                                    {
                                        var ModeAbilities = m.modes[ModeIndex].Abilities;
                                        var properties = ModeAbilities[SelectedAbility].Limits;

                                        foreach (var ab in ModeAbilities)
                                            ab.Limits = new ModeProperties(properties);

                                        Debug.Log("All Limits copied to all the Abilities in Mode: " + m.modes[ModeIndex].Name);
                                        EditorUtility.SetDirty(target);
                                    }
                                    break;
                                }
                            //Audio
                            case 3:
                                {
                                    EditorGUILayout.PropertyField(audioClip);
                                    EditorGUILayout.PropertyField(audioSource);
                                    EditorGUILayout.PropertyField(ClipDelay);
                                    EditorGUILayout.PropertyField(m_stopAudio);
                                    break;
                                }
                            //Events
                            case 4:
                                {
                                    var ab_name = Name.stringValue;
                                    EditorGUILayout.PropertyField(OnEnter, new GUIContent($"On [{ab_name}] Enter"));
                                    EditorGUILayout.PropertyField(OnExit, new GUIContent($"On [{ab_name}] Exit"));

                                    if ((AbilityStatus)Status.intValue == AbilityStatus.Charged)
                                        EditorGUILayout.PropertyField(OnCharged, new GUIContent($"On [{ab_name}] Charged"));
                                    break;
                                }
                        }
                    }
                }
            }
        }
        private void ShowStates()
        {
            EditorGUI.indentLevel++;

            Reo_List_States.index = SelectedState.intValue;

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(OverrideStartState, G_OverrideStartState);
                    CloneStates.boolValue = GUILayout.Toggle(CloneStates.boolValue, G_CloneStates, EditorStyles.miniButton, GUILayout.Width(85));
                }
                EditorGUI.indentLevel--;

                if (!CloneStates.boolValue)
                {
                    EditorGUILayout.HelpBox("Disable Clone States only when you are setting values and debugging while playing. ", MessageType.Warning);
                }
                Reo_List_States.DoLayoutList();        //Paint the Reordable List 
                DropAreaGUI();

                EditorGUILayout.Space();

                var index = Reo_List_States.index;


                if (index != -1 && Reo_List_States.serializedProperty.arraySize > index)
                {
                    var element = Reo_List_States.serializedProperty.GetArrayElementAtIndex(index);

                    if (element != null & m.states[index] != null)
                    {
                        if (MalbersEditor.Foldout(ShowStateInInspector, m.states[index].name))
                        {
                            if (element.objectReferenceValue != null)
                            {
                                Editor editor;

                                var key = element.propertyPath;

                                if (State_Editor.TryGetValue(key, out editor))
                                {
                                    editor = State_Editor[key];
                                }
                                else
                                {
                                    Editor.CreateCachedEditor(element.objectReferenceValue, null, ref editor);
                                    State_Editor.Add(key, editor);
                                }
                                editor.OnInspectorGUI();
                            }
                        }
                    }
                }
            }
        }

        private void ShowSpeeds()
        {
           // using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox("For [In Place] animations <Not Root Motion>, Increse [Position] and [Rotation] values for each Speed Set", MessageType.Info);
                Reo_List_Speeds.DoLayoutList();        //Paint the Reordable List speeds


                Reo_List_Speeds.index = SelectedSpeed;

                if (Reo_List_Speeds.index != -1)
                {

                    var SelectedSpeed = S_Speed_List.GetArrayElementAtIndex(Reo_List_Speeds.index);
                    var Speeds = SelectedSpeed.FindPropertyRelative("Speeds");
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(SelectedSpeed, false);
                        EditorGUI.indentLevel--;

                        if (SelectedSpeed.isExpanded)
                        {
                            EditorGUILayout.LabelField("Speed Index Values", EditorStyles.boldLabel);


                            var StarSpeed = m.speedSets[Reo_List_Speeds.index].StartVerticalIndex.Value;
                            var GCC = GUI.contentColor;
                             
                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            { 
                                using (new EditorGUI.DisabledGroupScope(true))
                                {
                                    for (int i = 0; i < Speeds.arraySize; i++)
                                    {

                                        var speedN = Speeds.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;

                                        if (StarSpeed - 1 == i)
                                        {
                                            GUI.contentColor = Color.yellow;
                                            speedN += " [Start]";
                                        }
                                        EditorGUILayout.FloatField(speedN, 1 + i);
                                        GUI.contentColor = GCC;
                                    }
                                } 
                            }

                            SpeedTabs = GUILayout.Toolbar(SpeedTabs, new string[2] { "General", "Speeds" });

                            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                if (SpeedTabs == 0)
                                {

                                    var states = SelectedSpeed.FindPropertyRelative("states");
                                    var stances = SelectedSpeed.FindPropertyRelative("stances");
                                    var StartVerticalSpeed = SelectedSpeed.FindPropertyRelative("StartVerticalIndex");
                                    var TopIndex = SelectedSpeed.FindPropertyRelative("TopIndex");
                                    var BackSpeedMult = SelectedSpeed.FindPropertyRelative("BackSpeedMult");


                                    var PitchLerpOn = SelectedSpeed.FindPropertyRelative("PitchLerpOn");
                                    var PitchLerpOff = SelectedSpeed.FindPropertyRelative("PitchLerpOff");
                                    var BankLerp = SelectedSpeed.FindPropertyRelative("BankLerp");
                                    var m_LockSpeed = SelectedSpeed.FindPropertyRelative("m_LockSpeed");
                                    var m_SprintIndex = SelectedSpeed.FindPropertyRelative("m_SprintIndex");
                                    var m_LockIndex = SelectedSpeed.FindPropertyRelative("m_LockIndex");



                                    StartVerticalSpeed.isExpanded = MalbersEditor.Foldout(StartVerticalSpeed.isExpanded, "Indexes");
                                    if (StartVerticalSpeed.isExpanded)
                                    {
                                        EditorGUILayout.PropertyField(StartVerticalSpeed, new GUIContent("Start Index", StartVerticalSpeed.tooltip));
                                        EditorGUILayout.PropertyField(TopIndex);
                                        EditorGUILayout.PropertyField(m_SprintIndex);
                                        EditorGUILayout.PropertyField(BackSpeedMult, new GUIContent("Back Speed Mult", BackSpeedMult.tooltip));
                                    }



                                    m_LockSpeed.isExpanded = MalbersEditor.Foldout(m_LockSpeed.isExpanded, "Lock Speed");
                                    if (m_LockSpeed.isExpanded)
                                    {
                                        EditorGUILayout.PropertyField(m_LockSpeed);
                                        EditorGUILayout.PropertyField(m_LockIndex);
                                    }


                                    PitchLerpOn.isExpanded = MalbersEditor.Foldout(PitchLerpOn.isExpanded, "Free Movement Lerp Values");

                                    if (PitchLerpOn.isExpanded)
                                    {
                                        EditorGUILayout.PropertyField(PitchLerpOn);
                                        EditorGUILayout.PropertyField(PitchLerpOff);
                                        EditorGUILayout.PropertyField(BankLerp);
                                    }


                                    BankLerp.isExpanded = MalbersEditor.Foldout(BankLerp.isExpanded, "Limits");

                                    if (BankLerp.isExpanded)
                                    {
                                        // EditorGUILayout.Space();
                                        EditorGUI.indentLevel++;
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.PropertyField(states, new GUIContent("States", "States that will activate these Speeds"), true);
                                        EditorGUILayout.PropertyField(stances, new GUIContent("Stances", "Stances that will activate these Speeds"), true);
                                        EditorGUI.indentLevel--;
                                        EditorGUI.indentLevel--;
                                    }

                                }
                                else
                                {

                                    // EditorGUILayout.Space();
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(Speeds, new GUIContent("Speeds", "Speeds for this speed Set"), true);
                                    EditorGUI.indentLevel--;
                                }
                            }
                        }
                    }
                }
            }

            DisplayActiveSpeed();
        }

        private void DisplayActiveSpeed()
        {
            if (Application.isPlaying)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.LabelField("Active Speed Modifier", EditorStyles.boldLabel);
                    EditorGUILayout.IntField("Current Index", m.CurrentSpeedIndex);
                    EditorGUILayout.Toggle("Using Custom Speed", m.CustomSpeed);
                    var cpM = serializedObject.FindProperty("currentSpeedModifier");
                    cpM.isExpanded = true;
                    EditorGUILayout.PropertyField(cpM, true);
                }
            }
        }

        private void ShowGeneral()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                EditorGUILayout.PropertyField(Player, G_Player);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(S_PivotsList, true);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                UseCameraInput.isExpanded = MalbersEditor.Foldout(UseCameraInput.isExpanded, "Movement");

                if (UseCameraInput.isExpanded)
                {
                    EditorGUILayout.PropertyField(UseCameraInput, new GUIContent("Camera Input", "The Animal uses the Camera Forward Diretion to Move"));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(alwaysForward, new GUIContent("Always Forward", "If true the animal will always go forward. useful for infinite runners"));
                    if (EditorGUI.EndChangeCheck() && Application.isPlaying) m.AlwaysForward = m.AlwaysForward;


                    EditorGUILayout.PropertyField(useCameraUp, new GUIContent("Use Camera Up", "Uses the Camera Up Vector to move UP or Down while flying or Swiming UnderWater. if this is false the Animal will need an UPDOWN Input to move higher or lower"));
                    EditorGUILayout.PropertyField(SmoothVertical, G_SmoothVertical);
                    EditorGUILayout.PropertyField(useSprintGlobal, G_useSprintGlobal);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(TurnMultiplier);
                    EditorGUILayout.PropertyField(InPlaceDamp);
                    EditorGUILayout.PropertyField(TurnLimit);
                    EditorGUILayout.PropertyField(AnimatorSpeed);
                    EditorGUILayout.PropertyField(m_TimeMultiplier);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) 
            {
                GroundLayer.isExpanded = MalbersEditor.Foldout(GroundLayer.isExpanded, "Ground");

                if (GroundLayer.isExpanded)
                {
                    EditorGUILayout.PropertyField(GroundLayer, G_GroundLayer);
                    EditorGUILayout.PropertyField(OrientToGround);
                    EditorGUILayout.PropertyField(DebreeTag);


                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(Height, G_Height);
                        if (GUILayout.Button(new GUIContent("C", "Calculate Height and Animal Center"), GUILayout.Width(26))) m.SetPivots();
                    }

                    EditorGUILayout.PropertyField(maxAngleSlope);
                    EditorGUILayout.PropertyField(deepSlope);

                    maxAngleSlope.isExpanded = MalbersEditor.Foldout(maxAngleSlope.isExpanded, "Ground Aligment");
                    if (maxAngleSlope.isExpanded)
                    {
                        EditorGUILayout.PropertyField(AlignPosLerp, G_AlignPosLerp);
                        EditorGUILayout.PropertyField(AlignPosDelta, G_AlignPosDelta);
                        EditorGUILayout.PropertyField(AlignRotLerp, G_AlignRotLerp);
                        EditorGUILayout.PropertyField(AlignRotDelta, G_AlignRotDelta);
                        EditorGUILayout.PropertyField(RayCastRadius, G_RayCastRadius);
                        EditorGUILayout.PropertyField(AlignLoop, G_AlignLoop);
                    }
                }
            }


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                m_gravity.isExpanded = MalbersEditor.Foldout(m_gravity.isExpanded, "Gravity");

                if (m_gravity.isExpanded)
                {
                    //  EditorGUILayout.LabelField("Gravity", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_gravity, G_gravityDirection);
                    EditorGUILayout.PropertyField(m_gravityPower, G_GravityForce);
                    EditorGUILayout.PropertyField(m_gravityTime, G_GravityCycle);
                    EditorGUILayout.PropertyField(m_gravityTimeLimit, G_GravityLimitCycle);
                    EditorGUILayout.PropertyField(ground_Changes_Gravity, new GUIContent("Ground Changes Gravity", "The Ground will change the gravity direction, allowing the animals to move in any surface"));
                }
            } 
            ShowStrafingVars();
        }
        private void ShowStrafingVars()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (m_CanStrafe.isExpanded = MalbersEditor.Foldout(m_CanStrafe.isExpanded, "Strafing"))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(m_CanStrafe, G_CanStrafe);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (m.Aimer == null)
                            {
                                m.Aimer = m.FindComponent<Aim>();
                                if (m.Aimer == null)
                                {
                                    m.Aimer = m.gameObject.AddComponent<Aim>();
                                }
                                EditorUtility.SetDirty(m);
                            }
                        }

                        if (GUILayout.Button("?", GUILayout.Width(20)))
                        {
                            Application.OpenURL("https://malbersanimations.gitbook.io/animal-controller/strafing");
                        }
                    }

                    if (m.CanStrafe)
                    {
                        EditorGUILayout.PropertyField(m_strafe, G_Strafe);
                        EditorGUILayout.PropertyField(m_StrafeNormalize, G_StrafeNormalize);
                        EditorGUILayout.PropertyField(m_StrafeLerp, G_StrafeLerp);
                    }
                }
            }
        }

        private void Draw_Header_Speed(Rect rect)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var name = new Rect(rect.x + 20, rect.y, rect.width / 2, height);

            EditorGUI.LabelField(name, " Speed Sets");

            Rect R_1 = new Rect(rect.x - 5, rect.y, 20, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(R_1, "?"))
                Application.OpenURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/speeds");

        }

        private void OnRemoveCallback_Speeds(ReorderableList list)
        {
            S_Speed_List.DeleteArrayElementAtIndex(list.index);
            list.index -= 1;

            if (list.index == -1 && S_Speed_List.arraySize > 0)  //In Case you remove the first one
            {
                list.index = 0;
            }

            EditorUtility.SetDirty(m);
        }

        private void OnAddCallback_Speeds(ReorderableList reo_List_Speeds)
        {
            if (m.speedSets == null) m.speedSets = new List<MSpeedSet>();

            m.speedSets.Add(new MSpeedSet());

            EditorUtility.SetDirty(m);
        }

        private void Draw_Element_Speed(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (S_Speed_List.arraySize <= index) return;

            var nameRect = rect;

            nameRect.y += 1;
            nameRect.height -= 3;
            Rect activeRect = nameRect;

            var speedSet = S_Speed_List.GetArrayElementAtIndex(index);
            var nameSpeedSet = speedSet.FindPropertyRelative("name");
            nameRect.width /= 2;
            nameRect.width += 10;
            EditorGUI.PropertyField(nameRect, nameSpeedSet, GUIContent.none);

            activeRect.x = rect.width / 2 + 60;
            activeRect.width = rect.width / 2 - 20;

            //    EditorGUI.TextField(activeRect, "(Active)");

            if (Application.isPlaying)
            {
                if (m.speedSets[index] == m.CurrentSpeedSet)
                {
                    EditorGUI.LabelField(activeRect, "(" + m.CurrentSpeedModifier.name + ")", EditorStyles.boldLabel);
                }
            }
        } 

        #region DrawStates 
        //-------------------------STATES-----------------------------------------------------------
        private void Draw_Header_State(Rect rect)
        {
            var r = new Rect( rect);
            r.x += 13;
            r.width -= 60;

            //var Head = "     States. ";
            var Head = "Selected: ";

            if (m.states != null && m.states.Count > 0 && SelectedState.intValue != -1 && SelectedState.intValue < m.states.Count) 
            {
                var s = m.states[SelectedState.intValue];
                Head += $" [{s.GetType().Name}]";

                if (s.ID)
                {
                    Head += $"  ID: [{s.ID.ID}]";
                    Head += $"  Tag: [{s.ID.name}]";
                }
            }

            EditorGUI.LabelField(r, new GUIContent(Head, "States are the core logic the Animals can do [Double clic to modify them]"),EditorStyles.boldLabel);

            Rect R_2 = new Rect(rect.width - 8, rect.y, 60, EditorGUIUtility.singleLineHeight - 3);
            EditorGUI.LabelField(R_2, new GUIContent("Priority", "Priority of the States, Higher value -> Higher priority"));
        }


        private void Draw_Element_State(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            rect.height += 2;
            if (S_StateList.arraySize <= index) return;

            var stateProperty = S_StateList.GetArrayElementAtIndex(index);


            var activeRect = new  Rect( rect);
            activeRect.width -= 20;
            activeRect.x += 20;


            var ActiveRect = new Rect(rect.x - 2, rect.y - 3, 20, activeRect.height);
            var StateRect = new Rect(activeRect.x - 5, activeRect.y, activeRect.width - 30, activeRect.height - 5);
            var PriorityRect = new Rect(activeRect.width + 45, activeRect.y, 25, activeRect.height - 2);

            if (Application.isPlaying) StateRect.width = activeRect.width / 2f + 10;


            State state = stateProperty.objectReferenceValue as State;

            // Remove the ability if it no longer exists.
            if (state == null)
            {
                EditorGUI.ObjectField(StateRect, stateProperty, GUIContent.none);
                return;
            }

            state.Active = EditorGUI.Toggle(ActiveRect, GUIContent.none, state.Active);

            var st_label = "";

            if (Application.isPlaying)
            {
                if (m.ActiveState == state)
                {
                    if (state.IsPending) st_label = "[Pending]";
                    else st_label = "[Active]";

                    if (state.IsPersistent) st_label += "[Pers]";
                }
                else if (state.IsSleepFromState) st_label = "[Sleep by State]";
                else if (state.IsSleepFromMode) st_label = "[Sleep by Mode]";
                else if (state.IsSleepFromStance) st_label = "[Sleep by Stance]";
                else if (state.OnActiveQueue) st_label = "[Active Queue]";
                else if (state.OnQueue) st_label = "[Queued]";
                else if (state.OnHoldByReset) st_label = "[On Hold Reset]";
            }

            EditorGUI.ObjectField(StateRect, stateProperty, GUIContent.none);

            var style = new GUIStyle(EditorStyles.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter };

            if (Application.isPlaying && m.isActiveAndEnabled && state != null)
            {
                var activestate = m.ActiveState;

                if (activestate != null)
                {
                    if (state.IsPersistent)
                    {
                        style.normal.textColor = Color.green;
                    }

                    if (state.Priority < activestate.Priority && activestate.IsPersistent)
                    {
                        style.normal.textColor = new Color(style.normal.textColor.r, style.normal.textColor.g, style.normal.textColor.b, style.normal.textColor.a / 2);
                    }
                }

                var Rect_Label = new Rect() { x = activeRect.width / 2 + 80, width = activeRect.width / 2 - 36, y = activeRect.y, height = activeRect.height };

                EditorGUI.LabelField(Rect_Label, st_label, style);
            }

            PriorityRect.height = 18;


            state.Priority = EditorGUI.IntField(PriorityRect, GUIContent.none, state.Priority);

            EditorGUI.BeginChangeCheck();
            activeRect = rect;
            activeRect.x += activeRect.width - 34;
            activeRect.width = 20;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "MAnimal Inspector Changed");
            }
        }
        private void OnReorderCallback_States(ReorderableList list)
        {
            for (int i = 0; i < S_StateList.arraySize; ++i)
            {
                m.states[i].Priority = S_StateList.arraySize - i;
                EditorUtility.SetDirty(m.states[i]);
            }
            EditorUtility.SetDirty(target);
        }
        private void OnReorderCallback_States_Details(ReorderableList list, int oldIndex, int newIndex)
        {
            for (int i = 0; i < m.states.Count; i++)
            {
                m.states[i].Priority = m.states.Count - i;
                EditorUtility.SetDirty(m.states[i]);
            }
            EditorUtility.SetDirty(target);
        }

        private void OnAddCallback_State(ReorderableList list)
        {
            addMenu = new GenericMenu();

            for (int i = 0; i < StatesType.Count; i++)
            {
                Type st = StatesType[i];

                bool founded = false;
                for (int j = 0; j < m.states.Count; j++)
                {
                    if (m.states[j].GetType() == st)
                    {
                        founded = true;
                    }
                }

                if (!founded)
                {
                    //Fast Ugly get the name of the Asset thing
                    State state = (State)CreateInstance(st);
                    var name = state.StateName;
                    DestroyImmediate(state);

                    addMenu.AddItem(new GUIContent(name), false, () => AddState(st, st.Name));
                }
            }
            addMenu.ShowAsContext();
        }

        /// <summary> The ReordableList remove button has been pressed. Remove the selected ability.</summary>
        private void OnRemove_State(ReorderableList list)
        {
            // bool DeleteAsset = false;
            State state = S_StateList.GetArrayElementAtIndex(list.index).objectReferenceValue as State;

            S_StateList.DeleteArrayElementAtIndex(list.index);

#if !UNITY_2020_1_OR_NEWER
                if (state != null)
                    S_StateList.DeleteArrayElementAtIndex(list.index); //HACK
#endif


            list.index -= 1;

           EditorUtility.SetDirty(m);
        }

        /// <summary>Adds a new State of the specified type.</summary>       
        private void AddState(Type selectedState, string name)
        {
            State state = (State)CreateInstance(selectedState);

            var nameS = m.name.RemoveSpecialCharacters();

            if (m.states != null && m.states.Count > 0)
            {
                var anySt = m.states[0];
                var path = AssetDatabase.GetAssetPath(anySt);

                path = System.IO.Path.GetDirectoryName(path);
                //Debug.Log("path = " + path);
                AssetDatabase.CreateAsset(state, $"{path}/{nameS} {name}.asset");
            }
            else
            {
                AssetDatabase.CreateAsset(state, $"Assets/{nameS} {name}.asset");
            }

            AssetDatabase.SaveAssets();

            // Pull all the information from the target of the serializedObject.
            S_StateList.serializedObject.Update();
            // Add a null array element to the start of the array then populate it with the object parameter.
            S_StateList.InsertArrayElementAtIndex(0);
            S_StateList.GetArrayElementAtIndex(0).objectReferenceValue = state;
            // Push all the information on the serializedObject back to the target.
            S_StateList.serializedObject.ApplyModifiedProperties();

            state.Priority = S_StateList.arraySize;  //Set the priority!! Important!

            state.SetSpeedSets(m);

            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(state);
        }

        #endregion

        //-------------------------MODES-----------------------------------------------------------
        private void Draw_Header_Modes(Rect rect)
        {
            var r = new Rect(rect);
            var a = new Rect(rect);
            a.width = 65;
            EditorGUI.LabelField(a, new GUIContent("  Active", "Is the Mode Enable or Disable"));
            r.x += 60;
            r.width = 60;
            EditorGUI.LabelField(r, new GUIContent("Mode", "Modes are the Animations that can be played on top of the States"));

            var activeRect = rect;
            activeRect.width -= 20;
            activeRect.x += 20;
            var prioRect = new Rect(activeRect.width + 30, activeRect.y, 45, activeRect.height);
            var IDRect = new Rect(activeRect.width + 5, activeRect.y, 35, activeRect.height);

            EditorGUI.LabelField(IDRect, new GUIContent("ID", "Mode ID:\n Numerical ID value for the Mode"));
            EditorGUI.LabelField(prioRect, new GUIContent("Pri", "Priority:\n If A mode has 'Ignore Lower Modes' enabled, it will play even if a Lower Mode is Playing"));
        }
        private void Draw_Element_Modes(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;
            if (S_Mode_List.arraySize <= index) return;

            EditorGUI.BeginChangeCheck();
            {
                var ModeProperty = S_Mode_List.GetArrayElementAtIndex(index);
                var active = ModeProperty.FindPropertyRelative("active");
                var ID = ModeProperty.FindPropertyRelative("ID");

                var rectan = new Rect(rect);
                rectan.width -= 20;
                rectan.x += 20;
                rectan.y -= 2;

                var activeRect1 = new Rect(rect.x, rect.y - 2, 20, rect.height);
                var IDRect = new Rect(rect.x + 40, rect.y, rect.width - 90, EditorGUIUtility.singleLineHeight);

                var IDVal = new Rect(rectan.width + 9, rectan.y + 3, 35, rectan.height);

                active.boolValue = EditorGUI.Toggle(activeRect1, GUIContent.none, active.boolValue);

                EditorGUI.PropertyField(IDRect, ID, GUIContent.none);

                var style = new GUIStyle(EditorStyles.boldLabel)
                { alignment = TextAnchor.UpperRight };

                if (m.modes[index].ID != null)
                {
                    EditorGUI.LabelField(IDVal, m.modes[index].ID.ID.ToString(), style);
                }

                var priorityRect = new Rect(rectan.width + 42, rectan.y, 25, rectan.height);

                EditorGUI.LabelField(priorityRect, "│" + (S_Mode_List.arraySize - index - 1));

            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Inspector");
                EditorUtility.SetDirty(target);
            }
        }
        private void OnAdd_Modes(ReorderableList list)
        {
            if (m.modes == null) m.modes = new List<Mode>();


            Ability newAbility = new Ability()
            {
                active = new BoolReference(true),
                Index = new IntReference(1),
                Name = "AbilityName"
            };

            var newMode = new Mode()
            {
                Abilities = new List<Ability>(1) { newAbility },
            };

            m.modes.Add(newMode);
            EditorUtility.SetDirty(m);
        }
        private void OnRemoveCallback_Mode(ReorderableList list)
        {
            // The reference value must be null in order for the element to be removed from the SerializedProperty array.
            S_Mode_List.DeleteArrayElementAtIndex(list.index);
            list.index -= 1;

            if (list.index == -1 && S_Mode_List.arraySize > 0)  //In Case you remove the first one
            {
                list.index = 0;
            }
            SelectedMode.intValue--;
            list.index = Mathf.Clamp(list.index, 0, list.index - 1);

            EditorUtility.SetDirty(m);
        }
        public void SetPivots()
        {
            m.Pivot_Hip = m.pivots.Find(item => item.name.ToUpper() == "HIP");
            m.Pivot_Chest = m.pivots.Find(item => item.name.ToUpper() == "CHEST");
        }

        #region GUICONTENT

        readonly GUIContent G_Rotator = new GUIContent("Rotator", "Used to add extra Rotations to the Animal");
        readonly GUIContent G_RootBone = new GUIContent("RootBone", "Bone to Identify the Main Root Bone of the Animal. Mainly Used for TimeLine and Flying Animals");
        //readonly GUIContent G_Center = new GUIContent("Center", "Center of the Animal to be used for AI and Targeting");
        readonly GUIContent G_RayCastRadius = new GUIContent("RayCast Radius", "Instead of using Raycast for checking the ground beneath the animal we use SphereCast, this is the Radius of that Sphere");
        readonly GUIContent G_AlignLoop = new GUIContent("Align Cycle", "When the Animal is grounded the Controller will check every X frame for the Ground... Higher values: better performance -> less acurancy");
        readonly GUIContent G_animalType = new GUIContent("Type", "Value set on the Animator for Additive Pose Fixing");
        //readonly GUIContent G_AnimatorUpdatePhysics = new GUIContent("Update Physics?", "if True the it will use FixedUpdate for all his calculations. Use this if you are using the creature as the Main Character");
        //readonly GUIContent G_UpdateParameters = new GUIContent("Update Params", "Update all Parameters in the Animator Controller");
      
        readonly GUIContent G_AbilityIndex = new GUIContent("Active", "Active Ability Index \n(if set to -99 it will Play a Random Ability )\n(if set to 0 it wont play anything)");
        readonly GUIContent G_DefaultIndex = new GUIContent("Default", "Default Ability Index to return to when exiting the mode \n(if set to -99 it will Play a Random Ability )");
        readonly GUIContent G_ResetToDefault = new GUIContent("R", "Reset to Default:\nWhen Exiting the Mode\nthe Active Index will reset\nto the Default");
        readonly GUIContent G_CloneStates = new GUIContent("Clone States", "Creates instances of the States so they cannot be overwritten by other animals using the same scriptable objects");
        readonly GUIContent G_Height = new GUIContent("Height", "Distance from Animal Hip to the ground");
        readonly GUIContent G_GroundLayer = new GUIContent("Ground Layer", "Layers the Animal considers ground");
        readonly GUIContent G_AlignPosLerp = new GUIContent("Align Pos Lerp", "Smoothness value to Snap to ground while Grounded");
        readonly GUIContent G_AlignPosDelta = new GUIContent("Align Pos Delta", "Smoothness Position value to Snap to ground when using a non Grounded State");
        readonly GUIContent G_AlignRotDelta = new GUIContent("Align Rot Delta", "Smoothness Rotaion value to Snap to ground when using a non Grounded State");
        readonly GUIContent G_AlignRotLerp = new GUIContent("Align Rot Lerp", "Smoothness value to Align to ground slopes while Grounded");
       
        readonly GUIContent G_Modifier = new GUIContent("Modifier", "Extra Logic to give the Animal when Entering or Exiting the Modes");

        readonly GUIContent G_gravityDirection = new GUIContent("Direction", "Direction of the Gravity applied to the animal");
        readonly GUIContent G_GravityForce = new GUIContent("Force", "Force of the Gravity, by Default it 9.8");
        readonly GUIContent G_GravityCycle = new GUIContent("Start Gravity Cycle", "Start the gravity with an extra time to push the animal down.... higher values stronger Gravity");
        readonly GUIContent G_GravityLimitCycle = new GUIContent("Limit Gravity Cycle", "If greater than zero, Limits the Gravity Cycle, making a steady fall after a time");

        readonly GUIContent G_useSprintGlobal = new GUIContent("Can Sprint", "Can the Animal Sprint?");
        readonly GUIContent G_CanStrafe = new GUIContent("Can Strafe", "Can the Animal Strafe?\nStrafing requires new sets of strafe animations. Make sure you have proper animations to Use this feature. Check the Help button for more Info [?]");
        readonly GUIContent G_Strafe = new GUIContent("Strafe", "Activate the Strafe on the Animal.");
        readonly GUIContent G_StrafeNormalize = new GUIContent("Normalize", "Normalize the value of the Strafe Angle on the Animation (-1 to 1 instead of -180 to 180)");
        readonly GUIContent G_StrafeLerp = new GUIContent("Lerp", "Lerp Value to smoothly enter the  Strafe");


        //readonly GUIContent G_sprint = new GUIContent("Sprint", " Sprint value of the Animal. Sprint allows increase 1 speed modifier");
        //readonly GUIContent G_Grounded = new GUIContent("Grounded", " Grounded value of the Animal. if True the animal will aling itself with the Terrain");


        readonly GUIContent G_SmoothVertical = new GUIContent("Smooth Vertical", "Used for Joysticks to increase the speed by the Stick Pressure");
      //  readonly GUIContent G_TurnMultiplier = new GUIContent("Turn Multiplier", "Global turn multiplier to increase rotation on the animal");
        //  readonly GUIContent G_UpDownLerp = new GUIContent("Up Down Lerp", "Lerp Value for the UpDown Axis");
        //readonly GUIContent G_rootMotion = new GUIContent("Root Motion", "Enable Disable the Root motion on the Animator");

        readonly GUIContent G_Player = new GUIContent("Player", "True if this will be your main Character Player, used for Respawing characters");
        readonly GUIContent G_OverrideStartState = new GUIContent("Override Start State", "Overrides the Start State");
        readonly GUIContent G_StartWithMode = new GUIContent("Start with Mode", "On Start .. Plays a Mode. Use the Mode ID.\nIf you want an specific Ability within the mode. Set the Mode and the Ability in the Format (Mode*1000+Ability). E.g Eat = 4008");
        #endregion

        //-------------------------STATES-----------------------------------------------------------
        void OnSceneGUI()
        {
            foreach (var pivot in m.pivots)
            {
                if (pivot.EditorModify)
                {
                    Transform t = m.transform;
                    EditorGUI.BeginChangeCheck();

                    Vector3 piv = t.TransformPoint(pivot.position);
                    Vector3 NewPivPosition = Handles.PositionHandle(piv, t.rotation);
                    //   pivot.position = m.transform.InverseTransformPoint(NewPivPosition);

                    float multiplier = Handles.ScaleSlider(pivot.multiplier, piv, pivot.WorldDir(t), Quaternion.identity, HandleUtility.GetHandleSize(piv), 0f);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m, "Pivots");
                        pivot.position = t.InverseTransformPoint(NewPivPosition);
                        pivot.multiplier = multiplier;
                    }
                }
            }
        }
    }
}