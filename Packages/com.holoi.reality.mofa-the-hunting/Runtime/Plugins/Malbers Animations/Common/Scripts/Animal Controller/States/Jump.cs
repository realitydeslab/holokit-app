 
using System.Collections.Generic;
using UnityEngine; 
using MalbersAnimations.Scriptables; 

namespace MalbersAnimations.Controller
{
    public class Jump : State
    {
        public override string StateName => "Jump/Root Motion Jump";
        

        /// <summary>If the Jump input is pressed, the Animal will keep going Up while the Jump Animation is Playing</summary>
        [Header("Jump Parameters")]
        [Tooltip("If the Jump input is pressed, the Animal will keep going Up while the Jump Animation is Playing")]
        public BoolReference JumpPressed;
        [Tooltip("Check if the Animal can control the Forward Movement of the Jump")]
        public BoolReference ForwardPressed;

        /// <summary>If the Forward input is pressed, the Animal will keep going Forward while the Jump Animation is Playing</summary>
        //[Tooltip("If the Forward input is pressed, the Animal will keep going Forward while the Jump Animation is Playing")]
        //public bool JumpForwardPressed;
        public float JumpPressedLerp = 5;
        private float JumpPressHeight_Value = 1;
        private float JumpPressForward = 1;
        private float JumpPressForwardAdditive = 0;

        //private float JumpPressForward_Value = 1;
        public BoolReference AirControl = new BoolReference(true);
        //private float JumpPressForward_Value = 1;
        public FloatReference AirRotation = new FloatReference(10);

#if UNITY_2020_3_OR_NEWER
        [NonReorderable]
#endif
        public List<JumpProfile> jumpProfiles = new List<JumpProfile>();
        protected MSpeed JumpSpeed;

        protected bool OneCastingFall_Ray = false;

        /// <summary> Current Jump Profile</summary>
        protected JumpProfile activeJump;
        private RaycastHit JumpRay;

        private bool CanJumpAgain;
        private Vector3 JumpStartDirection;

        public override bool TryActivate() => InputValue && CanJumpAgain;

        public override void ResetStateValues()
        {
            CanJumpAgain = true;
            JumpPressHeight_Value = 1;
            JumpPressForward = 1;
            JumpPressForwardAdditive = 0;
            OneCastingFall_Ray = false;
           // GoingDown = false;
        }

        public override void AwakeState()
        {
            if (string.IsNullOrEmpty(EnterTag)) EnterTag.Value = "JumpStart";
            if (string.IsNullOrEmpty(ExitTag)) ExitTag.Value = "JumpEnd";

            base.AwakeState();
        } 


        public override void Activate()
        {
            //SpeedSets  = new List<MSpeedSet> { animal.CurrentSpeedSet }; //Store the Speed from the Last State

            base.Activate();
            
            IgnoreLowerStates = true;            //Make sure while you are on Jump State above the list cannot check for Trying to activate State below him
           // IsPersistent = true;                 //IMPORTANT!!!!! DO NOT ELIMINATE!!!!!

            animal.currentSpeedModifier.animator = 1;
            General.CustomRotation = true;
            CanJumpAgain = false;
        }

        public override void EnterTagAnimation()
        {
            if (CurrentAnimTag == EnterTagHash)
            {
                Debugging($"[EnterTag - {EnterTag.Value}]");

                animal.DeltaPos = Vector3.zero;

                if (!animal.RootMotion)
                {
                    var JumpStartSpeed = new MSpeed(animal.CurrentSpeedModifier)
                    {
                        name = "JumpStartSpeed",
                        position = animal.HorizontalSpeed,
                        Vertical = animal.CurrentSpeedModifier.Vertical.Value,
                        animator = 1,
                        rotation = AirRotation.Value,
                    };

                    animal.SetCustomSpeed(JumpStartSpeed,false);       //Set the Current Speed to the Jump Speed Modifier

                }

                JumpStartDirection = animal.Forward;

                if (animal.TerrainSlope > 0) //Means we are jumping uphill
                    animal.UseCustomAlign = true;
            }
            else if (CurrentAnimTag == ExitTagHash && animal.hash_StateOn == 0) //Do not do this on State ON Trigger 
            {
                CanExit = true;
                Debugging($"[EnterTag - {ExitTag.Value}] - Allow Exit");
                AllowExit();
            }
        }

