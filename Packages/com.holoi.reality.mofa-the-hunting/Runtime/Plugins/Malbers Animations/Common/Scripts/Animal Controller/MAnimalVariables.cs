using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System;
using System.Collections;
using MalbersAnimations.Utilities;

namespace MalbersAnimations.Controller
{
    /// Variables
    public partial class MAnimal
    {
        ///<summary> List of States for this animal  </summary>
        public List<State> states = new List<State>();
        /// <summary>List of Stances Available on the animals</summary>
        public List<Stance> Stances;
        ///<summary> List of States for this animal  </summary>
        public List<Mode> modes = new List<Mode>();

        /// <summary>Sets a Bool Parameter on the Animator using the parameter Hash</summary>
        public System.Action<int, bool> SetBoolParameter { get; set; } = delegate { };
        /// <summary>Sets a float Parameter on the Animator using the parameter Hash</summary>
        public System.Action<int, float> SetFloatParameter { get; set; } = delegate { };

        /// <summary>Sets a Integer Parameter on the Animator using the parameter Hash</summary> 
        public System.Action<int, int> SetIntParameter { get; set; } = delegate { };

        /// <summary>Sets a Trigger Parameter on the Animator using the parameter Hash</summary> 
        public System.Action<int> SetTriggerParameter { get; set; } = delegate { };

        /// <summary>Check when a Animation State is Starting</summary>
        public System.Action<int> StateCycle { get; set; }

        /// <summary>Invoked after the ActiveState execute its code</summary>
        public System.Action<MAnimal> PreStateMovement = delegate { };

        /// <summary>Invoked after the ActiveState execute its code</summary>
        public System.Action<MAnimal> PostStateMovement = delegate { };

        /// <summary>Get all the Animator Parameters the Animal Controller has</summary>
        private List<int> animatorHashParams;
        //private Hashtable animatorParams;

        #region Static Properties
        /// <summary>List of all the animals on the scene</summary>
        public static List<MAnimal> Animals;
        /// <summary>Main Animal used as the player controlled by Input</summary>
        public static MAnimal MainAnimal;
        #endregion

        #region States
        /// <summary>NECESARY WHEN YOU ARE USING MULTIPLE ANIMALS</summary>
        public bool CloneStates = true;

        [Tooltip("The Animal gameObject will have no Parent on the Hierarchy. (Recommended)")]
        public bool NoParent = true;

      
        
        ///<summary>On Which State the animal should Start on Enable</summary>
        public StateID OverrideStartState;

        /// <summary> Current State. Changing this variable wil not exectute the Active State logic</summary> 
        internal State activeState;
        /// <summary> Store the Last State, This will not change the Last State parameter on the animator</summary> 
        internal State lastState;

        internal State queueState;

        /// <summary> Check if a State wsa queued and if it can be released because it was activated </summary> 
        public bool QueueReleased => QueueState != null && QueueState.OnActiveQueue && !QueueState.OnQueue;

        /// <summary> Store the current queue State </summary> 
        public State QueueState { get => queueState; internal set => queueState = value;}


        /// <summary> Store the Last State </summary> 
        public State LastState
        {
            get => lastState;
            internal set
            {
                if (value == null) return;

                lastState = value;


                LastState.EnterExitEvent?.OnExit.Invoke();
                LastState.ExitState();                               //Exectute the Exit State code on the Last State.
                var LastStateID = (QueueState == null) ? lastState.ID.ID : QueueState.ID.ID;
                SetIntParameter(hash_LastState, LastStateID);   //Sent to the Animator the previews Active State
                //    Debug.Log("LastStateID = " + LastStateID);
            }
        }

        ///<summary> Store a State (PreState) that can be used later</summary>
        protected State Pin_State;

        /// <summary>Used to call the Last State one more time before it changes to the new state </summary>
        public bool JustActivateState { get; internal set; }

        /// <summary> ID of the Current Active State</summary>
        public StateID ActiveStateID { get; private set; }


        /// <summary>State Float Value</summary>
        public float State_Float { get; private set; }

        /// <summary>Set/Get the Active State</summary>
        public State ActiveState
        {
            get => activeState;
            internal set
            {
                activeState = value;

                if (value == null) return;

                JustActivateState = true;

                if (LastState != null && LastState.ExitFrame) LastState.OnStateMove(DeltaTime);           //Play One Last Time the Last State

                this.Delay_Action(() => { JustActivateState = false; });            //Reset Just Activate State The next Frame
                ActiveStateID = activeState.ID;

                OnStateActivate.Invoke(activeState.ID);

                SetIntParameter(hash_State, activeState.ID.ID);                     //Sent to the Animator the value to Apply  
               // Debug.Log("hash_State = " + activeState.ID.ID);

                TryAnimParameter(hash_StateOn);                             //Use trigger in case the Animal is using Triggers
                Strafe = Strafe;                                                    // Execute the code inside in case has changed

                if (HasStances)
                {
                    //Check if the Active Stance cannot be used on the New State
                    if (ActiveStance != null && !ActiveStance.CanBeUsedOnState(ActiveStateID))
                    {
                        ActiveStance.SetPersistent(false); //Force to exit if it was persistent

                        if (ActiveStance.OnQueueState(ActiveStateID)) ActiveStance.Queued = true;
                         
                        Stance_Reset();
                    }
                }
                //Old Way
                else  if (!activeState.ValidStance(currentStance))
                    Stance_Reset();

                foreach (var st in states) st.NewStateActivated(activeState.ID); //Notify all states that a new state has been activated
                foreach (var sn in Stances) sn.NewStateActivated(activeState.ID); //Notify all stances that a new state has been activated

                Set_Sleep_FromStates(activeState);
                Check_Queue_States(activeState.ID);    //Check if a queue State was released


                if (IsPlayingMode && ActiveMode.StateCanInterrupt(ActiveStateID))//If a mode is playing check a State Change
                {
                    Mode_Interrupt();
                }
            }
        }

        /// <summary>When a new State is Activated, Make sure the other States are sent to sleep</summary>
        internal void Set_Sleep_FromStates(State state)
        {
            foreach (var st in states)
            {
                st.IsSleepFromState = st.SleepFromState.Contains(state.ID);        //Sent to sleep states that has some Avoid State
                st.IsSleepFromState ^= !st.IncludeSleepState;
            }
        }

        /// <summary>Check if there's a State that cannot be enabled when playing a mode </summary>
        internal virtual void Set_State_Sleep_FromMode(bool playingMode)
        {
            foreach (var state in states)
                state.IsSleepFromMode = playingMode && state.SleepFromMode.Contains(ActiveMode.ID);
        }


        /// <summary>Check if there's a State that cannot be enabled when playing a mode </summary>
        internal virtual void Set_State_Sleep_FromStance()
        {
            foreach (var state in states)
                state.IsSleepFromStance = state.SleepFromStance.Contains(Stance);
        }

        /// <summary>When a new State is Activated, Make sure the other States are sent to sleep</summary>
        internal virtual void Check_Queue_States(StateID ID)
        {
            foreach (var st in states)
            {
                st.OnQueue = st.QueueFrom.Contains(ID);        //Sent to sleep states that has some Avoid State
            }
        }

        #endregion

        #region General  
        /// <summary> Layers the Animal considers ground</summary>
        [SerializeField] private LayerReference groundLayer = new LayerReference(1);

        /// <summary> Layers the Animal considers ground</summary>
        public LayerMask GroundLayer => groundLayer.Value;

        /// <summary>Distance from the Pivots to the ground </summary>
        public float height = 1f;
        /// <summary>Height from the ground to the hip multiplied for the Scale Factor</summary>
        public float Height => (height) * ScaleFactor;

        /// <summary>The Scale Factor of the Animal.. if the animal has being scaled this is the multiplier for the raycasting things </summary>
        public float ScaleFactor => transform.localScale.y;

