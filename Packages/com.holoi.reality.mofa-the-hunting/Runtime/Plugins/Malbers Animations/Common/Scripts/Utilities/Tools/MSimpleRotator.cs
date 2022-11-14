using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Based on 3DKit Controller from Unity
    /// </summary>
    [AddComponentMenu("Malbers/Utilities/Transform/Simple Rotator")]
    [SelectionBase]
    public class MSimpleRotator : MSimpleTransformer
    {
        public Vector3Reference axis = new Vector3Reference(Vector3.up);
        public FloatReference startAngle;
        public FloatReference endAngle = new FloatReference(90f);


        public override void Evaluate(float position)
        {
            var curvePosition = m_Curve.Evaluate(position);
            var q = Quaternion.AngleAxis(Mathf.LerpUnclamped(startAngle, endAngle, curvePosition), axis);
            Object.localRotation = q;
        } 
    }
}