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

        #region Holsters
        protected virtual void PrepareHolsters()
        {
            if (holsters != null && holsters.Count == 0) return;

            for (int i = 0; i < holsters.Count; i++) holsters[i].Index = i; //Set the Index on each Holster


            foreach (var h in holsters)
                h.PrepareWeapon();
        }

        /// <summary>Message send by the Rider to Store the Mount </summary>
        public virtual void SetIgnoreTransform(Transform t) => IgnoreTransform = t;
        public virtual void ClearIgnoreTransform() => IgnoreTransform = null;


        public virtual void Holster_SetActive(int ID)
        {
            ActiveHolster = holsters.Find(x => x.GetID == ID);


            ActiveHolsterIndex = ActiveHolster != null ? ActiveHolster.Index : 0;

            if (ActiveHolster != null)
            {
                ActiveHolsterIndex = ActiveHolster.Index;
                Debugging($"Set Active Holster → [{ActiveHolster.ID.name}] → [{ActiveHolsterIndex}].");
            }
            else
            {
                Debug.LogWarning("The Current Default Holster does not exit on the Holster ID list", this);
            }
        }

        /// <summary>Equip the next holster weapon</summary>

        public void Holster_Next()
        {
            ActiveHolsterIndex = (ActiveHolsterIndex + 1) % holsters.Count;
            ActiveHolster = holsters[ActiveHolsterIndex];

            Draw_Weapon();
        }

        /// <summary>Equip the previous holster weapon</summary>
        public void Holster_Previus()
        {
            ActiveHolsterIndex = (ActiveHolsterIndex - 1) % holsters.Count;
            ActiveHolster = holsters[ActiveHolsterIndex];
            Draw_Weapon();
        }

        /// <summary>  Checks if the weapon action is one of the list items Bool Comparer! </summary>
        internal bool IsWeaponAction(params Weapon_Action[] w_actions)
        {
            for (int i = 0; i < w_actions.Length; i++)
                if (WeaponAction == w_actions[i]) return true;
            return false;
        }
        public void WeaponReady(bool value) => Weapon.WeaponReady(value);

        /// <summary> Equip a weapon that is located in a Holster  </summary>
        public virtual void Holster_Equip(HolsterID HolsterID) => Holster_Equip(HolsterID.ID);

        /// <summary>Clear a Holster by its ID</summary> 
        public virtual void Holster_Clear(HolsterID HolsterID) => Holster_Clear(HolsterID.ID);


        /// <summary> Equip a weapon that is located in a Holster (INPUT CONNECTION)  </summary>
        protected virtual void Holster_Equip(HolsterID HolsterID, bool value) { if (value) Holster_Equip(HolsterID.ID); }


        /// <summary>Clear a Holster by its ID</summary> 
        public virtual void Holster_Clear(int HolsterID)
        {
            if (UseHolsters && Active && !Paused)
            {
                //Do nothing if the Action is NOT Idle or None( DO NOT INCLUDE AIMING because Assasing Creed Style)
                if (!IsWeaponAction(Weapon_Action.None, Weapon_Action.Idle)) return;

                var ActiveHolster = holsters.Find(x => x.GetID == HolsterID);

                if (ActiveHolster != null)
                {
                    if (ActiveHolster.Weapon.IsEquiped) UnEquip_Fast(); //Make sure to unequip that weapon

                    ActiveHolster.SetWeapon((MWeapon)null);
                }
            }
        }



        /// <summary> Equip a weapon that is located in a Holster  </summary> 
        public virtual void Holster_Equip(int HolsterID)
        {
            if (UseHolsters && Active && !Paused)
            {
                //Do nothing if the Action is NOT Idle or None( DO NOT INCLUDE AIMING because Assasing Creed Style)
                if (!IsWeaponAction(Weapon_Action.None, Weapon_Action.Idle)) return;

                if (IgnoreDraw)
                {
                    if (!CombatMode)                    //There's no weapon equipped
                    {
                        Holster_SetActive(HolsterID);
                        Weapon = ActiveHolster.Weapon;
                        Equip_Fast();                   //So Equip
                    }
                    else
                    {
                        if (Weapon.Holster != HolsterID) //Meaning is calling the same holster
                        {
                            UnEquip_Fast();
                            Holster_SetActive(HolsterID);
                            Weapon = ActiveHolster.Weapon;
                            Equip_Fast();
                        }
                        //else
                        //{
                        //    UnEquip_Fast(); //So Unequip  (THIS DOES NOT WORK IF YOU APPLY ASSASSING CREED)
                        //}
                    }
                }
                else
                {
                    if (!CombatMode)        //There's no weapon equipped
                    {
                        Holster_SetActive(HolsterID);
                        Draw_Weapon();      //Draw a weapon if we are on Action None
                    }
                    else
                    {
                        if (Weapon.Holster == HolsterID) //Meaning is calling the same holster
                        {
                            Store_Weapon(); //So store the same Active Weapon
                        }
                        else
                        {
                            StartCoroutine(SwapWeaponsHolster(HolsterID));
                        }
                    }
                }
            }
        }


        /// <summary> Store a weapon on its holster</summary>
        public virtual void Holster_SetWeapon(GameObject WeaponGO)
        {
            if (WeaponGO == null) return;
            Holster_SetWeapon(WeaponGO.GetComponent<MWeapon>());
        }


        /// <summary> Store a weapon on its holster</summary>
        public virtual void Holster_SetWeapon(MWeapon Next_Weapon)
        {
            if (Next_Weapon != null)
            {
                var holster = holsters.Find(x => x.ID == Next_Weapon.HolsterID); //Find the holster you want the new weapon to be eqquipped

                if (holster != null)
                {
                    Debugging($"[Set Weapon on Holster] → [{holster.ID.name}] → [{Next_Weapon.name}]", "green");

                    var WasEquipped = false;

                    if (holster.Weapon != null) //Meaning there was another weapon there
                    {
                        WasEquipped = holster.Weapon == Weapon; //Meaning is the same Weapon Equipped
                        if (WasEquipped) UnEquip_Fast(); //If the weapon is the one holding right now then Unequip Fast
                    }

                    if (Next_Weapon.gameObject.IsPrefab()) Next_Weapon = Instantiate(Next_Weapon);              //if is a prefab instantiate on the scene


                    //Set the new Weapon in the Correct Holster Spot
                    Next_Weapon.gameObject.transform.parent = holster.GetSlot(Next_Weapon.HolsterSlot);
                    Next_Weapon.gameObject.transform.SetLocalTransform(Next_Weapon.HolsterOffset);

                    holster.SetWeapon(Next_Weapon);
                    holster.Weapon.DisablePhysics();

                    //   Debug.Log("**********************WasEquipped = " + WasEquipped);

                    if (WasEquipped) //Was the old weapon Equipped?
                    {
                        Weapon = Next_Weapon;
                        Equip_Fast(); //Equip the new weapon if the last one was eqqiuped
                    }
                }
                else
                {
                    Debugging($"Set Weapon on Holster Failed →" +
                        $" There's no Holster [{Next_Weapon.Holster.name}] on the Holster List for the Weapon [{Next_Weapon.name}]");
                }
            }
        }
        #endregion

        #region Equip Weapon
        /// <summary>Sets the weapon equipped by an External Source</summary>
        public virtual void Equip_External(GameObject WeaponGo)
        {
            var Next_Weapon = WeaponGo != null ? WeaponGo.GetComponent<MWeapon>() : null;
            Equip_External(Next_Weapon);
        }


        public virtual void Equip_External(MWeapon Next_Weapon)
        {
            //Do nothing if the Action is NOT Idle or None( DO NOT INCLUDE AIMING because Assasing Creed Style)
            if (!IsWeaponAction(Weapon_Action.None, Weapon_Action.Idle)) return;

            if (Active && UseExternal && !Paused)
            {
                StopAllCoroutines();

                if (Next_Weapon == null)                                    //That means Store the weapon
                {
                    Store_Weapon();
                    Debugging("Active Weapon is [Empty] or is not compatible. Store the Active Weapon");
                }
                else if (Weapon == null)                               //Means there's no weapon active so draw it
                {
                    TryInstantiateWeapon(Next_Weapon);
                    Draw_Weapon();

                }
                else if (Weapon.Equals(Next_Weapon))                         //You are trying to draw the same weapon
                {
                    if (!CombatMode)
                    {
                        Draw_Weapon();
                        Debugging("Active weapon is the same as the NEXT Weapon and we are NOT in Combat so DRAW");
                    }
                    else
                    {
                        Store_Weapon();
                        Debugging("Active weapon is the same as the NEXT Weapon and we ARE  in Combat so STORE");
                    }
                }
                else                                                                //If the weapons are different Swap it
                {
                    StartCoroutine(SwapWeaponsInventory(Next_Weapon));
                    Debugging("Active weapon is DIFFERENT to the NEXT weapon so Switch: " + Next_Weapon.name);
                }
            }
        }


        /// <summary> Don't remember why I'm using this ??</summary>
        private void TryInstantiateWeapon(MWeapon Next_Weapon)
        {
            if (InstantiateOnEquip || Next_Weapon.gameObject.IsPrefab())
            {
                var WeaponGO = Instantiate(Next_Weapon.gameObject, transform);      //Instanciate the Weapon GameObject
                WeaponGO.SetActive(false);                                          //Hide it to show it later
                Next_Weapon = WeaponGO.GetComponent<MWeapon>();                     //UPDATE THE REFERENCE
                Debugging($"{WeaponGO.name} Instantiated");
            }
            Weapon = Next_Weapon;
        }

        /// <summary>Is called to swap weapons</summary>
        private IEnumerator SwapWeaponsInventory(MWeapon nextWeapon)
        {
            Store_Weapon();

            while (WeaponAction == Weapon_Action.Store) yield return null;    // Wait for the weapon is Unequiped Before it can Draw Another

            TryInstantiateWeapon(nextWeapon);

            Draw_Weapon();                                                                  //Set the parameters so draw a weapon
        }


        /// <summary>Equip a new weapon an External Source</summary>
        public virtual void Equip_Fast(GameObject WeaponGo)
        {
            if (WeaponGo == null) return;
           
            var Next_Weapon = WeaponGo.GetComponent<MWeapon>(); //Find if there's a next weapon
            Equip_Fast(Next_Weapon);
        }

        public virtual void Equip_Fast(MWeapon Next_Weapon) 
        {
            //Do nothing if the Action is NOT Idle or None( DO NOT INCLUDE AIMING because Assasing Creed Style)
            if (!IsWeaponAction(Weapon_Action.None, Weapon_Action.Idle)) return;
            if (!Active) return;
            if (Next_Weapon == null) return;

            StopAllCoroutines();

            if (Weapon == null)                               //Means there's no ACTIVE weapon Equip the new one
            {
                if (UseExternal) TryInstantiateWeapon(Next_Weapon);
                Weapon = Next_Weapon;
                Holster_SetActive(Weapon.HolsterID);
                Equip_Fast();
            }
            else if (!Weapon.Equals(Next_Weapon))             //You are trying to Equip a different weapon, so Unequip the active one then equip the next one
            {
                UnEquip_Fast();
                if (UseExternal) TryInstantiateWeapon(Next_Weapon);
                Weapon = Next_Weapon;
                Holster_SetActive(Weapon.HolsterID);
                Equip_Fast();
                Debugging("Active weapon is DIFFERENT to the NEXT weapon so Switch: " + Next_Weapon.name);
            }
            //else             // Do Nothing Because the weapon is already equipped (For Invector)
            //{
            //   // Debugging("Trying to Equip the Current Equiped Weapon so ignore" + Next_Weapon.name);
            //    // UnEquip_Fast();
            //}
        }
        #endregion

        #region Empty Draw Store
        /// <summary>
        /// Execute Draw Weapon Animation without the need of an Active Weapon
        /// This is used when is Called Externally for other script (Integrations) </summary>
        /// <param name="holster">Which holster the weapon is going to be draw from</param>
        /// <param name="weaponType">What type of weapon</param>
        /// <param name="isRightHand">Is it going to be draw with the left or the right hand</param>
        public virtual void Draw_Weapon(int holster, int weaponType, bool isLeftHand)
        {
            ExitAim();
            ResetCombat();
            CustomWeaponAction((int)Weapon_Action.Draw, holster);
            WeaponType = weaponType;
            SetWeaponHand(isLeftHand);

            Debugging($"Draw with No Active Weapon");  //Debug
        }

        /// <summary>Execute Store Weapon Animation without the need of an Active Weapon
        /// This is used when is Called Externally for other script (Integrations) </summary>
        /// <param name="holster">The holster that the weapon is going to be Stored</param>
        /// <param name="isRightHand">is whe Weapon Right Handed?</param>
        public virtual void Store_Weapon(int holster, bool isRightHand)
        {
            Holster_SetActive(holster);
            WeaponAction = Weapon_Action.Store;

            ResetCombat();
            Debugging($"Store with No Active Weapon ");
        }
        #endregion

        #region Attack Callbacks

        public virtual void MainAttack()
        {
            MainAttack(0);
        }

        public virtual void _MainAttack()
        {
            MainAttack(0);
        }

        public virtual void SecondAttack()
        {
            if (!Aim)
            {
                MainAttack(1); 
            }
        }

        public virtual void MainAttack(int Branch)
        {
            if (!Active) return;
            if (MountingDismounting) return; //Do nothing if the character is mounting or dismounting

            if (comboManager) comboManager.SetBranch(Branch); //Set the Branch in the combo 

            Attack();
        }


        /// <summary> Start the Main Attack Logic </summary>
        public virtual void Attack()
        {
            if (!Active) return;
            if (MountingDismounting) return;    //Do nothing if the character is mounting or dismounting
            if (HigherPriorityMode) return;     //Do not attack if other High mode is played 


            //Debug.Log($"WeaponIsActive {WeaponIsActive}");
            //Debug.Log($"Weapon.CanAttack {Weapon.CanAttack}");
            if (WeaponIsActive)
            {
                if (Weapon.CanAttack)
                {
                    if (!Aimer.Active) Aimer.CalculateAiming(); //Quick Aim Calculation in case the Aimer is Disabled
                  //  Debug.Log("<-------------> Weapon Attack <------------->");
                    Weapon.MainAttack_Start(this);
                   // OnMainAttackStart.Invoke(Weapon.gameObject);
                }
            }
            else //Meaning there's no WEAPON!!!! you are doing NO WEAPONS ATTACKS (ONLY DO THIS WITH ANIMAL CONTROLLER)
            {
                if (Weapon && !Weapon.Enabled) { return; } //Meaning the weapon was Disabled!!!

                //Do Unharmed Attacks in case there's no weapons (Different from attacking with a weapon
                if (HasAnimal && !IsRiding)
                {
                    if (comboManager && comboManager.ActiveCombo != null) //if there's a combo manager use it
                    {
                        //Debug.Log("Unharmed Attack with Combo");
                        comboManager.Play();
                    }
                    else
                    {
                        // Debug.Log("Unharmed Attack with Modes");
                        UnArmedMode?.TryActivate(-99); //else do a normal  Animal Mode activation
                    }
                }
            }
        }


        /// <summary>Repeat while the Input is down. This is called in fixed update</summary>
        protected virtual void WeaponCharged(float time)
        {
            //If there's a Weapon Active
            if (Active && CombatMode && WeaponIsActive && Weapon.Input)
            {
                if (!HasAnimal || animal.ActiveMode == WeaponMode)
                    Weapon.Attack_Charge(this, time);
            }
        }

        /// <summary>Called to release the Main Attack (Ex release the Arrow on the Bow, the Melee Atack)</summary>
        public virtual void MainAttackReleased()
        {
            if (WeaponIsActive)
            {
                //  Debug.Log("Attack Release");
                Weapon.MainAttack_Released(this);
            }
        }

        public virtual void MainAttack(bool value)
        {
            if (value) MainAttack(); else MainAttackReleased();
        }


        public virtual void SecondAttack(bool value)
        {
            if (value) SecondAttack(); else SecondAttackReleased();
        }


        public virtual void SecondAttackReleased()
        {
            if (WeaponIsActive)
            {
                Weapon.SecondAttack_Released(this);
            }
        }

        /// <summary>This is the first task for reloading... (Connected to the Input)</summary>
        private void Reload(bool value)
        {
            if (value) ReloadWeapon();
          //  else ReloadInterrupt();
        }

        private void ReloadInterrupt()
        {
            if (WeaponIsActive && IsReloading) //Only Reload Once!
            {
                CheckAim();
            }
        }

        public virtual void ReloadWeapon()
        {
            // if (JustChangedAction) return; //DO Nothing if you just change actions

            if (WeaponIsActive && WeaponAction != Weapon_Action.Reload) //Only Reload Once!
            {
                Weapon.TryReload();
            }
        }
        #endregion

        #region Inputs


        /// <summary>Connects the State with the External Inputs Source</summary>
        internal void ConnectInput(IInputSource InputSource, bool connect)
        {
            //Connect Aim Input
            if (connect)
            {
                foreach (var a in holsters)
                {
                    ////Very important to use the same listener, so it can be added or removed.
                    if (a.InputListener == null) a.InputListener = (value) => Holster_Equip(a.ID, value);
                    InputSource.ConnectInput(a.Input, a.InputListener);
                }

                InputSource.ConnectInput(m_AimInput, Aim_Set);
                InputSource.ConnectInput(m_ReloadInput, Reload);
                InputSource.ConnectInput(m_MainAttack, MainAttack);
                InputSource.ConnectInput(m_SecondAttack, SecondAttack);
            }
            else
            {
                foreach (var a in holsters)
                {
                    ////Very important to use the same listener, so it can be added or removed.
                    if (a.InputListener != null)
                        InputSource.DisconnectInput(a.Input, a.InputListener);
                }

                InputSource.DisconnectInput(m_AimInput, Aim_Set);
                InputSource.DisconnectInput(m_ReloadInput, Reload);
                InputSource.DisconnectInput(m_MainAttack, MainAttack);
                InputSource.DisconnectInput(m_SecondAttack, SecondAttack);
            }
        }

        protected void GetAttack1Input(bool inputValue)
        {
            if (inputValue) MainAttack();
            else MainAttackReleased();
        }

        protected void GetReloadInput(bool inputValue)
        {
            if (inputValue) ReloadWeapon();
        }

        #endregion

        #region Animator Methods
        /// <summary>Messages Get from the Animator</summary>
        public virtual bool OnAnimatorBehaviourMessage(string message, object value)
        {
            bool w = Weapon ? Weapon.OnAnimatorBehaviourMessage(message, value) : false;
            return this.InvokeWithParams(message, value) || w;
        }

        /// <summary>  Sets on the Animator Controller the Hand Value (Left:true or Right:false)  </summary>
        public void SetWeaponHand(bool value)
        {
            if (Hash_LeftHand != 0)
                SetBoolParameter(Hash_LeftHand, value);
        }

        public void SetAnimParameter(int hash, int value) => Anim.SetInteger(hash, value);

        /// <summary>Set a float on the Animator</summary>
        public void SetAnimParameter(int hash, float value) => Anim.SetFloat(hash, value);

        /// <summary>Set a Bool on the Animator</summary>
        public void SetAnimParameter(int hash, bool value) => Anim.SetBool(hash, value);

        /// <summary>Set a trigger on the Animator</summary>
        public void SetAnimParameter(int hash) => Anim.SetTrigger(hash);


        //Send 0 if the Animator does not contain
        private int TryGetAnimParameter(string param)
        {
            var AnimHash = Animator.StringToHash(param);

            if (!animatorHashParams.Contains(AnimHash))
                return 0;
            return AnimHash;
        }

        public virtual void TryAnimParameter(int Hash, float value) { if (Hash != 0) SetFloatParameter(Hash, value); }

        public virtual void TryAnimParameter(int Hash, int value) { if (Hash != 0) SetIntParameter(Hash, value); }

        public virtual void TryAnimParameter(int Hash, bool value) { if (Hash != 0) SetBoolParameter(Hash, value); }

        public virtual void TryAnimParameter(int Hash) { if (Hash != 0) SetTriggerParameter(Hash); }
        #endregion

        /// <summary>Get a Callback From the RiderCombat Layer Weapons States</summary>
        public virtual void WeaponSound(int SoundID) => Weapon?.PlaySound(SoundID);
    }
}