        /// <summary>Does this Animal have an InputSource </summary>
        public IInputSource InputSource;

        [SerializeField] private Vector3 center;
        /// <summary>Center of the Animal to be used for AI and Targeting  based on World Position</summary>
        public Vector3 Center
        {
            private set => center = value;
            get => transform.TransformPoint(center);
        }
        #endregion

        #region Stance
        [SerializeField] private StanceID currentStance;
        [SerializeField] private StanceID defaultStance;

        /// <summary>Does the Animal has the new List of Stances</summary>
        public bool HasStances { get; private set; }
        public Stance ActiveStance { get; set; }
        public Stance LastActiveStance { get; set; }

        /// <summary>Last Stance the Animal was</summary>
        public int LastStanceID { get; private set; }

        public StanceID DefaultStanceID { get => defaultStance; set => defaultStance = value; }

        /// <summary>Current Active Stance</summary>
        public StanceID Stance
        {
            get => currentStance;
            set
            {
                if (value == null) return; //Do nothing with empty IDs
                if (!enabled) return;                      //Do nothing if is not active
                if (Sleep) return;                      //Do nothing if is not active

                if (value == currentStance) return; //Change only when the values are different

                if (HasStances)
                {
                    SetAdvancedStance(value);
                    return;
                }
                 
                if (!ActiveState.ValidStance(value)) return;        //Do not Activate any new stance if the STATE does not allow it

                LastStanceID = currentStance;
                currentStance = value;

                var exit = OnEnterExitStances.Find(st => st.ID.ID == LastStanceID);
                exit?.OnExit.Invoke();
                OnStanceChange.Invoke(value);
                var enter = OnEnterExitStances.Find(st => st.ID.ID == value);
                enter?.OnEnter.Invoke();

                Set_State_Sleep_FromStance();

                if (debugStances)
                {
                    Debug.Log($"<B>[{name}]</B> → <B>[Set Stance] → <color=yellow>{value.name} [{value.ID}]</color></B>");
                }

                TryAnimParameter(hash_Stance, currentStance.ID);      //Set on the Animator the Current Stance
                TryAnimParameter(hash_LastStance, LastStanceID);        //Set on the Animator the Last Stance
                TryAnimParameter(hash_StateOn);                       //Set on the Animator the Trigger Stance

                if (!JustActivateState) SetIntParameter(hash_LastState, ActiveStateID); //Sent to the Animator the previews Active State  (BUG)

                ActiveState.SetSpeed(); //Check if the speed modifier has changed when you have a new Stance

                if (IsPlayingMode && ActiveMode.StanceCanInterrupt(currentStance))//If a mode is playing check a State Change
                {
                    Mode_Interrupt();
                }
            }
        }

        private void SetAdvancedStance(StanceID value)
        {
            var new_stance = Stance_Get(value);

            if (new_stance != null)
            {
                if (new_stance.CanActivate())
                {
                    LastActiveStance = ActiveStance;
                    ActiveStance = new_stance;
                    LastStanceID = currentStance;
                    currentStance = value;

                    LastActiveStance.Exit();
                    ActiveStance.Activate();
                    OnStanceChange.Invoke(value);

                    //Enable and Disable Temporarly the Stances
                    foreach (var _st in Stances)
                    {
                        if (new_stance.DisableStances.Count > 0 &&
                            new_stance.DisableStances.Contains(_st.ID)) _st.Disable_Temp();
                        
                        if (LastActiveStance.DisableStances.Count > 0 &&
                            LastActiveStance.DisableStances.Contains(_st.ID)) _st.Enable_Temp();
                    }

                    Set_State_Sleep_FromStance();

                    if (debugStances)
                        Debug.Log($"<B>[{name}] → Set: <color=yellow>[Stance - {value.name} - {value.ID}]</color></B>", gameObject);

                    TryAnimParameter(hash_Stance, currentStance.ID);      //Set on the Animator the Current Stance
                    TryAnimParameter(hash_LastStance, LastStanceID);      //Set on the Animator the Last Stance
                 
                    if (!JustActivateState) SetIntParameter(hash_LastState, ActiveStateID); //Sent to the Animator the previews Active State  (BUG)

                    TryAnimParameter(hash_StateOn);                    //Set on the Animator the Trigger Stance

                    Strafe = Strafe && ActiveStance.CanStrafe;                 //Update Strafing

                    ActiveState.SetSpeed(); //Check if the speed modifier has changed when you have a new Stance

                    if (IsPlayingMode && ActiveMode.StanceCanInterrupt(currentStance))//If a mode is playing check a State Change
                    {
                        Mode_Interrupt();
                    }
                }
            }
            else
            {
                if (debugStances && new_stance == null)
                {
                    Debug.Log($"<B>[{name}]</B> - <B> <color=yellow>[Stance: {value.name}]</color> - Fail to Activate. [NOT Found]</B>", gameObject);
                }
            }

        }
        #endregion

        #region Movement

      
        [Tooltip("Global multiplier for the Animator Speed")]
        public FloatReference AnimatorSpeed = new FloatReference(1);

        [Tooltip("Local Time Multiplier for the Animal. Cool Slowmo Stuffs")]
        public FloatReference m_TimeMultiplier = new FloatReference(1);

        public float  TimeMultiplier { get => m_TimeMultiplier.Value; set => m_TimeMultiplier.Value = value; }

        //public FloatReference MovementDeathValue = new FloatReference(0.05f);
        [SerializeField] private BoolReference alwaysForward = new BoolReference(false);

        /// <summary>Sets to Zero the Z on the Movement Axis when this is set to true</summary>
        [Tooltip("Sets to Zero the Z on the Movement Axis when this is set to true")]
        [SerializeField] private BoolReference lockForwardMovement = new BoolReference(false);
        /// <summary>Sets to Zero the X on the Movement Axis when this is set to true</summary>
        [Tooltip("Sets to Zero the X on the Movement Axis when this is set to true")]
        [SerializeField] private BoolReference lockHorizontalMovement = new BoolReference(false);
        /// <summary>Sets to Zero the Y on the Movement Axis when this is set to true</summary>
        [Tooltip("Sets to Zero the Y on the Movement Axis when this is set to true")]
        [SerializeField] private BoolReference lockUpDownMovement = new BoolReference(false);

        ////public bool AdditiveX;
        ////public bool AdditiveY;
        //[Tooltip("The Up Down Input is interpreted as Additive (Spyro Underwater Movement Style)")]
        //public bool AdditiveUp;

        /// <summary>Multiplier to Add or Remove Forward Movement to the Animal, Used when the Animal Rotates in Place</summary>
       // public float ForwardMultiplier { get; set; }

        /// <summary>(Z), horizontal (X) and Vertical (Y) Movement Input</summary>
        public Vector3 MovementAxis;

        /// <summary>(Z), horizontal (X) and Vertical (Y) Raw Movement Input</summary>
        public Vector3 MovementAxisRaw;// { get; set; }
        //{
        //    get => movementAxisRaw;
        //    set
        //    {
        //        movementAxisRaw = value;
        //        Debug.Log($"movementAxisRaw: {movementAxisRaw} ");
        //    }
        //}
        //Vector3 movementAxisRaw;

        /// <summary>Current Raw Input Axis gotted from an Input Entry </summary>
        public Vector3 RawInputAxis;// { get; set; }
        //{
        //    get => rawInputAxis;
        //    set
        //    {
        //        rawInputAxis = value;
        //       Debug.Log($"RawInputAxis: {rawInputAxis} ");
        //    }
        //}
        //Vector3 rawInputAxis;


        /// <summary>The Animal is using Input instead of a Direction to move</summary>
        public bool UseRawInput { get; internal set; }
        //{
        //    get => useRawInput;
        //    set
        //    {
        //        useRawInput = value;
        //        Debug.Log($"useRawInput: {useRawInput} ");
        //    }
        //}
        //bool useRawInput;

