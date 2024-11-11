using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{

    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Play Mode")]
    public class PlayModeTask : MTask
    {
        public override string DisplayName => "Animal/Set|Play Mode";

        public enum PlayWhen { PlayOnce, PlayForever, Interrupt }

        [Tooltip("Mode you want to activate when the brain is using this task")]
        public ModeID modeID;
        [Tooltip("Ability ID for the Mode... if is set to -99 it will play a random Ability")]
        public IntReference AbilityID = new IntReference(-99);
        public FloatReference ModePower = new FloatReference();
        [Tooltip("Play the mode only when the animal has arrived to the target")]
        public bool near = false;

        [Space, Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        [Tooltip("Play Once: it will play only at the start of the Task. Play Forever: will play forever using the Cooldown property")]
        public PlayWhen Play = PlayWhen.PlayForever;
        [Tooltip("Time elapsed to Play the Mode again and Again")]
        public FloatReference CoolDown = new FloatReference(2f);
        [Tooltip("Play the Mode if the Animal is Looking at the Target. Avoid playing modes while the target is behind the animal when this value is set to 180")]
        [Range(0f, 360f)]
        public float ModeAngle = 360f;

        [Tooltip("Align with a Look At towards the Target when Playing a mode")]
        public bool lookAtAlign = false;

        [Tooltip("When the mode is said to Play Forever, it will ignore the first cooldown")]
        public bool IgnoreFirstCoolDown = true;
         
        [Tooltip("Align time to rotate towards the Target")]
        public float alignTime = 0.3f;

        public override void StartTask(MAnimalBrain brain, int index)
        {
            if (CoolDown <= 0)
            {
                if (Play == PlayWhen.PlayOnce)
                {
                    if (near && !brain.AIControl.HasArrived) return; //Dont play if Play on target is true but we are not near the target.
                    if (PlayMode(brain))
                    {
                        brain.TasksVars[index].boolValue = true; //Store on the Bool Variable of the Decision that it has already play the mode
                    }
                }
                else if (Play == PlayWhen.Interrupt)
                {
                    switch (affect)
                    {
                        case Affected.Self:
                            brain.Animal.Mode_Interrupt();
                            break;
                        case Affected.Target:
                            if (brain.TargetAnimal != null) brain.TargetAnimal.Mode_Interrupt();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override void UpdateTask(MAnimalBrain brain, int index)
        {
            if (near && !brain.AIControl.HasArrived)
            {
              //  brain.TasksStartTime[index] = Time.time; //Reset the time to the Arrive time
                return; //Dont play if Play on target is true but we are not near the target.
            }


            switch (Play)
            {
                case PlayWhen.PlayOnce:

                    if (!brain.TasksVars[index].boolValue) //Means the Mode has not played
                    {
                        if (MTools.ElapsedTime(brain.TasksStartTime[index], CoolDown) && PlayMode(brain))
                        {
                            brain.TasksVars[index].boolValue = true; //Set that te mode was Played Once!!
                            return; //Skip the code
                        }
                        return;
                    }
                    else
                    {
                        switch (affect)
                        {
                            case Affected.Self:
                                if (!brain.Animal.IsPlayingMode && !brain.Animal.IsPreparingMode) brain.TaskDone(index);
                                break;
                            case Affected.Target:
                                if (brain.TargetAnimal && !brain.TargetAnimal.IsPlayingMode && !brain.TargetAnimal.IsPreparingMode) brain.TaskDone(index);
                                break;
                            default:
                                break;
                        }
                    }

                    break;
                case PlayWhen.PlayForever:

                    if (!brain.TasksVars[index].boolValue && IgnoreFirstCoolDown) //Means the Mode has not played
                    {
                        if (PlayMode(brain))
                        {
                            brain.TasksStartTime[index] = Time.time;    // Reset the cooldown timer
                            brain.TasksVars[index].boolValue = true;    //Set that te mode was Played Once!!
                        }
                    }
                    
                    if (MTools.ElapsedTime(brain.TasksStartTime[index], CoolDown)) //If the animal is in range of the Target
                    {
                        if (PlayMode(brain))
                        {
                            brain.TasksStartTime[index] = Time.time; // Reset the cooldown timer
                        }
                    }
                    break;
                case PlayWhen.Interrupt:
                    if (!brain.TasksVars[index].boolValue) //Means the Mode has not played
                    {
                        if (MTools.ElapsedTime(brain.TasksStartTime[index], CoolDown))
                        {
                            brain.TasksVars[index].boolValue = true; //Set that te mode was Played Once!!


                            switch (affect)
                            {
                                case Affected.Self:
                                    brain.Animal.Mode_Interrupt();
                                    break;
                                case Affected.Target:
                                    if (brain.TargetAnimal != null) brain.TargetAnimal.Mode_Interrupt();
                                    break;
                                default:
                                    break;
                            }
                            brain.TaskDone(index);
                        }
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        private bool PlayMode(MAnimalBrain brain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = brain.Target != null ? (brain.Target.position - brain.Eyes.position) : brain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.Animal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, brain.AIControl.GetTargetPosition(), alignTime));

                            brain.Animal.Mode_SetPower(ModePower);
                            return true;
                        }
                    }
                    break;
                case Affected.Target:
                    Direction_to_Target = brain.Eyes.position - brain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(brain.Target.forward, brain.Animal.UpVector);

                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.TargetAnimal && brain.TargetAnimal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.TargetAnimal.transform, brain.transform, alignTime));

                            brain.TargetAnimal.Mode_SetPower(ModePower);
                            return true;
                        }
                    }
                    break;
                default:
                    break;

            }
            return false;
        }
         



#if UNITY_EDITOR
        public override void DrawGizmos(MAnimalBrain brain)
        {
            var Eyes = brain.Eyes;

            if (Eyes && brain.Animal && ModeAngle < 360)
            {
                Color c = (Color.green + Color.yellow) / 2;
                c.a = 1f;

                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);
                Vector3 rotatedForward = Quaternion.Euler(0, -ModeAngle * 0.5f, 0) * EyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(Eyes.position, Vector3.up, rotatedForward, ModeAngle, 2);
            }
        }
