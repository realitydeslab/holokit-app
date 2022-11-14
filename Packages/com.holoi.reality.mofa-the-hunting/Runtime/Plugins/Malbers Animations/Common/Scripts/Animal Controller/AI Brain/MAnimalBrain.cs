using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Events;
using UnityEngine.AI;
using MalbersAnimations.Scriptables;
using UnityEngine.Serialization;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller.AI
{
    [AddComponentMenu("Malbers/Animal Controller/AI/Animal Brain")]
    public class MAnimalBrain : MonoBehaviour, IAnimatorListener
    {
        /// <summary>Reference for the Ai Control Movement</summary>
        public IAIControl AIControl;
        [Obsolete("Use AIControl Instead")]
        public IAIControl AIMovement => AIControl;

        //[Tooltip("Use a temporal Brain from another animal (MOUNTING)")]
        //public MAnimalBrain TemporalBrain;
        //public MAnimalBrain Brain => TemporalBrain != null ? TemporalBrain : this;

        /// <summary>Transform used to raycast Rays to interact with the world</summary>
        [RequiredField, Tooltip("Transform used to raycast Rays to interact with the world")]
        public Transform Eyes;
        /// <summary>Time needed to make a new transition. Necesary to avoid Changing to multiple States in the same frame</summary>
        [Tooltip("Time needed to make a new transition. Necessary to avoid Changing to multiple States in the same frame")]
        public FloatReference TransitionCoolDown = new FloatReference(0.2f);

        /// <summary>Reference AI State for the animal</summary>
        [CreateScriptableAsset] public MAIState currentState;

        [Tooltip("Removes all AI Components when the Animal Dies. (Brain, AiControl, Agent)")]
        [FormerlySerializedAs("RemoveAIOnDeath")]
        public bool DisableAIOnDeath = true;
        public bool debug = false;
        public bool debugAIStates = false;


        public IntEvent OnTaskStarted = new IntEvent();
        public IntEvent OnTaskDone = new IntEvent();
        public IntEvent OnDecisionSucceeded = new IntEvent();
        public IntEvent OnAIStateChanged = new IntEvent();


        /// <summary>Last Time the Animal make a new transition</summary>
        private float TransitionLastTime;

        /// <summary>Last Time the Animal  started a transition</summary>
        public float StateLastTime { get; set; }


        /// <summary>Check if all the Task are done..</summary>
        public bool AllTasksDone()
        {
            foreach (var done in TasksDone)
            {
                if (!done) return false;
            }
            return true;
        }

        /// <summary>Check if an Specific Task is Done..</summary>
        public void TaskDone(MTask task)
        {
            if (currentState != null)
            {
                for (int i = 0; i < currentState.tasks.Length; i++)
                {
                    if (currentState.tasks[i] == task)
                    {
                        TaskDone(i);
                        break;
                    }
                }
            }
        }


        /// <summary>Check if an Specific Task is Done..</summary>
        public bool IsTasksDone(int index)
        {
            return TasksDone[index % TasksDone.Length];
        }

        /// <summary>Tasks Local Vars (1 Int,1 Bool,1 Float)</summary>
        public BrainVars[] TasksVars;
        /// <summary>Saves on the a Task that it has finish is stuff</summary>
        internal bool[] TasksDone;
        /// <summary>Current Decision Results</summary>
        internal bool[] DecisionResult;
        /// <summary>Store if a Task has Started</summary>
        internal bool[] TasksStarted;
        /// <summary>Decision Local Vars to store values on Prepare Decision</summary>
        public BrainVars[] DecisionsVars;
        internal bool BrainInitialize;

        


        #region Properties


        /// <summary>Reference for the Animal</summary>
        public MAnimal Animal { get; private set; }

        /// <summary>Reference for the AnimalStats</summary>
        public Dictionary<int, Stat> AnimalStats { get; set; }

        #region Target References
        /// <summary>Reference for the Current Target the Animal is using</summary>
        public Transform Target { get; set; }
        //{ 
        //    get => target; 
        //    set 
        //    {
        //    target = value;
        //    }
        //}
        //private Transform target;

        /// <summary>Reference for the Target the Animal Component</summary>
        public MAnimal TargetAnimal { get; set; }

        public Vector3 Position => AIControl.Transform.position;

        public float AIHeight => Animal.transform.lossyScale.y * AIControl.StoppingDistance;

        /// <summary>True if the Current Target has Stats</summary>
        public bool TargetHasStats { get; private set; }

        /// <summary>Reference for the Target the Stats Component</summary>
        public Dictionary<int, Stat> TargetStats { get; set; }
        #endregion

        /// <summary>Reference for the Last WayPoint the Animal used</summary>
        public IWayPoint LastWayPoint { get; set; }

        /// <summary>Time Elapsed for the Tasks on an AI State</summary>
       public float[] TasksStartTime  { get; set; }
       public float[] TasksUpdateTime  { get; set; }

        /// <summary>Time Elapsed for the State Decisions</summary>
        [HideInInspector] public float[] DecisionsTime;// { get; set; }

        #endregion

        #region Unity Callbakcs
        void Awake()
        {
            if (Animal == null) Animal = gameObject.FindComponent<MAnimal>();
            if (AIControl == null) AIControl = gameObject.FindInterface<IAIControl>();

            var AnimalStatscomponent = Animal.FindComponent<Stats>();
            if (AnimalStatscomponent) AnimalStats = AnimalStatscomponent.stats_D;

            Animal.isPlayer.Value = false; //If is using a brain... disable that he is the main player
           // ResetVarsOnNewState();
        }


        public void OnEnable()
        {
            //AIMovement.OnTargetArrived.AddListener(OnTargetArrived);
            //AIMovement.OnTargetPositionArrived.AddListener(OnPositionArrived);
            AIControl.TargetSet.AddListener(OnTargetSet);
            AIControl.OnArrived.AddListener(OnTargetArrived);

            Animal.OnStateChange.AddListener(OnAnimalStateChange);
            Animal.OnStanceChange.AddListener(OnAnimalStanceChange);
            Animal.OnModeStart.AddListener(OnAnimalModeStart);
            Animal.OnModeEnd.AddListener(OnAnimalModeEnd);



            Invoke(nameof(StartBrain), 0.1f); //Start AI a Frame later; 
        }

        public void OnDisable()
        {
            //AIMovement.OnTargetArrived.RemoveListener(OnTargetArrived);
            //AIMovement.OnTargetPositionArrived.RemoveListener(OnPositionArrived);
            AIControl.TargetSet.RemoveListener(OnTargetSet);
            AIControl.OnArrived.RemoveListener(OnTargetArrived);


            Animal.OnStateChange.RemoveListener(OnAnimalStateChange);
            Animal.OnStanceChange.RemoveListener(OnAnimalStanceChange);
            Animal.OnModeStart.RemoveListener(OnAnimalModeStart);
            Animal.OnModeEnd.RemoveListener(OnAnimalModeEnd);

            //  AIControl.Stop();
            StopAllCoroutines();

            if (currentState)
            {
                for (int i = 0; i < currentState.tasks.Length; i++)         //Exit the Current Tasks
                    currentState.tasks[i]?.ExitAIState(this, i);
            }
            BrainInitialize = false;
        }

        void Update()
        {
            if (BrainInitialize && currentState != null) currentState.Update_State(this);
        }

        #endregion


        public void StartBrain()
        {
            AIControl.SetActive(true);

            if (currentState)
            {
                for (int i = 0; i < currentState.tasks.Length; i++)
                {
                    if (currentState.tasks[i] == null)
                    {
                        Debug.LogError($"The [{currentState.name}] AI State has an Empty Task. AI States can't have empty Tasks. {Animal.name}", currentState);
                       // enabled = false;
                        return;
                    };

                }

                StartNewState(currentState);
            }
            else
            {
                enabled = false;
                return;
            }

            AIControl.AutoNextTarget = false;


            LastWayPoint = null;

            if (AIControl.Target)
                SetLastWayPoint(AIControl.Target);

            BrainInitialize = true;
        }

      

        public virtual void TransitionToState(MAIState nextState, bool decisionValue, MAIDecision decision, int Index)
        {
            if (MTools.ElapsedTime(TransitionLastTime, TransitionCoolDown)) //This avoid making transition on the same Frame ****IMPORTANT
            {
                if (nextState != null && nextState != currentState) //Do not transition to itself!
                {
                    TransitionLastTime = Time.time;

                    decision.FinishDecision(this, Index);


                    Debuging($"<color=white>Changed AI State from <B>[{currentState.name}]</B> to" +
                        $" <B>[{nextState.name}]</B>. Decision: <b>[{decision.name}]</b> = <B>[{decisionValue}]</B>.</color>", currentState);

                    InvokeDecisionEvent(decisionValue, decision);

                    StartNewState(nextState);
                }
            }
        }

        protected virtual void Debuging(string Log, UnityEngine.Object val) { if (debug) Debug.Log($"<B>[{Animal.name}] - </B> " + Log,val); }

        private void InvokeDecisionEvent(bool decisionValue, MAIDecision decision)
        {
            if (decision.send == MAIDecision.WSend.SendTrue && decisionValue)
            {
                OnDecisionSucceeded.Invoke(decision.DecisionID);
            }
            else if (decision.send == MAIDecision.WSend.SendFalse && !decisionValue)
            {
                OnDecisionSucceeded.Invoke(decision.DecisionID);
            }
        }

        public virtual void StartNewState(MAIState newState)
        {
            if (!enabled) enabled = true; //Make sure the Brain is enabled!!!! IMPORTANT

            StateLastTime = Time.time;      //Store the last time the Animal made a transition

            if (currentState != null && currentState != newState)
            {
                currentState.Finish_Tasks(this);                 //Finish all the Task on the Current State
             // currentState.Finish_Decisions(this);             //Finish all the Decisions on the Current State
            }

            currentState = newState;                            //Set a new State

            ResetVarsOnNewState();

            OnAIStateChanged.Invoke(currentState.ID);
            currentState.Start_AIState(this);                      //Start all Tasks on the new State
            currentState.Prepare_Decisions(this);               //Start all Tasks on the new State


            Debuging($"<color=white> Set AI State <B>[{currentState.name}]</B> </color>", currentState);

        }


        /// <summary>Prepare all the local variables on the New State before starting new tasks</summary>
        private void ResetVarsOnNewState()
        {
            if (currentState)
            {
                var tasks = (currentState.transitions != null && currentState.tasks.Length > 0) ? currentState.tasks.Length : 1;
                var transitions = (currentState.transitions != null && currentState.transitions.Length > 0) ? currentState.transitions.Length : 1;

                TasksVars = new BrainVars[tasks];                //Local Variables you can use on your tasks
                TasksUpdateTime = new float[tasks];              //Reset all the Tasks    Time elapsed time
                TasksStartTime = new float[tasks];               //Reset all the Tasks    Time elapsed time

                TasksDone = new bool[tasks];                     //Reset if they are Done
                TasksStarted = new bool[tasks];                  //Reset if they tasks are started

                DecisionsVars = new BrainVars[transitions];      //Local Variables you can use on your Decisions
                DecisionsTime = new float[transitions];          //Reset all the Decisions Time elapsed time
                DecisionResult = new bool[transitions];          //Reset if they tasks are started
            }
        }


        public bool IsTaskDone(int TaskIndex) => TasksDone[TaskIndex];

        public void TaskDone(int TaskIndex, bool value = true) //If the first task is done then go and do the next one
        {
            TasksDone[TaskIndex] = value;
            OnTaskDone.Invoke(currentState[TaskIndex].MessageID.Value); //Invoke when a task is done!!!


            if (TaskIndex + 1 < currentState.tasks.Length && currentState.tasks[TaskIndex + 1].WaitForPreviousTask) //Start the next task that needs to wait for the previus one
            {
             // Debug.Log($"*Task DONE!!!!: [{name}] [{TaskIndex}]-[{currentState.tasks[TaskIndex].name }]");
                currentState.StartWaitforPreviusTask(this, TaskIndex + 1);
            }
        }

        /// <summary> Check if the time elapsed of a task using a duration or CountDown time </summary>
        /// <param name="duration">Duration of the countDown|CoolDown</param>
        /// <param name="index">Index of the Task on the AI State Tasks list</param>
        public bool CheckIfDecisionsCountDownElapsed(float duration, int index)
        {
            DecisionsTime[index] += Time.deltaTime;
            return DecisionsTime[index] >= duration;
        }

        /// <summary>Set the time on which a task has started on the current AI State</summary>
        public void SetTaskStartTime(int Index)
        {
            TasksStartTime[Index] = Time.time;
        } 
        

        /// <summary>Reset the Time elapsed on a Decision using its index from the Transition List </summary>
        /// <param name="Index">Index of the Decision on the AI State Transition List</param>
        public void ResetDecisionTime(int Index) => DecisionsTime[Index] = 0;

        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

      

        #region SelfAnimal Event Listeners
        void OnAnimalStateChange(int state)
        {
            currentState?.OnAnimalStateEnter(this, Animal.ActiveState);
            currentState?.OnAnimalStateExit(this, Animal.LastState);


            if (state == StateEnum.Death) //meaning this animal has died
            {
                for (int i = 0; i < currentState.tasks.Length; i++)         //Exit the Current Tasks
                    currentState.tasks[i].ExitAIState(this, i);

                StopAllCoroutines();
                BrainInitialize = false;
                enabled = false;

                if (DisableAIOnDeath)
                {
                    AIControl.SetActive(false);
                }
            }
        }

        void OnAnimalStanceChange(int stance) => currentState.OnAnimalStanceChange(this, Animal.Stance.ID);

        void OnAnimalModeStart(int mode, int ability) => currentState.OnAnimalModeStart(this, Animal.ActiveMode);

        void OnAnimalModeEnd(int mode, int ability) => currentState.OnAnimalModeEnd(this, Animal.ActiveMode);


        #endregion

        #region TargetAnimal Event Listeners
        //void OnTargetAnimalStateChange(int state)
        //{
        //    currentState.OnTargetAnimalStateEnter(this, Animal.ActiveState);
        //    currentState.OnTargetAnimalStateExit(this, Animal.LastState);
        //}

        private void OnTargetArrived(Transform target) => currentState.OnTargetArrived(this, target);

        //private void OnPositionArrived(Vector3 position) => currentState.OnPositionArrived(this, position);
        #endregion


        /// <summary>Stores if the Current Target is an Animal and if it has the Stats component </summary>
        private void OnTargetSet(Transform target)
        {
            Target = target;

            if (target)
            {
                TargetAnimal = target.FindComponent<MAnimal>();// ?? target.GetComponentInChildren<MAnimal>();

                TargetStats = null;
                var TargetStatsC = target.FindComponent<Stats>();// ?? target.GetComponentInChildren<Stats>();

                TargetHasStats = TargetStatsC != null;
                if (TargetHasStats) TargetStats = TargetStatsC.stats_D;
            }
        }

        public bool CheckForPreviusTaskDone(int index)
        {
            if (index == 0) return true;

            if (!TasksStarted[index] && IsTaskDone(index - 1))
                return true;

            return false;
        }

        public void SetLastWayPoint(Transform target)
        {
            var newLastWay = target.gameObject.FindInterface<IWayPoint>();
            if (newLastWay != null)   LastWayPoint = target?.gameObject.FindInterface<IWayPoint>(); //If not is a waypoint save the last one
        }



        [SerializeField] private int Editor_Tabs1;
#if UNITY_EDITOR
        void Reset()
        {
         //   remainInState = MTools.GetInstance<MAIState>("Remain in State");
            AIControl = this.FindComponent<MAnimalAIControl>();

            if (AIControl != null)
            {
                AIControl.AutoNextTarget = false;
                AIControl.UpdateDestinationPosition = false;
                AIControl.LookAtTargetOnArrival = false;

                if (Animal) Animal.isPlayer.Value = false; //Make sure this animal is not the Main Player

            }
            else
            {
                Debug.LogWarning("There's No AI Control in this GameObject, Please add one");
            }
        }

        void OnDrawGizmos()
        {
            if (isActiveAndEnabled && currentState && Eyes)
            {
                Gizmos.color = currentState.GizmoStateColor;
                Gizmos.DrawWireSphere(Eyes.position, 0.2f);

                if (debug)
                {
                    if (currentState != null)
                    {
                        if (currentState.tasks != null)
                            foreach (var task in currentState.tasks)
                                task?.DrawGizmos(this);
                       

                        if (currentState.transitions != null)
                            foreach (var tran in currentState.transitions)
                                tran?.decision?.DrawGizmos(this);
                    }
                }

                if (Application.isPlaying && debugAIStates)
                {
                    string desicions = "";

                    var Styl =  new GUIStyle(EditorStyles.boldLabel); 
                    Styl.normal.textColor = Color.yellow;

                    UnityEditor.Handles.Label(Eyes.position, "State: " + currentState.name + desicions, Styl);
                }
            }
        }
#endif 
    }

    public enum Affected { Self, Target }
    public enum ExecuteTask { OnStart, OnUpdate, OnExit }

    [System.Serializable]
    public struct BrainVars
    {
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public Vector3 vector3; 
        public Component[] Components;
        public MonoBehaviour mono;
        public GameObject[] gameobjects;

        public Dictionary<int,int> ints;
        public Dictionary<int, float> floats;
        public Dictionary<int, bool> bools;
       // public Dictionary<int, Component> D_components;

        public void SetVar(int key, bool value) => bools[key] = value;
        public void SetVar(int key, int value) => ints[key] = value;
        public void SetVar(int key, float value) => floats[key] = value;
        public bool GetBool(int key) => bools[key];
        public int GetInt(int key) => ints[key];
        public float GetFloat(int key) => floats[key];


        public bool TryGetBool(int key, out bool value) => bools.TryGetValue(key, out value);
        public bool TryGetInt(int key, out int value) =>  ints.TryGetValue(key, out value);
        public bool TryGetFloat(int key, out float value) =>  floats.TryGetValue(key, out value);
       
        public void AddVar(int key, bool value)
        {
            if (bools == null) bools = new Dictionary<int, bool>();
            bools.Add(key, value);
        }

        public void AddVar(int key, int value)
        {
            if (bools == null) bools = new Dictionary<int, bool>();
            ints.Add(key, value);
        }

        public void AddVar(int key, float value)
        {
            if (bools == null) bools = new Dictionary<int, bool>();
            floats.Add(key, value);
        }

        public void AddComponents(Component[] components)
        {
            if (Components == null || Components.Length == 0) Components = components;
            else
            {
                Components = Components.Concat(components).ToArray();
            }
        }


        public void AddComponent(Component comp)
        {
            if (Components == null || Components.Length == 0) Components = new Component[1] { comp };
            else
            {
                var ComponentsL = Components.ToList();
                ComponentsL.Add(comp);
                Components = ComponentsL.ToArray();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MAnimalBrain)), CanEditMultipleObjects]
    public class MAnimalBrainEditor : Editor
    {
        SerializedProperty  Eyes, debug, TransitionCoolDown, DisableAIOnDeath, Editor_Tabs1, debugAIStates, OnTaskDone,
            currentState, OnTaskStarted, OnDecisionSucceded, OnAIStateChanged;

        protected string[] Tabs1 = new string[] { "AI States" , "Events" ,"Debug"};

        MAnimalBrain M;

        private void OnEnable()
        {
            M = (MAnimalBrain)target;

            Eyes = serializedObject.FindProperty("Eyes");
            TransitionCoolDown = serializedObject.FindProperty("TransitionCoolDown");
            DisableAIOnDeath = serializedObject.FindProperty("DisableAIOnDeath");
            currentState = serializedObject.FindProperty("currentState");

            OnTaskStarted = serializedObject.FindProperty("OnTaskStarted");
            OnTaskDone = serializedObject.FindProperty("OnTaskDone");
            OnDecisionSucceded = serializedObject.FindProperty("OnDecisionSucceeded");
            OnAIStateChanged = serializedObject.FindProperty("OnAIStateChanged");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            debug = serializedObject.FindProperty("debug");
          //  AISource = serializedObject.FindProperty("AISource");
            debugAIStates = serializedObject.FindProperty("debugAIStates");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Brain Logic for the Animal");
           // EditorGUILayout.BeginVertical(MTools.StyleGray);
            {

                Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);


                if (Editor_Tabs1.intValue == 0) DrawGeneral();
                else if (Editor_Tabs1.intValue == 1) DrawEvents();
                else DrawDebug();


                if (Eyes.objectReferenceValue == null) EditorGUILayout.HelpBox("The AI Eyes [Reference] is missing. Please add a transform the AI Eyes parameters", MessageType.Error);

            }
         //   EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebug()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(debugAIStates, new GUIContent("Debug On Screen"));

                if (Application.isPlaying)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    Repaint();


                    EditorGUILayout.ObjectField("Brain Target", M.Target, typeof(Transform), false);

                    if (M.enabled && M.BrainInitialize && M.currentState != null)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.ObjectField("AI State", M.currentState, typeof(MAIState), false);


                            EditorGUILayout.LabelField("Tasks", EditorStyles.boldLabel);

                            for (int i = 0; i < M.currentState.tasks.Length; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.ObjectField(GUIContent.none, M.currentState.tasks[i], typeof(MTask), false, GUILayout.MinWidth(100));
                                EditorGUILayout.LabelField($"  Started: {(M.TasksStarted[i] ? "☑" : "[  ]")}. Done: {(M.TasksDone[i] ? "☑" : "[  ]")}", GUILayout.MinWidth(100));
                                EditorGUILayout.LabelField($"Start Time: {M.TasksStartTime[i]:F2}", GUILayout.MinWidth(50));
                                EditorGUILayout.EndHorizontal();
                            }


                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            {
                                EditorGUILayout.LabelField("Task Variables", EditorStyles.boldLabel);
                                for (int i = 0; i < M.currentState.tasks.Length; i++)
                                {
                                    var TasksVars = serializedObject.FindProperty("TasksVars");
                                     
                                    if (TasksVars != null && TasksVars.arraySize > i)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.PropertyField(TasksVars.GetArrayElementAtIndex(i),
                                            new GUIContent(M.currentState.tasks[i].name), true);
                                        EditorGUI.indentLevel--;
                                    }
                                }
                            }
                            EditorGUILayout.EndVertical();

                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("Decision Variables", EditorStyles.boldLabel);
                        for (int i = 0; i < M.currentState.transitions.Length; i++)
                        {
                            var DecisionsVars = serializedObject.FindProperty("DecisionsVars");
                            var Des = M.currentState.transitions[i].decision;

                            var waiting = "";

                            if (Des.WaitForAllTasks && !M.AllTasksDone()) waiting = "[WAIT T*]";
                            if (Des.waitForTask != -1 && !M.IsTaskDone(Des.waitForTask)) waiting = "[WAIT T]";


                            EditorGUILayout.ObjectField($"Decision [{i }] {waiting}", Des, typeof(MAIDecision), false, GUILayout.MinWidth(100));
                            if (DecisionsVars != null && DecisionsVars.arraySize > i)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(DecisionsVars.GetArrayElementAtIndex(i),
                                    new GUIContent(M.currentState.transitions[i].decision.name), true);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGeneral()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(Eyes);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(currentState);
            MalbersEditor.DrawDebugIcon(debug);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
            // EditorGUILayout.PropertyField(remainInState);
            EditorGUILayout.PropertyField(TransitionCoolDown);
            EditorGUILayout.PropertyField(DisableAIOnDeath); 
            EditorGUILayout.EndVertical();
        }

        private void DrawEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(OnAIStateChanged);
            EditorGUILayout.PropertyField(OnTaskStarted);
            EditorGUILayout.PropertyField(OnTaskDone);
            EditorGUILayout.PropertyField(OnDecisionSucceded);
            EditorGUILayout.EndVertical();
        }
    }
#endif
}