        /// <summary>Forward (Z), horizontal (X) and Vertical (Y) Smoothed Movement Input AFTER aplied Speeds Multipliers (THIS GOES TO THE ANIMATOR)</summary>
        public Vector3 MovementAxisSmoothed;


        /// <summary>The animal will always move forward</summary>
        public bool AlwaysForward
        {
            get => alwaysForward.Value;
            set
            {
                alwaysForward.Value = value;
                MovementAxis.z = alwaysForward.Value ? 1 : 0;
                MovementDetected = AlwaysForward;
            }
        }

        /// <summary>[Raw Direction the Character will go Using Inputs or a Move Direction</summary>
        public Vector3 Move_Direction;
        // public Vector3 MoveDirection => Move_Direction;


        private bool movementDetected;

        /// <summary>Checking if the movement input was activated</summary>
        public bool MovementDetected
        {
            get => movementDetected;
            internal set
            {
                if (movementDetected != value)
                {
                    movementDetected = value;
                    OnMovementDetected.Invoke(value);
                    SetBoolParameter(hash_Movement, MovementDetected);
                }
            }
        }

        /// <summary>The Animal uses the Camera Forward Diretion to Move</summary>
        public BoolReference useCameraInput = new BoolReference(true);

        /// <summary>Use the Camera Up Vector to Move while flying or Swiming UnderWater</summary>
        public BoolReference useCameraUp = new BoolReference();

        /// <summary>Store the Starting Use camera Input Value</summary>
        public bool DefaultCameraInput { get; set; }

        public void ResetCameraInput() => UseCameraInput = DefaultCameraInput; //Restore to the default camera input on the Animal

        /// <summary>Use the Camera Up Vector to Move while flying or Swiming UnderWater</summary>
        public bool UseCameraUp { get => useCameraUp.Value; set => useCameraUp.Value = value; }

        /// <summary>The Animal uses the Camera Forward Direction to Move</summary>
        public bool UseCameraInput
        {
            get => useCameraInput.Value;
            set 
            { 
                useCameraInput.Value = UsingMoveWithDirection = value;
              // Debug.Log("useCameraInput.Value = " + useCameraInput.Value);
            }
        }

        /// <summary> Is the animal using a Direction Vector for moving(True) or a World Axis Input (False)</summary>
        public bool UsingMoveWithDirection  { set; get; }
        //{
        //    get => usingMoveWithDirection;
        //    set
        //    {
        //        usingMoveWithDirection = value;
        //        Debug.Log("UsingMoveWithDirection = " + value);
        //    }
        //}
       //private bool usingMoveWithDirection;

        /// <summary> Is the animal using a Direction Vector for rotate in place(true?)</summary>
        public bool Rotate_at_Direction { set; get; }

        /// <summary>Main Camera on the Game</summary>
        public TransformReference m_MainCamera = new TransformReference();

        public Transform MainCamera => m_MainCamera.Value;


        [SerializeField] private bool additivePosLog;
        [SerializeField] private bool additiveRotLog;


        private void DebLogAdditivePos()
        {
            additivePosLog ^= true;
            MTools.SetDirty(this);
        }

        private void DebLogAdditiveRot()
        {
            additiveRotLog ^= true;
            MTools.SetDirty(this);
        }
        [ContextMenuItem("Debug AdditivePos", nameof(DebLogAdditivePos))]
        [ContextMenuItem("Debug AdditiveRot", nameof(DebLogAdditiveRot))]

        /// <summary>Is this animal is the main Player?</summary>
        public BoolReference isPlayer = new BoolReference(true);


        /// <summary> Additive Position Modifications for the  animal (Terrian Snapping, Speed Modifiers Positions, etc)</summary>
        public Vector3 AdditivePosition//;
        {
            get => additivePosition;
            set
            {
                additivePosition = value;
                 if (additivePosLog)
                    Debug.Log($"Additive Pos:  {(additivePosition / DeltaTime)} ");
            }
        }
        internal Vector3 additivePosition;

        /// <summary> Additive Rotation Modifications for the  animal (Terrian Aligment, Speed Modifiers Rotations, etc)</summary>
        public Quaternion AdditiveRotation
        {
            get => additiveRotation;
            set
            {
                additiveRotation = value;
                if (additiveRotLog) Debug.Log($"Additive ROT:  {(additiveRotation):F3} ");
            }
        }
        Quaternion additiveRotation;




        /// <summary>Inertia Speed to smoothly change the Speed Modifiers </summary>
        public Vector3 InertiaPositionSpeed { get; internal set; }
        //{
        //    get => InertiaPPS;
        //    set
        //    {
        //        InertiaPPS = value;
        //        Debug.Log($"InertiaPositionSpeed:  {(InertiaPPS):F3} ");
        //    }
        //}
        //Vector3 InertiaPPS;
      
        
        [SerializeField] private BoolReference SmoothVertical = new BoolReference(true);
        
        [Tooltip("Global turn multiplier to increase rotation on the animal")]
        public FloatReference TurnMultiplier = new FloatReference(0f);
       
        [Tooltip("Smooth Damp Value to Turn in place, when using LookAt Direction Instead of Move()")]
        public FloatReference inPlaceDamp = new FloatReference(2f);

        /// <summary>Difference from the Last Frame and the Current Frame</summary>
        public Vector3 DeltaPos { get; internal set; }
        //{
        //    set
        //    {
        //        m_DeltaPos = value;
        //        Debug.Log($"DeltaPos POS:  {(m_DeltaPos / DeltaTime):F3} ");
        //    }
        //    get => m_DeltaPos;
        //}
        //Vector3 m_DeltaPos;


        /// <summary>World Position on the last Frame</summary>
        public Vector3 LastPos { get; internal set; }

        /// <summary>Velocity acumulated from the last Frame</summary>
        public Vector3 Inertia => DeltaPos / DeltaTime;

        /// <summary>Difference between the Current Rotation and the desire Input Rotation </summary>
        public float DeltaAngle { get; internal set; }

        /// <summary>Pitch direction used when Free Movement is Enable (Direction of the Move Input) </summary>
        public Vector3 PitchDirection { get; internal set; }
        /// <summary>Pitch Angle </summary>
        public float PitchAngle { get; internal set; }
        /// <summary>Bank</summary>
        public float Bank { get; internal set; }

        /// <summary>Speed from the Vertical input multiplied by the speeds inputs(Walk Trot Run) this is the value thats goes to the Animator, is not the actual Speed of the animals</summary>
        public float VerticalSmooth { get => MovementAxisSmoothed.z; internal set => MovementAxisSmoothed.z = value; }

        /// <summary>Direction from the Horizontal input multiplied by the speeds inputs this is the value thats goes to the Animator, is not the actual Speed of the animals</summary>
        public float HorizontalSmooth { get => MovementAxisSmoothed.x; internal set => MovementAxisSmoothed.x = value; }

        /// <summary>Direction from the Up Down input multiplied by the speeds inputs this is the value thats goes to the Animator, is not the actual Speed of the animals</summary>
        public float UpDownSmooth
        {
            get => MovementAxisSmoothed.y;
            internal set
            {
                MovementAxisSmoothed.y = value;
                // Debug.Log("UD" + value);
            }
        }


        /// <summary> Vertical (Y) Difference between Target and Current UpDown</summary>
        public float DeltaUpDown { get; internal set; }


        /// <summary> If true it will keep the Controller smooth push of the movement stick</summary>
        public bool UseSmoothVertical { get => SmoothVertical.Value; set => SmoothVertical.Value = value; }


        /// <summary> The current value of the Delta time the animal is using (Fixed or not)</summary>
        public float DeltaTime { get; private set; }

        #endregion

