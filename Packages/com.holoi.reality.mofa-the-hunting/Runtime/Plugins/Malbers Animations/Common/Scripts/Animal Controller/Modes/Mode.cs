using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Controller
{
    [System.Serializable]
    public class Mode
    {
        #region Public Variables
        /// <summary>Is this Mode Active?</summary>
        [SerializeField] private bool active = true;

        /// <summary>Enable Disable the modes temporarilly and internally by multiple outside sources</summary>
        private int TemporalActivation = 1;

        [SerializeField] private bool ignoreLowerModes = false;

        [Tooltip("The Abilities animations have cooldown. If this is set to false then the animations needs to finish before activating a new Ability")]
        [SerializeField] private bool hasCoolDown = false;

        /// <summary>Animation Tag Hash of the Mode</summary>
        protected int ModeTagHash;
        /// <summary>Which Input Enables the Ability </summary>
        public string Input;
        /// <summary>ID of the Mode </summary>
        [SerializeField] public ModeID ID;

        [CreateScriptableAsset]
        /// <summary>Modifier that can be used when the Mode is Enabled/Disabled or Interrupted</summary>
        [ExposeScriptableAsset]
        public ModeModifier modifier;

        [Tooltip("Elapsed time needed to interrupt the current ability by another Mode. [Has Cooldown needs to be true]")]
        public FloatReference CoolDown = new FloatReference(0);

        /// <summary>List of Abilities </summary>
        public List<Ability> Abilities;
        /// <summary>Active Ability index</summary>
        [SerializeField]
        private IntReference m_AbilityIndex = new IntReference(-99);
        public IntReference DefaultIndex = new IntReference(0);
        public IntEvent OnAbilityIndex = new IntEvent();
        public bool ResetToDefault = false;

        [SerializeField] private bool allowRotation = false;
        [SerializeField] private bool allowMovement = false;

        public UnityEvent OnEnterMode = new UnityEvent();
        public UnityEvent OnExitMode = new UnityEvent();

        public float PositionMultiplier => ActiveAbility.AdditivePosition;

        [Tooltip("Global Audio Source assigned to the Mode to Play Audio Clips")]
        public AudioSource m_Source;
        #endregion

        #region Properties

        /// <summary>Is THIS Mode Playing?</summary>
        public bool PlayingMode { get; set; }
        //{
        //    get  => playingMode;

        //    set
        //    {
        //        playingMode = value;
        //        Debug.Log($"{Animal.name} {ID.name} PlayingMode [{playingMode}]");
        //    }
        //}
        //private bool playingMode;

        /// <summary>Stored Value for the Actual charge of the Mode</summary>
        public float ChargeValue { get; set; }
        // public int EnterStateInfo { get; set; }

        /// <summary> Is the Mode In transition </summary>
        public bool IsInTransition { get; set; }

        /// <summary> Is the Mode Enabled</summary>
        public bool Active
        {
            get => active && TemporalActivation > 0;
            set
            {
                if (value != active)
                {
                    active = value;
                    Debugging($"<b><color=green>Set Active: </color>[{value}] </b>");
                }
            }
        }


        /// <summary>Priority of the Mode.  Higher value more priority</summary>
        public int Priority { get; internal set; }

        /// <summary>Allows Additive rotation while the mode is playing </summary>
        public bool AllowRotation { get => allowRotation; set => allowRotation = value; }

        /// <summary>Allows Additive Speeds while the mode is playing </summary>
        public bool AllowMovement { get => allowMovement; set => allowMovement = value; }

        public string Name => ID != null ? ID.name : string.Empty;

        /// <summary>Means the Ability needs to finish the Animation or it has cooldown and its on cooldown</summary>
       // public bool HasCoolDown => (CoolDown == 0) || InCoolDown;
        public bool HasCoolDown { get => hasCoolDown; set => hasCoolDown = value; }

        /// <summary>Is this mode in CoolDown?</summary>
        public bool InCoolDown { get; internal set; }
        //{
        //    get
        //    {
        //        return inCoolDown;
        //    }

        //    set
        //    {
        //        inCoolDown = value;
        //        Debug.Log($"{Animal.name} {ID.name} InCoolDown [{inCoolDown}]");
        //    }
        //}
        //private bool inCoolDown;

        public float ActivationTime;

        /// <summary>If enabled, it will play this Mode even if a Lower Mode is Playing </summary>
        public bool IgnoreLowerModes { get => ignoreLowerModes; set => ignoreLowerModes = value; }

        /// <summary> Active Ability Index of the mode</summary>
        public int AbilityIndex
        {
            get => m_AbilityIndex;
            set
            {
                m_AbilityIndex.Value = value;
                OnAbilityIndex.Invoke(value);
                //  Debug.Log($"{Animal.name} AbilityIndex [{m_AbilityIndex.Value}]");
            }
        }

        public void SetAbilityIndex(int index) => AbilityIndex = index;

        /// <summary>Interrupt this mode only if is the one playing</summary>
        public void Interrupt()
        {
            if (Animal.ActiveMode == this) Animal.Mode_Interrupt();
        }

        public MAnimal Animal { get; private set; }

        /// <summary> Current Selected Ability to Play on the Mode</summary>
        public Ability ActiveAbility { get; private set; }
        //{
        //    get => m_ActiveAbility;
        //    set
        //    {
        //        m_ActiveAbility = value;
        //        Debug.Log($"{Animal.name} ActiveAbility [{m_ActiveAbility}]");
        //    }
        //}
        //private Ability m_ActiveAbility;

        /// <summary>Current Value of the Input if this mode was called  by an Input</summary>
        public bool InputValue { get; internal set; }
        //{
        //    get => m_InputValue;
        //    set
        //    {
        //        m_InputValue = value;
        //        Debug.Log($"Mode [{ID}] Input: [{Input}] Value [{m_InputValue}]");
        //    }
        //}
        //private bool m_InputValue;
        #endregion


        internal void ConnectInput(IInputSource InputSource, bool connect)
        {
            //Mode Input
            if (connect)
                InputSource.ConnectInput(Input, ActivatebyInput);
            else
                InputSource.DisconnectInput(Input, ActivatebyInput);

            //Abilities Inputs
            foreach (var a in Abilities)
            {
                ////Very important to use the same listener, so it can be added or removed.
                if (a.InputListener == null) a.InputListener = (x) => ActivateAbilitybyInput(a, x);

                if (connect)
                    InputSource.ConnectInput(a.Input, a.InputListener);
                else
                    InputSource.DisconnectInput(a.Input, a.InputListener);
            }
        }

        /// <summary>Set everyting up when the Animal Script Start</summary>
        public virtual void AwakeMode(MAnimal animal)
        {
            Animal = animal;                                    //Cache the Animal
            OnAbilityIndex.Invoke(AbilityIndex);                //Make the first invoke
            ActivationTime = -CoolDown * 2;
            InCoolDown = false;
            TemporalActivation = 1;
        }

        /// <summary>Reset the current mode and ability</summary> 
        public virtual void ResetMode()
        {
            if (Animal.ActiveMode == this) //if is the same Mode then set the AnimalPlaying mode to false
            {
                Animal.Set_State_Sleep_FromMode(false);  //Restore all the States that are sleep from this mode
            }

            PlayingMode = false;
            //InCoolDown = false;


            modifier?.OnModeExit(this);
            if (ActiveAbility != null)
            {
                ActiveAbility.modifier?.OnModeExit(this);

                if (ActiveAbility.m_stopAudio)
                {
                    if (ActiveAbility.audioSource != null) ActiveAbility.audioSource.Stop();
                    if (m_Source != null) m_Source.Stop();
                }

                ActiveAbility.OnExit.Invoke();
                // OnExitInvoke();
            }

            if (ResetToDefault && !InputValue) //Important if the Input is still Active then Do not Reset to Default
                m_AbilityIndex.Value = DefaultIndex.Value;

            ActiveAbility = null;                           //Reset to the default
        }

        /// <summary>Reset the current mode inside the Animal</summary> 
        public virtual void ModeExit()
        {
            // Debugging("ModeExit");
            Animal.ModeTime = 0;            //Reset Mode Time 
            Animal.ModeAbility = 0;         //Reset the Mode Parameter on the Animator... 
            Animal.SetModeStatus(0);        //Reset/Interrupt the Mode Ability to 0

            //These two at the end!!! Super Important and needs to be at the end!!
            OnExitMode.Invoke();
            Animal.ActiveMode = null;
            //InCoolDown = false;
        }

        /// <summary>Resets the Ability Index on the  animal to the default value</summary>
        public virtual void ResetAbilityIndex()
        {
            if (!Animal.InZone) SetAbilityIndex(DefaultIndex); //Dont reset it if you are on a zone... the Zone will do it automatically if you exit it
        }

        /// <summary>Returns True if a mode has an Ability Index</summary>
        public bool HasAbilityIndex(int index) => Abilities.Find(ab => ab.Index == index) != null;

        public void SetActive(bool value) => Active = value;

        public void ActivatebyInput(bool Input_Value)
        {
            if (!Active) return;
            if (Animal != null && !Animal.enabled) return;
            if (Animal.LockInput) return;               //Do no Activate if is sleep or disable or lock input is Enable;

            if (InputValue != Input_Value)              //Only Change if the Inputs are Different
            {
                InputValue = Input_Value;

                if (InputValue)
                {
                    Debugging($"<B><color=yellow>[Try Activate by Input <{Input}>]</color></B>");


                    if (Animal.InZone && Animal.InZone.IsMode) //meaning the Zone its a Mode zone and it also changes the Status
                        Animal.InZone.ActivateZone(Animal);
                    else
                        TryActivate();
                }
                else
                {
                    if (PlayingMode && CheckStatus(AbilityStatus.Charged)) //if this mode is playing && is set to Hold by Input & the Input was true
                    {
                        Animal.Mode_Interrupt();
                        Debugging($"<B><color=yellow>[INTERRUPTED]</color> Ability: <color=white>[{ActiveAbility.Name}]</color> " +
                            $"Status: <color=white>[Input Released]</color></B>");
                    }
                }
            }
        }


        public void ActivateAbilitybyInput(Ability ability, bool Input_Value)
        {
            //Debug.Log(Name + "Input = " + Input_Value );

            if (ability.InputValue != Input_Value)              //Only Change if the Inputs are Different
            {
                ability.InputValue = Input_Value;

                if (!Active) return;
                if (!Animal.enabled) return;
                if (Animal.LockInput) return;               //Do no Activate if is sleep or disable or lock input is Enable;

                if (ability.InputValue)
                {
                    TryActivate(ability);
                }
                else
                {
                    if (PlayingMode && ActiveAbility.Index == ability.Index && CheckStatus(AbilityStatus.Charged)) //if this mode is playing && is set to Hold by Input & the Input was true
                    {
                        Animal.Mode_Interrupt();
                        Debugging($"<B><color=yellow>[INTERRUPTED]</color> Ability: <color=white>[{ActiveAbility.Name}]</color> " +
                            $"Status: <color=white>[Input Released]</color></B>");
                    }
                }
            }
        }

        /// <summary>Randomly Activates an Ability from this mode</summary>
        private void Activate(Ability newAbility, int modeStatus, string deb)
        {
            ActiveAbility = newAbility;
            Animal.SetModeParameters(this, modeStatus);

            ChargeValue = 0;

            ActiveAbility.modifier?.OnModeEnter(this); //Active Local Mode Modifier

            AudioSource source = ActiveAbility.audioSource != null ? ActiveAbility.audioSource : m_Source;
            if (source && source.isActiveAndEnabled)
            {
                if (!ActiveAbility.audioClip.NullOrEmpty())
                    source.clip = ActiveAbility.audioClip.GetValue();

                if (source.isPlaying) source.Stop();
                source.PlayDelayed(ActiveAbility.ClipDelay);
            }

            Debugging($"<B><color=yellow>[PREPARED]</color></B> Ability: <B><color=white>[{ActiveAbility.Name}] " +
                $"[{Mathf.Abs(ID * 1000) + Mathf.Abs(ActiveAbility.Index)}]</color>. {deb}</b>");
        }

        /// <summary>Force the Activation of a Mode using the Active Ability Index</summary>
        public bool ForceActivate() => ForceActivate(AbilityIndex);

        /// <summary>Force the Activation of a Mode using an Ability Index</summary>

        public bool ForceActivate(int abilityIndex)
        {
            if (abilityIndex != 0) AbilityIndex = abilityIndex;

            if (!Animal.IsPreparingMode)
            {
                Debugging($"<B><color=Cyan>[FORCED ACTIVATE] Next Ability:[{AbilityIndex}]</color></B>");

                if (Animal.IsPlayingMode)
                {
                    Animal.ActiveMode.ResetMode();
                    Animal.ActiveMode.ModeExit();                          //This allows to Play a mode again
                }
                return TryActivate();
            }
            return false;
        }
         


        public virtual bool TryActivate() => TryActivate(AbilityIndex);

        public virtual bool TryActivate(int index) => TryActivate(GetTryAbility(index));

        public virtual bool TryActivate(int index, AbilityStatus status, float time = 0)
        {
            var TryNextAbility = GetTryAbility(index);

            if (TryNextAbility != null)
            {
                TryNextAbility.Status = status;

                if (status == AbilityStatus.ActiveByTime)
                    TryNextAbility.AbilityTime = time;

                return TryActivate(TryNextAbility);
            }
            return false;
        }

        /// <summary>Checks if the ability can be activated</summary>
        public virtual bool TryActivate(Ability newAbility)
        {
            if (!Active) return false;  
             
            int ModeStatus = 0; //Default Mode Status on the Mode .. This is changed if It can transition from an old ability to another
            string deb = "<-->";    //Safe the Aproved Result
             
            if (newAbility == null)
            {
                Debugging($"<Color=red> Skip Ability is [NULL] Index is {AbilityIndex}.</color>");
                Animal.IsPreparingMode = false; //?!?!?!?!??!?! BUG?!?!? NOT??? MAYBE ???? 
                return false;
            } 

            // Debug.Log($"-----------------TryActivate {newAbility.Name} [{newAbility.Index.Value}]");
            
            if (Animal.IsPreparingMode)
            {
                Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play. Its already preparing another Mode [Skip]</color>");
                return false;
            }

            //RARE BUG!!!!!! JUST IN CASE (IF THIS MODE SAYS THAT IS PLAYING MODE BUT IS NOT)
            if (Animal.IsPlayingMode && this.PlayingMode && Animal.ActiveMode != this) PlayingMode = false;

            
            
            if (!newAbility.Active)
            {
                Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play. <Disabled></color>");
                return false;
            }

            if (StateCanInterrupt(Animal.ActiveState.ID, newAbility))       //Check if the States can block the mode
            {
                Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                    $" Active State [{Animal.ActiveStateID.name}] won't allow it</color>");
                return false;
            }

            if (StanceCanInterrupt(Animal.Stance, newAbility))       //Check if the States can block the mode
            {
                Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                   $" The current Stance won't allow it</color>");
                return false;
            }

            //If this IS the mode that the animal is playing
            if (this.PlayingMode)
            {
                //if is set to Toggle then if is already playing this mode then stop it
                if (ActiveAbility.Index == newAbility.Index && CheckStatus(AbilityStatus.Toggle))
                {
                    InputValue = false;                     //Reset the Input Value to false of this mode
                    Animal.Mode_Interrupt();
                    Debugging($"<B><color=yellow>[INTERRUPTED]</color> Ability: <Color=white>[{ActiveAbility.Name}]</color> " +
                        $"Status: <Color=white>[Toggle Off]</color></B>");
                    return false;
                }
                //Means it can transition from one ability to another
                else if (newAbility.HasTransitionFrom && newAbility.Limits.TransitionFrom.Contains(ActiveAbility.Index))
                {
                    ModeStatus = ActiveAbility.Index; //This is used to Transition from an Older Mode Ability to a new one
                    deb = ($"Last Ability [{ModeStatus}] is allowing it. <Check ModeBehaviour>");
                    ResetMode(); //GO TO THE END
                }
                //Means the Ability needs to finish its animation
                else if (!HasCoolDown)
                {
                    Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                        $"Ability [{ActiveAbility.Name}] needs to finish</color>");
                    return false;
                }
                //Means the Ability needs to finish its cooldown
                else if (InCoolDown)
                {
                    Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                        $"Ability [{ActiveAbility.Name}] is in cooldown</color>");
                    return false;
                }
                //Means the Ability was in cooldown but the coldown ended!!
                else if (!InCoolDown)
                {
                    ResetMode();//GO TO THE END
                    ModeExit(); //This allows to Play a mode again INT ID  = 0 to it can be available again
                    deb = ($"No Longer in Cooldown [Same Mode]");
                }
            }
            //If the Animal is playing a Different Mode
            else if (Animal.IsPlayingMode)
            {
                var ActiveMode = Animal.ActiveMode;
               // Debug.Log($"ActiveMode {ActiveMode.Name}");
               //Debug.Log($"Priority [{Priority}] .. ActiveMode.Priority: {ActiveMode.Priority} ....  INCO{ActiveMode.InCoolDown}");

                if (Priority > ActiveMode.Priority && IgnoreLowerModes)
                {
                    ActiveMode.ResetMode();
                    ActiveMode.InputValue = false;              //Set the Input to false so both modes don't overlap
                    ActiveMode.ModeExit();                      //This allows to Play a mode again
                    ActiveMode.InCoolDown = false;
                    deb = ($"Exit [{ActiveMode.Name}] Mode, New [{Name}] has Higher Priority");
                    //GO TO THE END
                }
                else if (!ActiveMode.HasCoolDown || ActiveMode.InCoolDown) //IF IT NEEDS TO FINISH ITS ANIMATIONS
                {
                    Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                        $"<b>[{ActiveMode.ID.name}]</b> needs to finish the current ability</color>");

                    return false;
                }
                else if (!ActiveMode.InCoolDown)    //Means that the Active mode can be Interrupted since is no longer on cooldown
                {
                    ActiveMode.ResetMode();
                    ActiveMode.ModeExit();          //This allows to Play a mode again INT ID  = 0 to it can be available again
                    deb = ($"[Mode {ActiveMode.Name}] is no Longer in Cooldown ");
                    //GO TO THE END
                }
            }
            else if (HasCoolDown && CoolDown > 0  && InCoolDown) //If This mode is in cooldown even if is not playing ... it has finished
            {
                Debugging($"<color=red><B>[{newAbility.Name}]</B> Failed to play." +
                  $" <b>[Mode: {Name}]</b> is still in Long Cooldown</color>");
                return false;
            }

            Activate(newAbility, ModeStatus, deb);

            return true;
        }

      

        /// <summary> Called by the Mode Behaviour on Entering the Animation State.
        ///Done this way to check for Modes that are on other Layers besides the Base Layer </summary>
        public void AnimationTagEnter(int AnimationPathHash)
        {
            if (ActiveAbility != null && !PlayingMode)
            {
                PlayingMode = true;
                Animal.IsPreparingMode = false;

                Animal.ActiveMode = this;

                Animal.Set_State_Sleep_FromMode(true);                          //Put to sleep the states needed

                OnEnterInvoke();                                                //Invoke the ON ENTER Event

                ActivationTime = Time.time;                                 //Store the time the Mode started

              //  if (!AllowMovement) Animal.InertiaPositionSpeed = Vector3.zero; //Remove Speeds if the Mode is Playing that does not allow Movement

                var AMode = ActiveAbility.Status;                    //Check if the Current Ability overrides the global properties

                var AModeName = AMode.ToString();

                int ModeStatus = -1;               //That means the Ability is Loopable

                if (AMode == AbilityStatus.PlayOneTime)
                {
                    ModeStatus = 1;                //That means the Ability is OneTime 
                }
                if (AMode == AbilityStatus.Charged)
                {
                    InputValue = true;               //Make sure the Input Value is se to true on Charged 
                }
                else if (AMode == AbilityStatus.ActiveByTime)
                {
                    float HoldByTime = ActiveAbility.AbilityTime;

                    Animal.StartCoroutine(Ability_By_Time(HoldByTime));
                    AModeName += ": " + HoldByTime;
                    InputValue = false;
                }
                else if (AMode == AbilityStatus.Toggle)
                {
                    AModeName += " On";
                    InputValue = false;
                }

                Debugging($"<B><color=yellow>[ANIM-ENTER]</color></B> Ability: " +
                    $"<B><color=white>[{ActiveAbility.Name}]</color> Status: <color=white> [{AModeName}]</color></B>");

                SetCoolDown();
                Animal.SetModeStatus(ModeStatus);
            }

        }

        internal void OnAnimatorMove(float deltaTime)
        {
            if (ActiveAbility.Status == AbilityStatus.Charged && ActiveAbility.AbilityTime > 0)
            {
                var elapsedTime = (Time.time - ActivationTime)/ActiveAbility.AbilityTime;
                var curve = ActiveAbility.ChargeCurve.Evaluate(elapsedTime);

                ChargeValue = curve * ActiveAbility.ChargeValue;

                Animal.Mode_SetPower(curve);
                ActiveAbility.OnCharged.Invoke(ChargeValue);

                //Release the Charged Ability
                if (elapsedTime > 1 && ActiveAbility.Release)
                {
                    InputValue = false;
                    Interrupt();
                }
            }    
        }

        /// <summary>Called by the Mode Behaviour on Exiting the  Animation State 
        /// Done this way to check for Modes that are on other Layers besides the base one </summary>
        public void AnimationTagExit(Ability exitingAbility, int ExitTransitionAbility)
        {
            //Debug.Log("Active Ability = " + ActiveAbility.Index.Value);
            //Debug.Log("Exiting Avility = " + exitingAbility.Index.Value);
            //Debug.Log("ActiveMode = " + Animal.ActiveMode);
            //Means that we just exiting the same animation that we entered IMPORTANT

              string deb = $"<B><color=red>[ANIM-EXIT]</color></B> Ability: " +
                $"<B><color=white>[{(exitingAbility != null ?  exitingAbility.Name: "NULL")}]</color> </B> ";
             
              var ExitTagLogic =  $"Status: <B><color=white>[Skip Exit Logic]</color></B>";

            if (Animal.ActiveMode == this && ActiveAbility != null && ActiveAbility.Index.Value == exitingAbility.Index.Value)
            {
                ExitTagLogic = $"Status: <B><color=white>[Mode Reseted] Status:[{ActiveAbility.Status}] " +
                    $"ExitAb:[{exitingAbility.Index.Value}]</color></B>";
                Debugging(deb + ExitTagLogic);


                //Set the cooldown after the mode has finish if is not set to Play one time
                if (ActiveAbility.Status != AbilityStatus.PlayOneTime) 
                    SetCoolDown(); 

                //OnExitInvoke();
                ResetMode();
                ModeExit();

                if (ExitTransitionAbility != -1)  //Meaning it will end in another mode
                {
                    IsInTransition = false;       //Reset that is in transition IMPORTANT

                    if (TryActivate(ExitTransitionAbility))
                    {
                        ExitTagLogic = $"Status: <B><color=white>[Exit to another Ability]</color></B>";
                        Debugging(deb + ExitTagLogic);

                        AnimationTagEnter(0);  //Do the animation Tag Enter since the next animation it may not be a entering mode animation
                    }
                }
                else
                {
                    if (!InCoolDown) //If the Mode is not in CoolDown
                    { 
                        if (InputValue)                             //Check if the Input is still Active so the mode can be reactivated again.
                            TryActivate();
                        else if (exitingAbility.InputValue)         //Check if the Input is still Active on th Ability so the mode can be reactivated again.
                            TryActivate(exitingAbility);
                    }
                }
            }
            else
            {
                Debugging(deb + ExitTagLogic);
            }
        } 

        public virtual Ability GetTryAbility(int index)
        {
            if (!Active) return null;                   //If the mode is disabled: Ignore
            AbilityIndex = index;
            
            //Check first if there's a modifier on Enter. Some mdifiers it will change the ABILITY INDEX...IMPORTANT 
            modifier?.OnModeEnter(this);
            
            if (AbilityIndex == 0) return null;        //if the Index is 0 Ignore 

            if (Abilities == null || Abilities.Count == 0)
            {
                Debugging("There's no Abilities Please set a list of Abilities");
                return null;
            }


            //Set the Index of the Ability for the Mode, Check for Random
            if (AbilityIndex == -99)
                return GetAbility(Abilities[Random.Range(0, Abilities.Count)].Index.Value);
                 
            return GetAbility(AbilityIndex); //Find the Ability
        }

        /// <summary> Returns an ability by its Index </summary>
        public virtual Ability GetAbility(int NewIndex) => Abilities.Find(item => item.Index == NewIndex);

        /// <summary> Returns an ability by its Name </summary>
        public virtual Ability GetAbility(string abilityName) => Abilities.Find(item => item.Name == abilityName);


        public virtual void OnModeStateMove(AnimatorStateInfo stateInfo, Animator anim, int Layer)
        {
            //IsInTransition = anim.IsInTransition(Layer) &&
            //(anim.GetNextAnimatorStateInfo(Layer).fullPathHash != anim.GetCurrentAnimatorStateInfo(Layer).fullPathHash);

            if (Animal.ActiveMode == this)
            {
                Animal.ModeTime = stateInfo.normalizedTime;
                modifier?.OnModeMove(this, stateInfo, anim, Layer);
                ActiveAbility.modifier?.OnModeMove(this, stateInfo, anim, Layer);
            }
        }

        /// <summary> Check for Exiting the Mode, If the animal changed to a new state and the Affect list has some State</summary>
        public virtual bool StateCanInterrupt(StateID ID, Ability ability = null)
        {
            if (ability == null) ability = ActiveAbility;

            var properties = ability.Limits;

            if (properties.affect == AffectStates.None) return false;

            if (ability.HasAffectStates)
            {
                if (properties.affect == AffectStates.Exclude && HasState(properties, ID)      //If the new state is on the Exclude State
                || (properties.affect == AffectStates.Include && !HasState(properties, ID)))   //OR If the new state is not on the Include State
                {
                    return true;
                }
            }
            return false;
        }


        public virtual bool StanceCanInterrupt(StanceID ID, Ability ability = null)
        {
            if (ability == null) ability = ActiveAbility;

            var properties = ability.Limits;

            if (properties.affect_Stance == AffectStates.None) return false;

            if (ability.HasAffectStances)
            {
                if (properties.affect_Stance == AffectStates.Exclude && HasStance(properties, ID)      //If the new state is on the Exclude State
                || (properties.affect_Stance == AffectStates.Include && !HasStance(properties, ID)))   //OR If the new state is not on the Include State
                {
                    Debugging($"Current Stance [{ID.name}] is Blocking <B>" + ability.Name + "</B>");

                    return true;
                }
            }
            return false;
        }

        /// <summary>Find if a State ID is on the Avoid/Include Override list</summary>
        protected static bool HasState(ModeProperties properties, StateID ID) => properties.affectStates.Exists(x => x.ID == ID.ID);
        protected static bool HasStance(ModeProperties properties, StanceID ID) => properties.Stances.Exists(x => x.ID == ID.ID);


        private void SetCoolDown()
        {
            if (HasCoolDown)
            {
                if (I_CoolDown != null)
                { 
                    Animal.StopCoroutine(I_CoolDown);
                }
                Animal.StartCoroutine(I_CoolDown = C_SetCoolDown(CoolDown));
            }
        }

        private IEnumerator I_CoolDown;

        public IEnumerator C_SetCoolDown(float time)
        {
            InCoolDown = true;
            yield return new WaitForSeconds(time);
            InCoolDown = false;

            //if (InputValue && ActiveAbility != null && ActiveAbility.Status == AbilityStatus.PlayOneTime) //Only Reset with PlayOneTime
            //{
            //    ResetMode();
            //    ModeExit();  
            //    TryActivate(AbilityIndex); //Check if the Input is still Active when there's a cooldown
            //}
        } 

        protected IEnumerator Ability_By_Time(float time)
        {
            yield return new WaitForSeconds(time);
            Animal.Mode_Interrupt();

            Debugging($"<B><color=yellow>[INTERRUPTED]</color> Ability: <Color=white>[{ActiveAbility.Name}]</color> " +
                        $"Status: <Color=white>[Time elapsed]</color></B>");
        }

        private void OnExitInvoke()
        {
            ActiveAbility.OnExit.Invoke();
            OnExitMode.Invoke();
        }

        private void OnEnterInvoke()
        {
            ActiveAbility.OnEnter.Invoke();
            OnEnterMode.Invoke();
        }


        private bool CheckStatus(AbilityStatus status)
        {
            if (ActiveAbility == null) return false;
            return ActiveAbility.Status == status;
        }

        /// <summary>Disable the Mode. If the mode is playing it check the status and it disable it properly </summary>
        public virtual void Disable()
        {
            Active = false;
            InputValue = false;
            InCoolDown = false;

            if (PlayingMode)
            {
                if (!CheckStatus(AbilityStatus.PlayOneTime))
                {
                    Animal.Mode_Interrupt();
                }
                else
                {
                    //Do nothing ... let the mode finish since is on AbilityStatus.PlayOneTime
                }
            }
        }

        public virtual void Enable() => Active = true;

        /// <summary> Enable the Mode temporarily by an external source, use Disable Temporal when using this</summary>
        public virtual void Enable_Temporal() => TemporalActivation++;


        /// <summary> Disable the Mode temporarily by an external source, use EnableTemporal to reset it back up </summary>
        public virtual void Disable_Temporal() => TemporalActivation--;


        /// <summary> Reset Temporal Activation</summary>
        public virtual void Reset_Temporal() => TemporalActivation = 1;


        internal void Debugging(string deb)
        {
#if UNITY_EDITOR
            if (Animal.debugModes) Debug.Log($"[{Animal.name}] → Mode <color=white> <b>[{ID.name}]</b> </color> - {deb}"); 
#endif
        }

    }
    /// <summary> Ability for the Modes</summary>
    [System.Serializable]
    public class Ability
    {
        /// <summary>Is the Ability Active</summary>
        public BoolReference active = new BoolReference(true);
        /// <summary>Name of the Ability (Visual Only)</summary>
        public string Name;
        /// <summary>index of the Ability </summary>
        public IntReference Index = new IntReference(0);

        [Tooltip("Unique Input to play for each Ability")]
        public StringReference Input;

        [Tooltip("Clip to play when the ability is played")]
        public AudioClipReference audioClip;

        [Tooltip("Clip Sound Delay")]
        public FloatReference ClipDelay = new FloatReference(0);

        [Tooltip("Local AudioSource for an specific Ability")]
        public AudioSource audioSource;

        [Tooltip("Stop the Audio sound on Ability Exit")]
        public bool m_stopAudio= true;

        [Tooltip("Local Mode Modifier to Add to the Ability")]
        [ExposeScriptableAsset]
        public ModeModifier modifier;

        /// <summary>Overrides Properties on the mode</summary>
        [UnityEngine.Serialization.FormerlySerializedAs("Properties")]
        public ModeProperties Limits;

        /// <summary>The Ability can Stay Active until it finish the Animation, by Holding the Input Down, by x time </summary>
        [Tooltip("The Ability can Stay Active until it finish the Animation, by Holding the Input Down, by x time ")]
        public AbilityStatus Status = AbilityStatus.PlayOneTime;


        /// <summary>The Ability can Stay Active by x seconds </summary>
        [Tooltip("The Ability will be completely charged after x seconds. If the value is zero, the charge logic will be ignored")]
        public FloatReference abilityTime = new FloatReference(3);

        [Tooltip("Curve value for the charged ability")]
        public AnimationCurve ChargeCurve = new AnimationCurve(MTools.DefaultCurve);

        [Tooltip("Charge maximun value for the Charged ability")]
        public FloatReference ChargeValue = new FloatReference(1);


        [Tooltip("Release the Charged Ability when it reaches is Time")]
        public bool Release;

        [Tooltip("Multiplier added to the Additive position when the mode is playing. This will fix the issue Additive Speeds to mess with RootMotion Modes")]
        public float AdditivePosition = 1f;



        /// <summary>Time value when the Status is set Time</summary>
        public float AbilityTime { get => abilityTime.Value ; set => abilityTime.Value = value; }

        /// <summary>It Has Affect states to check</summary>
        public bool HasAffectStates => Limits.affectStates != null && Limits.affectStates.Count > 0;

        /// <summary>It Has Affect stances to check</summary>
        public bool HasAffectStances => Limits.Stances != null && Limits.Stances.Count > 0;
        public bool HasTransitionFrom => Limits.TransitionFrom != null && Limits.TransitionFrom.Count > 0;

        public bool Active { get => active.Value; set => active.Value = value; }

        /// <summary>Internal Ability Input value</summary>
        public bool InputValue { get; internal set; }


        /// <summary> Used to connect the Inputs to the Abilities instead of the General Mode </summary>
        public UnityAction<bool> InputListener; //Store the Input Listener


    
        public UnityEvent OnEnter = new UnityEvent();
        public UnityEvent OnExit = new UnityEvent();
        public FloatEvent OnCharged = new FloatEvent();
    }

    public enum AbilityStatus
    {
        /// <summary> The Ability is Enabled One time and Exit when the Animation is finished </summary>
        PlayOneTime = 0,
        /// <summary> The Ability can be charged</summary>
        [InspectorName("Charged or Hold Input Down")]
        Charged = 1,
        /// <summary> The Ability is On for an x ammount of time</summary>
        ActiveByTime = 2,
        /// <summary> The Ability is ON and OFF everytime the Activate method is called</summary>
        Toggle = 3,
        /// <summary> The Ability is Play forever until is Mode Interrupt is called</summary>
        Forever = 4,
    }
    public enum AffectStates
    {
        None,
        Include,
        Exclude,
    }

    [System.Serializable]
    public class ModeProperties
    { 
        [Tooltip("Exclude: The mode will not be activated when is on a State of the List.\n" +
            "Include: The mode will only be actived when the Animal is on a State of the List")]
        public AffectStates affect = AffectStates.None;

        /// <summary>Include/Exclude the  States on this list depending the Affect variable</summary>
        [Tooltip("Include/Exclude the  States on this list depending the Affect variable")]
        public List<StateID> affectStates = new List<StateID>();

        [Tooltip("Exlcude: The mode will not be activated when is on a Stance of the List.\n" +
            "Include: The mode will only be actived when the Animal is on a Stance of the List")]
        public AffectStates affect_Stance = AffectStates.None;
        /// <summary>Include/Exclude the  Stances on this list depending the Affect variable</summary>
        [Tooltip("Include/Exclude the Stances on this list depending the Affect Stanes variable")]
        public List<StanceID> Stances = new List<StanceID>();

        [Tooltip("Modes can transition from other abilities inside the same mode. E.g Seat -> Lie -> Sleep")]
        public List<int> TransitionFrom = new List<int>();

        public ModeProperties(ModeProperties properties)
        {
            affect = properties.affect;
            affect_Stance = properties.affect_Stance;
            affectStates = new List<StateID>(properties.affectStates);
            Stances = new List<StanceID>(properties.Stances);
            TransitionFrom = new List<int>();
        }
    }
}