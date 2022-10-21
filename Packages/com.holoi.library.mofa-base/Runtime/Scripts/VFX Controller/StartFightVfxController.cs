using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class StartFightVfxController : MonoBehaviour
    {
        [SerializeField] Animator _3Animator;
        [SerializeField] Animator _2Animator;
        [SerializeField] Animator _1Animator;
        [SerializeField] Animator _fightAnimator;

        void Start()
        {
            StartCoroutine(PlayAnimation(_3Animator, 0f));
            StartCoroutine(PlayAnimation(_2Animator, 1f));
            StartCoroutine(PlayAnimation(_1Animator, 2f));
            StartCoroutine(PlayAnimation(_fightAnimator, 3f));

            Destroy(this.gameObject, 5f);
        }

        IEnumerator PlayAnimation(Animator animator, float time)
        {
            yield return new WaitForSecondsRealtime(time);
            animator.enabled = true;
            animator.speed = 1f;
        }
    }
}