        #region Alignment Ground
        /// <summary>Smoothness value to Snap to ground </summary>
        public FloatReference AlignPosLerp = new FloatReference(15f);
        /// <summary>Smoothness Position value when Entering from Non Grounded States</summary>
        public FloatReference AlignPosDelta = new FloatReference(2.5f);
        /// <summary>Smoothness Rotation value when Entering from Non Grounded States</summary>
        public FloatReference AlignRotDelta = new FloatReference(2.5f);

        /// <summary>Smoothness Position value when Entering from Non Grounded States </summary>
        public float AlignPosLerpDelta { get; private set; }

        /// <summary>Smoothness Rotation value when Entering from non Grounded States </summary>
        public float AlignRotLerpDelta { get; private set; }


        /// <summary>Smoothness value to Snap to ground  </summary>
        public FloatReference AlignRotLerp = new FloatReference(15f);


        public IntReference AlignLoop = new IntReference(1);

        [Tooltip("Tag your small rocks, debris,steps and stair objects  with this Tag. It will help the animal to recognize better the Terrain")]
        public StringReference DebrisTag = new StringReference("Stair");

        /// <summary>Maximun angle on the terrain the animal can walk </summary>
        [Range(1f, 90f), Tooltip("Maximun angle on the terrain the animal can walk")]
        public float maxAngleSlope = 45f;
        [Tooltip("Additional Angle to calculate Down Slope Limits")]
        public float m_deepSlope = 5f;

        /// <summary>Main Pivot Slope Angle</summary>
        public float MainPivotSlope { get; private set; }

        /// <summary>Used to add extra Rotations to the Animal</summary>
        public Transform Rotator;
        public Transform RootBone;

        /// <summary>Check if can Fall on slope while on the ground "Decline Slope"</summary>
        public bool DeepSlope => TerrainSlope < NegativeMaxSlope;
        public float NegativeMaxSlope =>  -(maxAngleSlope + m_deepSlope);
        public float PositiveMaxSlope => maxAngleSlope;

        ///// <summary>Check if the Animal is on any Slope</summary>
        //public bool isinSlope => Mathf.Abs(TerrainSlope) > maxAngleSlope;

        /// <summary>Speed of the Animal used on the Rigid Body (Used on Custom Speed Modifiers)</summary>
        public float HorizontalSpeed { get; private set; }

        /// <summary>Velocity of the Animal used on the RIgid Body (Useful for Speed Modifiers)</summary>
        public Vector3 HorizontalVelocity { get; private set; }


        /// <summary>Calculation of the Average Surface Normal</summary>
        public Vector3 SurfaceNormal { get; internal set; }

        /// <summary>Calculate slope Angle and normalize it with the Max Angle Slope</summary>
        public float SlopeNormalized => TerrainSlope / maxAngleSlope; //Normalize the AngleSlop by the MAX Angle Slope and make it positive(HighHill) or negative(DownHill)

        /// <summary>Slope Calculate from the Surface Normal. Positive = Higher Slope, Negative = Lower Slope </summary>
        public float TerrainSlope { get; private set; }


        [SerializeField] private BoolReference grounded = new BoolReference(false);
        /// <summary> Is the Animal on a surface, when True the Raycasting for the Ground is Applied</summary>
        public bool Grounded
        {
            get => grounded.Value;
            set
            {
                if (grounded.Value != value)
                {
                    grounded.Value = value;

                    if (!value)
                    {
                        platform = null; //If groundes is false remove the stored Platform 
                    }
                    else
                    {
                        ResetGravityValues();
                        Force_Reset();

                        UpDownAdditive = 0; //Reset UpDown Additive 
                        UsingUpDownExternal = false; //Reset UpDown Additive 
                        GravityMultiplier = 1;
                        ExternalForceAirControl = true; //Reset the External Force Air Control
                    }

                    SetBoolParameter(hash_Grounded, grounded.Value);
                }
                OnGrounded.Invoke(value);
              //  Debug.Log("Grounded: " + value);
            }
        }
        #endregion

        #region External Force

        /// <summary>Add an External Force to the Animal</summary>
        public Vector3 ExternalForce { get; set; }

        /// <summary>Current External Force the animal current has</summary>
        public Vector3 CurrentExternalForce { get; set; }
        public bool LocalForce { get; set; }
        //{
        //    set
        //    {
        //        m_CurrentExternalForce = value;
        //        Debug.Log($"CurrentExternalForce:  {m_CurrentExternalForce} ");
        //    }
        //    get => m_CurrentExternalForce;
        //}
        //Vector3 m_CurrentExternalForce;

        /// <summary>External Force Aceleration /summary>
        public float ExternalForceAcel { get; set; }

        /// <summary>External Force Air Control, Can it be controlled while on the air?? </summary>
        public bool ExternalForceAirControl { get; set; }

        public bool HasExternalForce => ExternalForce != Vector3.zero;
        #endregion

        #region References
        /// <summary>Returns the Animator Component of the Animal </summary>

        [RequiredField] public Animator Anim;
        [RequiredField] public Rigidbody RB;                   //Reference for the RigidBody

        /// <summary>Transform.UP (Stored)</summary>
        public Vector3 Up => transform.up;
        /// <summary>Transform.Right (Stored)</summary>
        public Vector3 Right => transform.right;
        /// <summary>Transform.Forward (Stored) </summary>
        public Vector3 Forward => transform.forward;


        #endregion

        #region Modes
        /// <summary>Allows the Animal Start Playing a Mode</summary>
        public IntReference StartWithMode = new IntReference(0);

        /// <summary>Status value for the Mode (-1: Loop, -2: Interrupted, 0: Available</summary>
        public int ModeStatus { get; private set; }

        /// <summary>Float value for the Mode</summary>
        public float ModePower { get; set; }

        // private int modeStatus;
        private Mode activeMode;


        /// <summary>Is Playing a mode on the Animator</summary>
        public bool IsPlayingMode => activeMode != null;

        /// <summary>A mode is Set, but the Animation has not started yet</summary>
        public bool IsPreparingMode //{ get; set; }
        {
            get => m_IsPreparingMode;
            internal set
            {
                m_IsPreparingMode = value;
              // Debug.Log($"[{name}] - <color=orange><b>[☼☼☼ IsPreparingMode::{value}]</b></color>");
            }
        }
        bool m_IsPreparingMode;

        /// <summary>Store if the Animal is on a Zone</summary>
        public Zone InZone { get; internal set; }

        /// <summary>ID Value for the Last Mode Played </summary>
        public int LastModeID { get; internal set; }

        /// <summary>ID Value for the Last Ablity Played </summary>
        public int LastAbilityIndex { get; internal set; }


        /// <summary>Set/Get the Active Mode, Prepare the values for the Animator... Does not mean the Mode is Playing</summary>
        public Mode ActiveMode
        {
            get => activeMode;
            internal set
            {
                var lastMode = activeMode;
                activeMode = value;
                ModeTime = 0;

                if (value != null)
                {
                    OnModeStart.Invoke(ActiveModeID = value.ID, value.AbilityIndex);
                    ActiveState.OnModeStart(activeMode);
                }
                else
                {
                    ActiveModeID = 0;
                   
                    //Rember to reset the trigger on the Mode ON. Just in case
                    if (hash_ModeOn != 0) 
                        Anim.ResetTrigger(hash_ModeOn);
                }

                if (lastMode != null)
                {
                    LastModeID = lastMode.ID;
                    LastAbilityIndex = lastMode.AbilityIndex;
                    OnModeEnd.Invoke(lastMode.ID, LastAbilityIndex);
                    ActiveState.OnModeEnd(lastMode);
                    // Stance = Stance; //Updates the Stance Code ??
                }
                //   Debug.Log("IsPlayingMode = " + IsPlayingMode);
            }
        }

