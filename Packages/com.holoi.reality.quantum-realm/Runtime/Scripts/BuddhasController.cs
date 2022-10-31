using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.ARUX;
using Apple.CoreHaptics;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasController : MonoBehaviour
    {
        public VisualEffect vfx;
        public Animator vfxAnimator;
        public Animator setaAnimator;

        [SerializeField] HoverableObject _hoverableObject;

        HandObject _ho;

        // haptics
        private CHHapticAdvancedPatternPlayer _textureHapticPlayer;
        [SerializeField] private TextAsset _textureAHAP;
        bool _isPlaying = false;



        void Start()
        {
            _ho = HandObject.Instance;

            _hoverableObject.OnLoadedEvents.AddListener(FindObjectOfType<QuantumBuddhasSceneManager>().SwitchToNextVFXNetWork);
            _hoverableObject.OnLoadedEvents.AddListener(StopHapticsPlayer);

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
                if (vfx.isActiveAndEnabled)
                {
                    if (_hoverableObject)
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
                            if (_isPlaying == true)
                            {
                                StopHapticsPlayer();
                                _isPlaying = false;
                            }
                        }
                    }
                    else
                    {
                        if (_isPlaying == true)
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


        public void StartHapticsPlayer()
        {
            if (_textureHapticPlayer != null)
                _textureHapticPlayer.Start();
        }

        public void StopHapticsPlayer()
        {
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

        private void OnDisable()
        {
            StopHapticsPlayer();
        }

        private void OnDestroy()
        {
            StopHapticsPlayer();
        }
    }
}