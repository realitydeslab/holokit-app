using UnityEngine;
using System.Collections;
using MalbersAnimations.Events;
 
using UnityEngine.AI;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MalbersAnimations.Controller.AI
{
    [AddComponentMenu("Malbers/Animal Controller/AI/AI Control")]
    public class MAnimalAIControl : MonoBehaviour, IAIControl, IAITarget, IAnimatorListener
    {
        #region Components and References
        /// <summary> Reference for the Agent</summary>
        [SerializeField] private NavMeshAgent agent;

        /// <summary> Reference for the Animal</summary>
        [RequiredField] public MAnimal animal;

        /// <summary>Cache if the Animal has an Input Source</summary>
        public IInputSource InputSource { get; internal set; }

        /// <summary>Cache if the Animal has an Interactor</summary>
        public IInteractor Interactor { get; internal set; }

        public bool ArriveLookAt => false; //do this later

        public virtual bool Active => enabled && gameObject.activeInHierarchy;

        #endregion

        #region Internal Variables
        /// <summary>Target Last Position (Useful to know if the Target is moving)</summary>
        protected Vector3 TargetLastPosition;

        /// <summary>Remaining Distance to the Destination Point</summary>
         public virtual float RemainingDistance  { get; set; }
        //{
        //    get => m_RemainingDistance;
        //    set
        //    {
        //        m_RemainingDistance = value;
        //        if (debug) Debug.Log($"Remaining Distance = {m_RemainingDistance:F3}");
        //    }
        //}
        //float m_RemainingDistance;


        /// <summary> Returns the Current Agent Remaining Distance </summary>
        public virtual float AgentRemainingDistance => Agent.remainingDistance;

        /// <summary>Store the Current Remaining Distance. This is used to slowdown the Animal when is circling around and it cannot arrive to the destination</summary>
        public virtual float MinRemainingDistance { get; set; }


        //  public float CircleAroundMultiplier { get; private set; }

        /// <summary>Used to Slow Down the Animal when its close the Destination</summary>
        public float SlowMultiplier
        {
            get
            {
                var result = 1f;
                if (CurrentSlowingDistance > CurrentStoppingDistance && RemainingDistance < CurrentSlowingDistance)
                    result = Mathf.Max(RemainingDistance / CurrentSlowingDistance, slowingLimit);

                return result;
            }
        }



        public Transform Transform => transform;

        [Tooltip("When the animal is on any of these States, The AI agent will be disable to improve performance.")]
        [ContextMenuItem("Set Default", "SetDefaulStopAgent")]
        public List<StateID> StopAgentOn;

        /// <summary>Stores the Agent Direction used to move the Animal</summary>
        public Vector3 AIDirection  { get; set; }
        //{
        //    get => m_AIDirection;
        //    set
        //    {
        //        m_AIDirection = value;
        //        Debug.Log($"<B>AI DIR: {m_AIDirection}</b>");
        //    }
        //}
        //Vector3 m_AIDirection;

        /// <summary>Is the Agent in a OffMesh Link</summary>       
        public bool InOffMeshLink  { get; set; }
        //{
        //    get => m_InOffMeshLink;
        //    set
        //    {
        //        m_InOffMeshLink = value;
        //        Debug.Log($"<B>AI OFFML: {m_InOffMeshLink}</b>");
        //    }
        //}
        //bool m_InOffMeshLink;

        public virtual bool AgentInOffMeshLink => Agent.isOnOffMeshLink;

        /// <summary>Store if the Animal is on a Blocking Agent State</summary>       
        public bool StateIsBlockingAgent { get; set; }

        /// <summary>Is the Agent Enabled/Active ?</summary>       
        public virtual bool ActiveAgent
        {
            get => agent.enabled && agent.isOnNavMesh;
            set
            {
                agent.enabled = value;
                if (agent.isOnNavMesh)   agent.isStopped = !value;
               // Debug.Log($"<B>{(agent.enabled? "[•]": "[  ]" )}</B> Agent Enable");
            }
        }

        /// <summary>Checks if the Animal Can Fly</summary>
        public virtual bool CanFly { get; private set; }

        /// <summary>Has the animal arrived to the destination</summary>
        public bool HasArrived { get; set; }
        //{
        //    get => m_hasarrived;
        //    set
        //    {
        //        m_hasarrived = value;
        //       Debug.Log($"<B>{(m_hasarrived ? "[•]" : "[  ]")}</B> Has Arrived");
        //    }
        //}
        //private bool m_hasarrived;

        /// <summary>Updates the Destination Position if the Target Moves</summary>
        public virtual bool UpdateDestinationPosition { get; set; }
        //{
        //    get => updateTargetPosition;
        //    set
        //    {
        //        updateTargetPosition = value;
        //        Debug.Log($"<B>{(updateTargetPosition ? "[•]" : "[  ]")}</B> UpdateTargetPosition");
        //    }
        //}
        //private bool updateTargetPosition;

        /// <summary>Destination Position to use on Agent.SetDestination()</summary>
        public virtual Vector3 DestinationPosition { get; set; }
        //{
        //    get => m_DestinationPosition;
        //    set
        //    {
        //        m_DestinationPosition = value;
        //        if (debug) Debug.Log($"Dest Pos: [{m_DestinationPosition:F3}]  Is AI [{IsAITarget != null}]  Targ:[{Target}]");
        //    }
        //}
        //Vector3 m_DestinationPosition;


        private IEnumerator I_WaitToNextTarget;
        private IEnumerator IFreeMoveOffMesh;
        private IEnumerator IClimbOffMesh;
        #endregion

        #region Public Variables
        [Min(0)] public float UpdateAI = 0.2f;
        private float CurrentTime;

        [Min(0)] [SerializeField] protected float stoppingDistance = 0.6f;
        [Min(0)] [SerializeField] protected float PointStoppingDistance = 0.6f;

        /// <summary>The animal will change automatically to Walk if the distance to the target is this value</summary>
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("walkDistance")]
        [Min(0)] protected float slowingDistance = 1f;

        [Min(0)] public float OffMeshAlignment = 0.15f;


        //[Tooltip("If the difference between the current direction and the desired direction is greater than this value; the animal will stop to turn around.")]
        //[Range(0, 180)]
        //public float TurnAngle = 90f;

        [Tooltip("Distance from the Animals Root to apply LookAt Target Logic when the Animal arrives to a target.")]
        [Min(0)] public float LookAtOffset = 1;

        [Tooltip("Limit for the Slowing Multiplier to be applied to the Speed Modifier")]
        [Range(0, 1)]
        [SerializeField] private float slowingLimit = 0.3f;

        [SerializeField] private Transform target;
        [SerializeField] private Transform nextTarget;

        /// <summary>When the AI Arrives to a Waypoint Target, it will set the Next Target from the AIWaypoint</summary>
        public bool AutoNextTarget { get; set; }

        /// <summary>The Animal will Rotate/Look at the Target when he arrives to it</summary>
        public bool LookAtTargetOnArrival { get; set; }

        public bool debug = false;
        public bool debugGizmos = true;
        public bool debugStatus = true;
        #endregion

        #region Properties 
        /// <summary>is the Animal, Flying, swimming, On Free Mode?</summary>
        public bool FreeMove { get; private set; }


        /// <summary>Default Stopping Distance</summary>
        public virtual float StoppingDistance { get => stoppingDistance; set => stoppingDistance = value; }

        protected float currentStoppingDistance;

        /// <summary>Current Stoping distance of the Current Target/Destination</summary>
        public virtual float CurrentStoppingDistance
        {
            get => currentStoppingDistance;
            set => Agent.stoppingDistance = currentStoppingDistance = value;
        }
       
        /// <summary>Default Slowing Distance</summary>
        public virtual float SlowingDistance => slowingDistance;

        public virtual float Height => Agent.height * animal.ScaleFactor;

        /// <summary>Current Slowing Distance from the Current AI Target</summary>
        public virtual float CurrentSlowingDistance { get; set; }

        /// <summary>Is the Animal Playing a mode</summary>
        public bool IsOnMode => animal.IsPlayingMode;

        /// <summary>  Stop all Modes that does not allow Movement  </summary>
        private bool IsOnNonMovingMode => (IsOnMode && !animal.ActiveMode.AllowMovement);

      

        /// <summary>Is the Target a WayPoint?</summary>
        public IWayPoint IsWayPoint { get; set; }

        /// <summary>Is the Target an AITarget</summary>
        public IAITarget IsAITarget { get; set; }

        /// <summary>AITarget Position</summary>
        public Vector3 AITargetPos => IsAITarget.GetPosition();      //Update the AI Target Pos if the Target moved

        /// <summary>Is the Target an AITarget</summary>
        public IInteractable IsTargetInteractable { get; protected set; }
        #endregion 

        #region Events
        [Space]
        public Vector3Event OnTargetPositionArrived = new Vector3Event();
        public TransformEvent OnTargetArrived = new TransformEvent();
        public TransformEvent OnTargetSet = new TransformEvent();

        public TransformEvent TargetSet => OnTargetSet;
        public TransformEvent OnArrived => OnTargetArrived;

        #endregion

        /// <summary>The Target is an Air Target</summary>
        internal bool IsAirDestination => IsAITarget != null && IsAITarget.TargetType == WayPointType.Air;
        internal bool IsGroundDestination => IsAITarget != null && IsAITarget.TargetType == WayPointType.Ground;

        public UnityEvent OnEnabled = new UnityEvent();
        public UnityEvent OnDisabled = new UnityEvent();


        #region Properties 
        /// <summary>Reference of the Nav Mesh Agent</summary>
        public virtual NavMeshAgent Agent => agent;

        public Transform AgentTransform;

        public virtual Vector3 GetPosition() => AgentTransform.position;

        public Vector3 GetCenter() => animal.Center;

        /// <summary> Self Target Type </summary>
        public virtual WayPointType TargetType => animal.FreeMovement ? WayPointType.Air : WayPointType.Ground;


        /// <summary>is the Target transform moving??</summary>
        public virtual bool TargetIsMoving { get; internal set; }


        /// <summary> Is the Animal waiting x time to go to the Next waypoint</summary>
        public virtual bool IsWaiting { get; internal set; }

        public virtual Vector3 LastOffMeshDestination{ get; set; }

      

        public Vector3 NullVector { get; set; }

        public virtual Transform NextTarget { get => nextTarget; set => nextTarget = value; }

        public virtual Transform Target { get => target; set => target = value; }

        /// <summary>Stores the Local Agent Position relative to the Animal</summary>
        protected Vector3 AgentPosition;

        #endregion
        public virtual void SetActive(bool value)
        {
           // Debug.Log("value = " + value);
            if (gameObject.activeInHierarchy)
                enabled = value;
        }

       


        #region Unity Functions 
        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);


        protected virtual void Awake()
        {
            if (animal == null) animal = gameObject.FindComponent<MAnimal>();
            ValidateAgent();

            Interactor = animal.FindInterface<IInteractor>();       //Check if there's any Interactor
            InputSource = animal.FindInterface<IInputSource>();     //Check if there's any Input Source
            animal.UseSmoothVertical = true;                        //This needs to be disable so the slow distance works!!!!!!

            LookAtTargetOnArrival = true;                           //By Default Look Target on Arrival set it to True
            AutoNextTarget = true;                                  //By Default Auto Next Target is set to True
            UpdateDestinationPosition = true;

            NullVector = new Vector3(-998.9999f, -998.9999f, -998.9999f);

            DestinationPosition = NullVector;

            CanFly = animal.HasState(StateEnum.Fly);                //Check if the Animal can Fly

            SetAgent();
        }

        /// <summary>  Set the Default properties for the Nav mesh Agent </summary>
        protected virtual void SetAgent()
        {
            if (agent == null) AgentTransform.GetComponent<NavMeshAgent>(); //Re-Check if the Agent is not properly assigned

            if (agent)
            {
                AgentPosition = Agent.transform.localPosition;
                Agent.angularSpeed = 0;
                Agent.speed = 1;                                                    //The Agent needs a speed different from 0 to calculate the velocity
                Agent.acceleration = 0;
                Agent.autoBraking = false;
                Agent.updateRotation = false;                                       //The Animal will control the rotation . NOT THE AGENT
                Agent.updatePosition = false;                                       //The Animal will control the  postion . NOT THE AGENT
                Agent.autoTraverseOffMeshLink = false;                              //Offmesh links are handled by animation
                Agent.stoppingDistance = StoppingDistance;
            }
        }

        protected virtual void OnEnable()
        {
            animal.OnStateActivate.AddListener(OnState);
            animal.OnModeStart.AddListener(OnModeStart);
            animal.OnModeEnd.AddListener(OnModeEnd);

            IsWaiting = true; //The AI Has not Started yet

            FreeMove = (animal.ActiveState.General.FreeMovement);
            if (FreeMove) ActiveAgent = false;
            if (Agent && !Agent.isOnNavMesh) ActiveAgent = false;
            HasArrived = false;
            TargetIsMoving = false;

            this.Delay_Action(() => StartAI());//Start AI a Frame later; 

            //Disable any Input Source in case it was active
            if (InputSource != null)
            {
                InputSource.MoveCharacter = false;
                Debuging("Input Move Disabled");
            }

            OnEnabled.Invoke();
        }

        protected virtual void OnDisable()
        {
            animal.OnStateActivate.RemoveListener(OnState);           //Listen when the Animations changes..
            animal.OnModeStart.RemoveListener(OnModeStart);           //Listen when the Animations changes..
            animal.OnModeEnd.RemoveListener(OnModeEnd);           //Listen when the Animations changes..
            Stop();
            StopAllCoroutines();
            OnDisabled.Invoke();

            animal.Rotate_at_Direction = false;

            //Disable any Input Source in case it was active
            if (InputSource != null)
            {
                InputSource.MoveCharacter = true;
                Debuging("Input Move Enabled");
            }
        }

        protected virtual void Update() { Updating(); }
        #endregion

        #region Animal Events Listen
        /// <summary>Called when the Animal Enter an Action, Attack, Damage or something similar</summary>
        public virtual void OnModeStart(int ModeID, int ability)
        {
            Debuging($"has started a Mode: <B>[{animal.ActiveMode.ID.name}]</B>. Ability: <B>[{animal.ActiveMode.ActiveAbility.Name}]</B>");
            if (animal.ActiveMode.AllowMovement) return; //Don't stop the Animal Movevemt if the Mode can make movements

            var Dest = DestinationPosition; //Store the Destination with modes
            Stop(); //If the Agent was moving Stop it
            DestinationPosition = Dest; //Restore the Destination with modes
        }

        /// <summary>  Listen if the Animal Has finished a mode  </summary>
        public virtual void OnModeEnd(int ModeID, int ability)
        {
            if (StateIsBlockingAgent) return; //Do nothing if the current State is blocking the agent.

            Debuging($"has ended a Mode: <B>[{ModeID}]</B>. Ability: <B>[{ability}]</B>");



            if (!HasArrived) //Don't move if there's no destination
            {
                CalculatePath();
                Move();
            }

            CompleteOffMeshLink();
            CheckAirTarget(); //Everytime a State Changes Check again in case it failed by mistake
        }


        /// <summary>Listen to the Animal when it changes States</summary>
        public virtual void OnState(int stateID)
        {
            if (IsWaiting) return; //Do nothing if the Agent is waiting

            FreeMove = (animal.ActiveState.General.FreeMovement); //Recheck if the current State is a FreeState
            if ( CheckAirTarget()) return; //Everytime a State Changes Check again in case it failed by mistake

            StateIsBlockingAgent = animal.ActiveStateID != 0 && StopAgentOn != null && StopAgentOn.Contains(animal.ActiveStateID); //Store the Active State Blocking


            if (StateIsBlockingAgent) //Check if we are on a State that does not require the Agent
            {
                ActiveAgent = false; //Disable the Agent
            }
            else
            {
                if (!IsOnNonMovingMode)
                {
                    CalculatePath();
                    Move();
                }

                CompleteOffMeshLink();
            }
        }
        #endregion

        public virtual void StartAI()
        {
          
            var targ = target; target = null;
            SetTarget(targ);                                                  //Set the first Target (IMPORTANT)  it also set the next future targets

            if (AgentTransform == animal.transform)
                Debug.LogWarning("The Nav Mesh Agent needs to be attached to a child Gameobject, not in the same gameObject as the Animal Component");
        }

        public virtual void Updating()
        {
            ResetAgentPosition();

            if (InOffMeshLink || IsWaiting) return;    //Do nothing while is in an offmeshLink or its Waiting
                                                
            CheckMovingTarget();

            if (FreeMove)
            {
                if (IsAirDestination && animal.ActiveStateID.ID != StateEnum.Fly)
                {
                    animal.State_Activate(StateEnum.Fly); //Forcing Fly if the animal was not flying
                    Debuging("Force! Flying!");
                }

                FreeMovement();
            }
            else
            {
                UpdateAgent();
            }
        }

        /// <summary>Reset the Agent Transform position to its Local Offset</summary>
        protected virtual void ResetAgentPosition()
        {
            AgentTransform.localPosition = AgentPosition;                  //Important! Reset the Agent Position to the default Position
            Agent.nextPosition = Agent.transform.position;                  //IMPORTANT!!!!Update the Agent Position to the Transform position 
        }

        /// <summary>Check if there's a path to go to</summary>
        public virtual bool PathPending() => ActiveAgent && Agent.isOnNavMesh && Agent.pathPending;

        /// <summary> Updates the Agents using he animation root motion </summary>
        public virtual void UpdateAgent()
        {
            if (HasArrived)
            {
                if (LookAtTargetOnArrival && LookAtOffset > 0)
                {
                    if (DestinationPosition == NullVector)
                    {
                        DestinationPosition = (target != null ? target.position : transform.position + transform.forward);
                    }

                    var Origin =  (animal.transform.position - animal.transform.forward * LookAtOffset * animal.ScaleFactor);

                    var LookAtDir = (target != null ? target.position : DestinationPosition) - Origin;



                    if (debugGizmos)
                    {
                        MTools.Draw_Arrow(Origin, LookAtDir, Color.magenta);
                        MTools.DrawWireSphere(Origin , Color.magenta, 0.1f);
                    }
                  
                    animal.RotateAtDirection(LookAtDir);
                }
                return;
            }

            if (ActiveAgent)
            {
                if (PathPending()) return;    //Means is still calculating the path to the Destination

                SetRemainingDistance(AgentRemainingDistance);

                if (!Arrive_Destination())   //if we havent't arrived to the destination ... Find the way 
                {
                    if (!CheckOffMeshLinks())
                    {
                        CalculatePath();
                        Move();   //Calculate the AI DIRECTION
                    }
                }
            }
        }

        /// <summary>Check if we have Arrived to the Destination</summary>
        public virtual bool Arrive_Destination()
        {
            if (CurrentStoppingDistance >= RemainingDistance)
            {
                if (IsPathIncomplete()) //Check when the Agent is trapped on an NavMesh that cannot exit
                {
                    Debuging($"[Agent Path Status: {Agent.pathStatus}]. Force Stop");
                    Stop();
                    StopWait();
                    HasArrived = true;
                    RemainingDistance = 0;                                  //Reset the Remaining Distance
                    AIDirection = Vector3.zero;                            //Reset AI Direction
                    return true;
                }

                if (!CheckDestinationHeight()) return false;

                HasArrived = true;
                RemainingDistance = 0;                                 //Reset the Remaining Distance
                AIDirection = Vector3.zero;                          //Reset AI Direction
                Move();

                OnTargetPositionArrived.Invoke(DestinationPosition);    //Invoke the Event On Target Position Arrived

                if (target)
                {
                    Debuging($"<color=green>has arrived to: <B>{target.name}</B> → {DestinationPosition} </color>");

                    OnTargetArrived.Invoke(target);                         //Invoke the Event On Target Arrived

                    CheckInteractions();

                    if (IsAITarget != null/* && IsAITarget.GetPosition() == DestinationPosition*/)  //If we have arrived to an AI Target and the Destination is the same one
                    {
                        IsAITarget.TargetArrived(animal.gameObject);                            //Call the method that the Target has arrived to the destination
                        LookAtTargetOnArrival = IsAITarget.ArriveLookAt;
                        if (IsAITarget.TargetType == WayPointType.Ground) FreeMove = false;     //if the next waypoing is on the Ground then set the free Movement to false
                        if (AutoNextTarget) MovetoNextTarget();                                 //Set and Move to the Next Target
                        else Stop();
                    }
                }
                else
                {
                    Debuging($"<color=green>has arrived to: <B>{DestinationPosition}</B>.  Stop</color>");
                    Stop(); //The target was removed
                }
                return true;
            }
            return false;
        }

        protected virtual bool IsPathIncomplete()
        {
            return ActiveAgent && !FreeMove && Agent.pathStatus != NavMeshPathStatus.PathComplete;
        }

        /// <summary>Check if the Height of the Destination is near the Animal</summary>
        protected virtual bool CheckDestinationHeight()
        {
            if (FreeMove) return true; //When Flying do not check the Height of the Point

            MTools.DrawWireSphere(DestinationPosition, Color.white, 0.1f);
           //if (IsWayPoint!= null) DestinationPosition = Agent.destination;

            var Result = NavMesh.SamplePosition(DestinationPosition, out _, Height, NavMesh.AllAreas);
            return Result;
        }

        /// <summary> Check if the Target is moving </summary>
        public virtual void CheckMovingTarget()
        {
            if (MTools.ElapsedTime(CurrentTime, UpdateAI))
            {
                if (Target)
                {
                    TargetIsMoving = (Target.position - TargetLastPosition).sqrMagnitude > (0.01f / animal.ScaleFactor);
                    TargetLastPosition = Target.position;

                    if (TargetIsMoving) Update_DestinationPosition();
                }
                CurrentTime = Time.time;
            }
        }


        /// <summary>Calculates the Direction to move the Animal using the Agent Desired Velocity</summary>
        public virtual void CalculatePath()
        {
            if (FreeMove) return; //Do nothing when its on Free Move
            //if (IsWaiting) return; //Do nothing when its waiting

            if (!ActiveAgent) //Enable the Agent in case is disabled
            {
                ActiveAgent = true;
                ResetFreeMoveOffMesh();
            }

            if (Agent.isOnNavMesh)
            {
                if (Agent.destination != DestinationPosition) //Calculate the New Path **ONLY** when the Destination is Different
                {
                    Agent.SetDestination(DestinationPosition);  //Set the Current Destination;

                    if (IsWayPoint != null) DestinationPosition = Agent.destination; //Important use the Cast value on the terrain.
                }
              
                if (Agent.desiredVelocity != Vector3.zero) AIDirection = Agent.desiredVelocity.normalized;
            }
        } 
     

        public virtual void Move()
        {
           //  animal.ForwardMultiplier = Mathf.Abs(animal.DeltaAngle) > TurnAngle ? 0 : 1; //Slow Down if the Animal can arrive to the target.
            animal.Move(AIDirection * SlowMultiplier);      //Move the Animal using the Agent Direction and the Slow Multiplier
        }

        /// <summary> Disable the AI Agent and it Stops the Animal</summary>
        public virtual void Stop()
        {
            ActiveAgent = false; //Disable the Agent
            AIDirection = Vector3.zero;
            DestinationPosition = NullVector;
            animal.StopMoving(); //Stop the Animal
            Debuging($"[Stopped]. Agent Disabled");
        }


        /// <summary>Update The Target Position </summary>
        protected virtual void Update_DestinationPosition()
        {
            if (UpdateDestinationPosition)
            {
                DestinationPosition = GetTargetPosition();                          //Update the Target Position 

                var DistanceOnMovingTarget = Vector3.Distance(DestinationPosition, AgentTransform.position); //Double check if the Animal is far from the target

                if (DistanceOnMovingTarget >= CurrentStoppingDistance)
                {
                    HasArrived = false;
                    CalculatePath();
                    Move();
                }
                else
                {
                    HasArrived = true; //Check if the animal hasn't arrived to a moving target
                }
            }
        }

        /// <summary>
        /// Store the remaining distance -- but if navMeshAgent is still looking for a path Keep Moving
        /// </summary>
        /// <param name="current"></param>
        protected virtual void SetRemainingDistance(float current) => RemainingDistance = current;



        #region Set Assing Target and Next Targets

        /// <summary> Resets al the Internal Values of the AI Control  </summary>
        public virtual void ResetAIValues()
        {
            StopWait();                                                 //If the Animal was waiting Reset the waiting IMPORTANT!!
            RemainingDistance = float.PositiveInfinity;                 //Set the Remaining Distance as the Max Float Value
            MinRemainingDistance = float.PositiveInfinity;                 //Set the Remaining Distance as the Max Float Value
            HasArrived = false;
        }

        /// <summary>
        /// Find the Closest
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private IAITarget ClosestTarget(IAITarget[] targets)
        {
            IAITarget result = null;

            if (targets != null)
            {
                float closeDist = float.PositiveInfinity;
                foreach (var t in targets)
                {
                    var Dist = (transform.position - t.GetPosition()).sqrMagnitude;

                    if (closeDist > Dist)
                    {
                        result = t;
                        closeDist = Dist;
                    }
                }
            }

            return result;
        }

        /// <summary>Set the next Target</summary>   
        public virtual void SetTarget(Transform newTarget, bool move)
        {
            // if (target == Target && !HasArrived) return;               //Don't assign the same target if we are travelling to that target (Breaks Wander Areas)

            target = newTarget;
            OnTargetSet.Invoke(newTarget);                                 //Invoked that the Target has changed.

            if (target != null)
            {
                TargetLastPosition = newTarget.position;                   //Since is a new Target "Reset the Target last position"
                DestinationPosition = newTarget.position;                  //Update the Target Position 


                var  AITargets = newTarget.FindInterfaces<IAITarget>(); //Find allthe AI Targets and find the closest one (Dragon Feet)
                IsAITarget = ClosestTarget(AITargets);

               // Debug.Log("isait = " + AITargets.Length);

                IsTargetInteractable = newTarget.FindInterface<IInteractable>();
                IsWayPoint = newTarget.FindInterface<IWayPoint>();

                NextTarget = null;

                if (IsWayPoint != null)
                {
                    NextTarget = IsWayPoint.NextTarget(); //Find the Next Target on the Waypoint
                }
              //  Debuging($"<color=yellow>New Target <B>[{newTarget.name}]</B> → [{DestinationPosition}]. Move = [{move}]</color>");

                CheckAirTarget();

                //Resume the Agent is MoveAgent is true
                if (move)
                {

                    ResetAIValues();
                    if (animal.IsPlayingMode) animal.Mode_Interrupt();      //In Case it was making any Mode Interrupt it because there's a new target to go to.
                    CurrentStoppingDistance = GetTargetStoppingDistance();
                    CurrentSlowingDistance = GetTargetSlowingDistance();

                    DestinationPosition = GetTargetPosition();

                    CalculatePath(); 

                    Move();
                    Debuging($"<color=yellow>is travelling to <B>Target: [{newTarget.name}]</B> → [{DestinationPosition}] </color>");
                }
            }
            else
            {
                IsAITarget = null;                  //Reset the AI Target
                IsTargetInteractable = null;        //Reset the AI Target Interactable
                IsWayPoint = null;                  //Reset the Waypoint

                if (move) Stop(); //Means the Target is null so Stop the Animal
            }
        }

        public virtual void SetTarget(GameObject target) => SetTarget(target, true);
        public virtual void SetTarget(GameObject target, bool move) => SetTarget(target != null ? target.transform : null, move);



        /// <summary>Remove the current Target and stop the Agent </summary>
        public virtual void ClearTarget() => SetTarget((Transform)null, false);

        /// <summary>Remove the current Target </summary>
        public virtual void NullTarget() => target = null;

        /// <summary>Assign a new Target but it does not move it to it</summary>
        public virtual void SetTargetOnly(Transform target) => SetTarget(target, false);
        public virtual void SetTargetOnly(GameObject target) => SetTarget(target, false);
        public virtual void SetTarget(Transform target) => SetTarget(target, true);

        /// <summary> Returns the Current Target Destination</summary>
        public virtual Vector3 GetTargetPosition()
        {
            var TargetPos = (IsAITarget != null) ? AITargetPos : target.position;
            if (TargetPos == Vector3.zero) TargetPos = target.position; //HACK FOR WHEN THE TARGET REMOVED THEIR AI TARGET COMPONENT
            return TargetPos;
        }

        public void TargetArrived(GameObject target) {/*Do nothing*/ }

        public virtual float GetTargetStoppingDistance() => IsAITarget != null ? IsAITarget.StopDistance() : StoppingDistance * animal.ScaleFactor;
        public virtual float GetTargetSlowingDistance() => IsAITarget != null ? IsAITarget.SlowDistance() : SlowingDistance * animal.ScaleFactor;

        /// <summary>Set the Next Target from  on the NextTargets Stored on the Waypoints or Zones</summary>

        public virtual void SetNextTarget(GameObject next)
        {
            NextTarget = next.transform;
            IsWayPoint = next.GetComponent<IWayPoint>(); //Check if the next gameobject is a Waypoint.
        }

        public virtual void MovetoNextTarget()
        {
            if (NextTarget == null)
            {
                Debuging("There's no Next Target");
                Stop();
                return;
            }

            if (IsWayPoint != null)
            {
                StopWait();
                I_WaitToNextTarget = C_WaitToNextTarget(IsWayPoint.WaitTime, NextTarget);   //IMPORTANT YOU NEED TO WAIT 1 FRAME ALWAYS TO GO TO THE NEXT WAYPOINT
                StartCoroutine(I_WaitToNextTarget);
            }
            else
            {
                SetTarget(NextTarget);
            }
        }

        public void StopWait()
        {
            IsWaiting = false;
            if (I_WaitToNextTarget != null) StopCoroutine(I_WaitToNextTarget); //Stop the coroutine in case it was playing
        }

        /// <summary> Check if the Next Target is a Air Target, if true then go to it</summary>
        internal virtual bool CheckAirTarget()
        {
            if (!CanFly) return false;

            if (IsAirDestination && !FreeMove)    //If the animal can fly, there's a new wayPoint & is on the Air
            {
                if (Target) Debuging($"Target {Target} is in the Air.  Activating Fly State", Target.gameObject);
                animal.State_Activate(StateEnum.Fly);
                FreeMove = true;
            }

            return IsAirDestination;
        }

        #endregion

        public virtual void SetDestination(Vector3 PositionTarget) => SetDestination(PositionTarget, true);

        /// <summary>Set the next Destination Position without having a target</summary>   
        public virtual void SetDestination(Vector3 newDestination, bool move)
        {
            LookAtTargetOnArrival = false; //Do not Look at the Target when its setting a destination

            if (newDestination == DestinationPosition) return;  //Means that you're already going to the same point so Skip the code

            CurrentStoppingDistance = PointStoppingDistance;    //Reset the stopping distance when Set Destination is used.

            ResetAIValues();

            if (IsOnNonMovingMode) animal.Mode_Interrupt();

            IsWayPoint = null;

            if (I_WaitToNextTarget != null)
                StopCoroutine(I_WaitToNextTarget);                          //if there's a coroutine active then stop it

            DestinationPosition = newDestination;                           //Update the Target Position 

            if (move)
            {
                CalculatePath();
                Move();
                Debuging($"<color=yellow>is travelling to: {DestinationPosition} </color>");
            }
        }

        /// <summary>Set the next Destination Position without having a target</summary>   
        public virtual void SetDestination(Vector3Var newDestination) => SetDestination(newDestination.Value);
       

        public virtual void SetDestinationClearTarget(Vector3 PositionTarget)
        {
            target = null;
            SetDestination(PositionTarget, true);
        }



        /// <summary>Check Interactions when Arriving to the Destination</summary>
        protected virtual void CheckInteractions()
        {
            if (IsTargetInteractable != null && IsTargetInteractable.Auto) //If the interactable is set to Auto!!!!!!!
            {
                if (Interactor != null)
                {
                    Interactor.Interact(IsTargetInteractable); //Do an Interaction if the Animal has an Interactor
                    Debuging($"Interact with : <b><{IsTargetInteractable.Owner.name}></b>. Interactor [{Interactor.Owner.name}]");
                }
                else
                {
                    IsTargetInteractable.Interact(0, animal.gameObject); //Do an Empty Interaction does not have an interactor
                    Debuging($"Interact with : <b><{IsTargetInteractable.Owner.name}></b>.  Interactor:Null");
                }
               
            }
        }

        /// <summary> Move Freely towards the Destination.. No Obstacle is calculated</summary>
        protected virtual void FreeMovement()
        {
            AIDirection = (DestinationPosition - animal.transform.position); //Important to be normalized!!
            SetRemainingDistance(AIDirection.magnitude);

            AIDirection = AIDirection.normalized * SlowMultiplier; //Important to be normalized!!

            animal.Move(AIDirection);
            Arrive_Destination();
        }


        protected virtual bool CheckOffMeshLinks()
        {
            if (AgentInOffMeshLink && !InOffMeshLink)                         //Check if the Agent is on a OFF MESH LINK (Do this once! per offmesh link)
            {
                InOffMeshLink = true;                                            //Just to avoid entering here again while we are on a OFF MESH LINK
                LastOffMeshDestination = DestinationPosition;

                OffMeshLinkData OMLData = Agent.currentOffMeshLinkData;

                if (OMLData.linkType == OffMeshLinkType.LinkTypeManual)        //Means that it has a OffMesh Link component
                {
                    var OffMesh_Link = OMLData.offMeshLink;                    //Check if the OffMeshLink is a Manually placed  Link

                    if (OffMesh_Link)
                    {
                        var AnimalLink = OffMesh_Link.GetComponent<MAIAnimalLink>();

                        //CUSTOM OFFMESHLINK
                        if (AnimalLink)
                        {
                            AnimalLink.Execute(this, animal);
                            return true;
                        }

                        Zone IsOffMeshZone =
                        OffMesh_Link.FindComponent<Zone>();                     //Search if the OFFMESH IS An ACTION ZONE (EXAMPLE CRAWL)

                        if (IsOffMeshZone)                                           //if the OffmeshLink is a zone and is not making an action
                        {
                            if (debug) Debuging($"<color=white>is on a <b>[OffmeshLink Zone]</b> -> [{IsOffMeshZone.name}]</color>");

                            IsOffMeshZone.ActivateZone(animal);                      //Activate the Zone
                            return true;
                        }


                        var NearTransform = transform.NearestTransform(OffMesh_Link.endTransform, OffMesh_Link.startTransform);
                        var FarTransform = transform.FarestTransform(OffMesh_Link.endTransform, OffMesh_Link.startTransform);

                        AIDirection = NearTransform.forward;
                        animal.Move(AIDirection);//Move where the AI DIRECTION FROM THE OFFMESH IS POINTINg


                        if (OffMesh_Link.CompareTag("Fly"))
                        {
                            Debuging($"<color=white>is On a <b>[OffmeshLink]</b> -> [Fly]</color>");
                            FlyOffMesh(FarTransform);
                        }
                        else if (OffMesh_Link.CompareTag("Climb"))
                        {
                            Debuging($"<color=white>is On a <b>[OffmeshLink]</b> -> [Climb] -> { OffMesh_Link.transform.name}</color>");
                            ClimbOffMesh();
                        }
                        else if (OffMesh_Link.area == 2)  //2 is Off mesh Jump
                        {
                            animal.State_Activate(StateEnum.Jump);       //if the OffMesh Link is a Jump type activate the jump
                            Debuging($"<color=white>is On a <b>[OffmeshLink]</b> -> [Jump]</color>");
                        }
                    }
                }
                else if (OMLData.linkType == OffMeshLinkType.LinkTypeJumpAcross)             //Means that it has a OffMesh Link component
                {
                    animal.State_Activate(StateEnum.Jump); //2 is Jump State
                }

                return true;
            }
            return false;
        }

      



        /// <summary> Completes the OffmeshLink in case the animal was in one </summary>
        public virtual void CompleteOffMeshLink()
        {
            if (InOffMeshLink)
            {
                CompleteAgentOffMesh();

                InOffMeshLink = false;
                DestinationPosition = LastOffMeshDestination;   //restore the OffMesh Link
                CalculatePath();
                Move();
            }
        }

        protected virtual void CompleteAgentOffMesh()
        {
            if (Agent && Agent.isOnOffMeshLink) Agent.CompleteOffMeshLink();                    //Complete an offmesh link in case the Agent was in one
        }

        protected virtual void FlyOffMesh(Transform target)
        {
            ResetFreeMoveOffMesh();
            IFreeMoveOffMesh = C_FlyMoveOffMesh(target);
            StartCoroutine(IFreeMoveOffMesh);
        }

        protected virtual void ClimbOffMesh()
        {
            if (IClimbOffMesh != null) StopCoroutine(IClimbOffMesh);
            IClimbOffMesh = C_Climb_OffMesh();
            StartCoroutine(IClimbOffMesh);
        }


        /// <summary>Check if the The animal was moving on a Free OffMesh Link </summary>
        protected virtual void ResetFreeMoveOffMesh()
        {
            if (IFreeMoveOffMesh != null)
            {
                InOffMeshLink = false;
                StopCoroutine(IFreeMoveOffMesh);
                IFreeMoveOffMesh = null;
            }
        }

        protected virtual IEnumerator C_WaitToNextTarget(float time, Transform NextTarget)
        {
            IsWaiting = true;

            if (time > 0)
            {
                yield return null; //SUUUUUUUUUPER  IMPORTANT!!!!!!!!!
                Debuging($"<color=white> is waiting <B>{time:F2}</B> seconds to go to <B>[{NextTarget.name}]</B> → {DestinationPosition} </color>");

                animal.Move(AIDirection = Vector3.zero); //Stop the Animal
                yield return new WaitForSeconds(time);
            }
            SetTarget(NextTarget);
        }

        protected virtual IEnumerator C_FlyMoveOffMesh(Transform target)
        {
            animal.State_Activate(StateEnum.Fly); //Set the State to Fly
            InOffMeshLink = true;
            float distance = float.MaxValue;

            while (distance > StoppingDistance)
            {
                if (target == null) break;
                animal.Move((target.position - animal.transform.position).normalized * SlowMultiplier);
                distance = Vector3.Distance(animal.transform.position, target.position);
                yield return null;
            }
            animal.ActiveState.AllowExit();

            Debuging("Exit Fly State Off Mesh");

            InOffMeshLink = false;
        }

        protected virtual IEnumerator C_Climb_OffMesh()
        {
            animal.State_Activate(StateEnum.Climb); //Set the State to Climb
            InOffMeshLink = true;
            yield return null;
            ActiveAgent = false;

            while (animal.ActiveState.ID == StateEnum.Climb)
            {
                animal.SetInputAxis(Vector3.forward); //Move Upwards on the Climb
                yield return null;
            }

            Debuging("Exit Climb State Off Mesh");

            InOffMeshLink = false;

            IClimbOffMesh = null;
        }

        public void ResetStoppingDistance() => CurrentStoppingDistance = StoppingDistance;
        public void ResetSlowingDistance() => CurrentSlowingDistance = SlowingDistance;
        public float StopDistance() => StoppingDistance;
        public float SlowDistance() => SlowingDistance;

        public virtual void ValidateAgent()
        {
            if (agent == null) agent = gameObject.FindComponent<NavMeshAgent>();

            AgentTransform = (agent != null) ? agent.transform : transform;
        }


        protected virtual void Debuging(string Log) { if (debug) Debug.Log($"<B>{animal.name} AI:</B> " + Log,this); }
        protected virtual void Debuging(string Log, GameObject obj) { if (debug) Debug.Log($"<B>{animal.name}:</B> " + Log, obj); }

