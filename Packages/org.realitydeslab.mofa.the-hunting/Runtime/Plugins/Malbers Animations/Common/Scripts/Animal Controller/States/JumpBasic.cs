using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.Controller
{
    public class JumpBasic : State
    {
        public override string StateName => "Jump/Basic Jump";

        private int JumpEndHash = Animator.StringToHash("JumpEnd");
        private int JumpStartHash = Animator.StringToHash("JumpStart");
        //  private int DoubleJumpHash = Animator.StringToHash("DoubleJump");


        [Header("Jump Parameters")]
        [Tooltip("Can the Animal be Rotated while Jumping?")]
        public BoolReference AirControl = new BoolReference(true);
        [Tooltip("How much Rotation the Animal can do while Jumping")]
        public FloatReference AirRotation = new FloatReference(10);
        [Tooltip("States that will Reset the Jump Count. By Default Idle and Locomotion Reset the value")]
        public FloatReference AirMovement = new FloatReference(5);
        [Tooltip("Smooth Value for Changing Speed Movement on the Air")]
        public FloatReference AirSmooth = new FloatReference(5);

        [Space, Tooltip("Wait for the Animation to Activate the Jump Logic\n Use [void ActivateJump()] on the Animator with a Messsage Behavior")]
        public bool WaitForAnimation;

        [Tooltip("Amount of jumps the Animal can do (Double and Triple Jumps)\n**IMPORTANT**\nWhen using multiple Jumps,Fall cannot be on the Sleep From State List")]
        public IntReference Jumps = new IntReference(1);
        //[Tooltip("For Multiple Jumps, time needed to activate the next jump logic")]
        //public FloatReference JumpTime = new FloatReference(0.3f);

        /// <summary>If the Jump input is pressed, the Animal will keep going Up while the Jump Animation is Playing</summary>
        [Space, Tooltip("If the Jump input is pressed, the Animal will keep going Up while the Jump Animation is Playing")]
        public BoolReference JumpPressed = new BoolReference(false);
        [Tooltip("Lerp Value for Pressing Jump")]
        public FloatReference JumpPressedLerp = new FloatReference(5);



        [Tooltip("The Jump can be interrupted if a ground is found in the middle of the jump. This is the multiplier to cast the Ray using the Animal Height.")]
        // [Range(0,99)]
        public FloatReference JumpInterruptRay = new FloatReference(0.9f);

        [Space, Tooltip("How much Movement the Animal can do while Jumping")]
        public List<StateID> ResetJump;

//#if UNITY_2020_3_OR_NEWER
//        [NonReorderable]
//#endif
        public List<JumpBasicProfile> profiles = new List<JumpBasicProfile>();
//#if UNITY_2020_3_OR_NEWER
//        [NonReorderable]
//#endif
        public List<JumpDoubleProfile> multipleJumps = new List<JumpDoubleProfile>();


        private JumpBasicProfile activeJump;
        protected MSpeed JumpSpeed;

        /// <summary>  This allows the Jump Logic to occur, its activated by the  </summary>
        private bool IsDoubleJump;

        private bool ActivateJumpLogic;
        //{
        //    get => activateJumpLogic;
        //    set
        //    {
        //        activateJumpLogic = value;
        //       //Debug.Log("activateJumpLogic = " + activateJumpLogic);
        //    }
        //}
        //private bool activateJumpLogic;

        public int JumpsPerformanced //{ get; set; }
        {
            get => jumpsPerformanced;
            set
            {
                jumpsPerformanced = value;
                //  Debug.Log("jumpsPerformanced = " + jumpsPerformanced);
            }
        }
        private int jumpsPerformanced;
        //private int GravityTime = 0;
        /// <summary>  Used on the Pressed feature so it cannot be pressed again on the middle </summary>
        private bool justJumpPressed;

        private float StartedJumpLogicTime;
        private float JumpPressHeight_Value = 1;


        public override void AwakeState()
        {
            base.AwakeState();
            if (EnterTagHash != 0) JumpStartHash = EnterTagHash;
            if (ExitTagHash != 0) JumpEndHash = ExitTagHash;
        }


        /// <summary> Restore Internal values on the State (DO NOT INLCUDE Animal Methods or Parameter calls here</summary>
        public override void ResetStateValues()
        {
            JumpPressHeight_Value = 1;
            ActivateJumpLogic = false;
            justJumpPressed = false;
            StartedJumpLogicTime = 0;
            IsDoubleJump = false;
            //Debugging("Reset Jump Values");
        }


        //Do not use the Try Activate
        public override bool TryActivate() => false;
        //public override bool TryActivate() => InputValue && (JumpsPerformanced < Jumps);


        public override void StatebyInput()
        {
            if (InputValue) Activate();
        }


        public void ActivateJump()
        {
            ActivateJumpLogic = true;
            justJumpPressed = true;
            animal.Grounded = false;
            StartedJumpLogicTime = 0;
            animal.GravityTime = activeJump.StartGravityTime;
            StartingSpeedDirection = animal.Forward;            //Store the Starting SpeedDirection
            Debugging("[Basic Jump] Activate JumpLogic");
        }

        public override void Activate()
        {
            if (JumpsPerformanced < Jumps)
            {
                base.Activate();
                animal.State_SetFloat(0);

                JumpsPerformanced++;
                // Debug.Log("JumpsPerformanced = " + JumpsPerformanced);

                General.Gravity = false;
                IsPersistent = true;                                //IMPORTANT!!!!! DO NOT ELIMINATE!!!!! 

                animal.currentSpeedModifier.animator = 1;
                animal.ResetGravityValues();                        //Reset the Gravity
                StartingSpeedDirection = animal.Forward;            //Store the Starting SpeedDirection
                FindJumpProfile();
            }
        }

        private void FindJumpProfile()
        {
            activeJump = (profiles != null && profiles.Count > 0) ? profiles[0] : new JumpBasicProfile() { };

            foreach (var j in profiles)                          //Save/Search the Current Jump Profile by the Lowest Speed available
            {
                if (j.LastState == null)
                {
                    if (j.VerticalSpeed <= animal.VerticalSmooth) activeJump = j; //Find the Closest 
                }
                else
                {
                    if (j.VerticalSpeed <= animal.VerticalSmooth &&
                        j.LastState == animal.LastState.ID)
                    {
                        activeJump = j;
                    }
                }
            }


            var JumpNumber = 0;

            //Search again if the Character can perform multiple jumps
            if (Jumps > 1 && JumpsPerformanced > 1 && multipleJumps != null && multipleJumps.Count > 0)
            {
                var PosibleJump = new JumpDoubleProfile() { JumpNumber = -1 };

                foreach (var j in multipleJumps)                          //Save/Search the Current Jump Profile by the Lowest Speed available
                {
                    if (JumpsPerformanced == j.JumpNumber) PosibleJump = j;
                }

                if (PosibleJump.JumpNumber != -1) //Means that  it found a Multiple Jump
                {
                    activeJump.name = PosibleJump.name;
                    activeJump.GravityPower = PosibleJump.GravityPower;
                    activeJump.Height = PosibleJump.Height;
                    activeJump.JumpTime = PosibleJump.JumpTime;
                    activeJump.StartGravityTime = PosibleJump.StartGravityTime;
                    JumpNumber = PosibleJump.JumpNumber;
                    IsDoubleJump = true;
                }
            }
            SetEnterStatus(JumpNumber);
        }

        public override void EnterCoreAnimation()
        {
            Debugging($"Jump Profile: [{activeJump.name}] Jumps Performanced:[{JumpsPerformanced}]");
            JumpPressHeight_Value = 1;

            var Speed = animal.HorizontalSpeed/ScaleFactor;

           // var passInertia = true;

            if (animal.HasExternalForce)
            {
                var HorizontalForce = Vector3.ProjectOnPlane(animal.ExternalForce, animal.UpVector); //Remove Horizontal Force
                var HorizontalInertia = Vector3.ProjectOnPlane(animal.Inertia, animal.UpVector); //Remove Horizontal Force

                var HorizontalSpeed = HorizontalInertia - HorizontalForce;
                Speed = HorizontalSpeed.magnitude;
               // passInertia = false;
            }


            if (!animal.ExternalForceAirControl)
            {
                Speed = 0; //Remove all Speed if the External Force does not allows it
            }

            JumpSpeed = new MSpeed(animal.CurrentSpeedModifier) //Inherit the Vertical and the Lerps
            {
                name = "Jump Basic Speed",
                position = General.RootMotion ? 0 : Speed, //Inherit the Horizontal Speed you have from the last state
                strafeSpeed = General.RootMotion ? 0 : Speed,
                lerpPosition = AirSmooth.Value,
                lerpStrafe = AirSmooth.Value,
                rotation = AirRotation.Value,
                animator = 1,
            };

            animal.SetCustomSpeed(JumpSpeed, false);                      //Set the Current Speed to the Jump Speed Modifier

            if (!WaitForAnimation) ActivateJump();                              //if it does not require to Wait for the Animator to call

            if (IsDoubleJump) ActivateJump();     //Mean is doing a double jump!
        }

        public override Vector3 Speed_Direction()
        {
            return AirControl.Value ? base.Speed_Direction() : StartingSpeedDirection;
        }

        private Vector3 StartingSpeedDirection;


        public override void EnterTagAnimation()
        {
            if (CurrentAnimTag == JumpStartHash && !animal.RootMotion)
            {
                var speed = animal.HorizontalSpeed / ScaleFactor;

                var JumpStartSpeed = new MSpeed(animal.CurrentSpeedModifier)
                {
                    name = "JumpStartSpeed",
                    position = speed,
                    Vertical = animal.CurrentSpeedModifier.Vertical,
                    animator = 1,
                    rotation = AirControl.Value ? (!animal.UseCameraInput ? AirRotation.Value : AirRotation.Value / 10f) : 0f,
                    strafeSpeed = speed,
                    lerpStrafe = AirSmooth
                };

                Debugging("[EnterTag-JumpStart]");
                animal.SetCustomSpeed(JumpStartSpeed, false);       //Set the Current Speed to the Jump Speed Modifier

                if (animal.TerrainSlope > 0) animal.UseCustomAlign = true; //Means we are jumping uphill

            }
            else if (CurrentAnimTag == JumpEndHash)
            {
                Debugging("[EnterTag-JumpEnd]");
                AllowExit();
            }
        }


        public override void OnStateMove(float deltaTime)
        {
            if (InCoreAnimation && ActivateJumpLogic)
            {
                if (JumpPressed.Value)
                {
                    if (!InputValue) justJumpPressed = false;

                    JumpPressHeight_Value = Mathf.Lerp(JumpPressHeight_Value, (InputValue && justJumpPressed) ? 1 : 0, deltaTime * JumpPressedLerp);
                }

                Vector3 ExtraJumpHeight = (UpVector * activeJump.Height.Value);
                animal.AdditivePosition += ExtraJumpHeight * deltaTime * JumpPressHeight_Value * ScaleFactor;     //Up Movement


                if (activeJump.ForwardPush > 0) animal.AdditivePosition += Forward * activeJump.ForwardPush * deltaTime * ScaleFactor;     //Forward Movement



                if (AirMovement > CurrentSpeedPos && AirControl)
                {
                    if (!animal.ExternalForceAirControl) return;
                    CurrentSpeedPos = Mathf.Lerp(CurrentSpeedPos, AirMovement, (AirSmooth != 0 ? (deltaTime * AirSmooth) : 1));
                }

                StartedJumpLogicTime += deltaTime;

                //Apply Fake Gravity (HAD TO TO IT)

                var GTime = deltaTime * animal.GravityTime;
                var GravityStoredVelocity = Gravity * ScaleFactor * animal.GravityPower * (GTime * GTime / 2) * animal.TimeMultiplier;
                
                //Add Gravity if is in use
                animal.AdditivePosition += 
                    GravityStoredVelocity * deltaTime * activeJump.GravityPower.Value;
                
                animal.GravityTime++;
            }
        }

        public override void TryExitState(float deltaTime)
        {
            if (!ActivateJumpLogic) return; //The Jump logic has not being activated yet

            Debug.DrawRay(animal.Main_Pivot_Point, Gravity * Height * JumpInterruptRay, Color.black);

            if (Physics.Raycast(animal.Main_Pivot_Point, Gravity, out _, Height * JumpInterruptRay, GroundLayer, IgnoreTrigger))
            {
                Debugging("[Allow Exit] - Interrupt Ray Touched Ground");
                AllowExit();
            }
            else if (StartedJumpLogicTime >= activeJump.JumpTime)
            {
                AllowExit();

                var lastGravityTime = animal.GravityTime;
                animal.State_Activate(StateEnum.Fall); //Seems Important
                animal.GravityTime = lastGravityTime;

                Debugging("[Allow Exit]");
            }
        }

        /// <summary>Is called when a new State enters</summary>
        public override void NewActiveState(StateID newState)
        {
            if (newState.ID == ID) return; //Do nothing if this we are entering the same state

            //Reset all the jumps (Idle and Locomotion) and all the extra States required
            if (newState <= 1 || ResetJump.Contains(newState))
            {
                JumpsPerformanced = 0;          //Reset the amount of jumps performanced
            }
                //If we were not jumping then increase the Double Jump factor when falling from locomotion
            else if (newState == StateEnum.Fall && animal.LastState.ID != ID) 
            {
                JumpsPerformanced++; //If we are in fall animation then increase a Jump perfomanced
            }
        }


#if UNITY_EDITOR

        public override void SetSpeedSets(MAnimal animal)
        {
            //Do nothing... the Fall is an automatic State, the Fall Speed is created internally
        }

        internal void Reset()
        {
            ID = MTools.GetInstance<StateID>("Jump");
            Input = "Jump";

            SleepFromState = new List<StateID>() { /*MTools.GetInstance<StateID>("Fall"),*/ MTools.GetInstance<StateID>("Fly") };
            SleepFromMode = new List<ModeID>() { MTools.GetInstance<ModeID>("Action"), MTools.GetInstance<ModeID>("Attack1") };

            EnterTag.Value = "JumpStart";
            ExitTag.Value = "JumpEnd";

            General = new AnimalModifier()
            {
                RootMotion = false,
                Grounded = false,
                Sprint = false,
                OrientToGround = false,
                CustomRotation = false,
                IgnoreLowerStates = true, //IMPORTANT!
                Persistent = true,
                AdditivePosition = true,
                AdditiveRotation = true,
                Gravity = false,
                modify = (modifier)(-1),
            };

            ExitFrame = false;

            profiles = new List<JumpBasicProfile>(1) { new JumpBasicProfile()
            {
                Height = new FloatReference(6),
                GravityPower = new FloatReference(1),
                JumpTime = 0.3f,
                VerticalSpeed = 0,
                name = "Default",
                StartGravityTime = 15,
                LastState = null,
            }
            };
    
        }
#endif
    }


    /// <summary>Different Jump parameters on different speeds</summary>
    [System.Serializable]
    public struct JumpBasicProfile
    {
        //[Tooltip("Name to identify the Jump Profile")]
        public string name;

      //  [Header("Conditions")]
      //  [Tooltip("Last State the animal was before making the Jump")]
        public StateID LastState;

      //  [Tooltip("Minimal Vertical Speed to Activate this Profile")]
        public float VerticalSpeed;
        
        public float ForwardPush;

      //  [Header("Values")]
      //  [Tooltip("Duration of the Jump logic")]
        public float JumpTime;

      //  [Tooltip("How High the animal can Jump")]
        public FloatReference Height;

      //  [Tooltip("Multiplier for the Gravity")]
        public FloatReference GravityPower;

     //   [Tooltip("Higher value makes the Jump more Arcady")]
        public int StartGravityTime;
    }


    /// <summary>Different Jump parameters for Multiple Jumps</summary>
    [System.Serializable]
    public struct JumpDoubleProfile
    {
        [Tooltip("Name to identify the Jump Profile")]
        public string name;

        [Tooltip("Multiple Jump Number (Is it a Double or a Triple Jump. Default is 0. This is the Value for the [Enter State Status]")]
        public int JumpNumber;

        [Header("Values")]
        [Tooltip("Duration of the Jump logic")]
        public float JumpTime;

        [Tooltip("How High the animal can Jump")]
        public FloatReference Height;

        [Tooltip("Multiplier for the Gravity")]
        public FloatReference GravityPower;

        [Tooltip("Higher value makes the Jump more Arcady")]
        public int StartGravityTime; 
    }
}
