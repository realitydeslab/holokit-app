using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;
using System.Collections;
using MalbersAnimations.Scriptables;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Weapons
{
    [AddComponentMenu("Malbers/Weapons/Shootable")]
    public class MShootable : MWeapon, IShootableWeapon, IThrower
    {
        #region Variables
        public enum Release_Projectile { Never, OnAttackStart, OnAttackReleased, ByAnimation }
        public enum Cancel_Aim { ReleaseProjectile, ResetWeapon}
        ///<summary> Does not shoot projectile when is false, useful for other controllers like Invector and ootii to let them shoot the arrow themselves </summary>
        [Tooltip("When the Projectile is Release?")]
        public Release_Projectile releaseProjectile = Release_Projectile.OnAttackStart;

        ///<summary> When Aiming is Cancel what the Weapon should do? </summary>
        [Tooltip("When the Projectile is Release?")]
        public Cancel_Aim CancelAim = Cancel_Aim.ResetWeapon;

        [Tooltip("Projectile prefab the weapon fires")]
        public GameObjectReference m_Projectile = new GameObjectReference();                                //Arrow to Release Prefab
        [Tooltip("Parent of the Projectile")]
        public Transform m_ProjectileParent;
        public Vector3Reference gravity = new Vector3Reference(Physics.gravity);

        public BoolReference UseAimAngle = new BoolReference(false);
        public BoolReference HasReloadAnim = new BoolReference(false);
        public FloatReference m_AimAngle = new FloatReference(0);

        /// <summary>This Curve is for Limiting the Bow Animations while the Character is on weird/hard Positions</summary>
        [MinMaxRange(-180, 180)]
        [Tooltip("Value to limit firing projectiles when the Character is on weird or dificult Positions. E.g. Firing Arrows on impossible angles")]
        public RangedFloat AimLimit = new RangedFloat(-180, 180);

        

        [SerializeField] private IntReference m_Ammo = new IntReference(30);                             //Total of Ammo for this weapon
        [SerializeField] private IntReference m_AmmoInChamber = new IntReference(1);                     //Total of Ammo in the Chamber
        [SerializeField] private IntReference m_ChamberSize = new IntReference(1);                       //Max Capacity of the Ammo in once hit
        [SerializeField] private BoolReference m_AutoReload = new BoolReference(false);                  //Press Fire one or continues 

        #endregion

        #region Events
        public GameObjectEvent OnLoadProjectile = new GameObjectEvent();
        public GameObjectEvent OnFireProjectile = new GameObjectEvent();
        public UnityEvent OnReload = new UnityEvent();
        #endregion

        #region Properties
        public GameObject Projectile { get => m_Projectile.Value; set => m_Projectile.Value = value; }

        /// <summary> Projectile Instance to launch from the weapon</summary>
        public GameObject ProjectileInstance { get; set; }

        /// <summary> Projectile Interface Reference</summary>
        public IProjectile I_Projectile { get; set; }
        public Transform ProjectileParent => m_ProjectileParent;

        public bool InstantiateProjectileOfFire = true;

        public Vector3 Gravity { get => gravity.Value; set => gravity.Value = value; }

        /// <summary> Adds a Throw Angle to the Aimer Direction </summary>
        public float AimAngle { get => m_AimAngle.Value; set => m_AimAngle.Value = value; }
        public Vector3 Velocity { get; set; }
        public Action<bool> Predict { get; set; }

        /// <summary> Total Ammo of the Weapon</summary>
        public int TotalAmmo { get => m_Ammo.Value; set => m_Ammo.Value = value; }

        public int AmmoInChamber
        { 
            get => m_AmmoInChamber.Value;
            set 
            {
              //  Debug.Log("AmmoInChamber: " + value);
                m_AmmoInChamber.Value = value; 
            }
        }

        /// <summary>When the Ammo in Chamber gets to Zero it will reload Automatically</summary>
        public bool AutoReload { get => m_AutoReload.Value; set => m_AutoReload.Value = value; }

        public int ChamberSize { get => m_ChamberSize.Value; set => m_ChamberSize.Value = value; }

        public override bool HasAmmo => TotalAmmo == -1 || AmmoInChamber > 0;

        /// <summary>Aim IK Weight</summary>
        public float AimWeight { get; private set; }

        /// <summary>With Aim Limit?</summary>
        public bool CanShootWithAimLimit { get; private set; }

        
        public override bool IsEquiped
        {
            get => base.IsEquiped;
            set
            {
                base.IsEquiped = value;
                if (!value)
                    DestroyProjectileInstance(); //If by AnyChange the Projectile is live Destroy it!!
            }
        }

        public override bool IsAiming
        {
            get => base.IsAiming;
            set
            {
                base.IsAiming = value;

                if (!value)
                {
                    if (CancelAim == Cancel_Aim.ReleaseProjectile) //if the weapon is set to Cancel the Aim
                    {
                        Debugging("Release Projectile. [Cancel Aim] ", this);
                        IsCharging = true;
                        Owner?.SendMessage("MainAttack_Released", SendMessageOptions.DontRequireReceiver); //?!?!?
                    }
                    else
                    {
                        WeaponReady(false);                                 //if is not aiming then set Ready to false
                        ResetCharge();                                   //Reset the Charge of the wapon
                    }
                }
            }
        }


        #endregion
        public override bool CanAim => true;

        /// <summary>  Set the total Ammo (Refill when you got some ammo)  </summary>
        public void SetTotalAmmo(int value)
        {
            if (AutoReload) TryReload();
        }

        void Awake()
        {
            Initialize();

            if (ChamberSize < 0) ChamberSize = 1; //Bug Fix
        }

        private void OnEnable()
        {
            if (!m_Ammo.UseConstant && m_Ammo.Variable != null) //Listen the Total ammo in case it changes
                m_Ammo.Variable.OnValueChanged += SetTotalAmmo;
        }

        private void OnDisable()
        {
            if (!m_Ammo.UseConstant && m_Ammo.Variable != null)
                m_Ammo.Variable.OnValueChanged -= SetTotalAmmo;
        } 

        internal override void MainAttack_Start(IMWeaponOwner RC)
        {
            base.MainAttack_Start(RC);

           //  OneChamberAmmo();

              //  Debug.Log("IsAiming && CanAttack && IsReady");
            if (IsAiming && CanAttack && IsReady && CanShootWithAimLimit) //and the Rider is not on any reload animation
            {
                   // Debug.Log("HasAmmo");
                if (HasAmmo)                                                                  //If there's Ammo on the chamber
                {
                    //Means the Weapon does not need to Charge  so Release the Projectile First!
                    if (!CanCharge || releaseProjectile == Release_Projectile.OnAttackStart)
                    {
                        //  Debug.Log("Does not need charge");
                       // CalculateAimLimit(RC);


                        Debugging($"<color=white> Weapon <b>[Fire Projectile No Charge] </b></color>", this);  //Debug
                        WeaponAction.Invoke((int)Weapon_Action.Attack);

                        if (releaseProjectile == Release_Projectile.OnAttackStart)
                            ReleaseProjectile();

                    }
                   
                }
                else
                {
                    PlaySound(WSound.Empty);                   //Play Empty Sound Which is stored in the 4 Slot  
                    Debugging("[Empty Ammo]", this);
                }

                CanAttack = false;  //Calcualte the Rate Fire of the arm
            }
        }

        ///// <summary>Check if the weapon is a one Chamber Ammo, meaning there can be only one projectile on the chamber and on the Weapon </summary>
        //private void OneChamberAmmo()
        //{
        //    if (!HasAmmo && TotalAmmo > 0 && ChamberSize == 1 && AutoReload)
        //    {
        //        AmmoInChamber = 1; //HACK for 1 Chamber Size Weapon


        //        if (debug) Debug.Log($"{name}:<color=white> <b>[HACK for the BOW ARROWS] </b>   </color>");  //Debug
        //    }
        //}

        internal override void MainAttack_Released(IMWeaponOwner RC)
        {
            Input = false;

            Debugging($"Main Attack Released", this);

            //CalculateAimLimit(RC);

            if (IsReady && HasAmmo && CanCharge && IsCharging && CanShootWithAimLimit)   //If we are not firing any arrow then try to Attack with the bow
            {
                WeaponAction?.Invoke((int)Weapon_Action.Attack);    //Play the Fire Animation on the CHaracter

                if (releaseProjectile == Release_Projectile.OnAttackReleased)
                    ReleaseProjectile();
            }
        }

        public virtual void ReduceAmmo(int amount)
        {
            AmmoInChamber -= amount;
            
            Debugging($"[Ammo: Reduced <b>-({amount})</b> ,Total<b>({TotalAmmo})</b>, In Chamber<b>({AmmoInChamber})</b>]",this);   
            
            if (AmmoInChamber <= 0 && AutoReload)
            {
                TryReload();
            }
        }


        /// <summary> Charge the Weapon!! </summary>
        internal override void Attack_Charge(IMWeaponOwner RC, float time)
        {
           // Debug.Log(" Shootable ATACKCHARGE"+Input);
            if (Input) //The Input For Charging is Down
            {
                if (Automatic && CanAttack && Rate > 0) //If is automatic then continue attacking ◘◘◘TEST THIS!!!!!
                {
                    MainAttack_Start(RC);
                    Debugging($"[Automatic Fire]",this);
                }

                if (IsReady && HasAmmo && CanCharge)  //Is the Weapon ready?? we Have projectiles and we can Charge
                {
                    //CalculateAimLimit(RC);

                    if (!CanShootWithAimLimit)
                    {
                        ResetCharge();
                        return;
                    }

                    if (!IsCharging && IsAiming)            //If Attack is pressed Start Bending for more Strength the Bow
                    {
                       
                        IsCharging = true;
                        ChargeCurrentTime = 0;
                        Predict?.Invoke(true);

                        PlaySound(WSound.Charge); //Play the Charge Sound

                        Debugging("[Charge: 0]",this);
                    }
                    else             // //If Attack is pressed Continue Bending the Bow for more Strength the Bow
                    {
                        Charge(time);
                    }
                }
            }
        }

        internal override void Weapon_LateUpdate(IMWeaponOwner RC)
        {
            CanShootWithAimLimit = (AimLimit.IsInRange(RC.HorizontalAngle)); // Calculate is there's an Imposible range to shoot 
        }


        public override void ResetCharge()
        {
            base.ResetCharge();
            Predict?.Invoke(false);
            Velocity = Vector3.zero; //Reset Velocity

          //  Debug.Log("ResetCharg_Shooteable");
        }

        public override void Charge(float time)
        {
            base.Charge(time);
            CalculateVelocity();
            //Predict?.Invoke(true);
        }


        /// <summary> Create an arrow ready to shooot CALLED BY THE ANIMATOR </summary>
        public virtual void EquipProjectile()
        {
            //Debug.Log("HasAmmo = " + HasAmmo);
            //Debug.Log("m_AmmoInChamber = " + m_AmmoInChamber.Value);

            if (!HasAmmo) return;                                           //means there's no Ammo

            if (ProjectileInstance == null)
            {
                var Pos = ProjectileParent ? ProjectileParent.position : AimOriginPos;
                var Rot = ProjectileParent ? ProjectileParent.rotation : AimOrigin.rotation;
                ProjectileInstance = Instantiate(Projectile, Pos, Rot, ProjectileParent);                  //Instantiate the Arrow in the Knot of the Bow
                
                
                if (ProjectileInstance.TryGetComponent<IProjectile>(out var projectile))
                {
                    I_Projectile = projectile; //Safe in a variable
                    ProjectileInstance.transform.Translate(I_Projectile.PosOffset, Space.Self);   //Translate in the offset of the arrow to put it on the hand
                    ProjectileInstance.transform.Rotate(I_Projectile.RotOffset, Space.Self);      //Rotate in the offset of the arrow to put it on the hand
                    //ProjectileInstance.transform.localScale = (projectile.ScaleOffset);       //Scale in the offset of the arrow to put it on the hand
                }

                if (ProjectileInstance.TryGetComponent<Rigidbody>(out var projectile_RB))
                {
                    projectile_RB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    projectile_RB.isKinematic = true;
                }


                //Disable projectile collider
                if (ProjectileInstance.TryGetComponent<Collider>(out var projectile_Col))
                {
                    projectile_Col.enabled = false;
                }


                OnLoadProjectile.Invoke(ProjectileInstance);

                Debugging($"◘ [Projectile Equiped] [{ProjectileInstance.name}] ", ProjectileInstance);
            }
        }

        public virtual void ReleaseProjectile()
        {
           // Debug.Log("ReleaseProjectile = ");

            if (!gameObject.activeInHierarchy) return; //Crazy bug ??

            Predict?.Invoke(false); 

            if (releaseProjectile == Release_Projectile.Never)
            {
                DestroyProjectileInstance();
                return;
            }
            else if (InstantiateProjectileOfFire)
            {
                EquipProjectile();
            }

            

            this.Delay_Action(2, ()=>  ReduceAmmo(1)); //Reduce the Ammo the next frame

            if (ProjectileInstance == null) return;


            ProjectileInstance.transform.parent = null;

         
            if (I_Projectile != null)
            {
                ProjectileInstance.transform.position = AimOrigin.position;                  //Put the Correct position to Throw the Arrow IMPORTANT!!!!!

                CalculateVelocity();

                ProjectileInstance.transform.forward = Velocity.normalized; //Align the Projectile to the velocity


                ProjectileInstance.transform.Translate(I_Projectile.PosOffset, Space.Self);  //Translate in the offset of the arrow to put it on the hand

                I_Projectile.Prepare(Owner, Gravity, Velocity, Layer, TriggerInteraction);
               
                if (HitEffect != null) I_Projectile.HitEffect = HitEffect; //Send the Hit Effect too

                var newDamage = new StatModifier(statModifier)
                { Value = Mathf.Lerp(MinDamage, MaxDamage, ChargedNormalized) };

                I_Projectile.PrepareDamage(newDamage, CriticalChance, CriticalMultiplier);

                Debugging($"◘ [Projectile Released] [{ProjectileInstance.name}]", ProjectileInstance);
                I_Projectile.Fire();   
            }

            OnFireProjectile.Invoke(ProjectileInstance);
            ProjectileInstance = null;
            I_Projectile = null;

            // WeaponReady(false); //Tell the weapon cannot be Ready until Somebody set it ready again

            PlaySound(WSound.Fire); //Play the Release Projectile Sound

            ResetCharge();
        }

        private void CalculateVelocity()
        {
            var Direction = (CurrentOwner.Aimer.AimPoint - AimOrigin.position).normalized;
        
            if (UseAimAngle.Value)
            {
                var RightV = Vector3.Cross(Direction, -Gravity);
                Velocity = Quaternion.AngleAxis(AimAngle, RightV) * Direction * Power;
            }
            else
                Velocity = Direction * Power;
        }


        /// <summary> Destroy the Active Arrow , used when is Stored the Weapon again and it had an arrow ready</summary>
        public virtual void DestroyProjectileInstance()
        {
            if (ProjectileInstance != null)
            {
                Destroy(ProjectileInstance);
                Debugging("[Destroy Projectile Instance]",this);

            }
            ProjectileInstance = null; //Clean the Projectile Instance
            I_Projectile = null; //Clean Projectile interface
        }

        /// <summary> This is where I call the Animations for the Reload.. not the Actual Reloading of the weapon</summary>
        public override bool TryReload()
        {
            if (TotalAmmo == 0) return false;                //Means the Weapon Cannot Reload
            if (ChamberSize == AmmoInChamber) return false;  //Means there's no need to Reload.. the Chamber is full!!

            if (HasReloadAnim.Value)
            {
                //Check First if a reload can be made??
                if (CanReload())
                {
                    PlaySound(WSound.Reload);
              
                    this.Delay_Action(() =>
                    {
                    WeaponAction.Invoke((int)Weapon_Action.Reload);
                        IsReloading = true;
                    }
                    );

                    return true;
                }
                else
                {
                    //Do Fail Reload

                    WeaponAction.Invoke((int)Weapon_Action.Idle);
                    PlaySound(WSound.Reload);
                    return false;
                }
            }
            else
            {
                IsReloading = false;
                // FinishReload();
                return ReloadWeapon();
            }
        }

        /// <summary> Check if the Reload Animation can be done</summary>
        public bool CanReload()
        {
            if (TotalAmmo == 0) return false;                       //Means the Weapon Cannot Reload, there's no more ammo
            if (ChamberSize == AmmoInChamber) return false;         //Means there's no need to Reload.. the Chamber is full!!
            if (TotalAmmo == -1) return true;                       //Means the Weapon  has infinite Ammo

            int ReloadAmount = ChamberSize - AmmoInChamber;                    //Ammo Needed to refill the Chamber
            int AmmoLeft = TotalAmmo - ReloadAmount;                           //Ammo Remaining

          //  Debug.Log("AmmoLeft = " + AmmoLeft);

            if (AmmoLeft > 0) return true; //Meaning it can Reload something
            return false;
        }


        /// <summary> This can be called also by the ANIMATOR </summary>
        public bool ReloadWeapon()
        {
            int RefillChamber = ChamberSize - AmmoInChamber;                    //Ammo Needed to refill the Chamber
            return Reload(RefillChamber);
        }

        public bool Reload(int ReloadAmount)
        {
            if (TotalAmmo == -1) //Means that you will have Infinity Ammo
            {
                AmmoInChamber = ChamberSize;
                OnReload.Invoke();
                return true;
            }


            if ((TotalAmmo == 0)    ||                                  //Means the Weapon Cannot Reload, there's no more ammo
                (ChamberSize == AmmoInChamber)) return false;                     //Means there's no need to Reload.. the Chamber is full!!


            ReloadAmount = Mathf.Clamp(ReloadAmount, 0, ChamberSize - AmmoInChamber);

            int AmmoLeft = TotalAmmo - ReloadAmount;                           //Ammo Remaining


            if (AmmoLeft >= 0)                                                  //If is there any Ammo 
            {
                AmmoInChamber += ReloadAmount;
                TotalAmmo -= ReloadAmount;
            }
            else
            {
                AmmoInChamber += TotalAmmo;                                     //Set in the Chamber the remaining ammo  
                TotalAmmo = 0;                                                  //Empty the Total Ammo
            }

            if (ChamberSize <= 1 && TotalAmmo == 0) AmmoInChamber = 0;           //Hack to use the AmmoInChamber

            Debugging($"[Reloading:{ReloadAmount}]", this);


            OnReload.Invoke();
            return true;
        }

        /// <summary> If finish reload but is still aiming go to the Aiming animation **CALLED BY THE ANIMATOR**</summary>
        public virtual void FinishReload()
        {  
            WeaponAction?.Invoke(IsAiming && !IsReady ? (int)Weapon_Action.Aim : (int)Weapon_Action.Idle); //Check if Aiming is still on
            IsReloading = false;
            Debugging("[Finish Reload]",this);
        }
    }


    #region INSPECTOR

#if UNITY_EDITOR
    [CanEditMultipleObjects, CustomEditor(typeof(MShootable))]
    public class MShootableEditor : MWeaponEditor
    {
        SerializedProperty  m_AmmoInChamber, m_Ammo, m_ChamberSize, releaseProjectile, m_Projectile, AimLimit, 
            m_AutoReload, InstantiateProjectileOfFire, ProjectileParent, CancelAim,
          //  AimID, FireID, ReloadID,
            OnReload, OnLoadProjectile, OnFireProjectile, gravity, UseAimAngle, m_AimAngle, HasReloadAnim;

        protected MShootable mShoot;

        private void OnEnable()
        {
            SetOnEnable();
            mShoot = (MShootable)target;
        }


        protected override void SetOnEnable()
        {
            WeaponTab = "Shootable";
            base.SetOnEnable();
            AimLimit = serializedObject.FindProperty("AimLimit");
            UseAimAngle = serializedObject.FindProperty("UseAimAngle");
            m_AimAngle = serializedObject.FindProperty("m_AimAngle");
            CancelAim = serializedObject.FindProperty("CancelAim");

            //AimID = serializedObject.FindProperty("AimID");
            //FireID = serializedObject.FindProperty("FireID");
            //ReloadID = serializedObject.FindProperty("ReloadID");


            m_AutoReload = serializedObject.FindProperty("m_AutoReload");
            HasReloadAnim = serializedObject.FindProperty("HasReloadAnim");
            InstantiateProjectileOfFire = serializedObject.FindProperty("InstantiateProjectileOfFire");
            
            releaseProjectile = serializedObject.FindProperty("releaseProjectile");
            m_Projectile = serializedObject.FindProperty("m_Projectile");
            ProjectileParent = serializedObject.FindProperty("m_ProjectileParent");
            

            m_AmmoInChamber = serializedObject.FindProperty("m_AmmoInChamber");
            m_Ammo = serializedObject.FindProperty("m_Ammo");
            m_ChamberSize = serializedObject.FindProperty("m_ChamberSize");

            OnReload = serializedObject.FindProperty("OnReload");
            OnLoadProjectile = serializedObject.FindProperty("OnLoadProjectile");
            OnFireProjectile = serializedObject.FindProperty("OnFireProjectile");
            gravity = serializedObject.FindProperty("gravity");
        }
         

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Projectile Weapons Properties");
            WeaponInspector(false);
            serializedObject.ApplyModifiedProperties();
        }

        protected override void UpdateSoundHelp()
        {
            SoundHelp = "0:Draw   1:Store   2:Shoot   3:Reload   4:Empty  5:Charge";
        }

        protected override string CustomEventsHelp()
        {
            return "\n\n On Fire Gun: Invoked when the weapon is fired \n(Vector3: the Aim direction of the rider), \n\n On Hit: Invoked when the Weapon Fired and hit something \n(Transform: the gameobject that was hitted) \n\n On Aiming: Invoked when the Rider is Aiming or not \n\n On Reload: Invoked when Reload";
        }

        protected override void DrawExtras()
        {

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                minForce.isExpanded = MalbersEditor.Foldout(minForce.isExpanded, "Physics Force");

                if (minForce.isExpanded)
                {
                    EditorGUILayout.PropertyField(minForce, new GUIContent("Min", "Minimun Force to apply to a hitted rigid body"));
                    EditorGUILayout.PropertyField(Force, new GUIContent("Max", "Maximun Force to apply to a hitted rigid body"));
                    EditorGUILayout.PropertyField(forceMode);
                    EditorGUILayout.PropertyField(gravity);
                }
            }
            DrawMisc();
        }

        protected override void DrawAdvancedWeapon()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                m_AimOrigin.isExpanded = MalbersEditor.Foldout(m_AimOrigin.isExpanded, "Aim Properties");

                if (m_AimOrigin.isExpanded)
                {
                    EditorGUILayout.PropertyField(m_AimOrigin);
                    EditorGUILayout.PropertyField(m_AimSide);
                    EditorGUILayout.PropertyField(CancelAim);
                    EditorGUILayout.PropertyField(AimLimit);

                    EditorGUILayout.PropertyField(UseAimAngle, new GUIContent("Use Aim Angle", " Adds a Throw Angle to the Aimer Direction?"));
                    if (mShoot.UseAimAngle.Value)
                    {
                        EditorGUILayout.PropertyField(m_AimAngle, new GUIContent("Aim Angle", " Adds a Throw Angle to the Aimer Direction"));
                    }
                }
            }


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                releaseProjectile.isExpanded = MalbersEditor.Foldout(releaseProjectile.isExpanded, "Projectile");

                if (releaseProjectile.isExpanded)
                {
                    EditorGUILayout.PropertyField(releaseProjectile);

                    if (releaseProjectile.intValue != 0)
                    {
                        EditorGUILayout.PropertyField(InstantiateProjectileOfFire,
                            new GUIContent("Inst Projectile on Fire", "Instanciate the Projectile when Firing the weapon." +
                            "\n E.g The Pistol Instantiate the projectile on Firing. The bow Instantiate the Arrow Before Firing"));
                        EditorGUILayout.PropertyField(m_Projectile);
                        EditorGUILayout.PropertyField(ProjectileParent);
                    }
                }
            }


            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                m_AutoReload.isExpanded = MalbersEditor.Foldout(m_AutoReload.isExpanded, "Ammunition");
                
                if (m_AutoReload.isExpanded)
                {
                    //EditorGUILayout.PropertyField(m_Automatic, new GUIContent("Automatic", "one shot at the time or Automatic"));
                    EditorGUILayout.PropertyField(m_AutoReload, new GUIContent("Auto Reload", "The weapon will reload automatically when the Ammo in chamber is zero"));
                    EditorGUILayout.PropertyField(HasReloadAnim, new GUIContent("Has Reload Anim", "If the Weapon have reload animation then Play it"));
                    EditorGUILayout.PropertyField(m_ChamberSize, new GUIContent("Chamber Size", "Total of Ammo that can be shoot before reloading"));

                    if (mShoot.ChamberSize > 1)
                    {
                        EditorGUILayout.PropertyField(m_AmmoInChamber, new GUIContent("Ammo in Chamber", "Current ammo in the chamber"));
                    }
                    EditorGUILayout.PropertyField(m_Ammo, new GUIContent("Total Ammo", "Total ammo for the weapon. Set it to -1 to have infinity ammo"));
                }
            }
        }


        protected override void ChildWeaponEvents()
        {
            EditorGUILayout.PropertyField(OnLoadProjectile);
            EditorGUILayout.PropertyField(OnFireProjectile);
            //EditorGUILayout.PropertyField(OnAiming);
            EditorGUILayout.PropertyField(OnReload);
        }
    }
#endif
    #endregion
}