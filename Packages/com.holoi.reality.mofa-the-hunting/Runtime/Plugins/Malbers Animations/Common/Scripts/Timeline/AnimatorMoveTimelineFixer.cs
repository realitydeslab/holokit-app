using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    /// <summary> This is used for all the components that use OnAnimator Move... it breaks the Timeline edition  </summary>
    [AddComponentMenu("Malbers/Timeline/Animator Move Timeline Fixer")]

    [ExecuteInEditMode]
    public class AnimatorMoveTimelineFixer : MonoBehaviour
    {
        public Animator anim;
        void Start()
        {
            if (Application.isEditor && Application.isPlaying) Destroy(this);
            anim = GetComponent<Animator>();
        }

        private void OnAnimatorMove() 
        {
            if (anim != null) 
                anim.ApplyBuiltinRootMotion(); 
        }

        private void Reset()
        { anim = GetComponent<Animator>(); }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AnimatorMoveTimelineFixer))]
    public class AnimatorMoveTimelineFixerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MalbersEditor.DrawDescription("This script fixes a bug with the Timeline when its play in the Editor, with Scripts that use the  OnAnimatorMove() callback, like AC ");
        }
    }
#endif
}
 