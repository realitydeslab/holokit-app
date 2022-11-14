using UnityEngine;
using MalbersAnimations.Scriptables;
using UnityEngine.Serialization;

namespace MalbersAnimations.Controller
{
    
    
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/fall")]
    public class Fall : State
    {
        //TODO: DO Fall Rotator

        public override string StateName => "Fall";
        public enum FallBlending { DistanceNormalized, Distance, VerticalVelocity }

        /// <summary>Air Resistance while falling</summary>
        [Header("Fall Parameters")]
        [Tooltip("Can the Animal be controller while falling?")]
        public BoolReference AirControl = new BoolReference(true);
        [Tooltip("Rotation while falling")]
        public FloatReference AirRotation = new FloatReference(10);
        [Tooltip("Maximum Movement while falling")]
        public FloatReference AirMovement = new FloatReference(0);
        [Tooltip("Lerp value for the Air Movement adjusment")]
        public FloatReference AirSmooth = new FloatReference(2);

        [Space]

        [Tooltip("Forward Offset Position of the Fall Ray")]
        public FloatReference Offset = new FloatReference();

        [Tooltip("Forward Offset Multiplier Position of the Fall Ray while moving")]
        [FormerlySerializedAs("FallRayForward")]
        public FloatReference MoveMultiplier = new FloatReference(0.1f);

        [Tooltip("Multiplier for the Fall Ray Length. The Default Value is the Animal's Height")]
        [FormerlySerializedAs("fallRayMultiplier")]
        public FloatReference lengthMultiplier = new FloatReference(1f);
         
        [Tooltip("RayHits Allowed on the Raycast NonAloc (Try Fall Logic)")]
        public IntReference RayHits = new IntReference(3);  

        [Tooltip("Decrease the Vertical Impulse when entering the Fall State")]
        [FormerlySerializedAs("AirDrag")]
        public float UpDrag = 1;

        [Space]
        public FallBlending BlendFall = FallBlending.DistanceNormalized;

        [Tooltip("Used to Set fallBlend to zero before reaching the ground")]
        public FloatReference LowerBlendDistance;

        /// <summary>Distance to Apply a Fall Hard Animation</summary>
        [Space, Header("Fall Damage")]
        public StatID AffectStat;

        [Tooltip("Minimum Distance to Apply a Soft Land Animation")]
        public FloatReference FallMinDistance = new FloatReference(5f);

        [Tooltip("Maximun Distance to Apply a Hard Land Animation")]
        public FloatReference FallMaxDistance = new FloatReference(15f);

        [Tooltip("The Fall State will set the Exit State Status Depending the Fall Distance (X: Distance Y:Exit Status Value)")]
#if UNITY_2020_3_OR_NEWER
        [NonReorderable]
#endif
        public Vector2[] landStatus;

        [Tooltip("Fix the animal when is stuck on weird places (Experimental)")]
        public bool StuckAnimal = true;

        [Tooltip("When Falling, the animal may get stuck falling. The animal will be force to move forward.")]
        public FloatReference PushForward = new FloatReference(2);
        /// <summary>Stores the max heigth before going Down</summary>
        public float MaxHeight { get; set; }

        /// <summary>Acumulated Fall Distance</summary>
        public float FallCurrentDistance { get; set; }

        protected Vector3 fall_Point;
        private RaycastHit[] FallHits;
        private RaycastHit FallRayCast;

        private GameObject GameObjectHit;
        private bool IsDebree;

        /// <summary>While Falling this is the distance to the ground</summary>
        private float DistanceToGround; 

        /// <summary> Normalized Value of the Height </summary>
        float Fall_Float;
        public Vector3 UpImpulse { get; private set; }
        public bool Has_UP_Impulse { get; private set; }

        private MSpeed FallSpeed = MSpeed.Default;

        public Vector3 FallPoint { get; private set; }

        /// <summary> UP Impulse was going UP </summary>

        private bool GoingDown;
        private int Hits;

        public override void AwakeState()
        {
            base.AwakeState();
            animalStats = animal.FindComponent<Stats>(); //Find the Stats
        }

