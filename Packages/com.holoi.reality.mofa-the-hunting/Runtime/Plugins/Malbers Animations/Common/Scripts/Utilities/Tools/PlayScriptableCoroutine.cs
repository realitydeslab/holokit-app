using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    public class PlayScriptableCoroutine : MonoBehaviour
    {
        public FloatReference time = new FloatReference(0.5f);
        public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


        public List<PresetItem> presets;

        private void Start()
        {
            PlayAll();
        }

        public virtual void PlayAll()
        {
            foreach (var p in presets)
            {
                p.Preset.Evaluate(this, p.Target, time,curve);
            }            
        }
    }

    [System.Serializable]
    public struct PresetItem
    {
        public ScriptableCoroutine Preset;
        public Transform Target;
    }
}
