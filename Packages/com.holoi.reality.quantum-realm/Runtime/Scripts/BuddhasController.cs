using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;
using Apple.CoreHaptics;
using Holoi.Library.HoloKitApp;
using System.Collections;

namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasController : MonoBehaviour
    {
        public VisualEffect vfx;
        public Animator vfxAnimator;
        public Animator seatAnimator;

        [SerializeField] HoverableObject _hoverableObject;

        HandObject _ho;

        // haptics
        private CHHapticAdvancedPatternPlayer _textureHapticPlayer;
        [SerializeField] private TextAsset _textureAHAP;
        bool _isPlaying = false;

        public bool IsActive = false;

        private void OnEnable()
        {
            IsActive = true;
        }

        private void OnDisable()
        {
            IsActive = false;
            StopHapticsPlayer();
        }

        void Start()
        {
            _ho = HandObject.Instance;

            if (HoloKitApp.Instance.IsHost)
            {
                _hoverableObject.OnLoadedEvents.AddListener(FindObjectOfType<QuantumBuddhasSceneManager>().SwitchToNextVFXNetWork);
                _hoverableObject.OnLoadedEvents.AddListener(StopHapticsPlayer);
            }

            var engine = FindObjectOfType<HapticsManager>().HapticsEngine;

            if (engine != null)
            {
                SetupHapticAdvancedPlayers(_textureAHAP);
            }
        }

        void Update()
        {
            if (_ho.IsValid)
            {
                vfx.SetVector3("Hand Position", _ho.transform.position);
            }
            else
            {
                vfx.SetVector3("Hand Position", new Vector3(0,-99,0));
            }

            if (HoloKitApp.Instance.IsHost)
            {
                if (IsActive)
                {
                    if (_hoverableObject.Interacted)
                    {
                        if (_isPlaying == false)
                        {
                            StartHapticsPlayer();
                            _isPlaying = true;
                        }
                    }
                    else
                    {
                        if (_isPlaying)
                        {
                            StopHapticsPlayer();
                            _isPlaying = false;
                        }
                    }
                }
                else
                {
                    if (_isPlaying)
                    {
                        StopHapticsPlayer();
                        _isPlaying = false;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            StopHapticsPlayer();
        }


        public void InitBuddha()
        {
            IsActive = true;

            vfxAnimator.Rebind();
            vfxAnimator.Update(0f);

            seatAnimator.Rebind();
            seatAnimator.Update(0f);
        }

       public void ExitBuddha()
        {
            IsActive = false;

            vfxAnimator.SetTrigger("Fade Out");
            seatAnimator.SetTrigger("Fade Out");
        }

        public void StartHapticsPlayer()
        {
            Debug.Log("Start Vibe");
            if (_textureHapticPlayer != null)
                _textureHapticPlayer.Start();
        }

        public void StopHapticsPlayer()
        {
            Debug.Log("Stop Vibe");
            if (_textureHapticPlayer != null)
            {
                _textureHapticPlayer.Stop();
            }
        }

        private void SetupHapticAdvancedPlayers(TextAsset textureAHAP)
        {
            var engine = FindObjectOfType<HapticsManager>().HapticsEngine;
            _textureHapticPlayer = engine.MakeAdvancedPlayer(new CHHapticPattern(textureAHAP));
            _textureHapticPlayer.LoopEnabled = true;
            _textureHapticPlayer.LoopEnd = 0f;
        }


        IEnumerator DisableGameObjectAfterTimes(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }
    }
}