        public override bool TryActivate()
        {
            float SprintMultiplier = (animal.VerticalSmooth);
            var fall_Pivot = animal.Main_Pivot_Point +  (animal.Forward * Offset * ScaleFactor) +
                (animal.Forward * SprintMultiplier * MoveMultiplier * ScaleFactor); //Calculate ahead the falling ray

            fall_Pivot += animal.DeltaPos; //Check for the Next Frame

            float Multiplier = animal.Pivot_Multiplier * lengthMultiplier*0.99f;
            return TryFallRayCasting(fall_Pivot, Multiplier);
        }

        private bool TryFallRayCasting(Vector3 fall_Pivot, float Multiplier)
        {
            FallHits = new RaycastHit[RayHits];

            var Direction = animal.TerrainSlope < 0 ? Gravity : -transform.up;

            var Radius = animal.RayCastRadius * ScaleFactor;
            Hits = Physics.SphereCastNonAlloc(fall_Pivot, Radius, Direction, FallHits, Multiplier, GroundLayer, IgnoreTrigger);

            if (debug)
            {
                Debug.DrawRay(fall_Pivot, Direction * Multiplier, Color.magenta);
                Debug.DrawRay(FallRayCast.point, FallRayCast.normal * ScaleFactor * 0.2f, Color.magenta);
            }

            var TerrainSlope = 0f;

            if (Hits > 0)
            {
                if (animal.Grounded) //Check when its grounded
                {
                    foreach (var hit in FallHits)
                    {
                        if (hit.collider != null)
                        {
                            TerrainSlope = Vector3.SignedAngle(hit.normal, animal.UpVector, animal.Right);
                            MTools.DrawWireSphere(fall_Pivot + Direction * DistanceToGround, Color.magenta, Radius);
                            FallRayCast = hit;
                           
                            if (TerrainSlope > -animal.maxAngleSlope) //Check for the first Good Fall Ray that does not break the Fall.
                                break;
                        }
                    }

                    if (FallRayCast.transform.gameObject != GameObjectHit) //Check if what the Fall Ray Hit was a Debree
                    {
                        GameObjectHit = FallRayCast.transform.gameObject;
                        IsDebree = GameObjectHit.CompareTag(animal.DebrisTag);
                    }

                    // Debug.Log("IsDebree = " + IsDebree, GameObjectHit);

                    if (animal.DeepSlope ||
                        (TerrainSlope < animal.NegativeMaxSlope //Calculate the Deep Slope here
                        && !IsDebree))
                    {
                        Debugging($"[Try] Slope is too deep [{FallRayCast.collider.transform.name}] | Hits: {Hits} | Slope: {TerrainSlope:F2}");
                        return true;
                    }
                }
                else   //If the Animal is in the air  NOT GROUNDED
                {
                    FallRayCast = FallHits[0];
                    DistanceToGround = FallRayCast.distance;

                    var FallSlope = Vector3.Angle(FallRayCast.normal, animal.UpVector);

                    if (FallSlope > animal.PositiveMaxSlope)
                    {
                        Debugging($"[Try] The Animal is on the Air and the angle SLOPE of the ground hitted is too Deep. {FallRayCast.transform.name}");

                        return true;
                    }

                    if (Height >= DistanceToGround) //If the distance to ground is very small means that we are very close to the ground
                    {

                        if (animal.ExternalForce != Vector3.zero) return true; //Hack for external forces

                        Debugging($"[Try Failed] Distance to the ground is very small means that we are very close to the ground. CHECK IF GROUNDED");
                        animal.CheckIfGrounded();//Hack IMPORTANT HACK!!!!!!!
                        return false;
                    }
                }
            }
            else
            {
                Debugging($"[Try] There's no Ground beneath the Animal");
                return true;
            }

            //Debug.Log("fa");
            return false;
        }

        public override void Activate()
        {
            StartingSpeedDirection = animal.ActiveState.Speed_Direction();  //Store the Last Speed Direction.... For AirControl False
            base.Activate();
            ResetStateValues();
            Fall_Float = animal.State_Float;
        }

