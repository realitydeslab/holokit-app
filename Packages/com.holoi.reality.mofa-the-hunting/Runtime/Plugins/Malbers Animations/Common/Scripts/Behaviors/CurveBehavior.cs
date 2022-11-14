using UnityEngine;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using System.Linq;

namespace MalbersAnimations
{
    /// <summary>Sends the Value of a curve to a monobehaviour attached to the Animator</summary>
    public class CurveBehavior : StateMachineBehaviour
    {
        [Tooltip("ID of the Curve")]
        public int ID;
        [Tooltip("Curve to send to the Animator")]
        public AnimationCurve Value = new AnimationCurve( new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });

        private List<IAnimatorCurve> curves;

        private bool hasICurves;

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (!hasICurves)
            {
                curves = animator.GetComponentsInChildren<IAnimatorCurve>().ToList();
                if (curves != null) hasICurves = true;
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (hasICurves)
            {
                for (int i = 0; i < curves.Count; i++)
                {
                    curves[i].AnimatorCurve(ID, Value.Evaluate(stateInfo.normalizedTime));
                }
            }
        }
    }

    /// <summary>  Use this interface on your Monobehaviours to  Read the Curves from the Animator Behaviour</summary>
    public interface IAnimatorCurve
    {
        void AnimatorCurve(int ID, float value);
    }
}