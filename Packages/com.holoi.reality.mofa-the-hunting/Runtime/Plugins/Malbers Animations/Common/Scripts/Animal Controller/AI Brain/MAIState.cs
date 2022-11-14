using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using MalbersAnimations.Scriptables;

using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/AI State", order = -100, fileName = "New AI State")]
    public class MAIState : ScriptableObject
    {
        [Tooltip("ID of the AI State. This is used on the AI Brain On AIStateChanged Event")]
        public IntReference ID = new IntReference();
        
        public MTask this[int index]
        {
            get => tasks[index];
            set => tasks[index] = value;
        }

        //[Tooltip("Creates the Decisions and Tasks inside this AI State")]
        //public bool internalData = true;

        [FormerlySerializedAs("actions")] public MTask[] tasks;
        public MAITransition[] transitions;
        public Color GizmoStateColor = Color.gray;

        [HideInInspector] public bool CreateTaskAsset = true;
        [HideInInspector] public bool CreateDecisionAsset = true;


        public virtual void Play(MAnimalBrain brain) => brain?.StartNewState(this);


        internal void Update_State(MAnimalBrain brain)
        {
            Update_Tasks(brain);
            Update_Transitions(brain);
        }

        private void Update_Transitions(MAnimalBrain brain)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (this != brain.currentState) return; //BUG BUG BUG FIXed

                var transition = transitions[i];
                var decision = transition.decision;
                if (decision == null) return; //Ignore Code

                //If the Decision is waiting for a task to be done and the tasks is NOT done, then ignore this decision
                if (decision.waitForTask != -1 && !brain.IsTasksDone(decision.waitForTask)) continue;

                //If the Decision is waiting for ALL tasks to be done tasks are NOT done then ignore this decision
                if (decision.WaitForAllTasks && !brain.AllTasksDone()) continue;

                if (decision.interval > 0)
                {
                    if (brain.CheckIfDecisionsCountDownElapsed(decision.interval, i))
                    {
                        brain.ResetDecisionTime(i);
                        Decide(brain, i, transition);
                    }
                }
                else
                {
                    Decide(brain, i, transition);
                }
            }
        }

        private void Decide(MAnimalBrain brain, int Index, MAITransition transition)
        {
            if (!transition.decision.active) return; //Skip if the Decision is not active

            bool decisionSucceded = brain.DecisionResult[Index] = transition.decision.Decide(brain, Index);

            // Debug.Log($"{brain.Animal.name} decisionSucceded "+ decisionSucceded);

            brain.TransitionToState(decisionSucceded ? transition.trueState : transition.falseState, decisionSucceded,
                transition.decision, Index);
        }

        /// <summary>When a new State starts this method is called for each Tasks</summary>
        internal void Start_AIState(MAnimalBrain brain)
        {
            for (int i = 0; i < tasks.Length; i++) StartTaks(brain, i);
        }

        internal void StartTaks(MAnimalBrain brain, int i)
        {
            if (tasks[i] == null)
            {
                Debug.LogError($"The  {name} AI State has an Empty Task. Please check all your AI States Tasks. {brain.Animal.name} brain is Disabled", this);
                brain.enabled = false;
                return;
            };

            if (tasks[i].active)
            {
                if (brain.TasksStarted[i]) return; //DO NOT START AN ALREADY STARTED TASK 

                if (i == 0 || !tasks[i].WaitForPreviousTask)
                {
                    // Debug.Log($"<B>{brain.Animal.name}:</B> Start Task: [{name}] [{i}]-[{tasks[i].name }]");
                    //Prepare the Task
                    brain.TasksStarted[i] = true;
                    brain.SetTaskStartTime(i);
                    tasks[i].StartTask(brain, i); //Start the Task after it has being prepared.
                    if (tasks[i].MessageID != 0)
                        brain.OnTaskStarted.Invoke(tasks[i].MessageID); //Send Events after the Task has started
                }
            };
        }

        internal void StartWaitforPreviusTask(MAnimalBrain brain, int i)
        {
            if (tasks[i] == null)
            {
                Debug.LogError($"The  {name} AI State has an Empty Task. Please check all your AI States Tasks. {brain.Animal.name} brain is Disabled", this);
                brain.enabled = false;
                return;
            };

            if (brain.TasksStarted[i]) return; //DO NOT START AN ALREADY STARTED TASK 

            if (tasks[i].WaitForPreviousTask)
            {
                //Prepare the Task
                brain.TasksStarted[i] = true;
                brain.SetTaskStartTime(i);

                tasks[i].StartTask(brain, i); //Start the Task after it has being prepared.

                if (tasks[i].MessageID != 0)
                    brain.OnTaskStarted.Invoke(tasks[i].MessageID); //Send Events after the Task has started
            }
        }

        /// <summary>When a new State starts this method is called for each Decisions</summary>
        internal void Prepare_Decisions(MAnimalBrain brain)
        {
            if (transitions != null)
            for (int i = 0; i < transitions.Length; i++)
            {
                transitions[i].decision.PrepareDecision(brain, i);
            }
        }

        internal void Update_Tasks(MAnimalBrain brain)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                if (brain.TasksStarted[i] && !brain.TasksDone[i] && tasks[i].active)
                {
                   tasks[i].InternalUpdateTask(brain, i);
                }
            }
        }

        internal void Finish_Tasks(MAnimalBrain brain)
        {
            for (int i = 0; i < tasks.Length; i++)
                if (tasks[i].active) tasks[i].ExitAIState(brain, i);
        }



        #region Target Event Listeners for Tasks 
        /// <summary> Notify the Tasks that the Animal has arrived to the Current assigned target</summary>
        internal void OnTargetArrived(MAnimalBrain brain, Transform target)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i] != null) tasks[i].OnTargetArrived(brain, target, i);
            }
        }

        #endregion

        #region Self Animal Listen Events

        internal void OnAnimalStateEnter(MAnimalBrain brain, State state)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i]?.OnAnimalStateEnter(brain, state, i);
        }

        internal void OnAnimalStateExit(MAnimalBrain brain, State state)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i]?.OnAnimalStateExit(brain, state, i);
        }

        internal void OnAnimalStanceChange(MAnimalBrain brain, int Stance)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i]?.OnAnimalStanceChange(brain, Stance, i);
        }


        internal void OnAnimalModeStart(MAnimalBrain brain, Mode mode)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i]?.OnAnimalModeStart(brain, mode, i);
        }

        internal void OnAnimalModeEnd(MAnimalBrain brain, Mode mode)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i]?.OnAnimalModeEnd(brain, mode, i);
        }

        #endregion

        #region Target Animal Listen Events

        public void OnTargetAnimalStateEnter(MAnimalBrain brain, State state)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].OnTargetAnimalStateEnter(brain, state, i);
        }

        public void OnTargetAnimalStateExit(MAnimalBrain brain, State state)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].OnTargetAnimalStateExit(brain, state, i);
        }

        public void OnTargetAnimalStanceChange(MAnimalBrain brain, int Stance)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].OnTargetAnimalStanceChange(brain, Stance, i);
        }


        public void OnTargetAnimalModeStart(MAnimalBrain brain, Mode mode)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].OnTargetAnimalModeStart(brain, mode, i);
        }

        public void OnTargetAnimalModeEnd(MAnimalBrain brain, Mode mode)
        {
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].OnTargetAnimalModeEnd(brain, mode, i);
        }

        #endregion 
    }

    [System.Serializable]
    public class MAITransition
    {
        public MAIDecision decision;
        public MAIState trueState;
        public MAIState falseState;
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(MAIState))]
    public class MAIStateEditor : Editor
    {
        MAIState m;
        private SerializedProperty tasks, transitions, GizmoStateColor, ID;
        private ReorderableList Reo_List_Tasks, Reo_List_Transitions;


        private List<Type> TasksType;
        private List<Type> DecisionType;

        private GUIContent plus;

        private void OnEnable()
        {
            if (plus == null) plus = EditorGUIUtility.IconContent("d_Toolbar Plus");


            m = (MAIState)target;
            tasks = serializedObject.FindProperty("tasks");
            transitions = serializedObject.FindProperty("transitions");
            GizmoStateColor = serializedObject.FindProperty("GizmoStateColor");
            ID = serializedObject.FindProperty("ID");
            GizmoStateColor = serializedObject.FindProperty("GizmoStateColor");

            TasksType = MTools.GetAllTypes<MTask>();
            DecisionType = MTools.GetAllTypes<MAIDecision>();

            TasksList();

            TransitionList();
        }


        private void OnAddCallback_Task(ReorderableList list)
        {
            var addMenu = new GenericMenu();

            ResizeTasks();

            for (int i = 0; i < TasksType.Count; i++)
            {
                Type st = TasksType[i];

                //Fast Ugly get the name of the Asset thing
                MTask t = (MTask)CreateInstance(st);
                var name = t.DisplayName;
                DestroyImmediate(t);

                //var Rname = Regex.Replace(st.Name, @"([a-z])([A-Z])", "$1 $2");
                addMenu.AddItem(new GUIContent(name), false, () => AddTask(st, m.tasks.Length - 1));
            }

            addMenu.AddSeparator("");
            addMenu.AddItem(new GUIContent("Empty"), false, () => { });

            addMenu.ShowAsContext();
        }


        private void AddTask(Type NewTask, int index)
        {
            MTask task = (MTask)CreateInstance(NewTask);
            task.hideFlags = HideFlags.None;
            task.name = "T_" + NewTask.Name;
            AssetDatabase.AddObjectToAsset(task, AssetDatabase.GetAssetPath(target));

            m.tasks[index] = task; //the other way was not working
            Reo_List_Tasks.index = index;
            Reo_List_Transitions.index = -1;

            EditorUtility.SetDirty(task);
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }


        private void TasksList()
        {
            Reo_List_Tasks = new ReorderableList(serializedObject, tasks, true, true, true, true)
            {
                onSelectCallback = (index) =>
                {
                    Reo_List_Transitions.index = -1; //Do not select the Tasks when selecting the transitions
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = tasks.GetArrayElementAtIndex(index);
                     

                    var indexR = 36;

                    var r = new Rect(rect) { x = rect.x + indexR, y = rect.y + 2, height = EditorGUIUtility.singleLineHeight, width = rect.width - indexR };
                    

                    var indRect = new Rect(rect) { y = rect.y + 2, height = EditorGUIUtility.singleLineHeight, width = indexR };

                    bool isWaiting = element.objectReferenceValue != null && (element.objectReferenceValue as MTask).WaitForPreviousTask;

                    if (isWaiting) r.width -= 20;



                    EditorGUI.LabelField(indRect, $"[{index}]");

                    string Path = AssetDatabase.GetAssetPath(element.objectReferenceValue);

                    var dC = GUI.contentColor;
                    if (element.objectReferenceValue != null & Path != AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                    {
                        GUI.contentColor = new Color(0.7f,0.8f,1);
                    }

                    var activeRect = new Rect(rect);
                    activeRect.width -= 20;
                    activeRect.x += 20;

                    if (element.objectReferenceValue != null)
                    {
                        MTask task = element.objectReferenceValue as MTask;

                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            task.active = EditorGUI.Toggle(activeRect, GUIContent.none, task.active);
                            if (cc.changed)
                            {
                                Reo_List_Transitions.index = -1;
                                element.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.Toggle(activeRect, GUIContent.none, false);
                    }


                    EditorGUI.PropertyField(r, element, GUIContent.none);
                    GUI.contentColor = dC; 
                    
                    var st = new GUIStyle(EditorStyles.whiteLabel)
                    {
                        fontSize = 18
                    };
                    st.normal.textColor = Color.green;

                    if (isWaiting)
                    {
                        var waitinR = new Rect(r)
                        {
                            width = 20,
                            height = r.height + 2,
                            y = r.y - 2,
                            x = rect.width + 25
                        };
                        EditorGUI.LabelField(waitinR, new GUIContent("⤴", "This task is waiting for the previous task to finish "), st);
                    } 
                },

                drawHeaderCallback = rect =>
                {
                    var r = new Rect(rect);
                    var ColorRect = new Rect(rect)
                    {
                        x = rect.width - 40,
                        width = 60,
                        height = rect.height - 2,
                        y = rect.y + 1,
                    };

                    EditorGUI.LabelField(rect, new GUIContent("   Tasks", "Tasks for the state"));
                    EditorGUI.PropertyField(ColorRect, GizmoStateColor, GUIContent.none);
                },

                onAddCallback = list => OnAddCallback_Task(list),

                onRemoveCallback = list =>
                {
                    var task = tasks.GetArrayElementAtIndex(list.index);

                    if (task != null && task.objectReferenceValue != null)
                    {
                        if (EditorUtility.DisplayDialog("Remove Task", "Deleting a Task cannot be undone. Are you sure you want to delete it?", "Yes", "No"))
                        {
                            string Path = AssetDatabase.GetAssetPath(task.objectReferenceValue);

                            if (Path == AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                            {
                                DestroyImmediate(task.objectReferenceValue, true); //Delete the internal asset!
                                task.objectReferenceValue = null;
                                tasks.DeleteArrayElementAtIndex(list.index); //Double Hack
                                AssetDatabase.SaveAssets();
                            }
                            else // is an Outside Element
                            {
                                tasks.DeleteArrayElementAtIndex(list.index);
                            }
                        }
                    }
                    else
                    {
                        tasks.DeleteArrayElementAtIndex(list.index);
                    }

                    tasks.serializedObject.ApplyModifiedProperties();

                    //Reset the selection on Remove
                    Reo_List_Transitions.index = -1;
                    Reo_List_Tasks.index = -1;


                    EditorUtility.SetDirty(target);
                    GUIUtility.ExitGUI();
                }
            };
        }

       

        private void TransitionList()
        {
            Reo_List_Transitions = new ReorderableList(serializedObject, transitions, true, true, true, true)
            {
                onSelectCallback = (index) =>
               {
                   Reo_List_Tasks.index = -1; //Do not select the Tasks when selecting the transitions
               },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var r1 = new Rect(rect) { y = rect.y + 2, height = EditorGUIUtility.singleLineHeight };
                    var r2 = new Rect(r1);
                    var r3 = new Rect(r2);
                    bool empty = false;

                    var element = transitions.GetArrayElementAtIndex(index);

                    var decision = element.FindPropertyRelative("decision");
                    var TrueState = element.FindPropertyRelative("trueState");
                    var FalseState = element.FindPropertyRelative("falseState");



                    string Path = AssetDatabase.GetAssetPath(decision.objectReferenceValue);

                    var dC = GUI.contentColor;
                   
                    if (decision.objectReferenceValue != null && Path != AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                    {
                        GUI.contentColor = Color.yellow+Color.gray;
                    }

                    var activeRect = new Rect(r1);
                    activeRect.width = 25;
                    activeRect.x += 20;

                    if (decision.objectReferenceValue != null)
                    {
                        MAIDecision des = decision.objectReferenceValue as MAIDecision;

                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            des.active = EditorGUI.Toggle(activeRect, GUIContent.none, des.active);
                            if (cc.changed)
                            {
                                Reo_List_Tasks.index = -1;
                                element.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.Toggle(activeRect, GUIContent.none, false);
                    }

                     
                    EditorGUI.PropertyField(r1, decision,
                        new GUIContent("[" + index + "]       Decision",
                            "If the Decision is true it will go to the True State, else it will go to the False state"));
                    GUI.contentColor = dC;

                    empty = TrueState.objectReferenceValue == null;

                    if (empty) r2.width -= 28;

                    r2.y += EditorGUIUtility.singleLineHeight + 3;
                    EditorGUI.PropertyField(r2, TrueState,
                        new GUIContent("True" + (empty ? " (Do Nothing)" : ""),
                            "If the Decision is TRUE, It will execute this state. if is Empty, it will do nothing"));


                    if (empty)
                    {
                        var AddButtonRect = new Rect(r2)
                        { x = rect.width + 18, width = 20, height = EditorGUIUtility.singleLineHeight };

                        if (GUI.Button(AddButtonRect, plus, EditorStyles.helpBox))
                            MTools.CreateScriptableAsset(TrueState, typeof(MAIState),
                                MTools.GetSelectedPathOrFallback());
                    }


                    empty = FalseState.objectReferenceValue == null;
                    if (empty) r3.width -= 28;
                    r3.y += (EditorGUIUtility.singleLineHeight + 3) * 2;


                    EditorGUI.PropertyField(r3, FalseState,
                        new GUIContent("False" + (empty ? " (Do Nothing)" : ""),
                            "If the Decision is FALSE, It will execute this state. if is Empty, it will do nothing"));

                    if (empty)
                    {
                        var AddButtonRect = new Rect(r3)
                        { x = rect.width + 18, width = 20, height = EditorGUIUtility.singleLineHeight };

                        if (GUI.Button(AddButtonRect, plus, EditorStyles.helpBox))
                            MTools.CreateScriptableAsset(FalseState, typeof(MAIState),
                                MTools.GetSelectedPathOrFallback());
                    }
                },

                drawHeaderCallback = rect =>
                    EditorGUI.LabelField(rect, new GUIContent("   Decisions", "Transitions for other States")),

                onAddCallback = list => OnAddCallback_Decision(),

                onRemoveCallback = list =>
                {
                    var decision = m.transitions[list.index].decision;

                    if (decision != null)
                    {
                        if (EditorUtility.DisplayDialog("Remove Decision", "Deleting a Decision cannot be undone. Are you sure you want to delete it?", "Yes", "No"))
                        {
                            string Path = AssetDatabase.GetAssetPath(decision);

                            if (Path == AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                            {
                                DestroyImmediate(decision, true); //Delete the internal asset!
                                m.transitions[list.index].decision = null;
                                transitions.DeleteArrayElementAtIndex(list.index);
                                transitions.serializedObject.ApplyModifiedProperties();
                                AssetDatabase.SaveAssets();
                            }
                            else
                            {
                                transitions.DeleteArrayElementAtIndex(list.index);
                                transitions.serializedObject.ApplyModifiedProperties();
                                list.index = -1;
                            }
                        }
                    }
                    else
                    {
                        transitions.DeleteArrayElementAtIndex(list.index);
                        transitions.serializedObject.ApplyModifiedProperties();
                    } 
                
                    //Reset the selection on Remove
                    Reo_List_Transitions.index = -1;
                    Reo_List_Tasks.index = -1;

                    EditorUtility.SetDirty(target);
                    GUIUtility.ExitGUI();
                },
                elementHeightCallback = (index) =>
                {
                    var DefaultHeight = EditorGUIUtility.singleLineHeight + 5;
                    return DefaultHeight * 3;
                },
            };
        }

        private void OnAddCallback_Decision()
        {
            var addMenu = new GenericMenu();
            ResizeTransitions();

            for (int i = 0; i < DecisionType.Count; i++)
            {
                Type st = DecisionType[i];


                //Fast Ugly get the name of the Asset thing
                MAIDecision t = (MAIDecision)CreateInstance(st);
                var Rname = t.DisplayName;
                DestroyImmediate(t);
                //var Rname = Regex.Replace(st.Name, @"([a-z])([A-Z])", "$1 $2");

                addMenu.AddItem(new GUIContent(Rname), false, () => AddDecision(st, m.transitions.Length - 1));
            }


            addMenu.AddSeparator("");
            addMenu.AddItem(new GUIContent("Empty"), false, () => { });
            addMenu.ShowAsContext();
        }

        private void AddDecision(Type desicion, int index)
        {
            MAIDecision des = (MAIDecision)CreateInstance(desicion);
            des.hideFlags = HideFlags.None;
            des.name = "D_" + desicion.Name;
            AssetDatabase.AddObjectToAsset(des, AssetDatabase.GetAssetPath(target));
            AssetDatabase.SaveAssets();


            m.transitions[index].decision = des; //the other way was not working

            EditorUtility.SetDirty(des);
            EditorUtility.SetDirty(target);

            transitions.serializedObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();

            Reo_List_Transitions.index = index;
            Reo_List_Tasks.index = -1;
        }

        private void ResizeTasks()
        {
            if (m.tasks == null)
            {
                m.tasks = new MTask[1];
            }
            else
            {
                Array.Resize<MTask>(ref m.tasks, m.tasks.Length + 1); //Hack for non UNITY  OBJECTS ARRAYS
            }

            EditorUtility.SetDirty(target);
        }

        private void ResizeTransitions()
        {
            if (m.transitions == null)
            {
                m.transitions = new MAITransition[1];
            }
            else
            {
                Array.Resize<MAITransition>(ref m.transitions,
                    m.transitions.Length + 1); //Hack for non UNITY  OBJECTS ARRAYS
            }

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(target.name, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ID);
            //EditorGUILayout.PropertyField(internalData);
            EditorGUILayout.Space();

            if (m.tasks != null)

                foreach (var t in m.tasks)
                {
                    if (t == null)
                    {
                        EditorGUILayout.HelpBox("The Brain cannot contain empty Tasks. Set the missing tasks", MessageType.Error);
                        break;
                    }
                }

            Reo_List_Tasks.DoLayoutList();

          //  if (Reo_List_Tasks.index != -1) Reo_List_Transitions.index = -1;


            if (m.transitions != null)
                foreach (var t in m.transitions)
                {
                    if (t.decision == null)
                    {
                        EditorGUILayout.HelpBox("The Brain cannot contain empty Decisions. Set the missing decisions", MessageType.Error);
                        break;
                    }
                }

            Reo_List_Transitions.DoLayoutList();

            //if (Reo_List_Transitions.index != -1) Reo_List_Tasks.index = -1;


            var indexTasks = Reo_List_Tasks.index;
            var indexTrans = Reo_List_Transitions.index;

            if (indexTasks != -1)
            {
                var element = tasks.GetArrayElementAtIndex(indexTasks);

                if (element != null && element.objectReferenceValue != null)
                {
                    var asset = element.objectReferenceValue;

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Task: " + asset.name, EditorStyles.boldLabel);

                        string Path = AssetDatabase.GetAssetPath(element.objectReferenceValue);
                        if (Path != AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                        {
                            EditorGUILayout.HelpBox("This Task is shared with other AI States", MessageType.Info);
                        }



                        using (new GUILayout.HorizontalScope())
                        {
                            asset.name = EditorGUILayout.TextField("Name", asset.name);
                            element.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(asset);

                            if (GUILayout.Button(new GUIContent("R", "Update the Asset name"), GUILayout.Width(20)))
                            {
                                string taskPath = AssetDatabase.GetAssetPath(asset);
                                string targetPath = AssetDatabase.GetAssetPath(target);

                                // Check if the asset itself is external or internal to the target
                                if (taskPath != targetPath)
                                    AssetDatabase.RenameAsset(taskPath, asset.name);

                                AssetDatabase.SaveAssets();
                                EditorGUIUtility.PingObject(asset); //Final way of changing the name of the asset... dirty but it works
                            }


                            if (GUILayout.Button(new GUIContent("E", "Extract the task into its own file"),
                                GUILayout.Width(20)))
                            {
                                ExtractTaskFromList(asset, element, indexTasks);
                                GUIUtility.ExitGUI();
                            }
                        }
                        MTools.DrawObjectReferenceInspector(element);
                    }
                }
            }

            if (indexTrans != -1)
            {
                var element = transitions.GetArrayElementAtIndex(indexTrans);
                var decision = element.FindPropertyRelative("decision");

                if (decision != null && decision.objectReferenceValue != null)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Decision: " + decision.objectReferenceValue.name, EditorStyles.boldLabel);


                        string Path = AssetDatabase.GetAssetPath(decision.objectReferenceValue);
                        if (Path != AssetDatabase.GetAssetPath(target)) //mean it was created inside the AI STATE
                        {
                            EditorGUILayout.HelpBox("This Decision is shared with other AI States", MessageType.Info);
                        }


                        var asset = decision.objectReferenceValue;

                        using (new GUILayout.HorizontalScope())
                        {
                            asset.name = EditorGUILayout.TextField("Name", asset.name);
                            decision.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(asset);

                            if (GUILayout.Button(new GUIContent("R", "Update the Asset name"), GUILayout.Width(20)))
                            {
                                string taskPath = AssetDatabase.GetAssetPath(asset);
                                string targetPath = AssetDatabase.GetAssetPath(target);

                                // Check if the asset itself is external or internal to the target
                                if (taskPath != targetPath)
                                    AssetDatabase.RenameAsset(taskPath, asset.name);

                                AssetDatabase.SaveAssets();
                                EditorGUIUtility.PingObject(asset); //Final way of changing the name of the asset... dirty but it works
                            }

                            if (GUILayout.Button(new GUIContent("E", "Extract the decision into its own file"),
                                GUILayout.Width(20)))
                            {
                                ExtractDecisionFromList(asset, element, indexTrans);
                            }
                        }

                        MTools.DrawObjectReferenceInspector(decision);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ExtractTaskFromList(Object asset, SerializedProperty element, int index)
        {
            string taskPath = AssetDatabase.GetAssetPath(asset);
            string targetPath = AssetDatabase.GetAssetPath(target);
            if (taskPath == targetPath)
            {
                Object clone = MTools.ExtractObject(asset, index);
                if (!clone) return;
                 
           
                DestroyImmediate(asset, true); //Remove all version
 
                element.objectReferenceValue = clone; //Use the new extracted one
                Reo_List_Tasks.index = index;
                Reo_List_Transitions.index = -1;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(clone);
            }
            else
            {
                // Checking this beforehand is quite in-efficient as the AssetDatabase.GetAssetPath() is slow
                // If there is a better way to check whether it's an internal or external asset then that could be used
                Debug.LogWarning("Cannot extract already extracted task");
            }
        }

        private void ExtractDecisionFromList(Object asset, SerializedProperty element, int index)
        {
            string taskPath = AssetDatabase.GetAssetPath(asset);
            string targetPath = AssetDatabase.GetAssetPath(target);

            if (taskPath == targetPath)
            {
                Object clone = MTools.ExtractObject(asset, index);
                if (!clone) return;

                // Remove from list
                DestroyImmediate(asset, true);

                // Add as external decision
                SerializedProperty decision = element.FindPropertyRelative("decision");
                decision.objectReferenceValue = clone;
              
                Reo_List_Tasks.index = -1;
                Reo_List_Transitions.index = index;
                
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(clone);
            }
            else
            {
                // Checking this beforehand is quite in-efficient as the AssetDatabase.GetAssetPath() is slow
                // If there is a better way to check whether it's an internal or external asset then that could be used
                Debug.LogWarning("Cannot extract already extracted decision");
            }
        }

    }
#endif
}