#if UNITY_EDITOR
        [HideInInspector] public int Editor_Tabs1;

        protected virtual void OnValidate()
        {
            if (animal == null) animal = gameObject.FindComponent<MAnimal>();
            ValidateAgent();
        }


        void Reset()
        {
            SetDefaulStopAgent();
        }

        void SetDefaulStopAgent()
        {
            StopAgentOn = new List<StateID>(3)
            {
                MTools.GetInstance<StateID>("Fall"),
                MTools.GetInstance<StateID>("Jump"),
                MTools.GetInstance<StateID>("Fly")
            };
        }
         
        private string CheckBool(bool val) => val ? "[X]" : "[  ]";

        protected virtual void OnDrawGizmos()
        {
            var isPlaying = Application.isPlaying;

            if (isPlaying && debugStatus)
            {
                string log = "\nTarget: [" + (Target != null ? Target.name : "-none-") + "]";
                log += "- NextTarget: [" + (NextTarget != null ? NextTarget.name : "-none-") + "]";
                log += "\nRemainingDistance: " + RemainingDistance.ToString("F2");
                log += "\nStopDistance: " + CurrentStoppingDistance.ToString("F2");
                log += "\n" + CheckBool(HasArrived) + " HasArrived";
                log += "\n" + CheckBool(ActiveAgent) + " Agent";
                log += "\n" + CheckBool(TargetIsMoving) + " Target is Moving";
                log += "\n" + CheckBool(IsAITarget != null) + "Target is AITarget";
                log += "\n" + CheckBool(IsWayPoint != null) + "Target is WayPoint";
                log += "\n" + CheckBool(IsWaiting) + " Waiting";
                log += "\n" + CheckBool(IsOnMode) + " On Mode";
                log += "\n" + CheckBool(FreeMove) + " Free Move";
                log += "\n" + CheckBool(InOffMeshLink) + " InOffMeshLink";

                var Styl = new GUIStyle(GUI.skin.box);
                Styl.normal.textColor = Color.white;
                Styl.fontStyle = FontStyle.Bold;
                Styl.alignment = TextAnchor.UpperLeft;


                UnityEditor.Handles.Label(transform.position, "AI Log:" + log, Styl);
            }
            if (!debugGizmos) return;


            //Paths
            if (Agent && ActiveAgent && Agent.path != null)
            {
                Gizmos.color = Color.yellow;
                for (int i = 1; i < Agent.path.corners.Length; i++)
                {
                    Gizmos.DrawLine(Agent.path.corners[i - 1], Agent.path.corners[i]);
                }
            }


            if (isPlaying)
            {
                MTools.Draw_Arrow(AgentTransform.position, AIDirection * 2, Color.white);

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(DestinationPosition, stoppingDistance);
            }
            if (AgentTransform)
            {
                var scale = animal ? animal.ScaleFactor : transform.lossyScale.y;
                var Pos = (isPlaying) ? DestinationPosition : AgentTransform.position;
                var Stop = (isPlaying) ? CurrentStoppingDistance : StoppingDistance * scale;
                var Slow = (isPlaying) ? CurrentSlowingDistance : SlowingDistance * scale;



                Gizmos.color = Color.red;
                Gizmos.DrawSphere(AgentTransform.position, 0.1f);
                if (Slow > Stop)
                {
                    UnityEditor.Handles.color = Color.cyan;
                    UnityEditor.Handles.DrawWireDisc(Pos, Vector3.up, Slow);
                }

                UnityEditor.Handles.color = HasArrived ? Color.green : Color.red;
                UnityEditor.Handles.DrawWireDisc(Pos, Vector3.up, Stop);
            }
        }
