using MalbersAnimations.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [System.Serializable]
    public class MSpeedSet : IComparable,IComparer
    {
        [Tooltip("Name of the Speed Set")]
        public string name;
        [Tooltip("States that will use the Speed Set")]
        public List<StateID> states;
        [Tooltip("Stances that will use the Speed Set")]
        public List<StanceID> stances;
        [Tooltip("Which Speed the Set will start, This value is the Index for the Speed Modifier List, Starting the first index with (1) instead of (0)")]
        public IntReference StartVerticalIndex;
        [Tooltip("Set the Top Index when Increasing the Speed using SpeedUP")]
        public IntReference TopIndex;

        [Tooltip("Index Value of the Sprint Speed")]
        public IntReference m_SprintIndex = new IntReference(10);

        [Tooltip("When the Speed is locked this will be the value s")]
        public IntReference m_LockIndex = new IntReference(1);

        [Tooltip("Lock the Speed Set to Certain Value")]
        public BoolReference m_LockSpeed = new BoolReference(false);

        [Tooltip("Backwards Speed multiplier: When going backwards the speed will be decreased by this value")]
        public FloatReference BackSpeedMult = new FloatReference(0.5f);

        [Tooltip("Lerp used to Activate the FreeMovement")]
        public FloatReference PitchLerpOn = new FloatReference(10f);

        [Tooltip("Lerp used to Deactivate the FreeMovement")]
        public FloatReference PitchLerpOff = new FloatReference(10f);

        [Tooltip("Lerp used to for the Banking on FreeMovement")]
        public FloatReference BankLerp = new FloatReference(10f);
        /// <summary> List of Speed Modifiers for the Speed Set</summary>
        public List<MSpeed> Speeds;

        /// <summary>THis Speed Set has no Stances
        public bool HasStances => stances != null && stances.Count > 0;
      // public bool HasStates => states != null && states.Count > 0;

        /// <summary> Current Active Index of the SpeedSet</summary>
        public int CurrentIndex { get; set; }

        /// <summary>Locked Index of a Speed Set</summary>
        public int LockIndex { get => m_LockIndex.Value; set => m_LockIndex.Value = value; }
        public int SprintIndex { get => m_SprintIndex.Value; set => m_SprintIndex.Value = value; }

        /// <summary>Locked Index of a Speed Set</summary>
        public bool LockSpeed
        {
            get => m_LockSpeed.Value;
            set
            {
                m_LockSpeed.Value = value;
               
                if (value)
                    LockedSpeedModifier = Speeds[Mathf.Clamp(LockIndex-1, 0, Speeds.Count - 1)]; //Extract the Lock Speed
            }
        }

        /// <summary>Store the current Lock Speed Modifier</summary>
        public MSpeed LockedSpeedModifier { get; set; }


        public MSpeedSet()
        {
            name = "Set Name";
            states = new List<StateID>();
            StartVerticalIndex = new IntReference(1);
            TopIndex = new IntReference(2);
            Speeds = new List<MSpeed>(1) { new MSpeed("SpeedName", 1, 4, 4) };
        }

        public MSpeed this[int index]
        {
            get => Speeds[index];
            set => Speeds[index] = value;
        }

        public MSpeed this[string name] => Speeds.Find(x => x.name == name);
       

        public bool HasStance(int stance)
        {
            if (!HasStances) return true;
            else  return stances.Find(s => s.ID == stance);
        }

        public int Compare(object x, object y)
        {
            bool XHas = (x as MSpeedSet).HasStances;
            bool YHas = (y as MSpeedSet).HasStances;

            if (XHas && YHas)
                return 0;
            else if (XHas && !YHas)
                return 1;
            else return -1;
        }

        public int CompareTo(object obj)
        {
            bool XHas = (obj as MSpeedSet).HasStances;
            bool YHas = HasStances;

            if (XHas && YHas)
                return 0;
            else if (XHas && !YHas)
                return 1;
            else return -1;
        }
    }
    [System.Serializable]
    /// <summary>Position, Rotation and Animator Modifiers for the Animals</summary>
    public struct MSpeed  
    {
        /// <summary>Default value for an MSpeed</summary>
        public static readonly MSpeed Default = new MSpeed("Default", 1, 4, 4);

        /// <summary>Name of this Speed</summary>
        public string name;



        ///// <summary>Name of the Speed converted to HashCode, easier to compare</summary>
        //public int nameHash;

        ///// <summary>Name of this Speed</summary>
        //public bool active = false;

        /// <summary>Vertical Mutliplier for the Animator</summary>
        public FloatReference Vertical;

        /// <summary>Add additional speed to the transfrom</summary>
        public FloatReference position;

        /// <summary> Smoothness to change to the Transform speed, higher value more Responsiveness</summary>
        public FloatReference lerpPosition;

        /// <summary> Smoothness to change to the Animator Vertical speed, higher value more Responsiveness</summary>
        public FloatReference lerpPosAnim;


        /// <summary>Add Aditional Rotation to the Speed</summary>
        public FloatReference rotation;
 
        /// <summary> Smoothness to change to the Rotation speed, higher value more Responsiveness </summary>
        public FloatReference lerpRotation;

        /// <summary> Smoothness to change to the Animator Vertical speed, higher value more Responsiveness</summary>
        public FloatReference lerpRotAnim;

        /// <summary>Changes the Animator Speed</summary>
        public FloatReference animator;

        /// <summary> Smoothness to change to the Animator speed, higher value more Responsiveness </summary>
        public FloatReference lerpAnimator;

        /// <summary>Strafe Stored Velocity</summary>
        public FloatReference strafeSpeed;


        /// <summary> Smoothness to change to the Rotation speed, higher value more Responsiveness </summary>
        public FloatReference lerpStrafe;

        public string Name { get => name; set => name = value; }

        public MSpeed(MSpeed newSpeed)
        {
            name = newSpeed.name;

            position = newSpeed.position;
            lerpPosition = newSpeed.lerpPosition;
            lerpPosAnim = newSpeed.lerpPosAnim;

            rotation = newSpeed.rotation;
            lerpRotation = newSpeed.lerpRotation;
            lerpRotAnim = newSpeed.lerpRotAnim;

            animator = newSpeed.animator;
            lerpAnimator = newSpeed.lerpAnimator;
            Vertical = newSpeed.Vertical;
            strafeSpeed = newSpeed.strafeSpeed;
            strafeSpeed = newSpeed.strafeSpeed;
            lerpStrafe = newSpeed.lerpStrafe;

            //nameHash = name.GetHashCode();
        }


        public MSpeed(string name, float lerpPos, float lerpanim)
        {
            this.name = name;
            Vertical = 1;

            position = 0;
            lerpPosition = lerpPos;
            lerpPosAnim = 4;

            rotation = 0;
            strafeSpeed = 0;
            lerpRotation = 4;
            lerpRotAnim = 4;
            lerpStrafe = 4;

            animator = 1;
            lerpAnimator = lerpanim;
           // nameHash = name.GetHashCode();
        }

        public MSpeed(string name, float vertical, float lerpPos, float lerpanim)
        {
            this.name = name;
            Vertical = vertical;

            position = 0;
            lerpPosition = lerpPos;
            lerpPosAnim = 4;

            rotation = 0;
            strafeSpeed = 0;
            lerpRotation = 4;
            lerpRotAnim = 4;
            lerpStrafe = 4;


            animator = 1;
            lerpAnimator = lerpanim;

           // nameHash = name.GetHashCode();
        }


        public MSpeed(string name)
        {
            this.name = name;
            Vertical = 1;
            
            position = 0;
            lerpPosition = 4;
            lerpPosAnim = 4;


            rotation = 0;
            strafeSpeed = 0;

            lerpRotation = 4;
            lerpRotAnim = 4;
            lerpStrafe = 4;


            animator = 1;
            lerpAnimator = 4;

           // nameHash = name.GetHashCode();
        }
    }
}