using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>  Float Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple  </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Variables/Int Range",order = 1000)]
    public class IntRangeVar : FloatVar
    {
        public IntReference minValue;
        public IntReference maxValue;

        /// <summary>Value of the Float Scriptable variable </summary>
        public override float Value
        {
            get =>  UnityEngine.Random.Range(minValue, maxValue+1);
            set {/*Do nothing on Set*/ }
        }
    } 
}