        /// <summary>Set the Values to the Animator to Enable a mode... Does not mean that the mode is enabled</summary>
        internal virtual void SetModeParameters(Mode value, int status)
        {
            if (value != null)
            {
                var ability = (value.ActiveAbility != null ? (int)value.ActiveAbility.Index : 0);

                int mode = Mathf.Abs(value.ID * 1000) + Mathf.Abs(ability);      //Convert it into a 4 int value Ex: Attack 1001

                //If the Mode is negative or the Ability is negative then Set the Animator Parameter negative too. (Right Left Abilities)
                ModeAbility = (value.ID < 0 || ability < 0) ? -mode : mode;


                TryAnimParameter(hash_ModeOn); //Activate the Optional Trigger
                if (hash_ModeOn != 0 && status != 0) //Only send the mode status when we are using Mode ON
                { 
                    SetModeStatus(status); 
                }
                else
                    SetModeStatus(status); //Normal way

                IsPreparingMode = true;
                ModeTime = 0;
            }
            else
            {
                SetModeStatus(Int_ID.Available);
                ModeAbility = 0;
            }
        }

        /// <summary>Current Mode ID and Ability Append Together (ModeID*100+AbilityIndex)</summary>
        public int ModeAbility
        {
            get => m_ModeIDAbility;
            internal set
            {
                //if (m_ModeIDAbility != value)
                {
                    m_ModeIDAbility = value;

                    if (debugModes)
                        Debug.Log($"[{name}] → <color=orange><b>Mode: [{m_ModeIDAbility}]</b></color>");

                    SetIntParameter.Invoke(hash_Mode, m_ModeIDAbility);
                }
            }
        }
        private int m_ModeIDAbility;
        /// <summary> Set the Parameter Int ID to a value and pass it also to the Animator </summary>
        public void SetModeStatus(int value)
        {
            if (debugModes)
                Debug.Log($"[{name}] → <color=orange><b>Mode Status: [{value}]</b></color>");

            SetIntParameter.Invoke(hash_ModeStatus, ModeStatus = value);
        }



        /// <summary>Current Animation Time of the Mode,used in combos</summary>
        public float ModeTime { get; internal set; }

        /// <summary>Active Mode ID</summary>
        public int ActiveModeID { get; private set; }

        public Mode Pin_Mode { get; private set; }

        #endregion

        #region Sleep
        [SerializeField] private BoolReference sleep = new BoolReference(false);

        /// <summary>Put the Controller to sleep, is like disalbling the script but internally</summary>
        public bool Sleep
        {
            get => sleep.Value;
            set
            {
                if (!value && Sleep) //Means is out of sleep
                {
                    //Set All Float values to their defaut (For all the Float Values on the Controller  while is not riding)
                    MTools.ResetFloatParameters(Anim);    
                    ResetController();
                }
                sleep.Value = value;

                //Debug.Log("Sleep" + Sleep);
                
                LockInput = LockMovement = value;                       //Also Set to sleep the Movement and Input
               
                if (Sleep)
                {
                    TryAnimParameter(hash_Random, 0);    //Set Random to 0
                                                                 //Reset FreeMovement.
                    if (Rotator) Rotator.localRotation = Quaternion.identity;
                    Bank = 0;
                    PitchAngle = 0;
                    PitchDirection = Vector3.forward;
                }
            }
        }
        #endregion 

        #region Strafe
        public BoolEvent OnStrafe = new BoolEvent();

        [SerializeField] private BoolReference m_strafe = new BoolReference(false);
        [SerializeField] private BoolReference m_CanStrafe = new BoolReference(false);
        [SerializeField] private BoolReference m_StrafeNormalize = new BoolReference(false);
        [SerializeField] private FloatReference m_StrafeLerp = new FloatReference(5f);


        public bool StrafeNormalize => m_StrafeNormalize.Value;

         
        /// <summary> Is the Animal on the Strafe Mode</summary>
        public bool Strafe
        {
            get => m_CanStrafe.Value && m_strafe.Value && ActiveState.CanStrafe;
            set
            {
                if (sleep) return;

                var newStrafe = value && m_CanStrafe.Value && ActiveState.CanStrafe;



                if (newStrafe != m_strafe.Value)
                {
                    m_strafe.Value = newStrafe;
                    StrafeLogic();
                }
            }
        }

        private void StrafeLogic()
        {
            if (debugStates) Debugging($"Strafe: [{m_strafe.Value}]");
            OnStrafe.Invoke(m_strafe.Value);
            TryAnimParameter(hash_Strafe, m_strafe.Value);

            if (ActiveState.StrafeAnimations)
               TryAnimParameter(hash_StateOn);                 // Check again that the Strafe is On!

            if (!JustActivateState) SetIntParameter(hash_LastState, ActiveStateID);   //Sent to the Animator the previews Active State  (BUG)

            if (!m_strafe.Value) //false
            {
                ResetCameraInput();
            }
            else
            {
                Aimer?.SetEnable(true); //Enable the Aimer just in case
            }
        }

        public bool CanStrafe { get => m_CanStrafe.Value; set => m_CanStrafe.Value = value; }

        private float StrafeDeltaValue;
        //private float HorizontalAimAngle_Raw;

        public Aim Aimer;


        #endregion

        #region Pivots

        internal RaycastHit hit_Hip;            //Hip and Chest Ray Cast Information
        internal RaycastHit hit_Chest;            //Hip and Chest Ray Cast Information

        public List<MPivots> pivots = new List<MPivots>
            { new MPivots("Hip", new Vector3(0,0.7f,-0.7f), 1), new MPivots("Chest", new Vector3(0,0.7f,0.7f), 1), new MPivots("Water", new Vector3(0,1,0), 0.05f) };

        public MPivots Pivot_Hip;
        public MPivots Pivot_Chest;

        public int AlignUniqueID { get; private set; }

        /// <summary>Does it have a Hip Pivot?</summary>
        public bool Has_Pivot_Hip;

        /// <summary>Does it have a Hip Pivot?</summary>
        public bool Has_Pivot_Chest;

        /// <summary> Do the Main (Hip Ray) found ground </summary>
        public bool MainRay { get; private set; }
        /// <summary> Do the Fron (Chest Ray) found ground </summary>
        public bool FrontRay { get; private set; }

        /// <summary>Main pivot Point is the Pivot Chest Position, if not the Pivot Hip Position one</summary>
        public Vector3 Main_Pivot_Point
        {
            get
            {
                Vector3 pivotPoint;
                if (Has_Pivot_Chest)
                {
                    pivotPoint = Pivot_Chest.World(transform);
                }
                else if (Has_Pivot_Hip)
                {
                    pivotPoint = Pivot_Hip.World(transform);
                }
                else
                {
                    pivotPoint = transform.TransformPoint(new Vector3(0, Height, 0));
                }

                return pivotPoint + DeltaVelocity;
            }
        }

        /// <summary> Delta Animal Velocity  </summary>
        public Vector3 DeltaVelocity { get; private set; }

        /// <summary> Does the Animal Had a Pivot Chest at the beggining?</summary>
        private bool Starting_PivotChest;

        /// <summary> Disable Temporally the Pivot Chest in case the animal is on 2 legs </summary>
        public void DisablePivotChest() => Has_Pivot_Chest = false;

        /// <summary> Used for when the Animal is on 2 feet instead of 4</summary>
        public void ResetPivotChest() => Has_Pivot_Chest = Starting_PivotChest;
        public void UsePivotChest(bool value) => Has_Pivot_Chest = value;


        /// <summary>Check if there's no Pivot Active </summary>
        public bool NoPivot => !Has_Pivot_Chest && !Has_Pivot_Hip;

        /// <summary> Gets the the Main Pivot Multiplier * Scale factor (Main Pivot is the Chest, if not then theHip Pivot) </summary>
        public float Pivot_Multiplier
        {
            get
            {
                float multiplier = Has_Pivot_Chest ? Pivot_Chest.multiplier : (Has_Pivot_Hip ? Pivot_Hip.multiplier : 1f);
                return multiplier * ScaleFactor * (NoPivot ? 1.5f : 1f);
            }
        }
        #endregion

        #region Speed Modifiers 