#endif

        void Reset() { Description = "Plays a mode on the Animal(Self or the Target)"; }
    }



    #region Inspector

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(PlayModeTask))]
    public class PlayModeTaksEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty Description, MessageID, CoolDown, modeID, AbilityID, near, affect, ModePower, UpdateInterval,
            Play, ModeAngle, lookAtAlign, WaitForPreviousTask, alignTime, IgnoreFirstCoolDown;

        private void OnEnable()
        {
            Description = serializedObject.FindProperty("Description");
            MessageID = serializedObject.FindProperty("MessageID");
            ModePower = serializedObject.FindProperty("ModePower");
            modeID = serializedObject.FindProperty("modeID");
            AbilityID = serializedObject.FindProperty("AbilityID");
            near = serializedObject.FindProperty("near");
            affect = serializedObject.FindProperty("affect");
            Play = serializedObject.FindProperty("Play");
            CoolDown = serializedObject.FindProperty("CoolDown");
            ModeAngle = serializedObject.FindProperty("ModeAngle");
            lookAtAlign = serializedObject.FindProperty("lookAtAlign");
            alignTime = serializedObject.FindProperty("alignTime");
            IgnoreFirstCoolDown = serializedObject.FindProperty("IgnoreFirstCoolDown");
            WaitForPreviousTask = serializedObject.FindProperty("WaitForPreviousTask");
            UpdateInterval = serializedObject.FindProperty("UpdateInterval");
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(Description);
            UnityEditor.EditorGUILayout.PropertyField(MessageID);
            UnityEditor.EditorGUILayout.PropertyField(WaitForPreviousTask);
            UnityEditor.EditorGUILayout.PropertyField(UpdateInterval);

            UnityEditor.EditorGUILayout.PropertyField(affect);
            UnityEditor.EditorGUILayout.PropertyField(Play, new GUIContent("Action"));
            var playtype = (PlayModeTask.PlayWhen)Play.intValue;
            if (playtype != PlayModeTask.PlayWhen.Interrupt)
            {
                UnityEditor.EditorGUILayout.PropertyField(near);

                var oldColor = GUI.color;
                GUI.color = modeID.objectReferenceValue == null ? (new Color(1, 0.4f, 0.4f, 1)) : oldColor;
                UnityEditor.EditorGUILayout.PropertyField(modeID);
                GUI.color = oldColor;


                UnityEditor.EditorGUILayout.PropertyField(AbilityID);
                UnityEditor.EditorGUILayout.PropertyField(ModePower);


                UnityEditor.EditorGUILayout.PropertyField(CoolDown);
               if (Play.intValue == 1 ) UnityEditor.EditorGUILayout.PropertyField(IgnoreFirstCoolDown);

                UnityEditor.EditorGUILayout.PropertyField(ModeAngle);

                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.PropertyField(lookAtAlign, new GUIContent("Quick Align"));

                UnityEditor.EditorGUILayout.Space();

                if (lookAtAlign.boolValue)
                {
                    UnityEditor.EditorGUIUtility.labelWidth = 33;
                    UnityEditor.EditorGUILayout.PropertyField(alignTime, new GUIContent("Time"));
                    UnityEditor.EditorGUIUtility.labelWidth = 0;
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    #endregion
}
