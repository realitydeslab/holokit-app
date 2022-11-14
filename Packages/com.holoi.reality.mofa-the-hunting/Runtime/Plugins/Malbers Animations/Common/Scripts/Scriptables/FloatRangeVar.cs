using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Float Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple  </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Variables/Float Range",order = 1000)]
    public class FloatRangeVar : FloatVar
    {
        public FloatReference minValue;
        public FloatReference maxValue;

        /// <summary>Value of the Float Scriptable variable </summary>
        public override float Value
        {
            get =>  UnityEngine.Random.Range(minValue, maxValue);
            set {/*Do nothing on Set*/ }
        }
    } 
}