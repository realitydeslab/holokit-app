using MalbersAnimations.Controller;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations
{
    [RequireComponent(typeof(Animator))]
    /// <summary>Sync the Parameters of an alave Animator to a Master Animator</summary>
    [AddComponentMenu("Malbers/Utilities/Animator/Animator Sync")]

    public class MAnimalAnimatorSync : MonoBehaviour
    {
        [RequiredField,Tooltip("Master Animator Reference to get the parameters values")] 
        public Animator MasterAnimator;
        
        [SerializeField]
        [Tooltip("Slave Animator to receive the parameters values")]  
        private Animator SlaveAnimator;

        [Tooltip("Which Layer Index will be used to sync to the Slave Animator")] 
        public int Layer = 0;
        [Tooltip("When the Animator is playing a blendtree or a loopable animation. it will check if both Animator times are in sync. If not it will force a synchronization")] 
        public bool Resynchronize = true;
        [Hide("Resynchronize"),Tooltip("Threshold to check if the slave animator is unsync")]
        public float Threshold = 0.1f;
        [Hide("Resynchronize"), Tooltip("Which State will be synced again")]
        public List<int> StateCheck = new List<int>();

        [Space]
        [Tooltip("Enables the Offset position and rotation from the Master Animator")]
        public bool useTransformOffset = false;
        [Tooltip("Offset the position relative to the Master Animator.\nRIGHT CLICK to Calculate the current offest")]
        [ContextMenuItem("Calculate Current Position Offset", "CalculateCurrentOffset")] 
        [Hide("useTransformOffset")] 
        public Vector3 PosOffset;
        [Tooltip("Offset the rotation relative to the Master Animator.")]
        [Hide("useTransformOffset")]
        public Vector3 RotOffset;

        private IMAnimator listenTo;
        private IAnimatorStateCycle StateCycle;

        private List<int> animatorParams; //Faster with a list and Contains than a dictionary when the Items are lower than 20

        const float crossFade = 0.2f;

        private void Awake()
        {
            animatorParams = new List<int>();
            if (MasterAnimator != null)
            {
                listenTo = MasterAnimator.GetComponent<IMAnimator>();
                StateCycle = MasterAnimator.GetComponent<IAnimatorStateCycle>();
            }

            SlaveAnimator = GetComponent<Animator>();

            foreach (AnimatorControllerParameter parameter in SlaveAnimator.parameters)
                animatorParams.Add(parameter.nameHash);
        }

        private void OnEnable()
        {
            listenTo.SetBoolParameter += SetAnimParameter;
            listenTo.SetIntParameter += SetAnimParameter;
            listenTo.SetFloatParameter += SetAnimParameter;
            listenTo.SetTriggerParameter += SetAnimParameter;
            if (Resynchronize) StateCycle.StateCycle += SyncStateCycle;
        }

        private void OnDisable()
        {
            listenTo.SetBoolParameter -= SetAnimParameter;
            listenTo.SetIntParameter -= SetAnimParameter;
            listenTo.SetFloatParameter -= SetAnimParameter;
            if (Resynchronize) StateCycle.StateCycle -= SyncStateCycle;
        }

        private void Update()
        {
            if (useTransformOffset)
            {
                transform.position = MasterAnimator.transform.position + PosOffset;
                transform.rotation = MasterAnimator.transform.rotation * Quaternion.Euler(RotOffset);
            }
        }

        private void SyncStateCycle(int currentState)
        {
            if (MasterAnimator.IsInTransition(0) || SlaveAnimator.IsInTransition(Layer)) return; //Do not Resync when is on  transition

            if (HasStateCheck(currentState))
            {
                var MasterStateInfo = MasterAnimator.GetCurrentAnimatorStateInfo(0);
                var SlaveStateInfo = SlaveAnimator.GetCurrentAnimatorStateInfo(Layer);


                var MainTime = MasterStateInfo.normalizedTime;            //Get the normalized time from the Rider
                var SlaveTime = SlaveStateInfo.normalizedTime;            //Get the normalized time from the Horse

                if (Mathf.Abs(MainTime - SlaveTime) >= Threshold)   //Checking if the animal and the rider are unsync by 0.2
                {
                    SlaveAnimator.CrossFade(SlaveStateInfo.fullPathHash, crossFade, Layer, MainTime);                 //Normalized with blend
                    //Debug.Log("Resync");
                }
            }
        }


        public bool HasStateCheck(int check)
        {
            if (StateCheck.Count == 0) return false;

            foreach (var st in StateCheck)
                if (st == check) return true;

            return false;
        }

        /// <summary>Set a Int on the Animator</summary>
        public void SetAnimParameter(int hash, int value) 
        {
            if (animatorParams.Contains(hash)) SlaveAnimator.SetInteger(hash, value);
        }

        /// <summary>Set a float on the Animator</summary>
        public void SetAnimParameter(int hash, float value)
        {
            if (animatorParams.Contains(hash)) SlaveAnimator.SetFloat(hash, value);
        }

        /// <summary>Set a Trigger on the Animator</summary>
        public void SetAnimParameter(int hash)
        {
            if (animatorParams.Contains(hash)) SlaveAnimator.SetTrigger(hash);
        }


        /// <summary>Set a Bool on the Animator</summary>
        public void SetAnimParameter(int hash, bool value) 
        {
            if (animatorParams.Contains(hash)) SlaveAnimator.SetBool(hash, value); 
        }


        private void OnValidate()
        {
           if (SlaveAnimator== null) SlaveAnimator = GetComponent<Animator>(); 
        }

        [ContextMenu("Calculate Current Position Offset")]
        private void CalculateCurrentOffset()
        {
            PosOffset = MasterAnimator.transform.InverseTransformPoint(transform.position);
        }
    }
}