        public override Vector3 Speed_Direction()
        {
            if (animal.HasExternalForce)
            {
                return Vector3.ProjectOnPlane(animal.ExternalForce, animal.UpVector);
            }
            else if (AirControl)
            {
                return base.Speed_Direction();
            }
            else
            {
                return JumpStartDirection;
            }
        }


        /// <summary> Make the Jump Start</summary>
        public override void EnterCoreAnimation()
        {
            Debugging($"[Enter Core Tag - [Jump]");

            FindJumpProfile();

            OneCastingFall_Ray = false;                                 //Reset Values IMPROTANT
            JumpPressHeight_Value = 1;
            JumpPressForward = 1;
            JumpPressForwardAdditive = 0;
            //IsPersistent = true;
            animal.UseGravity = false;
            animal.ResetGravityValues();

            JumpSpeed = new MSpeed(animal.CurrentSpeedModifier) //Inherit the Vertical and the Lerps
            {
                name = $"Jump [{activeJump.name}]",
                position = animal.RootMotion ? 0 : animal.HorizontalSpeed * activeJump.ForwardMultiplier, //Inherit the Horizontal Speed you have from the last state
                animator = 1,
                rotation = (AirRotation.Value),
                lerpPosAnim = JumpPressedLerp,
                lerpPosition = JumpPressedLerp,
            };


            animal.SetCustomSpeed(JumpSpeed);       //Set the Current Speed to the Jump Speed Modifier
            JumpStartDirection = animal.Forward;

            if (animal.TerrainSlope > 0)    //Means we are jumping uphill HACK
                animal.UseCustomAlign = true;
        }

        private void FindJumpProfile()
        {
            activeJump = jumpProfiles != null ? jumpProfiles[0] : new JumpProfile();

            foreach (var jump in jumpProfiles)                          //Save/Search the Current Jump Profile by the Lowest Speed available
            {
                if (jump.LastState == null)
                {
                    if (jump.VerticalSpeed <= animal.VerticalSmooth)
                    {
                        activeJump = jump;
                    }
                }
                else
                {
                    if (jump.VerticalSpeed <= animal.VerticalSmooth && jump.LastState == animal.LastState.ID)
                    {
                        activeJump = jump;
                    }
                }
            }

            if (activeJump.CliffTime.minValue == 0 && activeJump.CliffTime.maxValue == 0)
            {
                activeJump.CliffTime = new RangedFloat(0.333f, 0.666f);
            }

            Debugging($"Jump Profile: <B>[{ activeJump.name}]</B>");
        }

        public override void OnStateMove(float deltaTime)
        {
            if (InCoreAnimation)
            {
                IsPersistent = false; //HACK NO need to be persistant

                if (activeJump.JumpLandDistance == 0) return; //Meaning is a false Jump Like neigh on the Horse IMPORTANT!!!!

                if (JumpPressed)
                {
                    JumpPressHeight_Value = Mathf.MoveTowards(JumpPressHeight_Value, InputValue ? 1 : 0, deltaTime * JumpPressedLerp);
                }

                if (ForwardPressed)
                {
                    JumpPressForward = Mathf.MoveTowards(JumpPressForward, animal.MovementAxis.z, deltaTime * JumpPressedLerp);
                    JumpPressForwardAdditive = Mathf.MoveTowards(JumpPressForwardAdditive, animal.MovementAxis.z, deltaTime * JumpPressedLerp);
                }

                if (!General.RootMotion) //If the Jump is NOT Root Motion!!
                {
                    Vector3 ExtraJumpHeight = (animal.UpVector * activeJump.HeightMultiplier);
                    animal.AdditivePosition += ExtraJumpHeight * deltaTime * JumpPressHeight_Value;
                }
                else //If the Jump IS Root Motion!! ***********************************************************************
                {
                    Vector3 RootMotionUP = Vector3.Project(Anim.deltaPosition, animal.UpVector);         //Get the Up vector of the Root Motion Animation

                    bool isGoingUp = Vector3.Dot(RootMotionUP, animal.Up) > 0;  //Check if the Jump Root Animation is going  UP;

                    if (isGoingUp)
                    {
                        //Remove the default Root Motion Jump
                        animal.AdditivePosition -= RootMotionUP;                                    
                        
                        //Add the New Root Motion Jump scaled by the Height Multiplier 
                        animal.AdditivePosition +=
                            (RootMotionUP * activeJump.HeightMultiplier * JumpPressHeight_Value);   
                    }

                    // Vector3 RootMotionForward = Vector3.ProjectOnPlane(Anim.deltaPosition, animal.Up);
                    Vector3 RootMotionForward = Anim.deltaPosition - RootMotionUP;

                    //Remove the default Root Motion Jump
                    animal.AdditivePosition -= RootMotionForward;                                                           

                   //If is cannot controlled in air?
                    if (!AirControl.Value) 
                    {
                        //Add the New Root Motion Jump scaled by the Height Multiplier 
                        animal.AdditivePosition +=
                            JumpStartDirection * RootMotionForward.magnitude * activeJump.ForwardMultiplier;

                    }
                    else
                        //Add the New Root Motion Jump scaled by the Height Multiplier 
                        animal.AdditivePosition +=
                                 (RootMotionForward * activeJump.ForwardMultiplier * JumpPressForward)  +
                                 (animal.Forward * activeJump.ForwardPressed * JumpPressForwardAdditive * deltaTime);     
                }
            }
            else if (ExitTagHash == CurrentAnimTag) //meaning is on Exit Jump
            {
                if (!animal.CheckIfGrounded())
                {
                    animal.UseGravity = true;
                }
            }
        }


