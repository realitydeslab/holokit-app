using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/glide")]
    public class Glide : State
    {
        public override string StateName => "Glide";

        [Header("Glide Parameters")]
        public FloatReference GravityDrag = new FloatReference(3);

        [Tooltip("Minimun Height required to activate the glide")]
        public FloatReference StartHeight = new FloatReference(0);

        [Header("Free Movement Parameters")]
        [Range(0, 90),Tooltip("Bank amount used when turning")]
        public float Bank = 30;

        [Range(0, 90), Tooltip("Limit to go Pitch")]
        public float PitchLimit = 0;



        [Range(0, 90), Tooltip("Bank amount used when turning while straffing")]
        public float BankStrafe = 0;
        [Range(0, 90), Tooltip("Limit to go Up and Down while straffing")]
        public float PitchStrafe = 0;

        [Tooltip("When Entering the Glide State... The animal will keep the Velocity from the last State if this value is greater than zero")]
        public FloatReference InertiaLerp = new FloatReference(1);
         
        [Tooltip("The animal will move forward while Gliding, without the need to push the W Key, or Move forward Input")]
        public BoolReference AlwaysForward = new BoolReference(false);
        private bool LastAlwaysForward;

        [Tooltip("The animal will change the Camera Input while the Animal is using this State")]
        public BoolReference UseCameraInput = new BoolReference(true);
        private bool LastUseCameraInput;


        //[Tooltip("If the Animal has a force applied to it Remove the Force with this acceleration")]
        //public FloatReference RemoveForce = new FloatReference(5);


        [Header("Landing")]
        [Tooltip("Layers to Land on")]
        public LayerMask LandOn = (1);
        [Tooltip("Ray Length multiplier to check for ground near")]
        public FloatReference CheckLandDistance = new FloatReference(3f);
        [Tooltip("Ray Length multiplier to check for ground and automatically land (increases or decreases the MainPivot Lenght for the Fall Ray")]
        public FloatReference LandDistance = new FloatReference(1f);
        [Tooltip("Minimum Distance to Clamp the State Float")]
        public FloatReference LowerBlendDistance = new FloatReference(0.5f);
        public FloatReference LerpDistance = new FloatReference(2);
         
        protected Vector3 verticalInertia;
        protected Vector3 DownPush;

        private bool CheckStartHeight()
        {
            if (StartHeight <= 0) return true; //if there's no height skip checking Start Height
            
            //if we touch any ground send False. Meaning the Glide cannot play.
            return !Physics.Raycast(animal.Main_Pivot_Point, animal.Gravity, out _, animal.Height + StartHeight * ScaleFactor, animal.GroundLayer) ;
               
        }

        public override bool TryActivate()
        {
            return base.TryActivate() && CheckStartHeight();
        }

        public override void Activate()
        {
            base.Activate();
            LastAlwaysForward = animal.AlwaysForward; //Store the last AlwaysFoward the animal had
            animal.AlwaysForward = AlwaysForward; //Set the new Always forward
            LastUseCameraInput = animal.UseCameraInput; //Cache the Last Use Camera Inptu
            animal.UseCameraInput = LastUseCameraInput; 
            InputValue = true; //Make sure the Input is set to True when the flying is not being activated by an input player


            if (animal.ExternalForceAcel == 0) animal.Force_Remove(0);
        } 

        public override void EnterCoreAnimation()
        {
            verticalInertia = Vector3.Project(animal.DeltaPos, animal.UpVector); //Find the Up Inertia to keep it while entering the Core Anim
            animal.PitchDirection = animal.Forward;
            animal.InertiaPositionSpeed = animal.HorizontalVelocity * animal.DeltaTime; //Calculate the current Inertia!!
        } 
       
        public override void OnStateMove(float deltaTime)
        {
            if (InCoreAnimation) //While is flying
            {
                var value = animal.ExternalForce != Vector3.zero ? Vector3.zero : (GravityDrag * animal.ScaleFactor) * deltaTime * Gravity;
                    DownPush = Vector3.Lerp(DownPush, value, deltaTime*InertiaLerp);
                    animal.AdditivePosition += DownPush; //Glide Push Down
                
                
                var limit = PitchLimit;
                var bank = Bank;

                if (animal.Strafe)
                {
                    limit = PitchStrafe;
                    bank = BankStrafe;
                }

                animal.CalculateBank(bank); //Calculate default Bank

                animal.PitchAngle =  Mathf.Lerp(
                    animal.PitchAngle, 
                    limit * animal.VerticalSmooth, 
                    deltaTime * animal.CurrentSpeedSet.PitchLerpOn); //Calculate Custom Pitch Angle not the default one for Fly and Underwater Swim

                animal.CalculateRotator(); //Calculate the Rotator Rotation.

                if (InertiaLerp.Value > 0) animal.AddInertia(ref verticalInertia, InertiaLerp);
            }
        }

        public override void TryExitState(float DeltaTime)
        {
            var Distance = CheckLandDistance.Value * animal.ScaleFactor;
            if (Distance <= 0) Distance = animal.Height;

            var NormalizedDistance = 1f;



            Debug.DrawRay(animal.Main_Pivot_Point, Gravity * Distance, Color.yellow);
            Debug.DrawRay(animal.Main_Pivot_Point, Gravity * LandDistance * animal.ScaleFactor, Color.green);

            if (Physics.Raycast(animal.Main_Pivot_Point, Gravity, out RaycastHit hit, Distance, LandOn, IgnoreTrigger))
            {
                if (LandDistance >= hit.distance)
                {
                    Debugging($"[AllowExit] Can Land on <{hit.collider.name}> ");
                    FlyAllowExit();
                    return;
                }
                else
                {
                    NormalizedDistance = Mathf.Clamp01(1 - LandDistance / (hit.distance - LowerBlendDistance));
                }
            }
            animal.State_SetFloat(NormalizedDistance, LerpDistance);
        }

        private void FlyAllowExit()
        {
            animal.FreeMovement = false; //Disable the Free Movement
            animal.UseGravity = true;
            AllowExit();
        }
      
        public override void ResetStateValues()
        {
            verticalInertia = Vector3.zero;
            DownPush = Vector3.zero;
            InputValue = false;
        }

        public override void RestoreAnimalOnExit()
        {
            animal.FreeMovement = false;
            animal.AlwaysForward = LastAlwaysForward;
            animal.UseCameraInput = LastUseCameraInput; 

            animal.Speed_Lock(false);
            animal.InputSource?.SetInput(Input, false); //Hack to reset the toggle when it exit on Grounded 
            animal.LockUpDownMovement = false;
        }

        public override void AllowStateExit()
        {
            base.InputValue = false;        //release the base Input value
            base.ExitInputValue = false;    //release the base Input value
        }

        public override bool InputValue //lets override to Allow exit when the Input Changes
        {
            get => base.InputValue;
            set
            {
                base.InputValue = value; 

                if (InCoreAnimation && IsActiveState && !value && CanExit) //When the Fly Input is false then allow exit
                {
                    AllowExit();
                }
            }
        }

#if UNITY_EDITOR

        public override void SetSpeedSets(MAnimal animal)
        {
            var setName = "Glide";

            if (animal.SpeedSet_Get(setName) == null)
            {
                animal.speedSets.Add(
                    new MSpeedSet()
                    {
                        name = setName,
                        StartVerticalIndex = new IntReference(1),
                        PitchLerpOn = new FloatReference(3),
                        PitchLerpOff = new FloatReference(3),
                        TopIndex = new IntReference(2),
                        states = new List<StateID>(1) { ID },
                        Speeds = new List<MSpeed>() { new MSpeed(setName), new MSpeed(setName + " Fast", 2, 4, 4) { animator = new FloatReference(1.33f) } }
                    }
                    );
            }
        }


        void Reset()
        {
            ID = MTools.GetInstance<StateID>("Glide");
            Input = "Glide";

            General = new AnimalModifier()
            {
                RootMotion = true,
                Grounded = false,
                Sprint = true,
                OrientToGround = false,
                CustomRotation = false,
                IgnoreLowerStates = true,
                Gravity = false,
                modify = (modifier)(-1),
                AdditivePosition = true, 
                AdditiveRotation = true, 
                FreeMovement = true, 
            };
        }

        public override void StateGizmos(MAnimal animal)
        {
            if (!Application.isPlaying)
            {
                var Gravity = animal.Gravity;
                var width = 1.5f;

                var LandDistance = Gravity.normalized * (this.LandDistance) * animal.transform.lossyScale.y;
                var CheckDistance = Gravity.normalized * (CheckLandDistance) * animal.transform.lossyScale.y;

                Gizmos.color = Color.yellow;
                MTools.DrawLine(animal.Main_Pivot_Point, animal.Main_Pivot_Point + CheckDistance, width);

                Gizmos.color = Color.green;
                MTools.DrawLine(animal.Main_Pivot_Point, animal.Main_Pivot_Point + LandDistance, width);
            }
        }
#endif
    }
}