#endif
    }

    #region Inspector


#if UNITY_EDITOR
    
    [CustomEditor(typeof(MAnimalAIControl), true)]
    public class AnimalAIControlEd : Editor
    {
        private MAnimalAIControl M;

        protected SerializedProperty 
            stoppingDistance, SlowingDistance, LookAtOffset,targett, UpdateAI, slowingLimit,
            agent, animal, PointStoppingDistance, OnEnabled,OnTargetPositionArrived, OnTargetArrived,
            OnTargetSet, debugGizmos, debugStatus, debug, Editor_Tabs1, nextTarget, OnDisabled, AgentTransform, OffMeshAlignment,
            StopAgentOn//, TurnAngle
            ;

        protected virtual void OnEnable()
        {
            M = (MAnimalAIControl)target;

            animal = serializedObject.FindProperty("animal");
            AgentTransform = serializedObject.FindProperty("AgentTransform");
            GetAgentProperty();

            slowingLimit = serializedObject.FindProperty("slowingLimit");
           // TurnAngle = serializedObject.FindProperty("TurnAngle");

            OnEnabled = serializedObject.FindProperty("OnEnabled");
            OnDisabled = serializedObject.FindProperty("OnDisabled");

            OnTargetSet = serializedObject.FindProperty("OnTargetSet");
            OnTargetArrived = serializedObject.FindProperty("OnTargetArrived");
            OnTargetPositionArrived = serializedObject.FindProperty("OnTargetPositionArrived");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            PointStoppingDistance = serializedObject.FindProperty("PointStoppingDistance");
            SlowingDistance = serializedObject.FindProperty("slowingDistance");
            LookAtOffset = serializedObject.FindProperty("LookAtOffset");
            targett = serializedObject.FindProperty("target");
            nextTarget = serializedObject.FindProperty("nextTarget");
            OffMeshAlignment = serializedObject.FindProperty("OffMeshAlignment");

            debugGizmos = serializedObject.FindProperty("debugGizmos");
            debugStatus = serializedObject.FindProperty("debugStatus");
            debug = serializedObject.FindProperty("debug");

            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            StopAgentOn = serializedObject.FindProperty("StopAgentOn");

            UpdateAI = serializedObject.FindProperty("UpdateAI");


            if (M.StopAgentOn == null || M.StopAgentOn.Count == 0)
            {
                M.StopAgentOn = new System.Collections.Generic.List<StateID>(2) { MTools.GetInstance<StateID>("Fall"), MTools.GetInstance<StateID>("Fly") };
                StopAgentOn.isExpanded = true;
                MTools.SetDirty(M);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public virtual void GetAgentProperty()
        {
            agent = serializedObject.FindProperty("agent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("AI Source. Moves the animal using an AI Agent");

            EditorGUI.BeginChangeCheck();
            {
               // EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);

                Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, new string[] { "General", "Events", "Debug" });

                int Selection = Editor_Tabs1.intValue;

                if (Selection == 0) ShowGeneral();
                else if (Selection == 1) ShowEvents();
                else if (Selection == 2) ShowDebug();


                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Animal AI Control Changed");
                }
            }

            if (M.Agent != null && M.animal != null && M.Agent.transform == M.animal.transform)
            {
                EditorGUILayout.HelpBox("The NavMesh Agent needs to be attached to a child gameObject. " +
                    "It cannot be in the same gameObject as the Animal Component", MessageType.Error);
            }

         //   EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
        private void ShowGeneral()
        {
              EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                targett.isExpanded = MalbersEditor.Foldout(targett.isExpanded, "Targets");
                if (targett.isExpanded)
                {
                    EditorGUILayout.PropertyField(targett, new GUIContent("Target", "Target to follow"));
                    EditorGUILayout.PropertyField(nextTarget, new GUIContent("Next Target", "Next Target the animal will go"));
                }
            }
             EditorGUILayout.EndVertical();

             EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.BeginChangeCheck();
                {
                    UpdateAI.isExpanded = MalbersEditor.Foldout(UpdateAI.isExpanded, "AI Parameters");

                    if (UpdateAI.isExpanded)
                    {
                        // EditorGUILayout.LabelField("AI Parameters", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(UpdateAI, new GUIContent("Update Agent", " Recalculate the Path for the Agent every x seconds "));
                        EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stopping Distance", "Agent Stopping Distance"));
                        EditorGUILayout.PropertyField(SlowingDistance, new GUIContent("Slowing Distance", "Distance to Start slowing the animal before arriving to the destination"));
                        EditorGUILayout.PropertyField(LookAtOffset);
                        EditorGUILayout.PropertyField(PointStoppingDistance, new GUIContent("Point Stop Distance", "Stop Distance used on the SetDestination method. No Target Assigned"));
                       // EditorGUILayout.PropertyField(TurnAngle);
                        EditorGUILayout.PropertyField(slowingLimit);
                        EditorGUILayout.PropertyField(OffMeshAlignment);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (M.Agent)
                    {
                        M.Agent.stoppingDistance = stoppingDistance.floatValue;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
             EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                animal.isExpanded = MalbersEditor.Foldout(animal.isExpanded, "References");

                if (animal.isExpanded)
                {
                    EditorGUILayout.PropertyField(animal, new GUIContent("Animal", "Reference for the Animal Controller"));
                    EditorGUILayout.PropertyField(AgentTransform, new GUIContent("Agent", "Reference for the AI Agent Transform"));
                    //EditorGUILayout.PropertyField(agent, new GUIContent("Agent", "Reference for the Nav Mesh Agent")); 
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(StopAgentOn, new GUIContent($"{StopAgentOn.displayName} ({StopAgentOn.arraySize })"), true);

                    if (StopAgentOn.isExpanded && GUILayout.Button(new GUIContent ("Set Default Off States","By Default the AI should not be Active on Fly, Jump or Fall states"), GUILayout.MinWidth(150)))
                    {
                        M.StopAgentOn = new List<StateID>(3)
                    {
                        MTools.GetInstance<StateID>("Fall"),
                        MTools.GetInstance<StateID>("Jump"),
                        MTools.GetInstance<StateID>("Fly")
                    };
                        serializedObject.ApplyModifiedProperties();

                        Debug.Log("Stop Agent set to default: [Fall,Jump,Fly]");
                        MTools.SetDirty(target);
                    }
                    EditorGUI.indentLevel--;


                    M.ValidateAgent();

                    if (!M.AgentTransform)
                    {
                        EditorGUILayout.HelpBox("There's no Agent found on the hierarchy on this gameobject\nPlease add a NavMesh Agent Component", MessageType.Error);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(OnEnabled);
                EditorGUILayout.PropertyField(OnDisabled);
                EditorGUILayout.PropertyField(OnTargetPositionArrived, new GUIContent("On Position Arrived"));
                EditorGUILayout.PropertyField(OnTargetArrived, new GUIContent("On Target Arrived"));
                EditorGUILayout.PropertyField(OnTargetSet, new GUIContent("On New Target Set"));
            }
            EditorGUILayout.EndVertical();
        }

        protected GUIStyle Bold(bool tru) => tru ? EditorStyles.boldLabel : EditorStyles.miniBoldLabel;

        private void ShowDebug()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50f;
                EditorGUILayout.PropertyField(debug, new GUIContent("Console"));
                EditorGUILayout.PropertyField(debugGizmos, new GUIContent("Gizmos"));
                EditorGUIUtility.labelWidth = 80f;
                EditorGUILayout.PropertyField(debugStatus, new GUIContent("In-Game Log"));
                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndHorizontal();
                if (Application.isPlaying)
                {

                    Repaint();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(targett);
                    EditorGUILayout.ObjectField("Next Target", M.NextTarget, typeof(Transform), false);
                    EditorGUILayout.Vector3Field("Destination", M.DestinationPosition);
                    EditorGUILayout.Vector3Field("AI Direction", M.AIDirection);
                    EditorGUILayout.Space();
                    EditorGUILayout.FloatField("Current Stop Distance", M.StoppingDistance);
                    EditorGUILayout.FloatField("Remaining Distance", M.RemainingDistance);
                    EditorGUILayout.FloatField("Slow Multiplier", M.SlowMultiplier);
                    // EditorGUILayout.FloatField("Circling Around", M.CircleAroundMultiplier);
                    EditorGUILayout.Space();




                    EditorGUIUtility.labelWidth = 70;
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.ToggleLeft("Target is Moving", M.TargetIsMoving, Bold(M.TargetIsMoving));
                        EditorGUILayout.ToggleLeft("Target is AITarget", M.IsAITarget != null, Bold(M.IsAITarget != null));
                        EditorGUILayout.ToggleLeft("Target is WayPoint", M.IsWayPoint != null, Bold(M.IsWayPoint != null));
                        EditorGUILayout.Space();
                        EditorGUILayout.ToggleLeft("LookAt Target", M.LookAtTargetOnArrival, Bold(M.LookAtTargetOnArrival));
                        EditorGUILayout.ToggleLeft("Auto Next Target", M.AutoNextTarget, Bold(M.AutoNextTarget));
                        EditorGUILayout.ToggleLeft("UpdateDestinationPos", M.UpdateDestinationPosition, Bold(M.UpdateDestinationPosition));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.ToggleLeft("Is On Mode", M.IsOnMode, Bold(M.IsOnMode));
                        EditorGUILayout.ToggleLeft("Free Move", M.FreeMove, Bold(M.FreeMove));
                        EditorGUILayout.ToggleLeft("In OffMesh Link", M.InOffMeshLink, Bold(M.InOffMeshLink));

                        EditorGUILayout.Space();
                        EditorGUILayout.ToggleLeft("Waiting", M.IsWaiting, Bold(M.IsWaiting));
                        EditorGUILayout.ToggleLeft("Has Arrived to Destination", M.HasArrived, Bold(M.HasArrived));

                        EditorGUILayout.ToggleLeft("Active Agent", M.ActiveAgent, Bold(M.ActiveAgent));
                        if (M.Agent && M.ActiveAgent)
                        {
                            EditorGUILayout.ToggleLeft("Agent in NavMesh", M.Agent.isOnNavMesh, Bold(M.Agent.isOnNavMesh));
                        }
                        EditorGUILayout.EndVertical();

                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;

                    DrawChildDebug();


                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawChildDebug()
        {}
        
    }
#endif
    #endregion
}