        public override void TryExitState(float DeltaTime)
        { 
            if (animal.StateTime >= activeJump.fallingTime && !OneCastingFall_Ray)
                Check_for_Falling();

            if (activeJump.ExitTime >= activeJump.fallingTime &&  animal.StateTime >= activeJump.ExitTime)
            {
                Debugging($"[Allow Exit] - Exit Time [{activeJump.ExitTime}] ");
                animal.CheckIfGrounded();
                AllowExit(); //Using Exit Time Internally * New
            }

            CheckForGround(animal.StateTime);
        }


        private void CheckForGround(float normalizedTime)
        {
            if (activeJump.CliffLandDistance == 0) return; //Do nothing if RayLenght is zero
          
            if (activeJump.CliffTime.IsInRange(normalizedTime)) //Need to happen the First 1/3 of the Root Jump Animation
            {
                var RayLength = activeJump.CliffLandDistance * ScaleFactor;
                var MainPivot = animal.Main_Pivot_Point;

                if (debug)
                    Debug.DrawRay(MainPivot, -animal.Up * RayLength, Color.black, 0.1f);

                if (Physics.Raycast(MainPivot, -animal.Up, out JumpRay, RayLength, GroundLayer, IgnoreTrigger))
                {
                    if (debug) MTools.DebugTriangle(JumpRay.point, 0.1f, Color.black);

                    var TerrainSlope = Vector3.Angle(JumpRay.normal, animal.UpVector);
                    var DeepSlope = TerrainSlope > animal.maxAngleSlope;

                    if (!DeepSlope)       //Jump to a jumpable cliff not an inclined one
                    {
                        Debugging($"[Allow Exit] Cliff Time - Near Ground. Normalized time: {normalizedTime:F2} ");
                        AllowExit();
                        animal.CheckIfGrounded();
                        //  Debug.Break();
                    }
                }
            }
        }

        /// <summary>Check if the animal can change to fall state if there's no future ground to land on</summary>
        private void Check_for_Falling()
        {
            AllowExit(); //Set the Allow exit just in case
            OneCastingFall_Ray = true;

            if (activeJump.JumpLandDistance == 0)
            {
                animal.CheckIfGrounded(); //We are still on the ground
                return;  //Meaning that is a False Jump (like Neigh on the Horse)
            }

            float RayLength = animal.ScaleFactor * activeJump.JumpLandDistance; //Ray Distance with the Scale Factor
            var MainPivot = animal.Main_Pivot_Point;
            var Direction = -animal.Up;


            if (activeJump.JumpLandDistance > 0) //greater than 0 it can complete the Jump on an even Ground 
            {
                if (debug)
                    Debug.DrawRay(MainPivot, Direction * RayLength, Color.red, 0.25f);

                if (Physics.Raycast(MainPivot, Direction, out JumpRay, RayLength, GroundLayer, IgnoreTrigger))
                {
                    Debugging($"Min Distance to complete <B>[{ activeJump.name}]</B> -> { JumpRay.distance:F4}");
                    if (debug) MTools.DebugTriangle(JumpRay.point, 0.1f, Color.yellow);

                    var GroundSlope = Vector3.Angle(JumpRay.normal, animal.UpVector);
                    
                    if (GroundSlope > animal.maxAngleSlope)     //if we found something but there's a deep slope
                    {
                        Debugging($"[AllowExit] Try to Land but the Sloope was too Deep. Slope: {GroundSlope:F2}");
                        animal.UseGravity = General.Gravity;
                        return;
                    }


                    if (JumpRay.distance < animal.Height) //Meaning is near the ground
                    {
                        Debugging($"Allow Exit, Near the Ground  Jump Distance: [{JumpRay.distance}]. Height {animal.Height}");
                        AllowExit();
                        return;
                    }

                    IgnoreLowerStates = true;                           //Means that it can complete the Jump Ignore Fall Locomotion and Idle
                    Debugging($"Can finish the Jump. Going to Jump End");
                }
                else
                {
                    animal.UseGravity = General.Gravity;
                    Debugging($"[Allow Exit] - <B>Jump [{activeJump.name}] </B> Go to Fall..No Ground was found");
                }
            }
        }


