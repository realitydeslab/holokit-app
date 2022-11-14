using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Based on 3DKit Controller from Unity
    /// </summary>
    [AddComponentMenu("Malbers/Utilities/Transform/Simple Scaler")]
    [SelectionBase]
    public class MSimpleScaler : MSimpleTransformer
    {
        public Vector3Reference startScale = new Vector3Reference(Vector3.one);
        public Vector3Reference endScale = new Vector3Reference(new Vector3(1.5f,1.5f,1.5f));

        public override void Evaluate(float position)
        {
            Object.localScale = Vector3.Lerp(startScale, endScale, m_Curve.Evaluate(position));
        }


        protected override void Reset()
        {
            base.Reset();
            if (startScale.UseConstant)
                startScale.Value = Object.localScale;
        }
    }
}