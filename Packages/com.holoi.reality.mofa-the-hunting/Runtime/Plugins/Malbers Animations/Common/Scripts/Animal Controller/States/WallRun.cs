using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/wallrun")]

    public class WallRun : State
    {
        [Header("Glide Parameters")]
        [Tooltip("Find Walls to run automatically, without the need of an Input")]
        public BoolReference Automatic = new BoolReference();
        [Tooltip("Keep Pressing the input to maintain Wall Run")]
        public BoolReference InputPressed = new BoolReference(true);

        [Header("Pivot Center")]
        [Tooltip("Pivot to cast rays from the Animal, to find walls left and right")]
        public Vector3Reference Center = new Vector3Reference(0, 1, 0);

        [Header("Wall Parameters")]
        [Tooltip("Max Distance to find walls left and Right.")]
        public FloatReference WallCheck = new FloatReference(1);

        [Tooltip("Distance to align the Character to the Wall")]
        public FloatReference WallDistance = new FloatReference(0.01f);

        [Tooltip("Minimun Height required to activate the Wall Run")]
        public FloatReference StartHeight = new FloatReference(0);

        [Tooltip("What layers can be Wallrunned")]
        public LayerReference Layer = new LayerReference(1);

        [Tooltip("Another Filter to the walls")]
        public StringReference WallTag = new StringReference("WallRun");

        [Header("Smoothness and Lerping")]
        [Tooltip("Lerp value to Orient the Animal to the wall")]
        public FloatReference AlignLerp = new FloatReference(10);

        [Tooltip("Lerp value to rotate the Animal to the wall")]
        public FloatReference RotationLerp = new FloatReference(10);
      
        [Tooltip("Rotation Angle")]
        public FloatReference Bank = new FloatReference(0);

        [Header("Exit Impulse")]
        [Tooltip("Exit Rotation Angle")]
        public FloatReference ExitAngle = new FloatReference(30);
        [Tooltip("Time to rotate the animal to the Exit Angle ")]
        public FloatReference ExitAngleTime = new FloatReference(0.2f);
        [Tooltip("Next State to apply the exit push")]
        public List<StateID> PushStates;

        //[Tooltip("X: Force. Y: Enter Aceleration. Z Exit Drag")]
        //public Vector3 ForceValues;


        [Header("Intertia and Gravity")]
        [Tooltip("Wall Running pushing")]
        public FloatReference GravityPush = new FloatReference(0.5f);
        [Tooltip("Decrease the Vertical Impulse when entering the Fall State")]
        public float UpDrag = 1;

        private RaycastHit WallHit;
        private bool RightSide;

        public Vector3 UpImpulse { get; private set; }
        public bool Has_UP_Impulse { get; private set; }

        public override string StateName => "Wall Run";

        public override float GravityMultiplier => GravityPush;

        /// <summary> Is the Animal on a Valid Wall</summary>
        public Transform WallFound { get; private set; }

        private bool CheckWallRay()
        {
            var t = animal.transform;
            var ScaleFactor = animal.ScaleFactor;
            var Right = t.right * ScaleFactor;
            var WoldPos = t.TransformPoint(Center);


            Debug.DrawRay(WoldPos, Right * WallCheck, Color.green);
            Debug.DrawRay(WoldPos, -Right * WallCheck, Color.green);
            Debug.DrawRay(WoldPos, Right * WallDistance, Color.red);
            Debug.DrawRay(WoldPos, -Right * WallDistance, Color.red);

            //Do not activate if the animal is to close to the ground
            if (StartHeight > 0 && 
                Physics.Raycast(WoldPos, animal.Gravity, out _, animal.Height + StartHeight* ScaleFactor, animal.GroundLayer)) 
                return false;


            WallHit = new RaycastHit();

            if (Physics.Raycast(WoldPos, Right, out WallHit, WallCheck * ScaleFactor, Layer))
            {
                var newWall = WallHit.transform;
              
                if (debug)  MTools.DrawWireSphere(WallHit.point, Color.green, 0.05f, 0.5f);
               
                if (newWall != WallFound)
                {
                    WallFound = newWall;

                    if (WallTag.Empty || WallFound.CompareTag(WallTag)) //Check Wall Filter
                    {
                        RightSide = true; //Wall on the RIght Side
                        animal.SetPlatform(WallFound);
                        Debugging("[Try Activate] Wall detected on the Right");
                        return true;
                    }
                }
            }
            else if (Physics.Raycast(WoldPos, -Right, out WallHit, WallCheck * ScaleFactor, Layer))
            {
                var newWall = WallHit.transform;
                if (debug) MTools.DrawWireSphere(WallHit.point, Color.green, 0.05f, 0.5f);
               
                if (newWall != WallFound)
                {
                    WallFound = newWall;
                    if (WallTag.Empty || WallFound.CompareTag(WallTag)) //Check Wall Filter
                    {
                        RightSide = false; //Wall on the Left Side
                        animal.SetPlatform(WallFound);
                        Debugging("[Try Activate] Wall detected on the Left");
                        return true;
                    }
                }
            }
            else
            {
                WallFound = null; //Clean wall, no wall was found
            }

            return false;
        }
         

        /// <summary>Cast the Ray for checking walls on automatic too/ </summary>
        public override bool TryActivate()
        {
            return (Automatic.Value || InputValue) && CheckWallRay();
        }


        public override void Activate()
        {
            base.Activate();
            SetEnterStatus(RightSide ? 1 : -1); //Set the correct Animatino
            animal.ResetGravityValues();
            UpImpulse = Vector3.Project(animal.DeltaPos, animal.UpVector);   //Clean the Vector from Forward and Horizontal Influence    
            Has_UP_Impulse = Vector3.Dot(UpImpulse, animal.UpVector) > 0;
        }


        public override void OnStatePreMove(float deltatime)
        {
            animal.DeltaRootMotion = Vector3.zero;  //Remove any Movement Residual.
            animal.DeltaAngle = 0;  //Remove any horizontal value.
            animal.MovementAxisRaw.z = 1; //Always move forward
            animal.MovementAxis.z = 1; //Always move forward
            animal.HorizontalSmooth = 0; //Remove any horizontal value.
            animal.MovementAxis.x = 0; //Remove any horizontal value.
            animal.MovementAxisRaw.x = 0; //Remove any horizontal value.
        }

      


        public override void OnStateMove(float deltatime)
        {
            if (InCoreAnimation)
            {
                var t = animal.transform;
                var ScaleFactor = animal.ScaleFactor;
                var Dir = RightSide ? 1 : -1;
                var Right = Dir * t.right * ScaleFactor;
                var WoldPos = t.TransformPoint(Center);

                Debug.DrawRay(WoldPos, Right * WallCheck, Color.green);
                Debug.DrawRay(WoldPos, Right * WallDistance, Color.red);

              //  WallFound = null;


                if (Has_UP_Impulse)
                {
                    animal.AdditivePosition += UpImpulse;
                    UpImpulse = Vector3.Lerp(UpImpulse, Vector3.zero, deltatime * UpDrag); //Clean the Up impulse with air Drag
                }


                if (Physics.Raycast(WoldPos, Right, out WallHit, WallCheck * ScaleFactor, Layer))
                {
                    var newWall = WallHit.transform;

                    if (debug) MTools.DrawWireSphere(WallHit.point, Color.green, 0.05f, 0.5f);

                    if (newWall != WallFound)
                    {
                        WallFound = WallHit.transform;

                        if (!WallTag.Empty && !WallFound.CompareTag(WallTag)) //Check Wall Filter
                        {
                            WallFound = null; //No wall found so skip
                            return;
                        }
                    }

                    OrientToWall(Right, WallHit.normal, deltatime);
                    AlignToWall(Right, WallHit.distance, deltatime);
                }
                else
                {
                    WallFound = null; //No wall found so skip
                    return;
                }

                //Do Wall Bank
                animal.Bank = Mathf.Lerp(animal.Bank, Dir * Bank, deltatime * animal.CurrentSpeedSet.BankLerp);

                //Debug.Log(Vector3.SignedAngle(animal.Forward,animal.DeltaVelocity,-animal.Right));

                var Pich = Vector3.SignedAngle(animal.Forward, animal.DeltaVelocity, animal.Right);
                animal.PitchAngle = Mathf.Lerp(animal.PitchAngle, Pich, deltatime * animal.CurrentSpeedSet.PitchLerpOn);

                animal.State_SetFloat(animal.PitchAngle);

                animal.CalculateRotator(); //Calculate the Rotator Rotation.
            }
        }

       

        //Align the Animal to the Wall
        private void AlignToWall(Vector3 Direction, float distance, float deltatime)
        {
            float difference = distance - (WallDistance * animal.ScaleFactor);
            Vector3 align = Direction * difference * deltatime * AlignLerp;
            animal.AdditivePosition += align;
        }

        private void OrientToWall(Vector3 Right, Vector3 normal, float deltatime)
        {
            var TRot = transform.rotation;

            Quaternion AlignRot = Quaternion.FromToRotation(Right, -normal) * TRot;  //Calculate the orientation to Terrain 
            Quaternion Inverse_Rot = Quaternion.Inverse(TRot);
            Quaternion Target = Inverse_Rot * AlignRot;
            Quaternion Delta = Quaternion.Lerp(Quaternion.identity, Target, deltatime * RotationLerp); //Calculate the Delta Align Rotation
            animal.AdditiveRotation *= Delta;
        }


        public override void TryExitState(float DeltaTime)
        {
            if (!WallFound)  //If there's no longer a wall Allow Exit  
            {
                Debugging("[Allow Exit] Wall not Found");
                AllowExit();  
            }
            else if (!string.IsNullOrEmpty(Input) && InputPressed.Value && InputValue == false)     //If we are using Input and is no longer pressed
            {
                Debugging("[Allow Exit] The Input is no longer Pressed");
                AllowExit();
            }
            else if (animal.CheckIfGrounded())       //If the ground is near   Exit                                 
            {
                Debugging("[Allow Exit] Is near the ground");
                AllowExit(3);
            }
        }

        public override void AllowStateExit()
        {
            animal.ResetGravityValues();
        }

        public override void PostExitState()
        {
            if (animal.LastState == this &&  ExitAngle > 0 && animal.enabled)
            {
                if (PushStates == null || PushStates.Contains(CurrentActiveState.ID))
                {
                    var Dir = RightSide ? -1 : 1;

                    // animal.InertiaPositionSpeed = Quaternion.Euler(0, ExitAngle * Dir, 0) * Forward;
                    animal.StartCoroutine(
                        MTools.AlignTransform_Rotation(transform, transform.rotation * Quaternion.Euler(0, ExitAngle * Dir, 0), ExitAngleTime));
                    Debugging("[Exit Impulse] Push the Animal Away from the Wall");
                }
            }
        }

        public override void ResetStateValues()
        {
            WallFound = null;
            WallHit = new RaycastHit();
            UpImpulse = Vector3.zero;
            Has_UP_Impulse = false;
        }


     


#if UNITY_EDITOR
        public override void StateGizmos(MAnimal animal)
        {
            if (debug && !Application.isPlaying)
            {
                var t = animal.transform;
                var ScaleFactor = animal.ScaleFactor;

                var Right = t.right * ScaleFactor;

                var WoldPos = t.TransformPoint(Center);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(WoldPos, Right * WallCheck);
                Gizmos.DrawRay(WoldPos, -Right * WallCheck);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(WoldPos, Right * WallDistance);
                Gizmos.DrawRay(WoldPos, -Right * WallDistance);
            }
        }

        void Reset()
        {
            ID = MTools.GetInstance<StateID>("WallRun");

            Duration = new FloatReference(3f);

            EnterCooldown = new FloatReference(1f);
            ExitCooldown= new FloatReference(0.5f);

            SleepFromState = new List<StateID>(2) { MTools.GetInstance<StateID>("Idle"), MTools.GetInstance<StateID>("Locomotion") };
            PushStates = new List<StateID>(1) { MTools.GetInstance<StateID>("Jump")};

            General = new AnimalModifier()
            {
                modify = (modifier)(-1),
                RootMotion = true,
                AdditivePosition = true,
                AdditiveRotation = true,
                Grounded = false,
                Sprint = true,
                OrientToGround = false,
                Gravity = true,
                CustomRotation = true,
                FreeMovement = true,
                IgnoreLowerStates = true,
            };
        }


        public override void SetSpeedSets(MAnimal animal)
        {
            var setName = "Wall Run";

            if (animal.SpeedSet_Get(setName) == null)
            {
                animal.speedSets.Add(
                    new MSpeedSet()
                    {
                        name = setName,
                        StartVerticalIndex = new IntReference(1),
                        TopIndex = new IntReference(1),
                        states = new List<StateID>(1) { MTools.GetInstance<StateID>("WallRun") },
                        Speeds = new List<MSpeed>() { new MSpeed("Wall Run") }
                    }
                    );
            }
        }


#endif
    }
}
