using UnityEngine;

namespace MalbersAnimations
{
    public class SoundBehavior : StateMachineBehaviour
    {
        [Tooltip("Game Object to Store the Audio Source Component. This allows Animation States to share the same AudioSource")]
        public string m_source = "Animator Sounds";

        public AudioClip[] sounds;

        public bool playOnEnter = true;
        public bool Loop = false;
        public bool stopOnExit;
        [Hide("playOnEnter",  true)]
        public bool playOnTime;
        [Hide("playOnEnter",  true)]
        [Range(0, 1)]
        public float NormalizedTime = 0.5f;
        [Space]
        [MinMaxRange(-3, 3)]
        public RangedFloat pitch = new RangedFloat(1, 1);
        [MinMaxRange(0, 1)]
        public RangedFloat volume = new RangedFloat(1, 1);

        private AudioSource _audio;
        private Transform audioTransform;

        private void CheckAudioSource(Animator animator)
        {
            if (audioTransform == null)
            {
                var goName = m_source;

                if (string.IsNullOrEmpty(goName)) goName = "Animator Sounds";

                audioTransform = animator.transform.FindGrandChild(goName);

                if (!audioTransform)
                {
                    var go = new GameObject() { name = goName };
                    audioTransform = go.transform;
                    audioTransform.parent = animator.transform;
                }

                _audio = audioTransform.GetComponent<AudioSource>();

                if (!_audio)
                {
                    _audio = audioTransform.gameObject.AddComponent<AudioSource>();
                    _audio.spatialBlend = 1; //Make it 3D
                    _audio.loop = Loop;
                }
            }
        }



        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CheckAudioSource(animator);

            if (playOnEnter)
            {
                PlaySound();
                playOnTime = false; //IMPORTANT
            }
            else playOnTime = true;
        }

     

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playOnTime)
            {
                if (stateInfo.normalizedTime > NormalizedTime && !_audio.isPlaying && !animator.IsInTransition(layerIndex))
                {
                    PlaySound();
                    playOnTime = false;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stopOnExit && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash != stateInfo.fullPathHash) //dont stop the current animation if is this same animation
                _audio?.Stop();
        }

        public virtual void PlaySound()
        {
            if (_audio)
            {
                if (sounds.Length > 0 && _audio.enabled)
                {
                    _audio.Stop();
                    _audio.clip = sounds[Random.Range(0, sounds.Length)];
                   
                    if (_audio.clip != null)
                    {
                        _audio.pitch = pitch.RandomValue;
                        _audio.volume = volume.RandomValue;
                        _audio.Play();
                    } 
                }
            }
        }
    }
}