        public override void NewActiveState(StateID newState)
        {
            if (newState.ID <= 1) CanJumpAgain = true; //Can Jump again if the states are Idle or Locomotion
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

            SleepFromState = new List<StateID>() { MTools.GetInstance<StateID>("Fall"), MTools.GetInstance<StateID>("Fly") };
            SleepFromMode = new List<ModeID>() { MTools.GetInstance<ModeID>("Action"), MTools.GetInstance<ModeID>("Attack1") };


            EnterTag.Value = "JumpStart";
            ExitTag.Value = "JumpEnd";

            General = new AnimalModifier()
            {
                RootMotion = true,
                Grounded = false,
                Sprint = false,
                OrientToGround = false,
                CustomRotation = true,
                IgnoreLowerStates = true, //IMPORTANT!
                Persistent = false,
                AdditivePosition = true,
                AdditiveRotation = true,
                Gravity = false,
                modify = (modifier)(-1),
            };

            ExitFrame = false;

            jumpProfiles = new List<JumpProfile>()
            { new JumpProfile()
            { name = "Jump",  fallingTime = 0.7f,    CliffTime = new RangedFloat( 0.333f,0.666f),  ForwardMultiplier = 1,  HeightMultiplier =  1, JumpLandDistance = 1.7f}
            };
        }
#endif
    }




    /// <summary>Different Jump parameters on different speeds</summary>
    [System.Serializable]
    public struct JumpProfile
    {
        /// <summary>Name to identify the Jump Profile</summary>
        public string name;

        /// <summary>Maximum Vertical Speed to Activate this Jump</summary>
        [Tooltip("Minimal Vertical Speed to Activate this Jump")]
        [Min(0)] public float VerticalSpeed;

        /// <summary>Min Distance to Complete the Land when the Jump is on the Highest Point, this needs to be calculate manually</summary>
        [Tooltip("Min Distance to Complete the Land when the Jump is on the Highest Point")]
        [Min(0)] public float JumpLandDistance;

        /// <summary>Animation normalized time to change to fall animation if the ray checks if the animal is falling </summary>
        [Tooltip("Animation normalized time to change to fall animation if the ray checks if the animal is falling ")]
        [Range(0, 1)]
        public float fallingTime;


        /// <summary>Animation normalized time to change to fall animation if the ray checks if the animal is falling </summary>
        [Tooltip("Set Allow Exit to the Jump Profile (*New)")]
        [Range(0, 1)]
        public float ExitTime;

       
        [Tooltip("Animation normalized time to check if we can end the jump sooner. if its set to zero I will use 0.333 normalize value as default")]
        [MinMaxRange(0, 1)]
        public RangedFloat CliffTime;

        /// <summary>Maximum distance to land on a Cliff </summary>
        [Tooltip("Maximum distance to land on a Cliff")]
        [Min(0)] public float CliffLandDistance;

        /// <summary>Height multiplier to increase/decrease the Height Jump</summary>
        public float HeightMultiplier;
        ///// <summary>Forward multiplier to increase/decrease the Forward Speed of the Jump</summary>
        [Tooltip("Forward multiplier to increase/decrease the Forward  Rootmotion speed of the Jump")]
        public float ForwardMultiplier;
       
        [Tooltip("Extra forward Movement to move the Animal Forward")]
        public float ForwardPressed;

        [Tooltip("Last State the animal was before making the Jump")]
        public StateID LastState;
    }
}