        public override void EnterCoreAnimation()
        {
            SetEnterStatus(0);

            UpImpulse = Vector3.Project(animal.DeltaPos, animal.UpVector);   //Clean the Vector from Forward and Horizontal Influence    

            IgnoreLowerStates = false;

            var Speed = animal.HorizontalSpeed / ScaleFactor; //Remove the scaleFactor since it will be added later 

            if (animal.HasExternalForce)
            {
                var HorizontalForce = Vector3.ProjectOnPlane(animal.ExternalForce, animal.UpVector);    //Remove Horizontal Force
                var HorizontalInertia = Vector3.ProjectOnPlane(animal.Inertia, animal.UpVector);        //Remove Horizontal Force

                var HorizontalSpeed = HorizontalInertia - HorizontalForce;
                Speed = HorizontalSpeed.magnitude / ScaleFactor; //Remove the scaleFactor since it will be added later 
              //  passInertia = false;
            }

            //Remove all Speed if the External Force does not allows it
            if (!animal.ExternalForceAirControl)  Speed = 0; 

            FallSpeed = new MSpeed(animal.CurrentSpeedModifier)
            {
                name = "FallSpeed",
                position = Speed, 
                strafeSpeed = Speed,
                animator = 1,
                rotation = AirRotation.Value,
                lerpPosition = AirSmooth.Value,
                lerpStrafe = AirSmooth.Value, 
            };


            animal.SetCustomSpeed(FallSpeed, false);

            //Disable the Gravity if we are on an external Force (Wind, Spring) //IMPORTANT
            if (animal.HasExternalForce && animal.InZone) animal.UseGravity = false;

            Has_UP_Impulse = Vector3.Dot(UpImpulse, animal.UpVector) > 0;

            //means it was on locomotion or idle //Remove Up Impulse HACK
            if (MTools.CompareOR(animal.LastState.ID, 0, 1, StateEnum.Swim, StateEnum.Climb) && Has_UP_Impulse || animal.HasExternalForce)
                UpImpulse = Vector3.zero;

        }

        public override Vector3 Speed_Direction()
        {
            return AirControl.Value ? (base.Speed_Direction() + animal.ExternalForce).normalized : StartingSpeedDirection;
        }


        Vector3 StartingSpeedDirection;
        private Stats animalStats;

        public override void OnStateMove(float deltaTime)
        {
            if (InCoreAnimation)
            {
                if (animal.InZone && animal.HasExternalForce) animal.GravityTime = 0; //Reset the gravity when the animal is on a Force Zone.

                animal.AdditivePosition += UpImpulse;

                if (Has_UP_Impulse)
                    UpImpulse = Vector3.Lerp(UpImpulse, Vector3.zero, deltaTime * UpDrag); //Clean the Up impulse with air Drag

                if (AirControl.Value && AirMovement > 0 && AirMovement > CurrentSpeedPos)
                {
                    if (!animal.ExternalForceAirControl) return;

                    CurrentSpeedPos = Mathf.Lerp(CurrentSpeedPos, AirMovement, (AirSmooth != 0 ? (deltaTime * AirSmooth) : 1));
                }

               // if (!CanExit) TryExitState(deltaTime);
            }
        }
         
