using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using UnityEditor.Animations;
//using UnityEditorInternal;
using MalbersAnimations;
using MalbersAnimations.Controller;
using Object = UnityEngine.Object;
using System.Linq;
using System;

namespace MalbersAnimations
{
    [System.Serializable]
    public class ModeItemAnim
    {
        public string name;
        public int index;
        public Motion clip;
    }
     
    public class MalbersAnimatorTools : EditorWindow
    {
        private AnimatorState[] m_AnimatorStates;
        private AnimatorStateMachine[] m_StateMachines;
        private AnimatorTransitionBase[] m_Transitions;

        public AnimatorController controller;
        public MAnimal Animal;
         
        public StateID State;
        public StateID LastState;
        public ModeID Mode;
        public StanceID Stance;
        public StanceID LastStance;

        private int Editor_Tabs1;
        private int SelectedParameter;
        private int condition;
        private float value;

        private SerializedObject serializedObject;
        SerializedProperty p_Animal, p_Controller, p_Abilities;


        public float m_ExitTime;
        public bool m_FixedDuration;
        public float m_TransitionDuration;
        public float m_TransitionOffset;
        public TransitionInterruptionSource m_InterruptionSource;
        public bool m_OrderedInterruption = true;
        public bool m_TransitionToSelf = false;


        bool HAS_AS => m_AnimatorStates.Length > 0;
        bool HAS_ASM => m_StateMachines.Length > 0;
        bool HAS_T => m_Transitions.Length > 0;

        public Vector2 Scroll;

        public bool ModifyTV = true;
        public bool CreateNewConditionF = true;

        string[] ModeAbilities;
        public List<ModeItemAnim> Abilities;
        int[] ModeAbilitiesIndex;

        //List<string> popupOptions; 
        private GUIContent plus;
        private GUIContent GC_AS;
        private GUIContent GC_T;
        private GUIContent GC_ASM;
        private GUIContent updateB;
        private GUIContent gc_modes;

        /// <summary> Cached style to use to draw the popup button. </summary>
        private GUIStyle popupStyle;

        [MenuItem("Tools/Malbers Animations/Animator Tools")]
        public static void ShowWindow()
        {
            var window = (MalbersAnimatorTools)GetWindow(typeof(MalbersAnimatorTools), false, "Animator Tools");
            window.minSize = new Vector2(250, 120);

            window.titleContent = new GUIContent("AC Animator Tools", EditorGUIUtility.ObjectContent(null, typeof(AnimatorController)).image);
            //window.maxSize = new Vector2(650, 500); 
            window.Show();
        }

        private static GUIContent[] gs;


        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);

            p_Animal = serializedObject.FindProperty("Animal");
            p_Controller = serializedObject.FindProperty("controller");
            p_Abilities = serializedObject.FindProperty("Abilities");

            FindImages();

            m_AnimatorStates = Selection.GetFiltered<AnimatorState>(SelectionMode.Editable);
            m_StateMachines = Selection.GetFiltered<AnimatorStateMachine>(SelectionMode.Editable);
            m_Transitions = Selection.GetFiltered<AnimatorTransitionBase>(SelectionMode.Editable);

            OnSelectionChange();

