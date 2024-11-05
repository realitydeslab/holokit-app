using UnityEngine;
using System.Collections;
using MalbersAnimations.Scriptables;
using UnityEngine.Serialization;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/UI/Fade In-Out Graphic")]

    public class FadeInOutGraphic : MonoBehaviour
    {
        public CanvasGroup group;
        public FloatReference defaultAlpha = new FloatReference(0f);

        [FormerlySerializedAs("time")]
        public FloatReference timeEnter = new FloatReference(0.15f);
        public FloatReference timeExit = new FloatReference(0.15f);

        public FloatReference delayIn = new FloatReference(0);
        public FloatReference delayOut = new FloatReference(0);
        public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


        private void Start()
        {
            group.alpha = defaultAlpha;
        }

        private void Reset()
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
            group.interactable = false;
        }

        IEnumerator I_FadeIn, I_FadeOut;

        public virtual void Fade_In_Out(bool fade)
        {
            if (fade) Fade_In();

            else Fade_Out();
        }

        public virtual void Fade_In()
        {
           if (I_FadeIn != null || !isActiveAndEnabled) return; //Do nothing if its already fadein In
            StopAllCoroutines();
            I_FadeOut = null;
            StartCoroutine(I_FadeIn = C_Fade(1,timeEnter));
        }

        public virtual void Fade_Out()
        {
           if (I_FadeOut != null || !isActiveAndEnabled) return; //Do nothing if its already fadein out
            if (!isActiveAndEnabled) return;
            StopAllCoroutines();
            I_FadeIn = null;
            StartCoroutine(I_FadeOut = C_Fade(0,timeExit));
        }

        private IEnumerator C_Fade(float value, float time)
        {
            if (delayIn > 0 && value == 1) yield return new WaitForSeconds(delayIn);
            if (delayOut > 0 && value == 0) yield return new WaitForSeconds(delayOut);

            float elapsedTime = 0;
            float startAlpha = group.alpha;


            while (group.alpha != value || ((time > 0) && (elapsedTime <= time)))
            {
                float result = fadeCurve != null ? fadeCurve.Evaluate(elapsedTime / time) : elapsedTime / time;               //Evaluation of the Pos curve

                group.alpha = Mathf.Lerp(startAlpha, value, result);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            group.alpha = value;
            yield return null;

            if (value == 0) I_FadeOut = null;
            else I_FadeIn = null;
        }
    }
}
