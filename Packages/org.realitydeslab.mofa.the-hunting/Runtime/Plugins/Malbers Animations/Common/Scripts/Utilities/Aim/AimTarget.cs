using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
    /// <summary>For when someone with LookAt enters it will set this transform as the target</summary>
    [AddComponentMenu("Malbers/Utilities/Aiming/Aim Target")]
    public class AimTarget : MonoBehaviour, IAimTarget
    {
        /// <summary>All Active AimTargets in the current scene</summary>
        public static List<AimTarget> AimTargets;

        /// <summary>This will set AutoAiming for the Aim Logic</summary>
        [SerializeField,Tooltip("It will center the Aim Ray into this gameObject's collider")] 
        private bool aimAssist;

        /// <summary>This will set AutoAiming for the Aim Logic</summary>
        [SerializeField, Tooltip("The Aim Assist will use Own Trigers to find Aimers")]
        private bool UseOnTriggerEnter = true;

        [SerializeField, Tooltip("Transform Point for the Aim Assist")]
        private Transform m_AimPoint;

     //   public Vector3Reference Offset;



        public GameObjectEvent OnAimEnter = new GameObjectEvent();
        public GameObjectEvent OnAimExit = new GameObjectEvent();


        public bool debug;

        //public System.Action<AimTarget> OnAddedAimTarget { get; private set; } = delegate { };
        //public System.Action<AimTarget> OnRemovedAimTarget { get; private set; } = delegate { };


        /// <summary>This will set AutoAiming for the Aim Logic</summary>
        public bool AimAssist { get => aimAssist; set => aimAssist = value; }
        public bool IsBeingAimed { get; set; }
       // public bool AimedFocused { get; set; }
        public Transform AimPoint => m_AimPoint;


        protected virtual void OnEnable()
        {
            if (m_AimPoint == null) m_AimPoint = transform;
            if (AimTargets == null) AimTargets = new List<AimTarget>();
            AimTargets.Add(this);
          //  OnAddedAimTarget(this);
        }

        protected virtual void OnDisable()
        {
            AimTargets.Remove(this);
            //if (AimedFocused) OnAimExit.Invoke(null);
            // OnRemovedAimTarget(this);
        }

        private void OnValidate()
        {
            if (m_AimPoint == null) m_AimPoint = transform;
        }

        /// <summary>Is the target been aimed by the Aim Ray of the Aim Script</summary>
        public void IsBeenAimed(bool enter, GameObject AimedBy)
        {
           if (debug) Debug.Log($"Is Being Aimed by [{AimedBy.name}]",this);

            IsBeingAimed = enter;

            if (enter)
                OnAimEnter.Invoke(AimedBy);
            else
                OnAimExit.Invoke(AimedBy);
        }


        /// Aim Targets can be also used as Trigger Enter Exit 
        void OnTriggerEnter(Collider other)
        {
            if (!UseOnTriggerEnter) return; //Ignore if the Collider entering is a Trigger
            if (other.isTrigger) return; //Ignore if the Collider entering is a Trigger

            IAim Aimer = other.FindInterface<IAim>();
            if (Aimer == null)  Aimer = other.transform.root.FindInterface<IAim>(); //Search in the Root if it was not found

            if (Aimer != null)
            {
                if (debug) Debug.Log($"OnTrigger Enter [{other.name}]", this);
                Aimer.AimTarget = AimPoint;
                OnAimEnter.Invoke(other.gameObject);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!UseOnTriggerEnter) return;             //Ignore if we are not using OnTrigger Enter
            if (other.isTrigger) return;                //Ignore if the Collider exiting is a Trigger

            IAim Aimer = other.transform.root.FindInterface<IAim>();

            if (Aimer != null)
            {
                Aimer.AimTarget = null;
                OnAimExit.Invoke(other.gameObject);
            }
        }


        //public static bool AimAssit(Transform o)
        //{
        //    return AimTargets.Exists(x => x.transform == o);
        //}

        //public void SendOffset(MEvent _event) => _event.Invoke(Offset.Value);
    }
}