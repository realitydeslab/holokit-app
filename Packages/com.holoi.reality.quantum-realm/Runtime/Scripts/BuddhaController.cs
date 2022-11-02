using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitApp;
using Apple.CoreHaptics;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhaController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _vfx;

        [SerializeField] private Animator _vfxAnimator;

        [SerializeField] private Animator _seatAnimator;

        [SerializeField] private TextAsset _textureAHAP;

        [SerializeField] private AudioClip _startSound;

        private CHHapticAdvancedPatternPlayer _textureHapticsPlayer;

        private HoverableObject _hoverableObject;

        private bool _vibrationStarted;

        private const float FadeAwayDelay = 1f;

        private void OnEnable()
        {
            if (!HoloKitApp.Instance.GlobalSettings.PhaseEnabled)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = _startSound;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        private void Start()
        {
            _hoverableObject = GetComponent<HoverableObject>();
            _hoverableObject.OnStartedLoading += OnStartedLoading;
            _hoverableObject.OnStoppedLoading += OnStoppedLoading;

            if (((QuantumRealmRealityManager)HoloKitApp.Instance.RealityManager).CoreHapticsManager.IsValid)
            {
                var hapticsEngine = ((QuantumRealmRealityManager)HoloKitApp.Instance.RealityManager).CoreHapticsManager;
                _textureHapticsPlayer = hapticsEngine.Engine.MakeAdvancedPlayer(new CHHapticPattern(_textureAHAP));
                _textureHapticsPlayer.LoopEnabled = true;
                _textureHapticsPlayer.LoopEnd = 0f;
            }
        }

        private void OnDisable()
        {
            StopHapticsPlayer();
        }

        private void OnDestroy()
        {
            if (_hoverableObject != null)
            {
                _hoverableObject.OnStartedLoading -= OnStartedLoading;
                _hoverableObject.OnStoppedLoading -= OnStoppedLoading;
            }
        }

        private void Update()
        {
            _vfx.SetVector3("Hand Position",
                ((QuantumRealmRealityManager)HoloKitApp.Instance.RealityManager).HandTransform.transform.position);
            _vfx.SetFloat("Process", _hoverableObject.CurrentLoadPercentage);
        }

        public void OnActivated()
        {
            gameObject.transform.parent.gameObject.SetActive(true);
            _vfxAnimator.Rebind();
            _vfxAnimator.Update(0f);
            _seatAnimator.Rebind();
            _seatAnimator.Update(0f);
        }

       public void OnDeactivated()
        {
            _vfxAnimator.SetTrigger("Fade Out");
            _seatAnimator.SetTrigger("Fade Out");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(FadeAwayDelay, () =>
            {
                gameObject.transform.parent.gameObject.SetActive(false);
            }));
        }

        private void OnStartedLoading()
        {
            if (_vibrationStarted)
            {
                ResumeHapticsPlayer();
            }
            else
            {
                StartHapticsPlayer();
            }
        }

        private void OnStoppedLoading()
        {
            PauseHapticsPlayer();
        }

        public void StartHapticsPlayer()
        {
            if (_textureHapticsPlayer != null)
            {
                _textureHapticsPlayer.Start();
                _vibrationStarted = true;
            }  
        }

        public void StopHapticsPlayer()
        {
            if (_textureHapticsPlayer != null)
            {
                _textureHapticsPlayer.Stop();
                _vibrationStarted = false;
            }
        }

        public void PauseHapticsPlayer()
        {
            if (_textureHapticsPlayer != null)
            {
                _textureHapticsPlayer.Pause();
            }
        }

        public void ResumeHapticsPlayer()
        {
            if (_textureHapticsPlayer != null)
            {
                _textureHapticsPlayer.Resume();
            }
        }
    }
}