            p_Abilities.isExpanded = true;
        }

        private void FindImages()
        {
            var img_State = EditorGUIUtility.ObjectContent(CreateInstance<StateID>(), typeof(StateID)).image;
            var img_Mode = EditorGUIUtility.ObjectContent(CreateInstance<ModeID>(), typeof(ModeID)).image;
            var img_Stance = EditorGUIUtility.ObjectContent(CreateInstance<StanceID>(), typeof(StanceID)).image;
            var img_Transition = EditorGUIUtility.ObjectContent(null, typeof(AnimatorStateTransition)).image;
            var img_StateA = EditorGUIUtility.ObjectContent(null, typeof(AnimatorState)).image;
            var img_StateAM = EditorGUIUtility.ObjectContent(null, typeof(AnimatorStateMachine)).image;
            //var img_StateMachine = EditorGUIUtility.ObjectContent(null, typeof(AnimatorState)).image;


            GC_AS = new GUIContent("Add", img_StateA, "Add the new State");
            GC_T = new GUIContent("Add", img_Transition, "Add the new Transition");
            GC_ASM = new GUIContent("Add", img_StateAM, "Add the new Animator State");

            gc_modes = new GUIContent("Add", img_Mode);

            gs = new GUIContent[] {
                new GUIContent(" States", img_State),
                new GUIContent(" Modes", img_Mode),
                new GUIContent(" Stances", img_Stance),
                new GUIContent(" Transitions",img_Transition),
            };
        }


        private void OnSelectionChange()
        {
            m_AnimatorStates = Selection.GetFiltered<AnimatorState>(SelectionMode.Editable);

            m_StateMachines = Selection.GetFiltered<AnimatorStateMachine>(SelectionMode.Editable);
            m_Transitions = Selection.GetFiltered<AnimatorTransitionBase>(SelectionMode.Editable);
            //AnimatorLayers = Selection.GetFiltered<AnimatorControllerLayer>(SelectionMode.Unfiltered);

            //if (m_StateMachines.Length > 0)

            //{
            //    Debug.Log(" m_StateMachines[0].name = " + m_StateMachines[0].name);
            //    Debug.Log(" controller.layers[0].stateMachine.name = " + controller.layers[0].stateMachine.name);
            //}

            Abilities = new List<ModeItemAnim>();


            ModeAbilities = new string[m_AnimatorStates.Length];
            ModeAbilitiesIndex = new int[m_AnimatorStates.Length];


            for (int i = 0; i < ModeAbilitiesIndex.Length; i++)
            {
                ModeAbilitiesIndex[i] = i + 1;
            }

            for (int i = 0; i < m_AnimatorStates.Length; i++)
            {
                var nameAS = m_AnimatorStates[i].name;
                ModeAbilities[i] = nameAS;

                Abilities.Add(new ModeItemAnim() { name = m_AnimatorStates[i].name, index = i + 1, clip = m_AnimatorStates[i].motion });

                p_Abilities.serializedObject.Update();
                p_Abilities.serializedObject.ApplyModifiedProperties();

                //serializedObject.ApplyModifiedProperties();
            }

            var gameobjects = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel);

            foreach (var o in gameobjects)
            {
                var AA = o.GetComponent<Animator>();
                p_Animal.objectReferenceValue = o.GetComponent<MAnimal>();

                if (p_Animal.objectReferenceValue != null)
                {
                    var C = (AnimatorController)AA.runtimeAnimatorController;

                    //if (C.layers[0].name != "States") C.layers[0].name = "States";
                    //if (C.layers[0].stateMachine.name != "States") C.layers[0].stateMachine.name = "States";

                    p_Controller.objectReferenceValue = C;


                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return;
                }
            }


            string Path ;

            //Find the Animator Controller  via AssetPath
            if (m_StateMachines.Length > 0)
                Path = AssetDatabase.GetAssetPath(m_StateMachines[0]);
            else if (m_AnimatorStates.Length > 0)
                Path = AssetDatabase.GetAssetPath(m_AnimatorStates[0]);
            else if (m_Transitions.Length > 0)
                Path = AssetDatabase.GetAssetPath(m_Transitions[0]);
            else
            {
                return;
            }

                var CC = (AnimatorController)AssetDatabase.LoadAssetAtPath(Path, typeof(AnimatorController));
            if (p_Controller.objectReferenceValue != CC)
            {

                //if (C.layers[0].name != "States") C.layers[0].name = "States";
                //if (C.layers[0].stateMachine.name != "States") C.layers[0].stateMachine.name = "States";
                p_Controller.objectReferenceValue = CC;
                p_Animal.objectReferenceValue = null;
            }

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }



        private void OnGUI()
        {
          //  serializedObject.Update();

            MalbersEditor.DrawDescription("This tools helps create the correct transitions for any Animal Controller");

            //  EditorGUILayout.PropertyField(SP_AllStanceIDs, true);

            FindIcons();

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.LabelField("SELECTED: "+
                $" Anim States[{m_AnimatorStates.Length}]. " +
                $" States Machine [{m_StateMachines.Length}]. " +
                $" Transitions [{m_Transitions.Length}]", EditorStyles.boldLabel);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(p_Animal);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(p_Controller);

                    if (p_Controller.objectReferenceValue)
                    {
                        CheckParameters();
                    }
                }
            }

                Editor_Tabs1 = GUILayout.Toolbar(Editor_Tabs1, gs, GUILayout.Height(22));

           // Debug.Log("controller = " + controller);
            if (controller == null) return;



            if (Editor_Tabs1 == 0) { DoStates(); } //States
            if (Editor_Tabs1 == 1) { DoModes(); } //States
            if (Editor_Tabs1 == 2) { DoStances(); } //States
            if (Editor_Tabs1 == 3) { DoTransitions(); } //States

            serializedObject.ApplyModifiedProperties();

        }

        private void CheckParameters()
        {
            if (GUILayout.Button(new GUIContent("P", "Check/Add Main Parameters"), GUILayout.Width(28)))
            {
                CheckAnimParameter("StateOn", UnityEngine.AnimatorControllerParameterType.Trigger);
                CheckAnimParameter("ModeOn", UnityEngine.AnimatorControllerParameterType.Trigger);
                CheckAnimParameter("Vertical", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("Horizontal", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("State", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("LastState", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("StateStatus", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("StateExitStatus", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("StateFloat", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("Mode", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("ModeStatus", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("Grounded", UnityEngine.AnimatorControllerParameterType.Bool);
                CheckAnimParameter("Movement", UnityEngine.AnimatorControllerParameterType.Bool);
                CheckAnimParameter("SpeedMultiplier", UnityEngine.AnimatorControllerParameterType.Float);
            }

            if (GUILayout.Button(new GUIContent("p", "Check/Add Optional Parameters"), GUILayout.Width(28)))
            {
                CheckAnimParameter("UpDown", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("DeltaUpDown", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("TargetAngle", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("StrafeAnim", UnityEngine.AnimatorControllerParameterType.Bool);
                CheckAnimParameter("StrafeAngle", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("Stance", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("LastStance", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("Random", UnityEngine.AnimatorControllerParameterType.Int);
                CheckAnimParameter("ModePower", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("Slope", UnityEngine.AnimatorControllerParameterType.Float);
                CheckAnimParameter("Sprint", UnityEngine.AnimatorControllerParameterType.Bool);
            }
        }

        private void CheckAnimParameter(string PName, AnimatorControllerParameterType Ptype)
        {
            var AllParameters = controller.parameters.ToList();

            if (!AllParameters.Exists(p => p.name == PName))
            {
                var p =
                new AnimatorControllerParameter() { name = PName, type = Ptype };
                controller.AddParameter(p);
                Debug.Log($"Added to the Animator [{controller.name}] the  {Ptype.ToString()} Parameter [{PName}]");
            }
            else
            {
                Debug.Log($"Animator [{controller.name}] already has the {Ptype.ToString()} Parameter: [{PName}]");
            }
        }

        private void FindIcons()
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }


            if (plus == null)
            {
                plus = UnityEditor.EditorGUIUtility.IconContent("d_Toolbar Plus");
                plus.tooltip = "Add Condition to all selected transitions";
            }
            if (updateB == null)
            {
                updateB = UnityEditor.EditorGUIUtility.IconContent("d_RotateTool");
                updateB.tooltip = "Update Value";
            }
        }
         

        private void DoStates()
        {
            EditorGUILayout.Space(4);

            DrawStateConditions();

            EditorGUILayout.Space(4);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"   Actions", EditorStyles.boldLabel);

                if (State != null)
                {
                    if (m_StateMachines.Length == 1)
                    {
                        #region BUTTON CREATE NEW STATE
                        using (new GUILayout.HorizontalScope())
                        {
                            //
                            if (GUILayout.Button(GC_AS, GUILayout.Height(22), GUILayout.Width(80)))
                            {

                                var newAnimState = m_StateMachines[0].AddState(State.name);
                                newAnimState.tag = State.name;
                                newAnimState.speedParameter = "SpeedMultiplier";
                                newAnimState.speedParameterActive = true;
                            }
                            EditorGUILayout.LabelField($"*New [{State.name}] State", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }
                        #endregion


                        #region BUTTON CREATE NEW  STATE MACHINE
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(GC_ASM, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                var newAnimState = m_StateMachines[0].AddStateMachine(State.name);
                            }
                            EditorGUILayout.LabelField($"*New [{State.name}] StateMachine", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }
                        #endregion
                    }

                    //if there's any Anim State selecter
                    if (m_AnimatorStates.Length > 0 ||
                        //OR if theres any State Machine Selected but don't include the Base Layer State Machine
                        (m_StateMachines.Length > 0 && m_StateMachines[0].name != controller.layers[0].stateMachine.name))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                foreach (var AS in m_AnimatorStates)
                                {
                                    var Parent = FindAnyState(AS);

                                    var Any_ToState = Parent.AddAnyStateTransition(AS); //Add Any
                                    Create_AnyToState(Any_ToState);
                                }

                                foreach (var ASM in m_StateMachines)
                                {
                                    if (isLayerRoot(ASM)) continue; //Do not do the Root Layer


                                    Debug.Log("ASM = " + ASM.name);
                                    var Parent = FindAnyState(ASM);
                                    var Any_ToState = Parent.AddAnyStateTransition(ASM); //Add Any
                                    Create_AnyToState(Any_ToState);
                                }
                            }

                            EditorGUILayout.LabelField($"*New Transition [{State.name}] to Selected States [{m_AnimatorStates.Length}]. [CHECK *ANY*]",
                           EditorStyles.boldLabel, GUILayout.MinWidth(50));

                        }
                    }
                }
            }
        }

        private void DrawStateConditions()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Conditions", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("State"));
                    if (State != null)
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.IntField(State.ID, GUILayout.Width(50));
                    }
                }

                if (State != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LastState"));

                        if (LastState != null)
                        {
                            using (new EditorGUI.DisabledGroupScope(true))
                                EditorGUILayout.IntField(LastState.ID, GUILayout.Width(50));
                        }
                    }
                }
            }
        }

        private void Create_AnyToStateMachineMode(AnimatorStateTransition Any_ToState)
        {
            Any_ToState.name = "To: " + Mode.name;
            Any_ToState.duration = 0.2f;
            Any_ToState.offset = 0;
            Any_ToState.hasExitTime = false;

            Any_ToState.AddCondition(AnimatorConditionMode.If, 0, "ModeOn");

            Any_ToState.AddCondition(AnimatorConditionMode.Greater, Mode.ID*1000, "Mode");
            Any_ToState.AddCondition(AnimatorConditionMode.Less, Mode.ID * 1000 + 1000, "Mode");


            EditorUtility.SetDirty(Any_ToState); 
            p_Controller.serializedObject.ApplyModifiedProperties();
            p_Controller.serializedObject.Update();

            // EditorGUIUtility.PingObject(controller); //Final way of changing the name of the asset... dirty but it works
        }


        private void Create_AnyToState(AnimatorStateTransition Any_ToState)
        {
            Any_ToState.name = State.name;
            Any_ToState.duration = 0.2f;
            Any_ToState.offset = 0;
            Any_ToState.hasExitTime = false;

            Any_ToState.AddCondition(AnimatorConditionMode.If, 0, "StateOn");

            Any_ToState.AddCondition(AnimatorConditionMode.Equals, State.ID, "State");


            if (LastState != null)
            {
                Any_ToState.AddCondition(AnimatorConditionMode.Equals, LastState.ID, "LastState");
                Any_ToState.name = $"{State.name} from [{LastState.name}]";

            }

            p_Controller.serializedObject.ApplyModifiedProperties();
            p_Controller.serializedObject.Update();

           // EditorGUIUtility.PingObject(controller); //Final way of changing the name of the asset... dirty but it works
        }

        private void DoModes()
        {
            EditorGUILayout.Space(4);
            DrawModeConditions();
            EditorGUILayout.Space(4);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                //Show Action IDS
                using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField($"   Actions", EditorStyles.boldLabel);
                    if (GUILayout.Button("Check Mode [Action] Abilities",/* GUILayout.Height(22),*/ GUILayout.Width(180)))
                    {
                       var Modal = ShowIDWindow.ShowWindow();
                        Modal.Editor_Tabs1 = 3; //Show Actions
                        GUIUtility.ExitGUI();
                    }
                }

                if (HAS_AS)
                {
                    if (Mode != null)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                DoModeStateTransition();
                                EditorUtility.SetDirty(controller);

                                Debug.Log("<color=green><b>[Entry] Mode Transitions Created</b></color>");
                            }
                            EditorGUILayout.LabelField($"[Entry] Transition", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            //ADD Mode Behaviours!!!!
                            if (GUILayout.Button(gc_modes, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                foreach (var AS in m_AnimatorStates)
                                {

                                    bool hasModeB = AS.behaviours.ToList().Exists(x => x is ModeBehaviour);

                                    if (!hasModeB)
                                    {
                                        var ModeB = AS.AddStateMachineBehaviour<ModeBehaviour>();
                                        ModeB.ModeID = Mode;
                                    }
                                }

                                Debug.Log("<color=green><b>[Mode] Behaviour Created</b></color>");

                            }
                            EditorGUILayout.LabelField($"[Mode] Behaviour", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        //ADD EXIT AND INTERRUPTED TRANSITIONS
                        if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                        {
                            foreach (var AS in m_AnimatorStates)
                                ExitTransition(AS);
                            EditorUtility.SetDirty(controller);

                            Debug.Log("<color=green><b>[Exit] Mode Transtions Created</b></color>");

                        }
                        EditorGUILayout.LabelField($"[Exit] Transtion", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                    }


                    using (new GUILayout.HorizontalScope())
                    {
                        //INTERRUPTED TRANSITIONS
                        if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                        {
                            foreach (var AS in m_AnimatorStates)
                                ExitInterruptedMode(AS);
                            EditorUtility.SetDirty(controller);

                            Debug.Log("<color=green><b>[Interrupted *Old*] Mode Transtions Created</b></color>");

                        }
                        EditorGUILayout.LabelField($"[Interrupted *Old*] Transtion -> [ModeStatus = -2]", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                    }


                    using (new GUILayout.HorizontalScope())
                    {
                        //ADD LOOP TRANSITIONS!!
                        if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                        {
                            foreach (var AS in m_AnimatorStates)
                            {
                                LoopTransition(AS);
                            }

                            Debug.Log("<color=green><b>[Loop] Mode Transtions Created</b></color>");

                        }
                        EditorGUILayout.LabelField($"[Loop] Transition", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                    }
                }


                if (HAS_ASM)
                {
                    if (m_StateMachines.Length == 1 && isLayerStateMachine(m_StateMachines[0]))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(GC_AS, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                var newAnimState = m_StateMachines[0].AddState("Empty");
                                m_StateMachines[0].defaultState = newAnimState;
                            }
                            EditorGUILayout.LabelField($"*New Default Empty Animation State", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }
                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(GC_ASM, GUILayout.Height(22), GUILayout.Width(80)))
                            {
                                NewModeTransitions();
                            }
                            EditorGUILayout.LabelField($"*New Mode Transitions. <Check Any>", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                        }
                    }
                }
            }

            if (Animal && HAS_AS && Mode != null)
            {

                MalbersEditor.DrawDescription($"Animal [{Animal.name}] selected.\nModes and Abilities will be created on the MAnimal Component");

                using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    if (GUILayout.Button(gc_modes, GUILayout.Height(22), GUILayout.Width(80)))
                    {
                        AddModesAnimalComponent();

                        Debug.Log($"<color=green><b>[{Animal.name}]. Mode [{Mode.name}] Abilities Updated</b></color>");

                    }
                    EditorGUILayout.LabelField($"Mode and Abilities to the Animal Component.", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                }


                using (new GUILayout.HorizontalScope())
                {
                    //ADD NEW!!!!!! EXIT AND INTERRUPTED TRANSITIONS
                    if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                    {
                        for (int i = 0; i < m_AnimatorStates.Length; i++)
                        {
                            var AS = m_AnimatorStates[i];

                            var InterruptCondition = new AnimatorCondition
                            {
                                parameter = "Mode",
                                mode = AnimatorConditionMode.NotEqual,
                                threshold = (Mode.ID * 1000 + ModeAbilitiesIndex[i])
                            };

                            ExitTransition(AS, "Interrupted [N]", false, 0.8f, 0.2f, 0, 
                                TransitionInterruptionSource.None, new AnimatorCondition[] { InterruptCondition });
                        }
                        EditorUtility.SetDirty(controller);

                        Debug.Log("<color=green><b> [NEW * Interrupted] Mode Transtion Created</b></color>");

                    }
                    EditorGUILayout.LabelField($"[Interrupted] Transtion -> Mode [NOT EQUAL]", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                }

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                { 
                    using (var X = new GUILayout.ScrollViewScope(Scroll))
                    {
                        Scroll = X.scrollPosition;
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField($" Motion", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                            EditorGUILayout.LabelField($" Ability Name", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                            EditorGUILayout.LabelField($" Index", EditorStyles.boldLabel, GUILayout.Width(50));
                        }



                        for (int i = 0; i < m_AnimatorStates.Length; i++)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                using (new EditorGUI.DisabledGroupScope(true))
                                    EditorGUILayout.ObjectField(m_AnimatorStates[i].motion, typeof(Motion), false, GUILayout.MinWidth(50));

                                ModeAbilities[i] = EditorGUILayout.TextField(ModeAbilities[i], GUILayout.MinWidth(50));
                                ModeAbilitiesIndex[i] = EditorGUILayout.IntField(ModeAbilitiesIndex[i], GUILayout.Width(50));
                            }
                        }
                    } 
                }
            }
        }

        private void NewModeTransitions()
        {
            foreach (var ASM in m_StateMachines)
            {
                if (isLayerRoot(ASM)) continue; //Do not do the Root Layer

                Debug.Log("ASM = " + ASM.name);
                var Parent = FindAnyState(ASM);
                var Any_ToState = Parent.AddAnyStateTransition(ASM); //Add Any to State Machine
                Create_AnyToStateMachineMode(Any_ToState);
                Parent.AddStateMachineExitTransition(ASM);
            }
        }

        private void DrawModeConditions()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Conditions", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"));
                    if (Mode != null)
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.IntField(Mode.ID, GUILayout.Width(50));
                    }
                }
            }
        }

        private void AddModesAnimalComponent()
        {
            //Add on the Animal the mode and the new abilities
            Mode animalMode = Animal.Mode_Get(this.Mode);

            if (animalMode == null)
            {
                animalMode = new Mode()
                { ID = this.Mode, AllowRotation = true, AllowMovement = true, 
                    DefaultIndex = new Scriptables.IntReference(), AbilityIndex = 0};

                Animal.modes.Add(animalMode);
            }

            if (animalMode.Abilities == null) animalMode.Abilities = new List<Ability>();

            for (int i = 0; i < ModeAbilities.Length; i++)
            {
                var Index = ModeAbilitiesIndex[i];

                //do not add one alreadu creadte
                if (animalMode.Abilities.Exists(x => x.Name == ModeAbilities[i] || x.Index.Value == Index)) continue;

                animalMode.Abilities.Add(new Ability() { Name = ModeAbilities[i], Index = new Scriptables.IntReference(Index) });

                UnityEditor.EditorUtility.SetDirty(Animal);

                m_AnimatorStates[i].name = ModeAbilities[i]; //rename the Animator State
            }
            EditorUtility.SetDirty(controller);
        }

        private bool isLayerStateMachine(AnimatorStateMachine animatorStateMachine)
        {
            foreach (var item in controller.layers)
            {
                if (item.stateMachine == animatorStateMachine) return true;
            }
            return false;
        }

        private void DoModeStateTransition()
        { 
            for (int i = 0; i < m_AnimatorStates.Length; i++)
            {
                var AS = m_AnimatorStates[i];
                var Parent = FindAnyState(AS);
                var Any_ToState = Parent.AddAnyStateTransition(AS); //Add Any
                Any_ToState.duration = 0.2f;
                Any_ToState.offset = 0;
                Any_ToState.hasExitTime = false;


                var ModeOn = new AnimatorCondition
                {
                    parameter = "ModeOn",
                    mode = AnimatorConditionMode.If
                };

                var Mode__Value = new AnimatorCondition
                {
                    parameter = "Mode",
                    mode = AnimatorConditionMode.Equals,
                    threshold = (Mode.ID * 1000 + ModeAbilitiesIndex[i])
                };

                Any_ToState.conditions = new AnimatorCondition[2] { ModeOn, Mode__Value };
            }
        }
        private void DoStances()
        {
            if (controller == null) return;

            DrawStanceConditions();

            if (HAS_AS && Stance != null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                    {
                        foreach (var St in m_AnimatorStates)
                        {
                            var Parent = FindParentSM(St);
                            var T = Parent.AddEntryTransition(St);

                           if (State != null) St.tag = State.name;


                            T.AddCondition(AnimatorConditionMode.Equals, Stance.ID, "Stance");

                            T.name = $"to: [{Stance.name}]";

                            if (LastStance != null)
                            {
                                T.AddCondition(AnimatorConditionMode.Equals, LastStance.ID, "LastStance");

                                T.name = $"to: [{Stance.name}] from [{LastStance.name}]";
                            }
                        }
                    }
                    EditorGUILayout.LabelField($"[Start] Transition from <Entry>", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                    {
                        foreach (var St in m_AnimatorStates)
                        {
                            if (State != null) St.tag = State.name;

                            var Parent = FindParentSM(St);
                            var T = Parent.AddAnyStateTransition(St);

                            T.duration = 0.2f;
                            T.offset = 0;
                            T.hasExitTime = false;
                            T.name = $"Any to: [{Stance.name}]";

                            T.AddCondition(AnimatorConditionMode.If, 0, "StateOn");
                            T.AddCondition(AnimatorConditionMode.Equals, State.ID, "State");
                            T.AddCondition(AnimatorConditionMode.Equals, Stance.ID, "Stance");
                            if (LastStance != null)
                            {
                                T.AddCondition(AnimatorConditionMode.Equals, LastStance.ID, "LastStance");
                                T.name = $"Any to: [{Stance.name}] from [{LastStance.name}]";
                            }
                        }

                        Debug.Log("Created Transition. Please check Any State");
                    }
                    EditorGUILayout.LabelField($"[Start] Transition from <AnyState>", EditorStyles.boldLabel, GUILayout.MinWidth(50));
                }
            }
        }

        private void DrawStanceConditions()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Conditions", EditorStyles.boldLabel);
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("State"));
                    if (State != null)
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.IntField(State.ID, GUILayout.Width(50));
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Stance"));
                    if (Stance != null)
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.IntField(Stance.ID, GUILayout.Width(50));
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("LastStance"));
                    if (LastStance != null)
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.IntField(LastStance.ID, GUILayout.Width(50));
                    }
                }
            }
        }

        private void DoTransitions()
        { 
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"   Actions", EditorStyles.boldLabel);

                if (HAS_AS)
                {
                    #region Default Exit Transition
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(GC_AS, GUILayout.Height(22), GUILayout.Width(80)))
                        {
                            foreach (var AS in m_AnimatorStates)
                            {
                                ExitTransition(AS);
                            }

                            //foreach (var AS in m_StateMachines) //EXIT TRANSITIONS CANNOT BE CREATED  for STATE MACHINES
                            //{
                            //    ExitTransition(AS);
                            //}
                        }
                        EditorGUILayout.LabelField($"*New [Default] Exit Transition", EditorStyles.boldLabel);
                    }
                    #endregion


                }

                if (HAS_AS || HAS_ASM)
                {
                    #region Default Loop Mode Transition
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(GC_T, GUILayout.Height(22), GUILayout.Width(80)))
                        {
                            foreach (var AS in m_AnimatorStates)
                            {
                                LoopTransition(AS);
                            }

                            foreach (var AS in m_StateMachines)
                            {
                                LoopTransitionASM(AS);
                            }
                        }
                        EditorGUILayout.LabelField($"*New Loop Transition [Mode-Ability]" +
                            $"[{m_AnimatorStates.Length+ m_StateMachines.Length}]", EditorStyles.boldLabel);
                    }
                    #endregion
                }

                if (m_Transitions.Length > 0)
                {
                    if (controller)
                    {
                        string[] Params = new string[controller.parameters.Length];

                        for (int i = 0; i < Params.Length; i++)
                        {
                            Params[i] = controller.parameters[i].name;
                        }

                        var param = controller.parameters[SelectedParameter];
                        AnimatorConditionMode conditionMode = AnimatorConditionMode.If;

                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUI.indentLevel++;
                            CreateNewConditionF = GUILayout.Toggle(CreateNewConditionF, "Create *New* Condition", EditorStyles.foldoutHeader);
                            EditorGUI.indentLevel--;

                            if (CreateNewConditionF)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    SelectedParameter = EditorGUILayout.Popup(SelectedParameter, Params.ToArray());

                                    switch (param.type)
                                    {
                                        case AnimatorControllerParameterType.Float:
                                            condition = EditorGUILayout.Popup(condition, new string[2] { "Greater", "Less" });

                                            if (condition == 0) conditionMode = AnimatorConditionMode.Greater;
                                            else if (condition == 1) conditionMode = AnimatorConditionMode.Less;

                                            value = EditorGUILayout.FloatField(value);
                                            break;
                                        case AnimatorControllerParameterType.Int:
                                            condition = EditorGUILayout.Popup(condition, new string[4] { "Greater", "Less", "Equal", "Not Equal" });

                                            if (condition == 0) conditionMode = AnimatorConditionMode.Greater;
                                            else if (condition == 1) conditionMode = AnimatorConditionMode.Less;
                                            else if (condition == 2) conditionMode = AnimatorConditionMode.Equals;
                                            else if (condition == 3) conditionMode = AnimatorConditionMode.NotEqual;



                                            value = EditorGUILayout.FloatField(value);
                                            break;
                                        case AnimatorControllerParameterType.Bool:
                                            condition = EditorGUILayout.Popup(condition, new string[2] { "True", "False" });
                                            if (condition == 0) conditionMode = AnimatorConditionMode.If;
                                            else if (condition == 1) conditionMode = AnimatorConditionMode.IfNot;
                                            break;
                                        case AnimatorControllerParameterType.Trigger:
                                            break;
                                        default:
                                            break;
                                    }


                                    if (GUILayout.Button(plus, GUILayout.Width(28), GUILayout.Height(18)))
                                    {
                                        foreach (var transition in m_Transitions)
                                        {
                                            transition.AddCondition(conditionMode, value, param.name);
                                        }
                                        EditorUtility.SetDirty(controller);
                                    }
                                }
                            }
                        }
                    }

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUI.indentLevel++;
                        ModifyTV = GUILayout.Toggle(ModifyTV, "Update Transition Values", EditorStyles.foldoutHeader);
                        EditorGUI.indentLevel--;

                        if (ModifyTV)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                m_ExitTime = EditorGUILayout.FloatField("Exit Time", m_ExitTime);

                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).exitTime = m_ExitTime;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_FixedDuration = EditorGUILayout.Toggle("Fixed Duration", m_FixedDuration);

                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).hasFixedDuration = m_FixedDuration;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_TransitionDuration = EditorGUILayout.FloatField("Transition Duration", m_TransitionDuration);

                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).duration = m_TransitionDuration;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_TransitionOffset = EditorGUILayout.FloatField("Transition Offset", m_TransitionOffset);

                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).offset = m_TransitionOffset;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_InterruptionSource = (TransitionInterruptionSource)EditorGUILayout.EnumPopup("Interruption Source", m_InterruptionSource);


                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).interruptionSource = m_InterruptionSource;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_OrderedInterruption = EditorGUILayout.Toggle("Ordered Interruption", m_OrderedInterruption);


                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).orderedInterruption = m_OrderedInterruption;
                                }
                            }

                            using (new GUILayout.HorizontalScope())
                            {
                                m_TransitionToSelf = EditorGUILayout.Toggle("Can transition to Itself", m_TransitionToSelf);


                                if (GUILayout.Button(updateB, GUILayout.Width(28)))
                                {
                                    foreach (var transition in m_Transitions)
                                        if (transition is AnimatorStateTransition) (transition as AnimatorStateTransition).canTransitionToSelf = m_TransitionToSelf;
                                }
                            }
                        }
                    }

                    using (var X = new GUILayout.ScrollViewScope(Scroll))
                    {
                        Scroll = X.scrollPosition;

                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            foreach (var AS in m_Transitions)
                            {
                                var TName = AS.isExit ? "Exit" : "T";


                                if (AS.destinationState != null) TName = AS.destinationState.name;
                                else if (AS.destinationStateMachine != null) TName = AS.destinationStateMachine.name;

                                if (!string.IsNullOrEmpty(AS.name)) TName += $" [{AS.name}]";

                                EditorGUILayout.LabelField($"Transition to: {TName}");
                            }
                        }
                    }
                }
            }
        }

        private void LoopTransition(AnimatorState _AS)
        {
            var loopTransition = _AS.AddTransition(_AS, true);

            loopTransition.AddCondition(AnimatorConditionMode.Equals, -1, "ModeStatus");

            loopTransition.name = "Loop";
            loopTransition.exitTime = 0.55f;
            loopTransition.duration = 0.2f;
            loopTransition.offset = 0.2f;
            loopTransition.interruptionSource = TransitionInterruptionSource.Destination;

        }
        private void LoopTransitionASM(AnimatorStateMachine _AS)
        {
            var loopTransition = FindParentSM(_AS).AddStateMachineTransition(_AS, _AS);
            loopTransition.AddCondition(AnimatorConditionMode.Equals, -1, "ModeStatus");
            loopTransition.name = "Loop";

            Debug.Log("_AS = " + _AS.name);

            foreach (var SMM in _AS.stateMachines)
            {
                var SM = SMM.stateMachine;
                foreach (var item in _AS.GetStateMachineTransitions(SM))
                {
                    Debug.Log("item = " + item.name);
                }
            }


            //loopTransition.exitTime = 0.55f;
            //loopTransition.duration = 0.2f;
            //loopTransition.offset = 0.2f;
            //loopTransition.interruptionSource = TransitionInterruptionSource.Destination;
        }

         
        public void AddTransitionCondition(AnimatorTransitionBase transition, AnimatorControllerParameter param, AnimatorConditionMode condition, float value)
        {
            if (transition == null) return;

            transition.AddCondition(condition, value, param.name);
        }


        private void ExitTransition(AnimatorState AS)
        {
            ExitTransition(AS, "Exit", true, 0.8f, 0.2f);
        }

        private void ExitTransition(AnimatorStateMachine AS)
        {
            ExitTransition(AS, "Exit", null);
        }

        private void ExitInterruptedMode(AnimatorState AS)
        {
            var InterruptCondition = new AnimatorCondition
            {
                parameter = "ModeStatus",
                mode = AnimatorConditionMode.Equals,
                threshold = -2    //CAMBIAR A EXIT MODE
            };

            ExitTransition(AS, "Interrupted", false, 0.8f, 0.2f, 0, TransitionInterruptionSource.None,  new AnimatorCondition[1] { InterruptCondition });
        }


        private AnimatorStateTransition ExitTransition(AnimatorState AS, string name = "",
            bool hasExitTime = false, float exitTime = 0.8f, float duration = 0.2f,
            float offset = 0, TransitionInterruptionSource intSource = TransitionInterruptionSource.None, AnimatorCondition[] conditions = null)
        {
            var transition = AS.AddExitTransition();
            transition.hasExitTime = hasExitTime;
            transition.exitTime = exitTime;
            transition.duration = duration;
            transition.offset = offset;
            if (name != "") transition.name = name;
            transition.interruptionSource = intSource;
            transition.conditions = conditions;

            return transition;
        }


        private AnimatorTransition ExitTransition(AnimatorStateMachine ASM, string name = "", AnimatorCondition[] conditions = null)
        {
            var parent = FindParentSM(ASM);
            Debug.Log("transition = " + parent.name);

            var transition = ASM.AddStateMachineExitTransition(parent);
            transition.isExit = true;

            if (name != "") transition.name = name;
            transition.conditions = conditions;

            return transition;
        }
 


        private void AnyState()
        {
            foreach (var SM in m_StateMachines)
            {
                // SM.name
            }
        }

        public AnimatorStateMachine FindParentSM(AnimatorState child)
        {
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var layer = controller.layers[i]; //Store the First State Machine which is the Layer

                var result = FindParentSM(layer.stateMachine, child);
                if (result != null) return result;
            }
            return null;
        }


        public bool isLayerRoot(AnimatorStateMachine ASM)
        {
            var layer = controller.layers[0]; //Store the First State Machine which is the Layer
            if (layer.stateMachine == ASM) return true; //is the Same Layer SM... ignore!
            return false;
        }



        public AnimatorStateMachine FindParentSM(AnimatorStateMachine child)
        {
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var layer = controller.layers[i]; //Store the First State Machine which is the Layer

                if (layer.stateMachine == child) return layer.stateMachine; //is the Same Layer SM... ignore!

                var result = FindParentSM(layer.stateMachine, child);
                if (result != null) return result;
            }
            return null;
        }

        public AnimatorStateMachine FindParentSM(AnimatorStateMachine Parent, AnimatorState child)
        {
            AnimatorStateMachine result = null;

            //Check all State Machine States to see if it correspond to see if it the AnimatorState Parent
            for (int i = 0; i < Parent.states.Length; i++)
            {
                if (Parent.states[i].state == child)
                {
                    return Parent;
                }
            }

            //If the child was not found Search all the StateMachines children then
            for (int i = 0; i < Parent.stateMachines.Length; i++)
            {
                result = FindParentSM(Parent.stateMachines[i].stateMachine, child);
                if (result != null) return result;
            }
            return result;
        }


        public AnimatorStateMachine FindParentSM(AnimatorStateMachine Parent, AnimatorStateMachine child)
        {
            AnimatorStateMachine result = null;

            //Check all State Machine States to see if it correspond to see if it the AnimatorState Parent
            for (int i = 0; i < Parent.stateMachines.Length; i++)
            {
                if (Parent.stateMachines[i].stateMachine == child) return Parent;
            }

            //If the child was not found Search all the StateMachines children then
            for (int i = 0; i < Parent.stateMachines.Length; i++)
            {
                var SM = Parent.stateMachines[i].stateMachine;

                if (SM == child) return Parent;


                result = FindParentSM(SM, child);
                if (result != null) return result;
            }
            return result;
        }


        public AnimatorStateMachine FindAnyState(AnimatorState child)
        {
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var layer = controller.layers[i]; //Store the First State Machine which is the Layer

                var result = FindParentSM(layer.stateMachine, child);
                if (result != null) return layer.stateMachine;
            }
            return null;
        }

        public AnimatorStateMachine FindAnyState(AnimatorStateMachine child)
        {
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var layer = controller.layers[i]; //Store the First State Machine which is the Layer

                var result = FindParentSM(layer.stateMachine, child);
                if (result != null) return layer.stateMachine;
            }
            return null;
        }


        public static AnimatorState Animator_FindState(AnimatorStateMachine rStateMachine, string rName)
        {
            for (int i = 0; i < rStateMachine.states.Length; i++)
            {
                if (rStateMachine.states[i].state.name == rName)
                {
                    return rStateMachine.states[i].state;
                }
            }

            return null;
        }
        public static AnimatorStateTransition Animator_FindTransition(AnimatorState rFrom, AnimatorState rTo, int rIndex)
        {
            int lIndex = -1;

            for (int i = 0; i < rFrom.transitions.Length; i++)
            {
                if (rFrom.transitions[i].destinationState == rTo)
                {
                    lIndex++;
                    if (lIndex == rIndex)
                    {
                        return rFrom.transitions[i];
                    }
                }
            }

            return null;
        }
        public static AnimatorStateTransition Animator_FindAnyStateTransition(AnimatorStateMachine rFrom, AnimatorState rTo, int rIndex)
        {
            int lIndex = -1;

            for (int i = 0; i < rFrom.anyStateTransitions.Length; i++)
            {
                if (rFrom.anyStateTransitions[i].destinationState == rTo)
                {
                    lIndex++;
                    if (lIndex == rIndex)
                    {
                        return rFrom.anyStateTransitions[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a blend tree that we can assign to a node
        /// </summary>
        /// <param name="rName"></param>
        /// <param name="rAnimatorController"></param>
        /// <param name="rAnimatorLayer"></param>
        /// <returns></returns>
        public static BlendTree Animator_CreateBlendTree(string rName, AnimatorController rAnimatorController, int rAnimatorLayer)
        {
            // Create the blend tree. This is so bad, but apparently what we have to do or the blend
            // tree vanishes after we run.
            BlendTree lBlendTree;
            AnimatorState lDeleteState = rAnimatorController.CreateBlendTreeInController("DELETE_ME", out lBlendTree);

            lBlendTree.name = rName;
            lBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
            lBlendTree.blendParameter = "Input X";
            lBlendTree.blendParameterY = "Input Y";

            // Get rid of the dummy state we created to create the blend tree
            lDeleteState.motion = null;
            rAnimatorController.layers[rAnimatorLayer].stateMachine.RemoveState(lDeleteState);

            return lBlendTree;
        }
    } 
}