        /// <summary>What is the Rigid Body velocity the animal should have...</summary>
        public Vector3 DesiredRBVelocity { get; internal set; }

        /// <summary>True if the Current Speed is Locked</summary>
        public bool CurrentSpeedSetIsLocked => CurrentSpeedSet.LockSpeed;

        /// <summary>Speed Set for Stances</summary>
        public List<MSpeedSet> speedSets;
        /// <summary>Active Speed Set</summary>
        private MSpeedSet currentSpeedSet = new MSpeedSet();
        internal MSpeedSet defaultSpeedSet = new MSpeedSet()
        { name = "Default Set", Speeds = new List<MSpeed>(1) { new MSpeed("Default", 1, 4, 4) } }; //Create a Default Speed at Awake

        /// <summary>True if the State is modifing the current Speed Modifier</summary>
        public bool CustomSpeed;

        public MSpeed currentSpeedModifier = MSpeed.Default;
        internal MSpeed SprintSpeed = MSpeed.Default;
        //public List<MSpeed> speedModifiers = new List<MSpeed>();

        protected int speedIndex;

        /// <summary>What is the Speed modifier the Animal is current using (Walk? trot? Run?)</summary>
        public MSpeed CurrentSpeedModifier
        {
            get
            {
                if (CurrentSpeedSetIsLocked) return CurrentSpeedSet.LockedSpeedModifier; //Return the Locked
                if (Sprint && !CustomSpeed) return SprintSpeed;
                return currentSpeedModifier;
            }
            internal set
            {
                //Debug.Log("******value = " + value.name); 

                if (currentSpeedModifier.name != value.name)
                {
                    currentSpeedModifier = value;
                    OnSpeedChange.Invoke(CurrentSpeedModifier);
                    EnterSpeedEvent(CurrentSpeedIndex);
                    ActiveState?.SpeedModifierChanged(CurrentSpeedModifier, CurrentSpeedIndex);
                  //  Debug.Log("******CurrentSpeedModifier = " + currentSpeedModifier.name);
                }
            }
        }


        /// <summary>Current Speed Index used of the Current Speed Set E.G. (1 for Walk, 2 for trot)</summary>
        public int CurrentSpeedIndex
        {
            get => CurrentSpeedSet.LockSpeed ? CurrentSpeedSet.LockIndex : speedIndex; //Return the LockSpeed Index in case the speed is locked
            internal set
            {
                if (CustomSpeed || CurrentSpeedSet == null) return;

                var speedModifiers = CurrentSpeedSet.Speeds;

                var newValue = Mathf.Clamp(value, 1, speedModifiers.Count); //Clamp the Speed Index
                if (newValue > CurrentSpeedSet.TopIndex) newValue = CurrentSpeedSet.TopIndex;

                 newValue = Mathf.Clamp(value, 1, newValue); // TOP INDEX CANNOT BE SET OT ZERO
               

                if (speedIndex != newValue)
                {
                    speedIndex = newValue;

                    var sprintSpeed = Mathf.Clamp(CurrentSpeedSet.SprintIndex, 1, speedModifiers.Count);

                    CurrentSpeedModifier = speedModifiers[speedIndex - 1];

                    //Debug.Log("CurrentSpeedIndex: " + speedIndex);
                    SprintSpeed = speedModifiers[sprintSpeed - 1];

                    if (CurrentSpeedSet != null)
                        CurrentSpeedSet.CurrentIndex = speedIndex; //Keep the Speed saved on the state too in case the active speed was changed
                }
            }
        }

        /// <summary>Current Speed Set used on the Animal</summary>
        public MSpeedSet CurrentSpeedSet
        {
            get => currentSpeedSet;
            internal set
            {
                if (value.name != currentSpeedSet.name) //Calculate this only when the Set changes
                {
                    //Debug.Log("SpeedSet = " + currentSpeedSet.name);
                    //Debug.Log("currentSpeedSet = " + currentSpeedSet.CurrentIndex);
                    currentSpeedSet = value;
                    speedIndex = -1; //Reset the speed Index
                    JustChangedSpeedSet = true;
                    CurrentSpeedIndex = currentSpeedSet.CurrentIndex;
                    JustChangedSpeedSet = false;

                    EnterSpeedEvent(CurrentSpeedIndex);
                }
            }
        }

       bool JustChangedSpeedSet;

        private void EnterSpeedEvent(int index)
        {
            if (JustChangedSpeedSet) return;

            if (OnEnterExitSpeeds != null)
            {
                var SpeedEnterEvent = OnEnterExitSpeeds.Find(s => s.SpeedIndex == index && s.SpeedSet == CurrentSpeedSet.name);

                if (OldEnterExitSpeed != null && SpeedEnterEvent != OldEnterExitSpeed)
                {
                    OldEnterExitSpeed.OnExit.Invoke();
                    OldEnterExitSpeed = null;
                }


                if (SpeedEnterEvent != null)
                {
                    SpeedEnterEvent.OnEnter.Invoke();
                    OldEnterExitSpeed = SpeedEnterEvent;
                }
            }
        }

        private OnEnterExitSpeed OldEnterExitSpeed;

        /// <summary> Use Default SpeedSet 0 everything </summary>
        public void ResetSpeedSet() => CurrentSpeedSet = defaultSpeedSet;


        /// <summary> Value for the Speed  Global Multiplier Parameter on the Animator</summary>
        internal float SpeedMultiplier { get; set; }

        internal bool sprint;
        internal bool realSprint;

        /// <summary>Sprint Input</summary>
        public bool Sprint
        {
            get => UseSprintState && sprint && UseSprint && MovementDetected && !CurrentSpeedSetIsLocked;
            set
            {
                var newRealSprint = UseSprintState && value && UseSprint && MovementDetected && !CurrentSpeedSetIsLocked; //Check if the animal has movement

                //Debug.Log($"UseSprintState {UseSprintState} && value{value} && " +
                //    $"UseSprint{UseSprint} && MovementDetected{MovementDetected} && !SpeedChangeLocked {SpeedChangeLocked}");

                sprint = value; //Store only the Input value for the sprint I think it works

                if (realSprint != newRealSprint)
                {
                    realSprint = newRealSprint;

                    OnSprintEnabled.Invoke(realSprint);
                    TryAnimParameter(hash_Sprint, realSprint);        //Set on the Animator Sprint Value

                    int currentPI = CurrentSpeedIndex;
                    var speed = CurrentSpeedModifier;

                    if (realSprint)
                    {
                        speed = SprintSpeed;
                        currentPI++;
                    }

                    OnSpeedChange.Invoke(speed);       //Invoke the Speed again
                    EnterSpeedEvent(currentPI);

                    ActiveState?.SpeedModifierChanged(speed, currentPI);
                }
            }
        }

        public void SetSprint(bool value) => Sprint = value;

        /// <summary> Try State current Cycle </summary>
        internal int CurrentCycle { get; private set; }

        #endregion 

        #region Gravity
        [SerializeField] private Vector3Reference m_gravityDir = new Vector3Reference(Vector3.down);

        [SerializeField] private FloatReference m_gravityPower = new FloatReference(9.8f);

        [SerializeField] private IntReference m_gravityTime = new IntReference(10);
        [SerializeField] private IntReference m_gravityTimeLimit = new IntReference(0);


        public int StartGravityTime { get => m_gravityTime.Value ; internal set => m_gravityTime.Value = value; }
        public int LimitGravityTime { get => m_gravityTimeLimit.Value ; internal set => m_gravityTimeLimit.Value = value; }

        /// <summary>Multiplier Added to the  Gravity Direction</summary>
        public float GravityMultiplier { get; internal set; }


        public int GravityTime { get; internal set; }
        //{
        //    get => m_GravityTime;
        //    set
        //    {
        //        m_GravityTime = value;
        //        Debug.Log("m_GravityTime    " + m_GravityTime);
        //    }
        //}
        //int m_GravityTime;


