using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System;
using System.Collections;
using MalbersAnimations.Utilities;

namespace MalbersAnimations.Controller
{
    /// <summary>Class to identify stances on the Animal Controller </summary>
    [System.Serializable]
    public class Stance
    {
        [Tooltip("ID value for the Stance")]
        public StanceID ID;

        [Tooltip("Enable Disable the Stance")]
        public BoolReference enabled = new BoolReference(true);

        [Tooltip("Unique Input to play for each Ability")]
        public StringReference Input;

        [Tooltip("Lock the Stance if its Active. No other Stances can be enabled.")]
        public BoolReference persistent = new BoolReference();

        [Tooltip("Does this Stance allows Straffing?")]
        public BoolReference CanStrafe = new BoolReference();

        [Tooltip("After the Stance has exited, it cannot be activated again after the cooldown has passed")]
        public FloatReference CoolDown = new FloatReference(0);

        [Tooltip("If this Stance was activated, it cannot be Exit until the Exit cooldown has passed")]
        public FloatReference ExitAfter = new FloatReference(0);

        [Tooltip("Is/Is NOT active State on this list")]
        public bool Include = true;

        [Tooltip("Include/Exclude the States on this list that can be used with the Stance")]
        public List<StateID> states = new List<StateID>();

        /// <summary>Does the states list is not empty??</summary>
        public bool HasStates => states.Count > 0;

        [Tooltip("What States can queue the activation of this Stance")]
        public List<StateID> StateQueue = new List<StateID>();

        [Tooltip("Stances to Block while this stance is active")]
        public List<StanceID> DisableStances = new List<StanceID>();

        /// <summary> Current Stored Input Value </summary>
        public bool InputValue { get; set; }


        /// <summary>Set Block Values to check if anyone else have disabled this Stace</summary>
        public int DisableValue { get; set; }
        public bool DisableTemp => DisableValue < 0;

        /// <summary> The Stance is Enabled/Disable</summary>
        public bool Enabled { get => enabled.Value; set => enabled.Value = value; }

        /// <summary> Lock the Stance if its Active. No other Stances can be enabled.</summary>
        public bool Persistent { get; set; }
        //{
        //    get => persistent.Value;
        //    set
        //    {
        //        persistent.Value = value;
        //       // Debug.Log($" Persistent [{ID.name} ]" + value);
        //    }
        //}

        /// <summary>Current Activated Stance on the Animal</summary>
        public bool Active { get; set; }

        /// <summary>The State try to be activated but a state did not allowed. That State is on the QeueList so Lets queue it</summary>
        public bool Queued { get; set; }

        public MAnimal Animal { get; set; }

        /// <summary>  When was the Stance Activated? </summary>
        public float ActivationTime { get; private set; }

        /// <summary>  When was the Stance Exited? </summary>
        public float ExitTime { get; private set; }

        /// <summary>On Activation, Can the Stance Exit?</summary>
        public bool CanExit => ExitAfter == 0 || MTools.ElapsedTime(ActivationTime, ExitAfter);

        /// <summary> Remaining Time to activate again the stance </summary>
        public float CoolDownLeft => ExitTime + CoolDown - Time.time;
        /// <summary> Remaining Time to allow to exit the stance</summary>
        public float CanExitTimeLeft => ActivationTime + ExitAfter - Time.time;


        /// <summary>After Activation, can the Stance be activated again?</summary>
        public bool InCoolDown => CoolDown > 0 && !MTools.ElapsedTime(ExitTime, CoolDown);
        public OnEnterExitStance events { get; set; }

       
        internal virtual void AwakeStance(MAnimal animal)
        {
            if (ID == null)
            {
                Debug.LogWarning($"<B>[{Animal.name}]</B> Has Empty Stances. Please set the correct Stance ID ",animal.gameObject);
            }

            Animal = animal;
            events = animal.OnEnterExitStances.Find(x => x.ID == ID);
            ActivationTime = float.MinValue;
            ExitTime = float.MinValue; 
            Queued = false;
        }


        internal void ConnectInput(IInputSource InputSource, bool connect)
        {
            if (connect)
                InputSource.ConnectInput(Input, ActivatebyInput);
            else
                InputSource.DisconnectInput(Input, ActivatebyInput);
        }

