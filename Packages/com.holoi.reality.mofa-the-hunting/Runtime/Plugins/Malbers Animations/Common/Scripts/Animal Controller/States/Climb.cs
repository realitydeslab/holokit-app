using MalbersAnimations.Utilities;
using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
//using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/climb")]
    /// <summary>Climb Logic </summary>
    public class Climb : State
    {
        public override string StateName => "Climb/Free Climb";


        /// <summary>Air Resistance while falling</summary>
        [Header("Climb Parameters"), Space]
        [Tooltip("Layer to identify climbable surfaces")]
        public LayerReference ClimbLayer = new LayerReference(1);

        //[Tooltip("Tag used to identify climbable surfaces. Default: [Climb]")]
        //public StringReference SurfaceTag =  new StringReference("Climb");
        public PhysicMaterial Surface;
        [Tooltip("Climb automatically when is near a climbable surface")]
        public BoolReference automatic = new BoolReference();

        /// <summary>Air Resistance while falling</summary>
        [Header("Climb Pivot"), Space]

        [Tooltip("Pivot to Cast a Ray from the Chest")]
        public Vector3 ClimbChest = Vector3.up;
        [Tooltip("Pivot to Cast a Ray from the Hip")]
        public Vector3 ClimbHip = Vector3.zero;
        [Tooltip("Distance of the Climb Point Ray")]
        public float ClimbRayLength = 1f;

        [Tooltip("Radius of the Ray to find a Climable Wall")]
        [Min(0.01f)] public float  m_rayRadius= 0.1f;

        [Header("Wall Detection"), Space]
        [Tooltip("When aligning the Animal to the Wall, this will be the distance needed to separate it from it ")]
        public float WallDistance = 0.2f;
        [Tooltip("Smoothness value to align the animal to the wall")]
        public float AlignSmoothness = 10f;
        [Tooltip("Distance from the Hip Pivot to the Ground")]
        public float GroundDistance = 0.5f;
        [Tooltip("Length of the Horizontal Rays to detect Inner Corners")]
        [Min(0)] public float InnerCorner = 0.4f;
        [Tooltip("Speed Multiplier to move faster around outter corners")]
        [Min(0)] public float OuterCornerSpeed = 0.4f;
        [Range(0,90)] public float ExitSlope = 30f;


        [Header("Ledge Detection")]
        [Tooltip("Offset Position to Cast the First Ray on top on the animal to find the ledge")]
        public Vector3 RayLedgeOffset = Vector3.up;
        [Tooltip("Length of the Ledge Ray")]
        [Min(0)] public float RayLedgeLength = 0.4f;
        [Tooltip("MinDistance to exit Climb over a ledge")]
        [FormerlySerializedAs("LedgeMinExit")]
        [Min(0)] public float LedgeExitDistance = 0.175f; 


        [Header("Exit State Status")]
        [Tooltip("When the Exit Condition Climb Up an edge is executed. The State Exit Status will change to this value")]
        public int ClimbLedge = 1;
        [Tooltip("When the Exit Condition Climb Off is executed. The State Exit Status will change to this value")]
        public int ClimbOff = 2;
        [Tooltip("If the character is Climbing Down and a Ground is found. The State Exit Status will change to this value to execute the ClimbDown Animation")]
        public int ClimbDown = 3;
        [Tooltip("When the Exit Condition Climb Down to the Ground executed. The State Exit Status will change to this value")]
        public int ClimbExitSlope = 4;

        public float RayRadius => m_rayRadius * animal.ScaleFactor; 

        /// <summary> Reference for the current Climbable surface</summary>
        public Transform WallChest { get; private set; }

        /// <summary> The Animal is on an Inner Corner</summary>
        private bool InInnerCorner;
        ///// <summary> The Animal is on an Outer Corner</summary>
        //private bool InOuterCorner; 
        
        private readonly RaycastHit[] EdgeHit = new RaycastHit[1];

        private RaycastHit HitChest;
        private RaycastHit HitHip;
        private RaycastHit HitSide; 
    
       // private bool DefaultCameraInput;
      //private bool WallClimbTag;

        /// <summary> World Position of the Climb Pivot </summary>
        public Vector3 ClimbPivotChest(Transform transform) => transform.TransformPoint(ClimbChest);
        public Vector3 ClimbPivotHip(Transform transform) => transform.TransformPoint(ClimbHip);

        
        /// <summary> Average Normal of the Wall</summary>
        public Vector3 AverageNormal { get; private set; }

        /// <summary>Check if the Current Wall its a valid wall</summary>
        public bool ValidWall { get; private set; }

        /// <summary>Valid Exit on Ledge</summary>
        public bool ExitOnLedge { get; private set; }

        /// <summary> Angle From the Gravity and the Wall Normal</summary>
        public float WallAngle { get; private set; }

        //public override void AwakeState()
        //{
        //    DefaultCameraInput = animal.UseCameraInput; //Store the Animal Current CameraInput
        //    base.AwakeState();
        //}

        public override void StatebyInput()
        {
           // Debug.Log($"StatebyInput InputValue {InputValue} ExitInputValue{ExitInputValue} {CheckClimbRay()}");
            if (InputValue && /*!ExitInputValue &&*/ CheckClimbRay())
            {
                Activate();
            }
        }

        public override void StateExitByInput()
        {
            SetExitStatus(ClimbOff);
          //  AllowExit();
         //   AllowExit(StateEnum.Fall, ClimbOff); //Force the Fall State
            Debugging($"Exit with Climb Input [{ExitInput.Value}]");
        }

        public override void Activate()
        {
            if (CheckClimbRay()) //it cannot be activated there's no Wall to Climb
            {
                base.Activate();
                animal.UseCameraInput = false;       //Climb cannot use Camera Input
                animal.DisablePivotChest();
                InInnerCorner = /*InOuterCorner =*/ false;
            }
        }

        public override bool TryActivate()
        {
            if (automatic && !ExitInputValue)
            {
                return CheckClimbRay();
            }
            return false;
        }


        public override void ResetStateValues()
        {
            WallChest = null;
            WallAngle = 90;
            LedgeHitDifference = 0;
            ExitOnLedge = false;
            animal.ResetCameraInput();
            InInnerCorner = /*InOuterCorner =*/false;
            ExitInputValue = false;
        }

        public override void RestoreAnimalOnExit()
        {
            animal.ResetPivotChest();
            animal.ResetCameraInput();
        }



        /// <summary>Current Direction Speed Applied to the Additional Speed, by default is the Animal Forward Direction</summary>
        public override Vector3 Speed_Direction()
        {
            return Up * (animal.VerticalSmooth) + Right * animal.HorizontalSmooth; //IMPORTANT OF ADDITIONAL SPEED
        }

        private bool CheckClimbRay()
        {
            if (InInnerCorner) return false; //Do nothing when the Animal is changing on the inner corner


            var Point_Chest = ClimbPivotChest(transform) + DeltaPos;
            var Point_Hip = ClimbPivotHip(transform) + DeltaPos;
            var Length = animal.ScaleFactor * ClimbRayLength;

            HitChest = new RaycastHit();
            HitHip = new RaycastHit();


            Debug.DrawRay(Point_Chest, Forward * ClimbRayLength, Color.green);
            Debug.DrawRay(Point_Chest, Forward * WallDistance, Color.red);

            Debug.DrawRay(Point_Hip, Forward * ClimbRayLength, Color.green);
            Debug.DrawRay(Point_Hip, Forward * WallDistance, Color.red);

            ValidWall = false;
            AverageNormal = Forward;

            if (Physics.SphereCast(Point_Chest, RayRadius, Forward, out HitChest, Length, ClimbLayer.Value, IgnoreTrigger))
            {
                ValidWall = HitChest.collider.sharedMaterial == Surface;

                if (ValidWall)
                {
                    AverageNormal = HitChest.normal;
                    DebugRays(HitChest.point, HitChest.normal);
                    ValidWall = false; //Reset again just to check that the second cast works too

                    if (Physics.SphereCast(Point_Hip, RayRadius, Forward, out HitHip, Length, ClimbLayer.Value, IgnoreTrigger))
                    {
                        ValidWall = HitHip.collider.sharedMaterial == Surface;
                        DebugRays(HitHip.point, HitHip.normal);
                        AverageNormal += HitHip.normal;
                    }
                }
            }

            if (animal.platform != HitChest.transform && HitChest.transform != null)  animal.SetPlatform(HitChest.transform); //Set new Platform

            WallAngle = Vector3.SignedAngle(AverageNormal, animal.UpVector, animal.Right);

          //  if (!automatic.Value) Debugging($"Try Activate: Valid Wall {ValidWall}");

            return ValidWall;
        }

        private void DebugRays(Vector3 p,Vector3 Normal)
        {
            MTools.DrawWireSphere(p + (Normal * RayRadius), Color.green, RayRadius);
            Debug.DrawRay(p, Normal * RayRadius * 2, Color.green);
        }

        public override void OnStateMove(float deltatime)
        {
            if (ExitOnLedge) //Meaning It can Exit on Ledge so smooth the exit
            {
                var dif = LedgeHitDifference *   deltatime * AlignSmoothness;
                animal.AdditivePosition -= dif * animal.UpVector;
                LedgeHitDifference -= Mathf.Clamp(dif, 0, dif);
                //Debug.Log("LedgeHitDifference: " + LedgeHitDifference);
                animal.PlatformMovement();
                return;
            }

            if (InCoreAnimation)
            {
                var Right = animal.Right;

                if (CheckClimbRay())
                {
                    if (HitChest.transform != null)
                        animal.PlatformMovement();
                       //CheckMovingWall(HitHip.transform, deltatime);

                    if (MovementRaw.x > 0) //Means is going Right
                    {
                        CalculateSideClimbHit(Right);
                    }
                    else if (MovementRaw.x < 0)//Means is going Left
                    {
                        CalculateSideClimbHit(-Right);
                    }
                    else //Not moving Horizontally
                    {
                    }

                    AlignToWall(HitChest.distance, deltatime);
                    OrientToWall(AverageNormal, deltatime);
                }
                else if (InInnerCorner)
                {
                    OrientToWall(AverageNormal, deltatime);
                    var Angle = Vector3.Angle(animal.Forward, -AverageNormal);
                    if (Angle < 5f) InInnerCorner = false;
                }
            }
            //Climb from ground.
            else if (InEnterAnimation/* || InExitAnimation*/)  //If we are on Climb Start do a quick alignment to the Wall.
            {
                if (CheckClimbRay())
                {
                    OrientToWall(AverageNormal, deltatime);
                    AlignToWall(HitChest.distance, deltatime);
                    animal.PlatformMovement();
                }
            }
        }
       
        private float LedgeHitDifference;

        private void CalculateSideClimbHit(Vector3 Direction)
        {
            var Ray1 =  Color.blue;
            var Ray2 = Color.blue;
            var Ray3 = Color.blue;

            var Forward = animal.Forward;
           // var ScaleFactor = animal.ScaleFactor;
            var CornerLength = InnerCorner * animal.ScaleFactor;
            var point = ClimbPivotChest(transform);

            MovementAxisMult.x = 1;

            if (Physics.Raycast(point, Direction, out HitSide, CornerLength, ClimbLayer, IgnoreTrigger))
            { 
                Ray1 = Color.green;

                if (HitSide.collider.sharedMaterial == Surface) //Next Surface is Climbable
                {
                    AverageNormal = HitSide.normal;
                    InInnerCorner = true;
                }
            }
            else
            {
                var SecondPoint = point + Direction * CornerLength;
                if (Physics.Raycast(SecondPoint, Forward, out HitSide, CornerLength, ClimbLayer, IgnoreTrigger))
                {
                    Ray2 = Color.green;
                    if (HitSide.transform != HitChest.transform) //Stop if the Surface is not climbable
                    {
                        if (HitSide.collider.sharedMaterial != Surface)
                        {
                            MovementAxisMult.x = 0;
                            animal.additivePosition.x = 0;
                        }
                    }
                }
                else
                {
                    var ThirdPoint = SecondPoint + Forward * CornerLength;

                    if (Physics.Raycast(ThirdPoint, -Direction, out HitSide, CornerLength, ClimbLayer, IgnoreTrigger))
                    {
                        Ray3 = Color.green;
                        if (HitSide.collider.sharedMaterial != Surface)
                        {
                            MovementAxisMult.x = 0;
                            Ray3 = Color.red;
                        }
                        else 
                        {
                            animal.AdditivePosition += Direction * animal.DeltaTime * OuterCornerSpeed; //Make a Fast Movement to quickly move to the next corner
                        }
                    }

                    Debug.DrawRay(ThirdPoint, -Direction * CornerLength, Ray3);
                }

                Debug.DrawRay(SecondPoint, Forward * CornerLength, Ray2);
            }

            Debug.DrawRay(point, Direction * CornerLength, Ray1);
        }


        //Align the Animal to the Wall
        private void AlignToWall(float distance, float deltatime)
        {
            float difference = distance - WallDistance * animal.ScaleFactor;

            if (!Mathf.Approximately(distance, WallDistance * animal.ScaleFactor))
            {
                Vector3 align = animal.Forward * difference * deltatime * AlignSmoothness;
                animal.AdditivePosition += align;
            }
        }


        private void OrientToWall(Vector3 normal, float deltatime)
        {
            Quaternion AlignRot = Quaternion.FromToRotation(Forward, -normal) * transform.rotation;  //Calculate the orientation to Terrain 
            Quaternion Inverse_Rot = Quaternion.Inverse(transform.rotation);
            Quaternion Target = Inverse_Rot * AlignRot;
            Quaternion Delta = Quaternion.Lerp(Quaternion.identity, Target, deltatime * AlignSmoothness); //Calculate the Delta Align Rotation
            animal.AdditiveRotation *= Delta;

            //Update the Rotation to always look Upwards
            var UP = Vector3.Cross(Forward, UpVector);
            UP = Vector3.Cross(UP, Forward);
            AlignRot = Quaternion.FromToRotation(transform.up, UP) * transform.rotation;  //Calculate the orientation to Terrain 
            Inverse_Rot = Quaternion.Inverse(transform.rotation);
            Target = Inverse_Rot * AlignRot;
            animal.AdditiveRotation *= Target; 
        }
         
        public override void TryExitState(float DeltaTime)
        {  
            var MainPivot = ClimbPivotChest(transform) + animal.AdditivePosition;


            if (Mathf.Abs(WallAngle) < ExitSlope)//Exit when the angle is max from the slope
            {
                Debugging("[Allow Exit] Slope is walkable");
                AllowExit(StateEnum.Locomotion, ClimbExitSlope); //Force the Idle State to be the next State
                animal.CheckIfGrounded();
                return;
            }

            //PRESSING DOWN
            if (MovementRaw.z < 0) //Means the animal is going down
            {
                Debug.DrawRay(MainPivot, -Up * ScaleFactor * GroundDistance, Color.white);


                //Means that the Animal is going down and touching the ground
                if (Physics.Raycast(MainPivot, -Up, out _, ScaleFactor * GroundDistance, animal.GroundLayer, IgnoreTrigger)) 
                {
                    Debugging("[Allow Exit] when Grounded and pressing Down and touched the ground");
                    AllowExit(StateEnum.Idle, ClimbDown); //Force the Idle State to be the next State
                    animal.CheckIfGrounded();
                }
            }
            else if (!ValidWall)  //Means the Animal did not touch a Wall Tagged Climb
            {
                Debugging("[Allow Exit] Exit when Wall is not Climbable");
                AllowExit();
            }
            else if (!ExitOnLedge) //Means the Animal going Up
            {
                CheckLedgeExit();
            }
        }

        private void CheckLedgeExit()
        {
            var LedgePivotUP = transform.TransformPoint(ClimbChest+RayLedgeOffset);

            //Check Upper Ground legde Detection
            bool LedgeHit = Physics.RaycastNonAlloc(LedgePivotUP, Forward, EdgeHit, ScaleFactor * RayLedgeLength, ClimbLayer.Value, IgnoreTrigger) > 0;
            var SecondRayPivot = new Ray(LedgePivotUP, Forward).GetPoint(RayLedgeLength);

            Debug.DrawRay(LedgePivotUP, Forward * RayLedgeLength, Color.green);
            Debug.DrawRay(SecondRayPivot, Gravity * RayLedgeLength*2, Color.green);
            Debug.DrawRay(SecondRayPivot, Gravity * LedgeExitDistance, Color.red);

            if (!LedgeHit)
            {
                LedgeHit = Physics.RaycastNonAlloc(SecondRayPivot, Gravity, EdgeHit, ScaleFactor * RayLedgeLength*2, ClimbLayer.Value, IgnoreTrigger) > 0;

                if (LedgeHit)
                {
                    var hit = EdgeHit[0];
                    if (hit.distance > LedgeExitDistance * ScaleFactor)
                    {
                        LedgeHitDifference = (hit.distance - LedgeExitDistance * ScaleFactor);
                        ExitOnLedge = true; //Activate Exit OnLedge
                        Debugging("Allow Exit - Exit on a Ledge");
                        SetExitStatus(ClimbLedge); //Keep this State as the Active State
                        //AllowExit(StateEnum.Locomotion, ClimbLedge);   //Force Locomotion State to be the next state, it also set the Exit Status
                    }
                }
            }
        }

        public override void StateGizmos(MAnimal animal)
        {
            if (debug && !Application.isPlaying)
            {
                var Forward = animal.Forward;
                var Right = animal.Right;
                var t = animal.transform;
                var Gravity = animal.Gravity;
                var ScaleFactor = animal.ScaleFactor;


                var Chest_Point = ClimbPivotChest(t);
                var Hip_Point = ClimbPivotHip(t);

                var LedgePivotUP = t.TransformPoint(ClimbChest+RayLedgeOffset);
                var SecondRayPivot = new Ray(LedgePivotUP, animal.Forward * ScaleFactor).GetPoint(RayLedgeLength);


                Gizmos.color = Color.green;
                Gizmos.DrawRay(SecondRayPivot, Gravity * ScaleFactor * RayLedgeLength*2);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(SecondRayPivot, Gravity * ScaleFactor * LedgeExitDistance);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(LedgePivotUP, Forward * ScaleFactor * RayLedgeLength);

                Gizmos.DrawRay(Chest_Point, Forward * ScaleFactor * ClimbRayLength);
                Gizmos.DrawRay(Hip_Point, Forward * ScaleFactor * ClimbRayLength);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(Chest_Point, Forward * ScaleFactor * WallDistance);
                Gizmos.DrawRay(Hip_Point, Forward * ScaleFactor * WallDistance);


                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Chest_Point + Forward * (ClimbRayLength - (m_rayRadius * ScaleFactor)), m_rayRadius * ScaleFactor);
                Gizmos.DrawWireSphere(Hip_Point + Forward * (ClimbRayLength - (m_rayRadius * ScaleFactor)), m_rayRadius * ScaleFactor);
                Gizmos.DrawRay(Chest_Point, Right * ScaleFactor * InnerCorner);
                Gizmos.DrawRay(Chest_Point, -Right * ScaleFactor * InnerCorner);
                Gizmos.color = Color.white;
                var MainPivot = ClimbPivotChest(t);
                Gizmos.DrawRay(MainPivot, -animal.Up * ScaleFactor * GroundDistance);
            }
        }

        private void OnValidate()
        {
            LedgeExitDistance = Mathf.Clamp(LedgeExitDistance, 0, RayLedgeLength);
        }


#if UNITY_EDITOR
        void Reset()
        {
            ID = MTools.GetInstance<StateID>("Climb");

            Surface = MTools.GetResource<PhysicMaterial>("Climbable");

            General = new AnimalModifier()
            {
                modify = (modifier)(-1),
                RootMotion = true,
                AdditivePosition = true,
                AdditiveRotation = false,
                Grounded = false,
                Sprint = true,
                OrientToGround = false,
                Gravity = false,
                CustomRotation = true,
                FreeMovement = false,
                IgnoreLowerStates = true, 
            };
        }

        public override void SetSpeedSets(MAnimal animal)
        {
            var setName = "Climb";

            if (animal.SpeedSet_Get(setName) == null)
            {
                animal.speedSets.Add(
                    new MSpeedSet()
                    {
                        name = setName,
                        StartVerticalIndex = new IntReference(1),
                        TopIndex = new IntReference(1),
                        states = new List<StateID>(1) { ID },
                        Speeds = new List<MSpeed>() { new MSpeed(setName) }
                    }
                    );
            }
        }
#endif
    }
}