        public float GravityPower { get => m_gravityPower.Value * (GravityMultiplier * ActiveState.GravityMultiplier); set => m_gravityPower.Value = value; }


        /// <summary>Stored Gravity Velocity when the animal is using Gravity</summary>
        public Vector3 GravityStoredVelocity { get; internal set; }

        /// <summary> Direction of the Gravity </summary>
        public Vector3 Gravity { get => m_gravityDir.Value; set => m_gravityDir.Value = value; }

        /// <summary> Up Vector is the Opposite direction of the Gravity dir</summary>
        public Vector3 UpVector => -m_gravityDir.Value;

        /// <summary>if True the gravity will be the Negative Ground Normal Value</summary>
        public BoolReference ground_Changes_Gravity =  new BoolReference(false);

        #endregion

        #region Advanced Parameters

        [Range(0,180),Tooltip("Slow the Animal when the Turn Angle is ouside this limit")]
        public float TurnLimit = 120;

        public BoolReference rootMotion = new BoolReference(true);

        /// <summary> Raudius for the Sphere Cast</summary>
        public FloatReference rayCastRadius = new FloatReference(0.05f);

        /// <summary>RayCast Radius for the Alignment Raycasting</summary>
        public float RayCastRadius => rayCastRadius.Value + 0.001f;
        /// <summary>This parameter exist to Add Additive pose to correct the animal</summary>
        public IntReference animalType = new IntReference(0);
        #endregion

        #region Use Stuff Properties  
        /// <summary>Does the Active State uses Additive Position Speed?</summary>
        public bool UseAdditivePos// { get; internal set; } 
        {
            get => useAdditivePos; 
            set
            {
                useAdditivePos = value;
               if (!useAdditivePos)  ResetInertiaSpeed(); 
            }
        }
        private bool useAdditivePos;

        /// <summary>Does the Active State uses Additive Position Speed?</summary>
        public bool UseAdditiveRot { get; internal set; }

        /// <summary>Does the Active State uses Sprint?</summary>
        public bool UseSprintState { get; internal set; }

        /// <summary>Custom Alignment done by some States.. (E.g. Swim) </summary>
        public bool UseCustomAlign { get; set; }
        /// <summary>The Animal is on Free Movement... which means is flying or swiming underwater</summary>
        public bool FreeMovement { get; set; }
        /// <summary>Enable Disable the Global Sprint</summary>
        public bool UseSprint
        {
            get => useSprintGlobal; 
            set
            { 
                useSprintGlobal.Value = value;
                Sprint = sprint; //Update the Sprint value  IMPORTANT
            }
        }
        /// <summary>Enable Disable the Global Sprint (SAME AS USE SPRINT)</summary>
        public bool CanSprint { get => UseSprint; set => UseSprint = value; }

        /// <summary>Locks Input on the Animal, Ingore inputs like Jumps, Attacks , Actions etc</summary>
        public bool LockInput
        {
            get => lockInput.Value;
            set
            {
                lockInput.Value = value;
                OnInputLocked.Invoke(lockInput);
            }
        }

        /// <summary>Enable/Disable RootMotion on the Animator</summary>
        public bool RootMotion
        {
            get => rootMotion;
            set => Anim.applyRootMotion = rootMotion.Value = value;
        }

        /// <summary>  This store the DeltaRootMotion everytime its Deactivated/Activated  </summary>
        public Vector3 DeltaRootMotion { get; set; }

        private bool useGravity;
        /// <summary>Does it use Gravity or not? </summary>
        public bool UseGravity
        {
            get => useGravity;
            set
            {
                useGravity = value;

                if (!useGravity) ResetGravityValues();//Reset Gravity Logic when Use gravity is false
              //  Debug.Log("useGravity = " + useGravity);
            }
        }

        /// <summary>Locks the Movement on the Animal</summary>
        public bool LockMovement
        {
            get => lockMovement;
            set
            {
                lockMovement.Value = value;
                OnMovementLocked.Invoke(lockMovement);
            }
        }


        /// <summary>Sets to Zero the Z on the Movement Axis when this is set to true</summary>
        public bool LockForwardMovement
        {
            get => lockForwardMovement;
            set
            {
                lockForwardMovement.Value = value;
                LockMovementAxis.z = value ? 0 : 1;
            }
        }

        /// <summary>Sets to Zero the X on the Movement Axis when this is set to true</summary>
        public bool LockHorizontalMovement
        {
            get => lockHorizontalMovement; 
            set
            {
                lockHorizontalMovement.Value = value;
                LockMovementAxis.x = value ? 0 : 1;
            }
        }

        /// <summary>Sets to Zero the Y on the Movement Axis when this is set to true</summary>
        public bool LockUpDownMovement
        {
            get => lockUpDownMovement;
            set
            {
                lockUpDownMovement.Value = value;
                LockMovementAxis.y = value ? 0 : 1;
            }
        }

        private Vector3 LockMovementAxis;
        private bool useOrientToGround;

        /// <summary>if True It will Aling it to the ground rotation depending the Front and Back Pivots</summary>
        public bool UseOrientToGround 
        { 
            get => useOrientToGround && m_OrientToGround.Value;
            set => useOrientToGround = value;
        }
        
        public bool GlobalOrientToGround
        {
            get => m_OrientToGround.Value;
            set
            {
                m_OrientToGround.Value = value;
                Has_Pivot_Chest = value ? Pivot_Chest != null : false; //Hide the Pivor Chest
            }
        }

        [SerializeField, Tooltip("Global Orient to ground. Disable This for Humanoids")]
        private BoolReference m_OrientToGround = new BoolReference(true);


        [SerializeField,Tooltip("Locks Input on the Animal, Ignore inputs like Jumps, Attacks, Actions etc")]
        private BoolReference lockInput = new BoolReference(false);
        
        [SerializeField,Tooltip("Locks the Movement entries on the animal. (Horizontal, Vertical,Up Down)")] 
        private BoolReference lockMovement = new BoolReference(false);
        
        [SerializeField] 
        private BoolReference useSprintGlobal = new BoolReference(true);
        #endregion

        #region Animator States Info
        internal AnimatorStateInfo m_CurrentState;             // Information about the base layer of the animator cached.
        internal AnimatorStateInfo m_NextState;
        internal AnimatorStateInfo m_PreviousCurrentState;    // Information about the base layer of the animator from last frame.
        internal AnimatorStateInfo m_PreviousNextState;

        /// <summary> If we are  in any  animator transition </summary>
        internal bool m_IsAnimatorTransitioning;
      //  internal bool FirstAnimatorTransition;
        protected bool m_PreviousIsAnimatorTransitioning;


        /// <summary>Returns the Current Animation State Tag of animal, if is in transition it will return the NextState Tag</summary>
        public AnimatorStateInfo AnimState { get; set; }

        public int currentAnimTag;
        /// <summary>Current Active Animation Hash Tag </summary>
        public int AnimStateTag
        {
            get => currentAnimTag;
            private set
            {
                if (value != currentAnimTag)
                {
                    currentAnimTag = value;
                    activeState.AnimationTagEnter(value);

                        //If the new Animation Tag is not on the New Active State try to activate it on the last State
                    if (ActiveState.IsPending)                      
                        LastState.AnimationTagEnter(value);
                    //Debug.Log("value = " + value);
                }
            }
        }
        #endregion

        #region Platform
        public Transform platform;
        protected Vector3 platform_LastPos;
        protected Quaternion platform_Rot;
        #endregion  

        #region Extras
        /// <summary>Used for Disabling Additive Position and Additive Rotation on the ANimal (The Pulling Wagons on the Horse Car  take care of it)</summary>?????
        internal bool DisablePositionRotation = false;
        ///// <summary>Used for Disabling Additive Position and Additive Rotation on the ANimal (The Pulling Wagons on the Horse Car  take care of it)</summary>?????
        //internal bool DisableRotation = false;