        public override void TryExitState(float DeltaTime)
        {
            var Radius = animal.RayCastRadius * ScaleFactor;

            float SprintMultiplier = (animal.VerticalSmooth);
            var FallPoint = animal.Main_Pivot_Point + (animal.Forward * Offset * ScaleFactor) +
               (animal.Forward * (SprintMultiplier * MoveMultiplier * ScaleFactor)); //Calculate ahead the falling ray
 
            //var Gravity = this.Gravity;
            var Gravity = animal.DeepSlope ? this.Gravity :  -animal.Up;

            //fall_Pivot += animal.DeltaPos; //Check for the Next Frame
            //FallPoint = animal.Main_Pivot_Point;
            float DeltaDistance = 0;

            GoingDown = Vector3.Dot(DeltaPos, Gravity) > 0; //Check if is falling down

            if (GoingDown)
            {
                DeltaDistance = Vector3.Project(DeltaPos, Gravity).magnitude/ScaleFactor;
                FallCurrentDistance += DeltaDistance;
            }

            if (animal.debugGizmos && debug)
            {
                MTools.DrawWireSphere(FallPoint, Color.magenta, Radius);
                MTools.DrawWireSphere(FallPoint + Gravity * Height, (Color.red + Color.blue) / 2, Radius);
                Debug.DrawRay(FallPoint, Gravity * 100f, Color.magenta);
            }

            var FoundGround = (Physics.Raycast(FallPoint, Gravity, out FallRayCast, 100f, GroundLayer, IgnoreTrigger));


            //var RBMovement = Vector3.ProjectOnPlane(animal.RB.velocity * DeltaTime, animal.UpVector); //This is for Helping on Slopes
            //animal.InertiaPositionSpeed = RBMovement;

            if (FoundGround)
            {
                DistanceToGround = FallRayCast.distance;

                if (animal.debugGizmos && debug)
                {
                    MTools.DrawWireSphere(FallRayCast.point, (Color.blue + Color.red) / 2, Radius);
                    MTools.DrawWireSphere(FallPoint, (Color.red), Radius);
                }

                switch (BlendFall)
                {
                    case FallBlending.DistanceNormalized:
                        {
                            var realDistance = DistanceToGround - Height;

                            if (MaxHeight < realDistance)
                            {
                                MaxHeight = realDistance; //get the Highest Distance the first time you touch the ground
                                Fall_Float = Mathf.Lerp(Fall_Float, 0, DeltaTime * 5); //Small blend in case there's a new ground found
                                animal.State_SetFloat(Fall_Float); //Blend between High and Low Fall
                            }
                            else
                            {
                                realDistance -= LowerBlendDistance;
                                Fall_Float = Mathf.Lerp(Fall_Float, 1 - realDistance / MaxHeight, DeltaTime * 10); //Small blend in case there's a new ground found
                                animal.State_SetFloat(Fall_Float); //Blend between High and Low Fall
                            }
                        }
                        break;
                    case FallBlending.Distance:
                        animal.State_SetFloat(FallCurrentDistance);
                        break;
                    case FallBlending.VerticalVelocity:
                        var UpInertia = Vector3.Project(animal.DeltaPos, animal.UpVector).magnitude;   //Clean the Vector from Forward and Horizontal Influence    
                        animal.State_SetFloat(UpInertia / animal.DeltaTime * (GoingDown ? 1 : -1));
                        break;
                    default:
                        break;
                }

                if (Height >= DistanceToGround || ((DistanceToGround - DeltaDistance) < 0)) //Means has touched the ground
                {
                    var FallRayAngle = Vector3.SignedAngle(FallRayCast.normal, animal.UpVector, animal.Right);

                  //  Debug.Log("FallRayAngle = " + FallRayAngle);
                   // Debug.Log("animal.maxAngleSlope + animal.m_deepSlope = " + (FallRayAngle < 0 && FallRayAngle < animal.NegativeMaxSlope));
                    
                    var DeepSlope =
                        ( FallRayAngle > 0 && FallRayAngle > animal.maxAngleSlope)
                        || FallRayAngle < 0 && FallRayAngle < animal.NegativeMaxSlope; //Slope Error

                    if (!DeepSlope) //Check if we are not on a deep slope
                    {
                        AllowExit();
                        animal.CheckIfGrounded();

                        animal.Grounded = true;
                        animal.UseGravity = false;

                        var GroundedPos = Vector3.Project(FallRayCast.point - animal.transform.position, Gravity);  //IMPORTANT HACk FOR when the Animal is falling to fast

                        if (DeltaDistance > 0.1f)
                        {
                            animal.Teleport_Internal(animal.transform.position + GroundedPos); //SUPER IMPORTANT!!! this is when the Animal is falling from a great height
                            animal.ResetUPVector(); //IMPORTANT!
                        }
                        Debugging($"[Try Exit] (Grounded) + [Terrain Angle ={FallRayAngle}]. DeltaDist: {DeltaDistance:F3}");
                        animal.InertiaPositionSpeed = Vector3.ProjectOnPlane(animal.RB.velocity * DeltaTime, animal.UpVector); //This is for Helping on Slopes
                        return;
                    }
                    else
                    {
                        FallCurrentDistance = 0;
                        return; //Do not check if the rigidbody has Increase Velocity
                    }
                }
            }

            ResetRigidbody(DeltaTime, Gravity);
        }


