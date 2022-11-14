using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using System;

namespace MalbersAnimations.Utilities
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/utilities/lock-on-target")]
    public class LockOnTarget : MonoBehaviour
    {
        [Tooltip("The Lock On Target will activate automatically if any target is stored on the list")]
        public BoolReference Auto =  new BoolReference(false);


        [Tooltip("The Lock On Target requires an Aim Component")]
        [RequiredField] public Aim aim;

        [Tooltip("Set of the focused 'potential' Targets")]
        [RequiredField] public RuntimeGameObjects Targets;

       // public BoolReference UseAimTargets = new BoolReference(false);

        private int CurrentTargetIndex = -1;
        public bool debug;
        

        [Header("Events")]
        public TransformEvent OnTargetChanged = new TransformEvent();
        public BoolEvent OnLockingTarget = new BoolEvent();

        /// <summary> </summary>
        public Transform LockedTarget
        { get => locketTarget;

            private set
            {
                locketTarget = value;
                aim.SetTarget(value);
                OnTargetChanged.Invoke(value);
            }
        }
        private Transform locketTarget;


      //  private Transform DefaultAimTarget;

        public bool LockingOn { get; private set; }
        


        public AimTarget IsAimTarget { get; private set; }
        public GameObject Owner => transform.root.gameObject;

        private void Awake()
        {
            Targets.Clear();

            //DefaultAimTarget = null;
            //if (aim != null) DefaultAimTarget = aim.AimTarget;
        }

        private void OnEnable()
        {
            if (Targets != null)
            {
                Targets.OnItemAdded.AddListener(OnItemAdded);
                Targets.OnItemRemoved.AddListener(OnItemRemoved);
            }

            ResetLockOn();
        }

       


        private void OnDisable()
        {
            if (Targets != null)
            {
                Targets.OnItemAdded.RemoveListener(OnItemAdded);

                Targets.OnItemRemoved.RemoveListener(OnItemRemoved);
            }
            ResetLockOn();
        }

        private void OnItemAdded(GameObject arg0)
        {
            if (Auto.Value && !LockingOn)
            {
                LockTarget(true);
            }
        }

        public void LockTargetToggle()
        {
            LockingOn ^= true;
            if (debug) Debug.Log("Locked Target = " + LockingOn, this); 
            LookingTarget();
        }



        public void LockTarget(bool value)
        {
            LockingOn = value;
            if (debug) Debug.Log("Locked Target = " + LockingOn, this);
            LookingTarget();
        }

        private void LookingTarget() 
        {
            if (LockingOn)
            {
                if (Targets != null && Targets.Count > 0) //if we have a focused Item
                {
                    FindNearestTarget();
                    OnLockingTarget.Invoke(true);
                }
            }
            else
            {
                ResetLockOn();
            }
        }

       //Reset the values when the Lock Target is off
        private void ResetLockOn()
        {
                CurrentTargetIndex = -1;
                LockedTarget = null;
                OnLockingTarget.Invoke( LockingOn = false);


                //if (IsAimTarget != null)
                //{ 
                //    IsAimTarget.IsBeenAimed(false, Owner);
                //    IsAimTarget = null;
                //}
        }

        private void FindNearestTarget()
        {
            var closest = Targets.Item_GetClosest(gameObject);  //When Lock Target is On.. Get the nearest Target on the Set 

            if (closest)
            {
                LockedTarget = closest.transform;
                CurrentTargetIndex = Targets.items.IndexOf(closest);    //Save the Index so we can cycle to all the targets.

                //if (UseAimTargets)
                //{
                //    if (IsAimTarget) IsAimTarget.IsBeenAimed(false, Owner); //Check if there's an Active Aim Target

                //    IsAimTarget = AimTarget.AimTargets.Find(x => x.transform == LockedTarget);

                //    if (IsAimTarget) IsAimTarget.IsBeenAimed(true, Owner);

                //}
            }
            else
            {
                ResetLockOn();
            }
        }

        public void Target_Next()
        {
            if (Targets != null && LockedTarget != null && CurrentTargetIndex != -1) //Check everything is working
            {
                CurrentTargetIndex++;
                CurrentTargetIndex %= Targets.Count; //Cycle to the first in case we are on the last item on the list
                LockedTarget = Targets.Item_Get(CurrentTargetIndex).transform;     //Store the Next Target
            }
        }

        public void Target_Previous()
        {
            if (Targets != null && LockedTarget != null && CurrentTargetIndex != -1) //Check everything is working
            {
                CurrentTargetIndex--;
                if (CurrentTargetIndex == -1) CurrentTargetIndex = Targets.Count - 1;
                LockedTarget = Targets.Item_Get(CurrentTargetIndex).transform;     //Store the Next Target
            }
        }

        private void OnItemRemoved(GameObject _)
        {
            if (LockingOn) //If we are still on Lock Mode then find the next Target
                this.Delay_Action(() => FindNearestTarget()); //Find the nearest target the next frame
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!aim) aim = this.FindComponent<Aim>();
        }


        private void Reset()
        {
            aim = this.FindComponent<Aim>();

            Targets = MTools.GetInstance<RuntimeGameObjects>("Lock on Targets");
            var lockedTarget = MTools.GetInstance<TransformVar>("Locked Target");

            var CamEvent = MTools.GetInstance<MEvent>("Set Camera LockOnTarget");


            UnityEditor.Events.UnityEventTools.AddPersistentListener(OnTargetChanged, CamEvent.Invoke);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(OnTargetChanged, lockedTarget.SetValue);
        }


        //[ContextMenu("Create Input")]
        //private void CreateInput()
        //{
            
        //}
#endif
    }
}