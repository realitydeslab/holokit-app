using MalbersAnimations.Scriptables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller.AI
{

    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Movement Task", fileName = "New Move Task")]
    public class MoveStopTask : MTask
    {
        public override string DisplayName => "Movement/Movement-Stop";

        private static readonly int circleAround = "circleAround".GetHashCode();


        public enum MoveType
        {
            MoveToCurrentTarget,
            MoveToNextTarget,
            LockAnimalMovement,
            Stop,
            RotateInPlace,
            Flee,
            CircleAround,
            KeepDistance,
            MoveToLastKnownDestination
        };
        public enum CircleDirection { Left, Right };


        [Space, Tooltip("Type of the Movement task")]
        public MoveType task = MoveType.MoveToCurrentTarget;
        /// <summary> Distance for the Flee, Circle Around and keep Distance Task</summary>
        public FloatReference distance = new FloatReference(10f);
        /// <summary> Distance Threshold for the Keep Distance Task</summary>
        public FloatReference distanceThreshold = new FloatReference(1f);
        /// <summary> Custom Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference stoppingDistance = new FloatReference(0.5f);
        /// <summary> Custom slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new FloatReference(0);

        /// <summary> Custom Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public CircleDirection direction = CircleDirection.Left;

        /// <summary> Amount of Target Position around the Target</summary>
        public int arcsCount = 12;

        public bool LookAtTarget = false;
        

        public Color debugColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);

        public override void StartTask(MAnimalBrain brain, int index)
        {
            brain.AIControl.LookAtTargetOnArrival = LookAtTarget;      //IMPORTANT or the animal will try to Move if the Target moves

            switch (task)
            {
                case MoveType.MoveToCurrentTarget:
                    if (brain.AIControl.Target)
                    {
                        brain.AIControl.SetTarget(brain.AIControl.Target, true); //Reset the Target
                        brain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
                    }
                    else
                    {
                        Debug.LogWarning("The Animal does not have a current Target",this);
                    }
                    break;
                case MoveType.MoveToNextTarget:
                    brain.AIControl.MovetoNextTarget();
                    break;
                case MoveType.Stop:
                    brain.AIControl.Stop();
                    brain.AIControl.UpdateDestinationPosition = false;         //IMPORTANT or the animal will try to Move if the Target moves
                    brain.TaskDone(index);
                    break;
                case MoveType.LockAnimalMovement:
                    brain.Animal.LockMovement = true;
                    brain.TaskDone(index);
                    break;
                case MoveType.RotateInPlace:
                    brain.AIControl.RemainingDistance = 0;
                    brain.AIControl.DestinationPosition = brain.AIControl.Transform.position;//Set yourself as the Destination Pos
                    brain.AIControl.LookAtTargetOnArrival = true;          //Set the Animal to look Forward to the Target
                    brain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
                    brain.AIControl.HasArrived = true;      //Set the Stopping Distance to almost nothing that way the animal keeps trying to go towards the target
                    brain.AIControl.Stop();
                    brain.TaskDone(index);
                    break;
                case MoveType.Flee:
                    brain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    Flee(brain, index);
                    break;
                case MoveType.KeepDistance:
                    brain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    KeepDistance(brain, index);
                    break;
                case MoveType.CircleAround:
                    brain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    CalculateClosestCirclePoint(brain, index);
                    break;
                case MoveType.MoveToLastKnownDestination:
                    var LastDestination = brain.AIControl.DestinationPosition; //Store the Last Destination
                    Debug.DrawRay(brain.Position, Vector3.up, Color.white, 1);
                    brain.AIControl.DestinationPosition = Vector3.zero;
                    brain.AIControl.SetDestination(LastDestination, true); //Go to the last Destination position
                    brain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
                    brain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    break;
                default:
                    break;
            }
        }

        public override void UpdateTask(MAnimalBrain brain, int index)
        {
            switch (task)
            {
                case MoveType.Flee: Flee(brain, index); break;
                case MoveType.KeepDistance: KeepDistance(brain, index); break;
                case MoveType.CircleAround: CircleAround(brain, index); break;
                default: break;
            }
        }

        public override void ExitAIState(MAnimalBrain brain, int index)
        {
            switch (task)
            {
                case MoveType.LockAnimalMovement:
                    brain.Animal.LockMovement = false; break;
                default: break;
            }
        }


        public override void OnTargetArrived(MAnimalBrain brain, Transform target, int index)
        {
            switch (task)
            {
                case MoveType.MoveToCurrentTarget:
                    brain.TaskDone(index);
                    break;
                case MoveType.MoveToNextTarget:
                    brain.TaskDone(index);
                    break;
                case MoveType.LockAnimalMovement:
                    brain.TaskDone(index);
                    break;
                case MoveType.Stop: 
                    brain.TaskDone(index);
                    break;
                case MoveType.RotateInPlace:
                    brain.TaskDone(index);
                    break;
                case MoveType.Flee:
                    brain.AIControl.Stop();
                    brain.TaskDone(index);
                    break;
                case MoveType.CircleAround:
                    break;
                case MoveType.KeepDistance:
                    break;
                case MoveType.MoveToLastKnownDestination:
                    brain.AIControl.Stop();
                    brain.TaskDone(index);
                    break;
                default:
                    break;
            }
        }

        private void CalculateClosestCirclePoint(MAnimalBrain brain, int index)
        {
            float arcDegree = 360.0f / arcsCount;
            int Dir = direction == CircleDirection.Right ? 1 : -1;
            Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree, 0);

            Vector3 currentDirection = Vector3.forward;
            Vector3 MinPoint = Vector3.zero;
            float minDist = float.MaxValue;

            int MinIndex = 0;

            for (int i = 0; i < arcsCount; ++i)
            {
                var CurrentPoint = brain.Target.position + (currentDirection.normalized * distance);

                float DistCurrentPoint = Vector3.Distance(CurrentPoint, brain.transform.position);

                if (minDist > DistCurrentPoint)
                {
                    minDist = DistCurrentPoint;
                    MinIndex = i;
                    MinPoint = CurrentPoint;
                }

                currentDirection = rotation * currentDirection;
            }

            brain.AIControl.UpdateDestinationPosition = false;
            brain.AIControl.StoppingDistance = stoppingDistance;

            brain.TasksVars[index].intValue = MinIndex;   //Store the Point index on the vars of this Task
            brain.TasksVars[index].boolValue = true;      //Store true on the Variables, so we can seek for the next point
           // brain.TaskAddBool(index, circleAround, true);          //Store true on the Variables, so we can seek for the next point

            brain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
            brain.AIControl.SetDestination(MinPoint, true);
            brain.AIControl.HasArrived = false;
        }

        private void CircleAround(MAnimalBrain brain, int index)
        {

          //  Debug.Log("circle aound = ");
            if (brain.AIControl.HasArrived) //Means that we have arrived to the point so set the next point
            {
                brain.TasksVars[index].intValue++;
                brain.TasksVars[index].intValue = brain.TasksVars[index].intValue % arcsCount;
                brain.TasksVars[index].boolValue = true;      //Set this so we can seek for the next point
               //brain.TaskSetBool(index, circleAround, true);   //Set this so we can seek for the next point
            }

            if (brain.TasksVars[index].boolValue || brain.AIControl.TargetIsMoving)
           // if (brain.TaskGetBool(index, circleAround) || brain.AIControl.TargetIsMoving)
            {
                int pointIndex = brain.TasksVars[index].intValue;

                float arcDegree = 360.0f / arcsCount;
                int Dir = direction == CircleDirection.Right ? 1 : -1;
                Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree * pointIndex, 0);


               // var distance = this.distance * brain.Animal.ScaleFactor; //Remember to use the scale

                Vector3 currentDirection = Vector3.forward;
                currentDirection = rotation * currentDirection;

                // Debug.Log(brain.Target);

                Vector3 CurrentPoint = brain.Target.position + (currentDirection.normalized * distance);

                Debug.DrawRay(CurrentPoint, Vector3.up, Color.green, UpdateInterval);


                brain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
                brain.AIControl.SetDestination(CurrentPoint, true);

               brain.TasksVars[index].boolValue = false;           //Set this so we can seek for the next point
               //brain.TaskSetBool(index, circleAround, false);      //Set this so we can seek for the next point
            }
        }

        private void KeepDistance(MAnimalBrain brain, int index)
        {
            if (brain.Target)
            {
                brain.AIControl.StoppingDistance = stoppingDistance;
                brain.AIControl.LookAtTargetOnArrival = false;

                Vector3 KeepDistPoint = brain.Animal.transform.position;

                var DirFromTarget = KeepDistPoint - brain.Target.position;

                float halThreshold = distanceThreshold * 0.5f;
                float TargetDist = DirFromTarget.magnitude;

                var distance = this.distance * brain.Animal.ScaleFactor; //Remember to use the scale

                if ((TargetDist) < distance - distanceThreshold) //Flee 
                {
                    float DistanceDiff = distance - TargetDist;
                    KeepDistPoint = CalculateDistance(brain, index, DirFromTarget, DistanceDiff, halThreshold);
                   // brain.TaskDone(index,false);


                }
                else if (TargetDist > distance + distanceThreshold) //Go to Target
                {
                    float DistanceDiff = TargetDist - distance;
                    KeepDistPoint = CalculateDistance(brain, index, -DirFromTarget, DistanceDiff, -halThreshold);
                    //brain.TaskDone(index, false);
                }
                else
                {
                    brain.AIControl.HasArrived = true;
                    brain.AIControl.LookAtTargetOnArrival = true;
                    brain.AIControl.StoppingDistance = distance + distanceThreshold; //Force to have a greater Stopping Distance so the animal can rotate around the target
                    brain.AIControl.RemainingDistance = 0; //Force the remaining distance to be 0
                  //  brain.TaskDone(index,false);
                }

                if (brain.debug)
                    Debug.DrawRay(KeepDistPoint, brain.transform.up, Color.cyan, UpdateInterval);
            }
        }

        private Vector3 CalculateDistance(MAnimalBrain brain, int index, Vector3 DirFromTarget, float DistanceDiff, float halThreshold)
        {
            Vector3 KeepDistPoint = brain.transform.position + DirFromTarget.normalized * (DistanceDiff + halThreshold);
            brain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
            brain.AIControl.StoppingDistance = stoppingDistance;
            brain.AIControl.SetDestination(KeepDistPoint, true);
            return KeepDistPoint;
        }


        private void Flee(MAnimalBrain brain, int index)
        {
            if (brain.Target)
            {
                brain.AIControl.UpdateDestinationPosition = false;                      //Means the Animal Wont Update the Destination Position with the Target position.
                brain.AIControl.LookAtTargetOnArrival = false;

                var CurrentPos = brain.Animal.transform.position;

                var AgentDistance = Vector3.Distance(brain.Animal.transform.position, brain.Position);
                var TargetDirection = CurrentPos - brain.Target.position;

                float TargetDistance = TargetDirection.magnitude;

                var distance = this.distance * brain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDistance < distance)
                {
                    //player is too close from us, pick a point diametrically oppossite at twice that distance and try to move there.
                    Vector3 fleePoint = brain.Target.position + (TargetDirection.normalized * (distance + AgentDistance * 2f));

                    brain.AIControl.StoppingDistance = stoppingDistance;

                    Debug.DrawRay(fleePoint, Vector3.up * 3, Color.blue, 2f);

                    if (Vector3.Distance(CurrentPos, fleePoint) > stoppingDistance) //If the New flee Point is not in the Stopping distance radius then set a new Flee Point
                    {
                        brain.AIControl.UpdateDestinationPosition = false; //Means the Animal wont Update the Destination Position with the Target position.
                        brain.AIControl.SetDestination(fleePoint, true);

                        if (brain.debug)
                            Debug.DrawRay(fleePoint, brain.transform.up, Color.blue, 2f);
                    }

                    brain.TaskDone(index, false);
                }
                else
                {
                    brain.AIControl.StoppingDistance = distance * 100;  //Force a big Stopping distance to ensure the animal can look at the Target
                    brain.AIControl.DestinationPosition = (brain.Target.position);
                    brain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    brain.TaskDone(index);
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawGizmos(MAnimalBrain brain)
        {
            Handles.color = Gizmos.color = debugColor;

            var scale = brain.Animal ? brain.Animal.ScaleFactor : brain.transform.root.localScale.y;
          

            switch (task)
            {
                case MoveType.Flee:
                    Gizmos.DrawWireSphere(brain.transform.position, distance*scale);
                    break;
                case MoveType.CircleAround:

                    var origin = brain.Target != null ? brain.Target : brain.transform;
                    float arcDegree = 360.0f / arcsCount;
                    Quaternion rotation = Quaternion.Euler(0, -arcDegree, 0);

                    Vector3 currentDirection = Vector3.forward * scale;

                    Handles.DrawWireDisc(origin.position, Vector3.up, distance * scale);

                    for (int i = 0; i < arcsCount; ++i)
                    {
                        var CurrentPoint = origin.position + (currentDirection.normalized * distance * scale);
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(CurrentPoint, 0.2f*scale);
                        currentDirection = rotation * currentDirection;
                    }
                    break;


                case MoveType.KeepDistance:
                    Handles.DrawWireDisc(brain.transform.position, Vector3.up, (distance - distanceThreshold) * scale);
                    Handles.DrawWireDisc(brain.transform.position, Vector3.up, (distance + distanceThreshold) * scale);
                    break;


                case MoveType.MoveToCurrentTarget: break;
                case MoveType.MoveToNextTarget: break;
                case MoveType.LockAnimalMovement: break;
                case MoveType.Stop: break;
                case MoveType.RotateInPlace: break;
                case MoveType.MoveToLastKnownDestination: break;

                default: break;
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MoveStopTask)), CanEditMultipleObjects]
    public class MoveTaskEditor : Editor
    {
        SerializedProperty
            Description, distance, debugColor, distanceThreshold, active, WaitForPreviousTask,
            stoppingDistance, task, Direction, UpdateInterval, slowingDistance,
            MessageID, arcsCount, LookAtTarget;
            MonoScript script;
        private void OnEnable()
        {
            script = MonoScript.FromScriptableObject((ScriptableObject)target);

            active = serializedObject.FindProperty("active");
            Description = serializedObject.FindProperty("Description");
            WaitForPreviousTask = serializedObject.FindProperty("WaitForPreviousTask");
            task = serializedObject.FindProperty("task");
            arcsCount = serializedObject.FindProperty("arcsCount");
            distance = serializedObject.FindProperty("distance");
            Direction = serializedObject.FindProperty("direction");
            distanceThreshold = serializedObject.FindProperty("distanceThreshold");
            UpdateInterval = serializedObject.FindProperty("UpdateInterval");
            MessageID = serializedObject.FindProperty("MessageID");
            debugColor = serializedObject.FindProperty("debugColor");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            LookAtTarget = serializedObject.FindProperty("LookAtTarget");
            slowingDistance = serializedObject.FindProperty("slowingDistance");
          //  Interact = serializedObject.FindProperty("Interact");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //MalbersEditor.DrawDescription("Movement Task for the AI Brain");

            EditorGUI.BeginChangeCheck();
          // MalbersEditor.DrawScript(script);

            EditorGUILayout.PropertyField(active);
            EditorGUILayout.PropertyField(Description);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(MessageID);
            EditorGUILayout.PropertyField(debugColor, GUIContent.none, GUILayout.MaxWidth(40));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(WaitForPreviousTask);
            EditorGUILayout.PropertyField(UpdateInterval);

            MoveStopTask.MoveType taskk = (MoveStopTask.MoveType)task.intValue;

            string Help = GetTaskType(taskk);
            EditorGUILayout.PropertyField(task, new GUIContent("Task", Help));

            switch (taskk)
            {
                case MoveStopTask.MoveType.LockAnimalMovement: LookAt_T(); break;
                case MoveStopTask.MoveType.Stop: LookAt_T(); break;
                case MoveStopTask.MoveType.RotateInPlace: break;
                case MoveStopTask.MoveType.MoveToCurrentTarget:
                    LookAt_T();   Interact_();   break;
                case MoveStopTask.MoveType.MoveToNextTarget:
                    LookAt_T();   Interact_();   break;

                case MoveStopTask.MoveType.Flee:
                    EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Flee Safe Distance away from the Target"));
                    EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stop Distance", "Stopping Distance of the Flee point"));
                    EditorGUILayout.PropertyField(slowingDistance);
                    LookAt_T();
                    break;
                case MoveStopTask.MoveType.CircleAround:
                    EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Flee Safe Distance away from the Target"));
                    EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stop Distance", "Stopping Distance of the Circle Around Points"));
                    EditorGUILayout.PropertyField(slowingDistance);
                    EditorGUILayout.PropertyField(Direction, new GUIContent("Direction", "Direction to Circle around the Target... left or right"));
                    EditorGUILayout.PropertyField(arcsCount, new GUIContent("Arc Count", "Amount of Point to Form a Circle around the Target"));
                    
                    break;
                case MoveStopTask.MoveType.KeepDistance:
                    EditorGUILayout.PropertyField(distance);
                    EditorGUILayout.PropertyField(distanceThreshold);
                    EditorGUILayout.PropertyField(stoppingDistance);
                    EditorGUILayout.PropertyField(slowingDistance);
                    LookAt_T();
                    break;
                default:
                    break;
            }


            EditorGUILayout.Space();
            MalbersEditor.DrawDescription(taskk.ToString() + ":\n" + Help);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Movement Task Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Interact_()
        {
          //  EditorGUILayout.PropertyField(Interact, new GUIContent("Interact", "If we Arrived to the Target and is Interactable, Interact!"));
        }

        private void LookAt_T()
        {
            EditorGUILayout.PropertyField(LookAtTarget, new GUIContent("Look at Target", "If we Arrived to the Target then Keep Looking At it"));
        }

        private string GetTaskType(MoveStopTask.MoveType taskk)
        {
            switch (taskk)
            {
                case MoveStopTask.MoveType.MoveToCurrentTarget:
                    return "The Animal will move towards current assigned Target.";
                case MoveStopTask.MoveType.MoveToNextTarget:
                    return "The Animal will move towards the Next Target, if it has a Current Target that is a Waypoint and has a Next Target.";
                case MoveStopTask.MoveType.LockAnimalMovement:
                    return "The Animal will Stop moving. [Animal.LockMovement] will be |True| at Start; and it will be |False| at the end of the Task.";
                case MoveStopTask.MoveType.Stop:
                    return "The Animal will Stop the Agent from moving. Calling AIAnimalControl.Stop(). \nIt will keep the Current Target Assigned.";
                case MoveStopTask.MoveType.RotateInPlace:
                    return "The Animal will not move but it will rotate on the Spot towards the current Target Direction.";
                case MoveStopTask.MoveType.Flee:
                    return "The Animal will move away from the current target until it reaches a the safe distance.";
                case MoveStopTask.MoveType.CircleAround:
                    return "The Animal will Circle around the current Target from a safe distance.";
                case MoveStopTask.MoveType.KeepDistance:
                    return "The Animal will Keep a safe distance from the target\nIf the distance is too close it will flee\nIf the distance is too far it will come near the Target.";
                case MoveStopTask.MoveType.MoveToLastKnownDestination:
                    return "The Animal will Move to the Last Know destination of the Previous Target";
                default:
                    return string.Empty;
            }
        }
    }
#endif
}