        public virtual void SetPersistent(bool value)
        {
            if (Active || Queued)
            {
                Debugging($"Persistent [{value}]");
                Persistent = value;
            }
            else
            {
                Debugging("Cannot Set Persistent. This is not the Active Stance");
            }
        }

        public virtual void Enable(bool value) => Enabled = value;
        public virtual void SetQueued(bool value)
        {
            Queued = value;
            Debugging($"Queued [{value}]");
        }

        public void ActivatebyInput(bool Input_Value)
        {
            if (CanActivate())
            {
                InputValue = Input_Value;

                if (Input_Value)
                {
                    Animal.Stance = ID;         //Set the State
                }
                else
                {
                    Animal.Stance_Reset();
                    Queued = false;
                }
            }
        }

        public void Enable_Temp()
        {
            DisableValue++;
            //Debug.Log($" {ID.name} DisableValue : {DisableValue}" );
        }

        public void Disable_Temp()
        {
            DisableValue--;
            //Debug.Log($" {ID.name} DisableValue : {DisableValue}");
        }

        /// <summary> Verifies if the Stance can be activated </summary>
        public bool CanActivate()
        {
            if (!Enabled)  { Debugging("Failed. Stance is Disabled"); return false; }
            if (!Animal.enabled)  { Debugging("Failed. Animal disabled"); return false; }
            if (DisableTemp) { Debugging($"Failed. Disable by External [{DisableValue}]"); return false; }

            if (Animal.ActiveStance.Persistent) { Debugging($"Ignored. Active Stance [{Animal.ActiveStance.ID.name}] is Persistent"); return false; }
            if (InCoolDown) { Debugging($"Failed. Stance in CoolDown. Time left {CoolDownLeft:F2}"); return false; }
            if (!Animal.ActiveStance.CanExit)
            { Debugging($"Failed. Active Stance [{Animal.ActiveStance.ID.name}] can't exit yet. Exit After {(Animal.ActiveStance.CanExitTimeLeft):F2}"); 
                return false; }

            if (HasStates)
            {
                var ActiveState = Animal.ActiveStateID;

                var ContainState = states.Contains(ActiveState); //Find if the Active State is on the list

                if (ContainState && !Include)
                { 
                    if (OnQueueState(ActiveState)) { Queued = true; }
                    Debugging($"Failed. Active State [{ActiveState.name}] is Excluded from the allowed States. Set Queued[{Queued}]"); 
                    return false;
                }
                if (!ContainState && Include)
                { 
                    if (OnQueueState(ActiveState)) { Queued = true; }
                    Debugging($"Failed. Active State [{ActiveState.name}] is Not Included in the allowed States. Set Queued[{Queued}]");
                    return false;
                }
            }
            return true;
        }

    
        internal void Reset()
        {
            InputValue = false;
            Persistent = false;
            Queued = false;
        }

        internal void Activate()
        {
            ActivationTime = Time.time;
            Active = true;
            Queued = false;
            events?.OnEnter.Invoke();
        }

        internal void Exit()
        {
            Active = false;
            ExitTime = Time.time;
            events?.OnExit.Invoke();
        }

        /// <summary> A new State has been activated</summary>
        internal void NewStateActivated(StateID stateID)
        {
            if (CanBeUsedOnState(stateID) && Queued)
            {
                SetQueued(false);
                Animal.Stance = this.ID; //Try to activate a queue Stance
            }
        }

        /// <summary>  Checks if this stance can be used on a given state   </summary>
        internal bool CanBeUsedOnState(StateID activeStateID)
        {
            if (!HasStates) return true; //Return true since it has no limitations

            var ContainState = states.Contains(activeStateID); //Find if the Active State is on the list
            if (ContainState && !Include) return false;
            if (!ContainState && Include) return false;

            return true;
        }


        internal bool OnQueueState(StateID activeStateID)
        {
            return StateQueue.Contains(activeStateID); //Find if the Active State is on the list
        }
          

        private void Debugging(string value)
        {
#if UNITY_EDITOR    
            if (Animal.debugStances)
            {
                Debug.Log($"<B>[{Animal.name}]</B> - <B><color=yellow>[Stances:{ID.name}:{ID.ID}]</color> - <color=white>{value}</color></B>", Animal.gameObject);
            }
#endif
        }
    }
}
