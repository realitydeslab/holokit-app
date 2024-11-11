using UnityEngine;
using System.Collections;
using MalbersAnimations.Weapons;
using MalbersAnimations.Utilities;
using System.Collections.Generic;
using System;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations
{
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// LOGIC
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    public partial class MWeaponManager
    {
        #region INITIALIZE
        /// <summary>Get all the Animator Parameters the Animal Controller has</summary>
        private List<int> animatorHashParams;
        protected virtual void Awake()
        {
            if (Anim == null)
                Anim = this.FindComponent<Animator>();                                       //Get the Animator 

            Aimer = this.FindInterface<IAim>();
            Rider = this.FindInterface<IRider>();
            MInput = this.FindInterface<IInputSource>();

            DefaultAnimUpdateMode = Anim.updateMode;

            defaultAimSide = Aimer.AimSide;

            StoreAfterTime = new WaitForSeconds(StoreAfter.Value);

            GetHashIDs();

            Head = Anim.GetBoneTransform(HumanBodyBones.Head);                     //Get the Rider Head transform
            Chest = Anim.GetBoneTransform(HumanBodyBones.Chest);                   //Get the Rider Head transform

            RightHand = Anim.GetBoneTransform(HumanBodyBones.RightHand);           //Get the Rider Right Hand transform
            LeftHand = Anim.GetBoneTransform(HumanBodyBones.LeftHand);             //Get the Rider Left  Hand transform

            RightShoulder = Anim.GetBoneTransform(HumanBodyBones.RightUpperArm);   //Get the Rider Right Shoulder transform
            LeftShoulder = Anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);     //Get the Rider Left  Shoulder transform }

            if (UseHolsters) ActiveHolster = holsters[0]; //Set the Default Holster to be the first one.
        }
        private void OnEnable()
        {
            //Connect to the animator
            SetBoolParameter += SetAnimParameter;
            SetIntParameter += SetAnimParameter;
            SetFloatParameter += SetAnimParameter;
            SetTriggerParameter += SetAnimParameter;

            if (HasAnimal)
            {
                animal.OnModeStart.AddListener(AnimalModeStart);
                animal.OnModeEnd.AddListener(AnimalModeEnd);
                animal.OnStateActivate.AddListener(AnimalStateActivate);
                animal.OnStrafe.AddListener(CheckStrafing);
            }

            if (Rider != null) Rider.RiderStatus += GetRiderStatus;        //Get the notifications from the Rider
            if (MInput != null) ConnectInput(MInput, true);                //Connect the inputs from the Input Source

            IsRiding = MountingDismounting = false;

            ResetWeaponManager();
        }
        private void OnDisable()
        {
            if (CombatMode)
            {
                UnEquip_Fast();
                animal.Mode_Interrupt();
            }

            if (HasAnimal)
            {
                animal.OnModeStart.RemoveListener(AnimalModeStart);
                animal.OnModeEnd.RemoveListener(AnimalModeEnd);
                animal.OnStateActivate.RemoveListener(AnimalStateActivate);
                animal.OnStrafe.RemoveListener(CheckStrafing);
            }


            if (Rider != null) Rider.RiderStatus -= GetRiderStatus;    //Disconnect the notifications from the Rider
            if (MInput != null) ConnectInput(MInput, false);           //Disconnect the inputs from the Input Source

            //Disconnect from the Animator
            SetBoolParameter -= SetAnimParameter;
            SetIntParameter -= SetAnimParameter;
            SetFloatParameter -= SetAnimParameter;
            SetTriggerParameter -= SetAnimParameter;


        }



        /// <summary>Sets IsInCombatMode=false, ActiveAbility=null,WeaponType=None and Resets the Aim Mode. DOES **NOT** RESET THE ACTION TO NONE
        /// This one is Used Internally... since the Action will be set by the Store and Unequip Weapons</summary>
        public virtual void ResetCombat()
        {
            WeaponType = 0;
            WeaponAction = Weapon_Action.None;
            Weapon?.ResetWeapon();
            CombatMode = Aim = false;
            OnCanAim.Invoke(false);
            ExitAim();
           // UnEquip_Fast();
            Debugging($"Reset Combat");
        }

        private void ResetWeaponManager()
        {
            PrepareAnimalController();

            if (UseHolsters) PrepareHolsters(); //Prepare the holster if we are using holsers

            SmoothEquip = true;

            if (startWeapon.Value != null)
            {
                var StartWComponent = startWeapon.Value.GetComponent<MWeapon>();

                if (StartWComponent == null)
                {
                    Debug.LogWarning("The Start Weapon does not contain a MWeapon Component. Equiping weapon on start will be ignored.");
                    return;
                }

                if (startWeapon.Value.IsPrefab())
                {
                    Weapon = GameObject.Instantiate(StartWComponent);
                    Weapon.name = Weapon.name.Replace("(Clone)", "");

                    Debugging("[Start Weapon Instantiated]", "orange");
                }
                else
                {
                    Debugging("[Start Weapon Equiped]", "orange");
                    Weapon = StartWComponent;
                }

                if (Weapon)
                {
                    Holster_SetActive(Weapon.HolsterID); //Set the Active Holster the Weapon One
                    ActiveHolster?.SetWeapon(Weapon);
                    Equip_Fast();
                    Weapon.IsCollectable?.Pick();
                    AutoStoreWeapon();
                }
            }
            else
            {
                //Set to unarmed Combo ID
                comboManager?.SetActiveCombo(UnarmedModeID);
            }
        }
        protected virtual void GetHashIDs()
        {
            animatorHashParams = new List<int>();

            foreach (var parameter in Anim.parameters)
            {
                animatorHashParams.Add(parameter.nameHash);
            }

            Hash_LeftHand = TryGetAnimParameter(m_LeftHand);
            Hash_IKAim = TryGetAnimParameter(m_IKAim);              //Get Aim IK
            Hash_IKFreeHand = TryGetAnimParameter(m_IKFreeHand);    //Get Free/Unused Hand IK    


            Hash_WType = TryGetAnimParameter(m_WeaponType);

            //MODE STUFFS (WITHOUT ANIMAL)
            hash_Mode = TryGetAnimParameter(m_Mode);
            hash_ModeOn = TryGetAnimParameter(m_ModeOn);
            Hash_WPower = TryGetAnimParameter(m_WeaponPower);

            //Hash_WAction = TryGetAnimParameter(m_WeaponAction);
        }
        private void PrepareAnimalController()
        {
            if (HasAnimal)  //Get all the Modes the animal may have 
            {
                DrawMode = animal.Mode_Get(DrawWeaponModeID);
                StoreMode = animal.Mode_Get(StoreWeaponModeID);
                UnArmedMode = animal.Mode_Get(UnarmedModeID);
            }
            else comboManager = null; //Safe remove the Combo Manager too if there's no animal
        }



        #endregion

        /// <summary>  store if the animal was strafing </summary>
        void CheckStrafing(bool value) => WasStrafing = value;

        #region UPDATE FIXED UPDATE


        void FixedUpdate()
        {
            WeaponCharged(Time.fixedDeltaTime);
        }

        void LateUpdate()
        {
            if (CombatMode && WeaponIsActive)
                Weapon.Weapon_LateUpdate(this);         //If there's an Active Ability do the Late Ability thingy
        }
        #endregion

        #region Rider
        /// <summary>  Gets notify when the Rider Mount Dismount the Horse </summary>
        private void GetRiderStatus(RiderAction status)
        {
            var newRiding = status == RiderAction.EndMount;
            MountingDismounting = status == RiderAction.StartMount || status == RiderAction.StartDismount;

            //RECHECK EVERYTHING SO THE WEAPONS CHANGE VALUE TO RIDING
            if (IsRiding != newRiding)
            {
                IsRiding = newRiding;

                if (CombatMode)
                {
                    comboManager?.SetActiveCombo(IsRiding ? Weapon.RidingCombo : Weapon.GroundCombo);
                    CheckReinHandsEquip();
                }
            }

            if (CombatMode)
            {
                if (status == RiderAction.StartMount || status == RiderAction.EndMount) //If is Mounting or Is Riding??
                {
                    WeaponType = Weapon.RidingArmPose ? Weapon.WeaponType : 0;
                }
                else if (status == RiderAction.EndDismount) //If it has finished dismounting (GROUNDED)!!!
                {
                    WeaponType = Weapon.GroundArmPose ? Weapon.WeaponType : 0;  //Set the Weapon Type On Ground
                    comboManager?.SetActiveCombo(Weapon.GroundCombo);
                    SetWeaponStance();

                    if (Aim && Weapon.StrafeOnAim) animal.Strafe = true;        //Restore the Strafing 
                }
            }
        }

        #endregion

        #region Reins
        private void CheckReinHandsEquip()
        {
            if (Rider != null && Weapon != null)
            {
                if (Weapon.IsRightHanded) Rider.ReinRightHand(false);
                else Rider.ReinLeftHand(false);
            }
        }

        public void GrabReinsBothHands()
        {
            if (Rider != null)
            {
                Rider.ReinLeftHand(true);
                Rider.ReinRightHand(true);
            }
        }
        public void ReleaseReinsFromHands()
        {
            if (Rider != null)
            {
                Rider.ReinLeftHand(false);
                Rider.ReinRightHand(false);
            }
        }


        /// <summary>Use the Free Hand on the Weapon</summary>
        public void FreeHandUse()
        {
            Weapon?.FreeHandUse();
            ReleaseReinsFromHands();
        }

        /// <summary>Release the Free Hand </summary>
        public void FreeHandRelease()
        {
            Weapon?.FreeHandRelease();
            CheckReinHandsEquip();
        }
        #endregion

        #region IK Weapons

        void OnAnimatorIK()
        {
            if (!Anim.isHuman) return; //this only works for Humans

            if (MountingDismounting) return; //Do not do any IK while mounting Dismounting

            if (CombatMode && WeaponIsActive && Anim.isHuman)
            {
                Do_Aim_IK();
                Do_2Hands_IK();
            }
        }

        private void Do_Aim_IK()
        {
            if (Weapon.AimI)
            {
                if (Hash_IKAim != 0) IKAimWeight = Anim.GetFloat(Hash_IKAim);
                if (IKAimWeight != 0) Weapon.AimI.ApplyOffsets(Anim, Aimer.AimOrigin.position, AimDirection, IKAimWeight);
            }
        }

        private void Do_2Hands_IK()
        {
            //REMEMBER TO SET THE WEAPON IK THAT IS NOT WORKING WHEN DRAWING A WEAPON
            if (Weapon.TwoHandIK && Weapon.IKHandPoint.Value)
            {
                if (Hash_IKAim != 0) IK2HandsWeight = Anim.GetFloat(Hash_IKFreeHand);

                if (IK2HandsWeight != 0)
                {
                    var ikGoal = !Weapon.IsRightHanded ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;

                    Anim.SetIKPosition(ikGoal, Weapon.IKHandPoint.position);
                    Anim.SetIKPositionWeight(ikGoal, IK2HandsWeight);

                    Anim.SetIKRotation(ikGoal, Weapon.IKHandPoint.rotation);
                    Anim.SetIKRotationWeight(ikGoal, IK2HandsWeight);
                }
            }
        }
        #endregion

        #region AIMING
        public bool AimingSide => Aimer.AimingSide;


        /// <summary>Is the Rider Aiming?</summary>
        public bool Aim
        {
            private set
            {
                if (!WeaponIsActive) return; //Do nothing if the weapon is not active
                if (!Weapon.CanAim) return;  //Do nothing if the weapon cannot aim

                if (aim != value)
                {
                    aim.Value = value; //Do Store the Value of the Aiming

                    //Let know the Rider is Aiming. So if is using Straigth Spine, it stops. (REVIEW)!!!!!!!!!***
                    if (Rider != null) Rider.IsAiming = value;

                    Debugging($"Aim → [{aim.Value}]", "gray");

                    Weapon.IsAiming = value;    //Update the Aim Value on the Weapon to the active weapon  that the Rider is/isn't aiming

                    SetAimLogic(value);
                }
            }
            get => aim.Value;
        }

        public virtual void Aim_Set(bool value) => Aim = value;

        public void SetAimLogic(bool value)
        {
            aim.Value = value; //Do Store the Value of the Aiming

            //Let know the Rider is Aiming. So if is using Straigth Spine, it stops. (REVIEW)!!!!!!!!!***
            if (Rider != null) Rider.IsAiming = value;

            Debugging($"Aim → [{value}]", "gray");

            if (Weapon) Weapon.IsAiming = value;    //Update the Aim Value on the Weapon to the active weapon  that the Rider is/isn't aiming

            if (aim)
            {
                if (Weapon)
                {
                    Aimer.AimSide = Weapon.AimSide;         //Send to the Aimer the Corret Side.
                                                            //Enable Strafing if the Weapon Need if the animal was not strafing and the weapon need it
                    if (!WasStrafing && HasAnimal && Weapon.StrafeOnAim)
                    {
                        animal.Strafe = true;
                        WasStrafing = false;
                    }




                }

                //DO NOT AIM IF THE ANIMAL IS DODGING or doing a high priority mode
                if (HasAnimal && WeaponMode != null && animal.IsPlayingMode && animal.ActiveMode.Priority > WeaponMode.Priority)
                { return; }


                //if We are not Reloading then we can set the Action Aim
                if (WeaponAction != Weapon_Action.Reload)
                    WeaponAction = Weapon_Action.Aim;

                Aimer.Active = true;                                //Activate the Aimer (Invoke the Side Events)
            }
            else
            {
                //if We are not Reloading then we can set the Action Aim
                if (WeaponAction != Weapon_Action.Reload)
                {
                    WeaponAction = CombatMode ? Weapon_Action.Idle : Weapon_Action.None;
                }

                ExitAim();
            }
        }

        /// <summary>This will recieve the messages Animator Behaviors the moment the rider make an action on the weapon</summary>
        public virtual void CheckAim()
        {
            WeaponAction = Aim ? (Weapon_Action.Aim) : CombatMode ? Weapon_Action.Idle : Weapon_Action.None;
        }

        /// <summary>Exit the Aiming Logic </summary>
        public void ExitAim()
        {
            //Disable Straffing
            if (HasAnimal && Weapon && Weapon.StrafeOnAim && !WasStrafing) animal.Strafe = false;

            Aimer.ExitAim();
        }


        /// <summary> Returns the Normalized Angle Around the Y Axis (from -180 to 180) regarding the Target position</summary>
        public float HorizontalAngle => Aimer.HorizontalAngle;
        #endregion

        #region Weapon Action Stuff

        /// <summary>  DO NOT Interrupt Higher Priority Modes (Check if the Animal is Playing a Higher Priority Mode)  </summary>  
        protected virtual bool HigherPriorityMode => WeaponMode != null && animal.IsPlayingMode && animal.ActiveMode.Priority > WeaponMode.Priority;


        /// <summary>Which Action is currently using the RiderCombat. See WeaponActions Enum for more detail</summary>
        public Weapon_Action WeaponAction
        {
            get => weaponAction;
            set
            {
                // if (weaponAction != value) //Do it only when the value is different (Maybe there's) a problem with the attacks I think?
                {
                    // lastWeaponAction = weaponAction; //Store the last weapon action

                    weaponAction = value;
                    Debugging($"[Weapon Action] -> [{value}] - [{(int)value}]", "green");

                    JustChangedAction = true;
                    this.Delay_Action(() => JustChangedAction = false); //reset it the next frame

                    switch (weaponAction)
                    {
                        case Weapon_Action.None:
                            GrabReinsBothHands();
                            break;
                        case Weapon_Action.Idle:
                            DoIdleWeaponAnims();
                            AutoStoreWeapon();
                            break;
                        case Weapon_Action.Attack:
                            DoWeaponAttackAnims();
                            break;
                        case Weapon_Action.Draw:
                            TryDrawWeaponAnims();
                            break;
                        case Weapon_Action.Store:
                            TryStoreWeaponAnims();
                            break;
                        case Weapon_Action.Aim:
                            DoAimAnimations();
                            break;
                        case Weapon_Action.Reload:
                            DoReloadAnimations();
                            break;
                        default:
                            break;
                    }

                    OnWeaponAction.Invoke((int)weaponAction);

                    if (StoreAfter.Value > 0)
                    {
                        if (IStoreAfter != null) StopCoroutine(IStoreAfter);
                        if (weaponAction == Weapon_Action.Idle) IStoreAfter = StartCoroutine(C_StoreAfter());
                    }

                    if (weaponAction == Weapon_Action.None) GrabReinsBothHands(); //THIS?!?!?!?!?!?!? test atfter
                }
            }
        }


        /// <summary>  RECHECK THIS I BELIEVE I NEED TO DO MORE ?!?! </summary>
        private void DoIdleWeaponAnims()
        {
            Weapon.IsReloading = false; //Reset The Reloading since is on the Idle

            if (!HasAnimal) //If we are not using an animal ?
            {
                CustomWeaponAction(0, 0);
            }
            else if (WeaponMode != null)
            {
                // Debug.Log("IDLE = " + WeaponMode.Name);
                animal.Mode_Interrupt();
                WeaponMode.InputValue = false; //Make sure the Input value is set to false
            }
        }

        //Remember to check While Riding
        private void DoAimAnimations()
        {
            if (CombatMode && Weapon.CanAim)
            {
                if (HasAnimal)
                {
                    if (HigherPriorityMode) return; //Avoid Forcing a new Mode if the Animal is Rolling or Dodging.... Doing a Higher Mode.
                    WeaponMode.ForceActivate((int)Weapon_Action.Aim);
                }
                else
                {
                    CustomWeaponAction(Weapon.WeaponType.ID, (int)Weapon_Action.Aim);
                }
            }
        }

        private void DoReloadAnimations()
        {
            if (HigherPriorityMode) return; //Avoid Forcing a new Mode if the Animal is Rolling or Dodging.... Doing a Higher Mode.

            if (!IsReloading)
            {
                if (HasAnimal)
                {
                    WeaponMode.ForceActivate((int)Weapon_Action.Reload);
                    // Debug.Log("FORCE RELOAD ANIMATION" + WeaponMode.Name);
                }
                else
                {
                    CustomWeaponAction(Weapon.WeaponType.ID, (int)Weapon_Action.Reload);
                }
            }
        }


        /// <summary> Try Activate the Holster  DRAW Animation for the Weapon </summary>
        public virtual void TryDrawWeaponAnims()
        {
            if (HasAnimal)
            {
                if (DrawMode != null)
                    DrawMode.ForceActivate(Weapon.HolsterAnim);
                else
                    Equip_Fast();
            }
            else
            {
                CustomWeaponAction((int)Weapon_Action.Draw, Weapon.HolsterAnim);
            }
        }
        /// <summary> Try Activate the Holster STORE Animation for the Weapon </summary>
        public virtual void TryStoreWeaponAnims()
        {
            if (HasAnimal)
            {
                if (StoreMode != null)
                    StoreMode.ForceActivate(Weapon.HolsterAnim);
                else
                    UnEquip_Fast();
            }
            else
            {
                CustomWeaponAction((int)Weapon_Action.Store, Weapon.HolsterAnim);
            }
        }

        /// <summary> Store the value sent to the Animator </summary>
        public int WeaponAnimAction { get; set; }

        private void CustomWeaponAction(int mode, int value)
        {
            SetTriggerParameter(hash_ModeOn); //Set Directly the Mode to 0
            WeaponAnimAction = mode * 1000 + value;
            SetIntParameter(hash_Mode, WeaponAnimAction); //Set Directly the Mode to 0
        }

        public void SetWeaponCharge(float Charge)
        {
            var RealCharge = Charge * Weapon.ChargeCharMultiplier;

            if (HasAnimal)
            { animal.Mode_SetPower(RealCharge); }
            else
            {
                SetFloatParameter(Hash_WPower, RealCharge);
            }
        }


        /// <summary> If Auto Store weapon is enabled... do it</summary>
        protected virtual void AutoStoreWeapon()
        {
            if (StoreAfter <= 0) return; //Ignore is Store After is 0

            if (IStoreAfter != null) StopCoroutine(IStoreAfter);
            IStoreAfter = null;
            IStoreAfter = StartCoroutine(C_StoreAfter()); //Start Coroutine Store After
        }

        protected virtual void DoWeaponAttackAnims()
        {
            if (Weapon is MMelee)
            {
                var WeaponMelee = Weapon as MMelee;

                //Check if the weapon can play a Combo 
                if (comboManager && comboManager.ActiveCombo != null)
                {
                    Debugging($"[Melee Attack] → [{Weapon.name} <AC>]. Combo[{comboManager.ActiveCombo.Name}] Branch: [{comboManager.Branch}]", "orange");

                    if (comboManager.TryPlay())
                    {
                        Weapon.CanAttack = true; //Reset the Attack Rate... don't use Default Attack Rate
                    }
                    else
                    {
                        Debugging($"[Melee Attack] → [{Weapon.name} <AC>] <COMBO FAILED>", "red");
                        Action((int)Weapon_Action.Idle); //Combo Failed
                    }
                }
                else
                {
                    // Debug.Log("Weapon.CanAttack = " + Weapon.CanAttack);

                    if (Weapon.CanAttack) //Check Weapon Rate
                    {
                        if (WeaponMelee.RidingAttackAbilities == null)
                        { Debug.LogWarning($"The Weapon {Weapon.name} does not have Riding Attack Abilities", this); return; }

                        if (WeaponMelee.GroundAttackAbilities == null)
                        { Debug.LogWarning($"The Weapon {Weapon.name} does not have Riding Attack Abilities", this); return; }

                        var random = UnityEngine.Random.Range(0,
                               IsRiding ? WeaponMelee.RidingAttackAbilities.Length : WeaponMelee.GroundAttackAbilities.Length);

                        random = IsRiding ? WeaponMelee.RidingAttackAbilities[random] : WeaponMelee.GroundAttackAbilities[random];

                        if (HasAnimal)
                        {
                            if (IsRiding)
                            {
                                if (WeaponMelee.UseCameraSide)
                                {
                                    random *= Aimer.AimingSide ? -1 : 1;
                                    if (WeaponMelee.InvertCameraSide) random *= -1;
                                }
                            }

                            if (WeaponMode.ForceActivate(random))
                            {
                                Debugging($"[Melee Attack] → [{Weapon.name} <AC>] <NO Combo>", "orange");
                                Weapon.CanAttack = false;
                                // WeaponMode.InputValue = false; //Needs to reset the weapon Input Mode (For Charging Modes)
                            }
                            else
                            {
                                Action((int)Weapon_Action.Idle); //Mode Failed
                                Debugging($"[Melee Attack] → [{Weapon.name} <AC>] <MODE FAILED>", "gray");
                            }
                        }
                        else
                        {
                            CustomWeaponAction(weaponType, random);
                        }
                    }
                }
            }
            else if (Weapon is MShootable)
            {
                if (HasAnimal)
                {
                    WeaponMode.ForceActivate((int)Weapon_Action.Attack); //DO the Weapon Attack Animation
                    Debugging($"[Fire Projectile] [AC] → [{Weapon.name} <with AC>]", "orange");
                }
                else
                {
                    CustomWeaponAction(Weapon.WeaponType, (int)Weapon_Action.Attack);
                }
            }
        }


        private void AnimalStateActivate(int state)
        {
            if (CombatMode)
            {
                //Store& unequip Weapon if the animal is on any of these states.
                if (ExitOnState.Contains(animal.ActiveStateID))
                {
                    ExitByState = true;

                    if (ExitFast)
                        UnEquip_Fast();
                    else
                        Store_Weapon();
                }
                else
                {
                    ExitByState = false;
                }
            }

            if (ExitByState && UseHolsters && !ExitOnState.Contains(animal.ActiveStateID))
            {
                Weapon = ActiveHolster.Weapon;  //Get the new Weapon from the Holster

                if (ExitFast)
                    Equip_Fast();
                else
                    Draw_Weapon();

                ExitByState = false;
            }

        }

        private bool ExitByState;
        private bool ExitByMode;


        private void AnimalModeStart(int ModeID, int ablility)
        {
            if (CombatMode)
            {  //Store& unequip Weapon if the animal is on any of these states.
                if (ExitOnModes.Contains(animal.ActiveMode.ID))
                {
                    ExitByMode = true;
                    UnEquip_Fast();
                }
                else
                {
                    ExitByMode = false;
                }
            }
        }


        private void AnimalModeEnd(int ModeID, int ablility)
        {
            if (animal.IsPreparingMode) return; //Do not change if we are already Doing a Mode
            if (JustChangedAction) return;      //Do not change back to Aim until attack finishes

            if (animal.ActiveMode != WeaponMode) //Meaning is other Mode not the Weapon Mode so check if we were Aiming?
                CheckAim();

            if (ExitByMode && UseHolsters/* && !ExitOnModes.Exists(x => x.ID == ModeID)*/)
            {
                Weapon = ActiveHolster.Weapon;  //Get the new Weapon from the Holster
                Equip_Fast();
                ExitByMode = false;
            }
        }


        #endregion

        #region Draw Store Equip Unequip Weapons 
        public virtual void Equip_Fast()
        {
            SmoothEquip = false;
            Equip_Weapon();
        }

        /// <summary>Equip Weapon from holster or from Inventory  (Called by the Animator)</summary>
        public virtual void Equip_Weapon()
        {
            //DO NOT Equip is the Active state does not allow it
            if (HasAnimal && ExitOnState.Contains(animal.ActiveStateID)) return;

            if (!Active) return;
            if (Weapon == null) return;
            //if (Weapon.gameObject.IsPrefab()) return;   //Means the Weapon is a prefab and is not instantiated yet (MAKE A WAIT COROUTINE????)

            if (!Weapon.Enabled)
            {
                Debugging("The weapon is Disabled. It cannot be equipped");
                return;
            }


            Debugging($"EQUIP → [{Weapon.name}] T:{Time.time:F2}", "orange");

            ExitAim();

            Equip_Weapon_Data_Ground_Riding();
            EquipWeapon_AnimalController();

            CheckAim();

            //WeaponAction = Weapon_Action.Idle;                          //Set the Action to Equip
            CombatMode = true;
            Weapon.Equip(this);
            OnEquipWeapon.Invoke(Weapon.gameObject);                    //Let everybody know that the weapon is equipped

            OnCanAim.Invoke(Weapon.CanAim);

            Weapon.PlaySound(WSound.Equip);                             //Play Equip Sound

            CheckReinHandsEquip();
            ParentWeapon();

            if (UseHolsters)                                                             //If Use holster Means that the weapons are on the holster
            {
                var Offset = Weapon.IsRightHanded ? Weapon.RightHandOffset : Weapon.LeftHandOffset; //Store the HandOffset

                if (IgnoreHandOffset.Value)
                {
                    Offset = new TransformOffset(0)
                    {
                        Scale = Weapon.transform.localScale
                    };
                }

                if (SmoothEquip)
                {
                    //Smoothly put the weapon in the hand
                    StartCoroutine(
                        MTools.AlignTransformLocal(Weapon.transform, Offset.Position, Offset.Rotation, Offset.Scale, HolsterTime));
                }
                else
                {
                    //Local position when is Parent to the weapon
                    Weapon.transform.SetLocalTransform(Offset.Position, Offset.Rotation, Offset.Scale);
                }

                SmoothEquip = true;
            }
            else //if (UseExternal)                           //If Use Inventory means that the weapons are on the inventory
            {
                //Apply the Offset Hand Value to the 
                if (!IgnoreHandOffset.Value)
                {
                    Weapon.ApplyOffset();
                }
                else
                {
                    Weapon.transform.localPosition = Vector3.zero;
                    Weapon.transform.localRotation = Quaternion.identity;
                    //Do not change the scale
                }
            }

            Weapon.gameObject.SetActive(true);            //Set the Game Object Instance Active    
        }



        /// <summary>  Set the proper values for the Weapon while is grounded or Riding  </summary>
        protected void Equip_Weapon_Data_Ground_Riding()
        {
            if (!IsRiding) //GROUNDED
            {
                WeaponType = Weapon.GroundArmPose ? Weapon.WeaponType : 0;
                comboManager?.SetActiveCombo(Weapon.GroundCombo);
            }
            else //RIDING
            {
                WeaponType = Weapon.RidingArmPose ? Weapon.WeaponType : 0;
                comboManager?.SetActiveCombo(Weapon.RidingCombo);
            }
        }

        private void EquipWeapon_AnimalController()
        {
            if (HasAnimal)
            {
                SetWeaponStance();

                WeaponMode = animal.Mode_Get(Weapon.WeaponType);               //Cache the Weapon Mode, if it has one

                //Disable all the modes included on the Disable Mode list
                EnableModesAC(false);

                if (WeaponMode != null)
                {
                    WeaponMode.SetActive(true); //Activate the Mode
                }
                else
                {
                    Debug.LogWarning("The Animal Controller does not have a mode for the Equipped Weapon!!");
                    Weapon.Enabled = false; //Disable the weapon... it cannot be used!!!
                }
            }
        }

        private void SetWeaponStance()
        {
            if (Weapon.stance)
                animal?.Stance_Set(Weapon.stance);           //Set the Stance to use on the Animal Controller
        }

        private void EnableModesAC(bool enable)
        {
            foreach (var m in DisableModes)
            {
                if (enable)
                    animal.Mode_Enable_Temporal(m);
                else
                    animal.Mode_Disable_Temporal(m);
            }
        }

        private bool JustChangedAction;
        /// <summary>Unequip Weapon from holster or from Inventory (Called by the Animator)</summary>
        public virtual void Unequip_Weapon()
        {
            ResetCombat();
            if (Weapon == null) return;
            Debugging($"UNEQUIP → [{Weapon.name}] T:{Time.time:F2}", "orange");  //Debug

            //Has_IKAim = false;
            IKAimWeight = 0;
            WeaponType = 0;                                                  //Set the weapon ID to None (For the correct Animations)
            OnUnequipWeapon.Invoke(Weapon.gameObject);                      //Let the rider know that the weapon has been unequiped.

            if (UseHolsters)                                                //If Use holster Parent the ActiveMWeapon the the holster
            {
                Weapon.transform.parent = ActiveHolster.GetSlot(Weapon.HolsterSlot);        //Parent the weapon to his original holster

                if (SmoothEquip)
                    StartCoroutine(MTools.AlignTransform(Weapon.transform, Weapon.HolsterOffset, HolsterTime));
                else
                    Weapon.transform.SetLocalTransform(Weapon.HolsterOffset); //Set the Holster Offset Option

                SmoothEquip = true;
            }
            else// if (UseExternal)
            {
                if (DestroyOnUnequip)
                    Destroy(Weapon.gameObject);
                //else
                //    Weapon.gameObject.SetActive(false);
            }

            UnequipWeapon_AnimalController();

            Weapon = null;     //IMPORTANT
        }

        /// <summary>Ignores the Draw and Store Animations</summary>
        public bool SmoothEquip;


        public void UnEquip() => UnEquip_Fast();

        public void UnEquip_Fast()
        {
            SmoothEquip = false; //Skip the Smooth Equipment.
            Unequip_Weapon();
        }


        public virtual void UnequipWeapon_AnimalController()
        {
            if (HasAnimal)
            {
                if (Weapon.stance != null && !IsRiding) animal.Stance_Reset(); //Reset Stance if the animal was using a stance for the weapon

                if (comboManager)  //Set to unarmed Combo ID
                    comboManager.SetActiveCombo(UnarmedModeID);


                //Enable Back all the modes included on the Disable Mode list
                EnableModesAC(true);

                foreach (var m in DisableModes)
                {
                    animal.Mode_Enable(m);
                }

                WeaponMode?.SetActive(false); //Disable Weapon Mode
                WeaponMode = null;
            }
        }


        /// <summary> Parents the Weapon to the Correct Hand</summary>
        public virtual void ParentWeapon()
        {
            if (Weapon.IsRightHanded && RightHandEquipPoint)  //Parent to the Right Hand Equip Point
            {
                Weapon.transform.parent = RightHandEquipPoint; //Parent to the 
            }
            else if (LeftHandEquipPoint)
            {
                Weapon.transform.parent = LeftHandEquipPoint;
            }
        }


        /// <summary> Draw (Set the Correct Parameters to play Draw Weapon Animation) </summary>
        public virtual void Draw_Weapon()
        {
            if (!Active) return;

            //DO NOT Equip is the Active state does not allow it
            if (HasAnimal && ExitOnState.Contains(animal.ActiveStateID)) return;

            ExitAim(); //DO NOT AIM WHEN DRAWING WEAPONS

            //If is using External Equip
            if (UseExternal)
            {
                //Set the Current holster to the weapon asigned holster (THE WEAPON IS ALREADY SET)
                // if (Weapon != null) Holster_SetActive(Weapon.HolsterID);
            }
            else //if (UseHolsters) 
                Weapon = ActiveHolster.Weapon;  //Get the new Weapon from the Holster


            if (Weapon)
            {
                if (Weapon.IgnoreDraw || IgnoreDraw)
                {
                    Equip_Fast();
                    return;
                }

                WeaponAction = Weapon_Action.Draw;

                CheckReinHandsEquip();

                Debugging($"Draw → {(Weapon.IsRightHanded ? "Right Hand" : "Left Hand")} → [{Weapon.Holster.name} → {Weapon.name}]", "yellow");  //Debug
            }
        }


        /// <summary>Store (Set the Correct Parameters to play Store Weapon Animation) </summary>
        public virtual void Store_Weapon()
        {
            if (Weapon == null) return;                    //Skip if there's no Active Weapon or is not inCombatMode, meaning there's an active weapon

            ExitAim();

            if (Weapon.IgnoreStore || IgnoreStore)
            {
                UnEquip_Fast();
                return;
            }

            WeaponAction = Weapon_Action.Store;                 //Set the  Weapon Action to Store Weapons 

            Debugging($"[Store → {(Weapon.IsRightHanded ? "Right Hand" : "Left Hand")}] → [{Weapon.Holster.name}] → [{Weapon.name}]", "cyan");  //Debug
        }


        public void ActivateDamager(int value, float multiplier)
        {
            if (Weapon) Weapon.ActivateDamager(value, multiplier);
        }



        private int LastAttackTriggerHash;
        public void DamagerAnimationStart(int hash)
        {
            LastAttackTriggerHash = hash;
        }

        public void DamagerAnimationEnd(int hash)
        {
            //Go to Idle because we are finishing in the same animations. if not is doing another attack before finishing the one that we have
            if (!HasAnimal && LastAttackTriggerHash == hash)
            {
                WeaponAction = Weapon_Action.Idle;
            }
        }




        /// <summary> Is called to swap weapons</summary>
        private IEnumerator SwapWeaponsHolster(int HolstertoSwap)
        {
            if (Weapon)
            {
                Store_Weapon();

                while (WeaponAction == Weapon_Action.Aim) yield return null;    // Wait for the weapon is Unequiped Before it can Draw Another
                while (WeaponAction == Weapon_Action.Store) yield return null;    // Wait for the weapon is Unequiped Before it can Draw Another
            }

            Holster_SetActive(HolstertoSwap);
            Draw_Weapon();                                  //Set the parameters so draw a weapon
            yield return null;
        }



        #endregion
    }
}