        //[Tooltip("When Falling and the animal get stuck falling, the animal will be force to move forward.")]
        //public FloatReference FallForward = new FloatReference(2);

        protected List<IMDamager> Attack_Triggers;      //List of all the Damage Triggers on this Animal.


        /// <summary>All Colliders Inside the Animals> summary>
        public List<Collider> colliders = new List<Collider>();

        /// <summary>Animator Normalized State Time for the Base Layer  </summary>
        public float StateTime { get; private set; }

        ///// <summary>Store from where the damage came from</summary>
        //public Vector3 HitDirection { set; get; }

        #endregion

        #region Events
        public IntEvent OnAnimationChange;
        public UnityEvent OnMaxSlopeReached = new UnityEvent();
        private bool HasReachedMaxSlope;
        public BoolEvent OnInputLocked = new BoolEvent();          //Used for Sync Animators
        public BoolEvent OnMovementLocked = new BoolEvent();        //Used for Sync Animators
        public BoolEvent OnSprintEnabled = new BoolEvent();       //Used for Sync Animators
        public BoolEvent OnGrounded = new BoolEvent();       //Used for Sync Animators
        public BoolEvent OnMovementDetected = new BoolEvent();       //Used for Sync Animators

        /// <summary> Invoked when a new State is Activated</summary>
        public IntEvent OnStateActivate = new IntEvent();      
        /// <summary> Invoked when a new State has entered any of its Animations</summary>
        public IntEvent OnStateChange = new IntEvent();         
        /// <summary> Invoked when a new Mode start</summary>
        public Int2Event OnModeStart = new Int2Event();          
        /// <summary> Invoked when a new Mode ends</summary>
        public Int2Event OnModeEnd = new Int2Event();           
        /// <summary> Invoked when a new Stance is Activated</summary>
        public IntEvent OnStanceChange = new IntEvent();        //Invoked when is Changed to a new Stance
        public SpeedModifierEvent OnSpeedChange = new SpeedModifierEvent();        //Invoked when a new Speed is changed
        public Vector3Event OnTeleport = new  Vector3Event();        //Invoked when a new Speed is changed


        ///<summary>List of Events to Use on the States</summary>
        public List<OnEnterExitState> OnEnterExitStates;
        ///<summary>List of Events to Use on the Stances</summary>
        public List<OnEnterExitStance> OnEnterExitStances;
        ///<summary>List of Events to Use on the Speeds</summary>
        public List<OnEnterExitSpeed> OnEnterExitSpeeds;
        #endregion

        #region Random
        public int RandomID { get; private set; }
        public int RandomPriority { get; private set; }

        /// <summary>Let States have Random Animations</summary>
        public bool Randomizer { get; set; }
        #endregion 

        #region Animator Parameters

        [SerializeField, Tooltip("Forward (Z) Movement for the Animator")] private string m_Vertical = "Vertical";
        [SerializeField, Tooltip("Horizontal (X) Movement for the Animator")] private string m_Horizontal = "Horizontal";
        [SerializeField, Tooltip("Vertical (Y) Movement for the Animator")] private string m_UpDown = "UpDown";
        [SerializeField, Tooltip("Vertical (Y) Difference between Target and Current UpDown")] private string m_DeltaUpDown = "DeltaUpDown";

        [SerializeField, Tooltip("Is the animal on the Ground? ")] private string m_Grounded = "Grounded";
        [SerializeField, Tooltip("Is the animal moving?")] private string m_Movement = "Movement";

        [SerializeField, Tooltip("Active/Current State the animal is")]
        private string m_State = "State";

        [SerializeField, Tooltip("Trigger to Notify the Activation of a State")]
        private string m_StateOn = "StateOn";

        [SerializeField, Tooltip("Trigger to Notify the Activation of a Mode")]
        private string m_ModeOn = "ModeOn";

        [SerializeField, Tooltip("The Active State can have multiple status to change inside the State itself")]
        private string m_StateStatus = "StateEnterStatus";
        [SerializeField, Tooltip("The Active State can use this parameter to activate exiting animations")]
        private string m_StateExitStatus = "StateExitStatus";
        [SerializeField, Tooltip("Float value for the States to be used when needed")]
        private string m_StateFloat = "StateFloat";
        [SerializeField, Tooltip("Last State the animal was")]
        private string m_LastState = "LastState";

        [SerializeField, Tooltip("Active State Time for the States Animations")]
        private string m_StateTime = "StateTime";

        [SerializeField, Tooltip("Speed Multiplier for the Animations")]
        private string m_SpeedMultiplier = "SpeedMultiplier";

        [SerializeField, Tooltip("Active Mode the animal is... The Value is the Mode ID plus the Ability Index. Example Action Eat = 4002")]
        private string m_Mode = "Mode";

        [SerializeField, Tooltip("Store the Modes Status (Available=0  Started=1  Looping=-1 Interrupted=-2)")]
        private string m_ModeStatus = "ModeStatus";
        [SerializeField, Tooltip("Mode Float Value, Used to have a float Value for the modes to be used when needed")]
        private string m_ModePower = "ModePower";

        [SerializeField, Tooltip("Sprint Value")]
        private string m_Sprint = "Sprint";

        [SerializeField, Tooltip("Active/Current stance of the animal")] private string m_Stance = "Stance";
        [SerializeField, Tooltip("Previus/Last stance of the animal")] private string m_LastStance = "LastStance";
        [SerializeField, Tooltip("Normalized value of the Slope of the Terrain")] private string m_Slope = "Slope";
        [SerializeField, Tooltip("Type of animal for the Additive corrective pose")] private string m_Type = "Type";

        [SerializeField, Tooltip("Random Value for Animations States with multiple animations")] private string m_Random = "Random";
        [SerializeField, Tooltip("Target Angle calculated from the current forward  direction to the desired direction")] private string m_DeltaAngle = "DeltaAngle";
        [SerializeField, Tooltip("Does the Animal Uses Strafe")] private string m_Strafe = "Strafe";
       
        //[SerializeField, Tooltip("Horizontal Angle For the Target or Camera. Old [strafeAngle]")]
        //    [FormerlySerializedAs("m_strafeAngle")] 
        //private string m_TargetHorizontal = "TargetHorizontal";

        internal int hash_Vertical;
        internal int hash_Horizontal;
        internal int hash_UpDown;

        internal int hash_DeltaUpDown;

        internal int hash_Movement;
        internal int hash_Grounded;
        internal int hash_SpeedMultiplier;

        internal int hash_DeltaAngle;

        internal int hash_State;
        internal int hash_StateOn;
        internal int hash_StateEnterStatus;
        internal int hash_StateExitStatus;
        internal int hash_StateFloat;
        internal int hash_StateTime;
        internal int hash_LastState;

        internal int hash_Mode;
        internal int hash_ModeOn;
        internal int hash_ModeStatus;
        internal int hash_ModePower;

        internal int hash_Stance;
        //internal int hash_StanceOn;
        internal int hash_LastStance;

        internal int hash_Slope;
        internal int hash_Sprint;
        internal int hash_Random;
        internal int hash_Strafe;
      //  internal int hash_TargetHorizontal;
        #endregion
    }


    [System.Serializable]
    public class OnEnterExitSpeed
    {
        [Tooltip("Which is the Speed Set (By its Name) changed. Case Sensitive")]
        public string SpeedSet;
        [Tooltip("Which is the Speed Modifier (By its Name) changed. This is Ignored if is set to 1. Case Sensitive")]
        public int SpeedIndex;
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
    }


    [System.Serializable]
    public class OnEnterExitState
    {
        public StateID ID;
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
    }

    [System.Serializable]
    public class OnEnterExitStance
    {
        public StanceID ID;
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
    }

    [System.Serializable]
    public class SpeedModifierEvent : UnityEvent<MSpeed> { }
}
