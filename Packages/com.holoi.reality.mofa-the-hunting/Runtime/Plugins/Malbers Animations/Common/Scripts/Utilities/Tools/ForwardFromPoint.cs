using UnityEngine;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Utilities/Tools/Forward from Point")]

    public class ForwardFromPoint : MonoBehaviour
    {
        [Header("Use Point to Aim at it using Transform.forward")]
        public TransformReference Point;

        private void OnEnable()
        {
            if (Point.Value == null) enabled = false; //disable if it does not have a Vector3Var
        }
        void Update()
        {
           // transform.rotation = Quaternion.LookRotation((Point.position - transform.position).normalized, Vector3.up);
           transform.forward = (Point.position - transform.position).normalized;
        }
    }
}