        public override void ExitState()
        {
            var status = 0;
            if (landStatus != null && landStatus.Length >= 1)
            {

                foreach (var ls in landStatus)
                    if (ls.x < FallCurrentDistance) status = (int)ls.y;

            }
            SetExitStatus(status);  //Set the Landing Status!! IMPORTANT for Multiple Landing Animations

            if (AffectStat != null && animalStats != null
                && FallCurrentDistance > FallMinDistance.Value && animal.Grounded) //Meaning if we are on the safe minimun distance we do not get damage from falling
            {
                var StatFallValue = (FallCurrentDistance) * 100 / FallMaxDistance;
                animalStats.Stat_ModifyValue(AffectStat, StatFallValue, StatOption.ReduceByPercent);
            }
            base.ExitState();
        }


     

        private void ResetRigidbody(float DeltaTime, Vector3 Gravity)
        {
            if (StuckAnimal && GoingDown)
            {
                //Debug.Log("GoinDown");

                var RBOldDown = Vector3.Project(animal.RB.velocity, Gravity);
                var RBNewDown = Vector3.Project(animal.DesiredRBVelocity, Gravity);
                var NewDMagn = RBNewDown.magnitude;
                var Old_DMagn = RBOldDown.magnitude;

                MTools.Draw_Arrow(animal.Main_Pivot_Point+Forward*0.02f, RBOldDown*0.5f,Color.red);
                MTools.Draw_Arrow(animal.Main_Pivot_Point + Forward * 0.04f, RBNewDown*0.5f, Color.green);

                ResetCount++;



                if (NewDMagn == Old_DMagn) return;

                if ( NewDMagn > (Old_DMagn * Old_DMagn) &&  //New Desired Velocity is greatere 
                    Old_DMagn < 0.1f && 
                    ResetCount > 5) //5 seems to be good
                {
                    if (animal.DesiredRBVelocity.magnitude > Height)
                    {
                        Debugging($"Reset Rigidbody Velocity. Animal may be stuck");

                        animal.ResetUPVector();
                        animal.GravityTime = animal.StartGravityTime;

                        if (PushForward > 0)
                            animal.InertiaPositionSpeed = animal.Forward * animal.ScaleFactor * DeltaTime * PushForward;  //Force going forward HACK

                        ResetCount = 0;
                    }
                }
            }
            //else
            //{
            //    //ResetCount = 0;
            //}
        }

        /// <summary> This is for cleaning the Ribidbody with unnecesary velocity </summary>
        private int ResetCount;

        public override void ResetStateValues()
        {
            DistanceToGround = float.PositiveInfinity;
            GoingDown = false;
            IsDebree = false;
            FallSpeed = new MSpeed();
            FallRayCast = new RaycastHit();
            GameObjectHit = null;
            FallHits = new RaycastHit[RayHits];
            UpImpulse = Vector3.zero;
            MaxHeight = float.NegativeInfinity; //Resets MaxHeight
            FallCurrentDistance = 0;
            Fall_Float = 0; //IMPORTANT
        }


        public override void StateGizmos(MAnimal animal)
        {
            if (!Application.isPlaying)
            {
                var fall_Pivot = animal.Main_Pivot_Point + (animal.Forward * Offset * animal.ScaleFactor); //Calculate ahead the falling ray
                float Multiplier = animal.Pivot_Multiplier * lengthMultiplier;

                Debug.DrawRay(fall_Pivot, animal.Gravity.normalized * Multiplier, Color.magenta);
            }
        }

#if UNITY_EDITOR


        public override void SetSpeedSets(MAnimal animal)
        {
            //Do nothing... the Fall is an automatic State, the Fall Speed is created internally
        }



        /// <summary>This is Executed when the Asset is created for the first time </summary>
        private void Reset()
        {
            ID = MTools.GetInstance<StateID>("Fall");
            General = new AnimalModifier()
            {
                RootMotion = false,
                AdditivePosition = true,
                AdditiveRotation = true,
                Grounded = false,
                Sprint = false,
                OrientToGround = false,

                Gravity = true,
                CustomRotation = false,
                modify = (modifier)(-1),
            };

            LowerBlendDistance = 0.1f;
            MoveMultiplier = 0.1f;
            lengthMultiplier = 1f;

            FallSpeed.name = "FallSpeed";

            //SleepFromState = new System.Collections.Generic.List<StateID>() {   MTools.GetInstance<StateID>("Fly") };

            ExitFrame = false; //IMPORTANT
        }
#endif
    }
}