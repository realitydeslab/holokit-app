using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.CoreHaptics;


namespace Holoi.Reality.QuantumRealm
{
    public class BuddhasDisplay : MonoBehaviour
    {
        // corehaptics:
        private CHHapticEngine _engine;

        private CHHapticPatternPlayer _interactionHapticPlayer;

        [SerializeField] private TextAsset _interactionAHAP;

        // others:
        private QuantumBuddhasSceneManager _manager;


        private void Awake()
        {
            OnBuddhasDisplayBitrh();
        }

        void Start()
        {
            _manager = FindObjectOfType<QuantumBuddhasSceneManager>();
            if (_manager is null)
            {
                Debug.LogError("Could not find the GameManager.");
            }
            else
            {
                _engine = _manager.HapticEngine;

                SetupHapticPlayers();

                //if (HasTexture)
                //{
                //    _textureHapticPlayer.Start();
                //}
            }
        }

        public void OnBuddhasDisplayBitrh()
        {
            var manager = FindObjectOfType<QuantumBuddhasSceneManager>();

            manager.vfxs.Clear();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var vfx = transform.GetChild(i).GetComponent<BuddhasController>().vfx;
                manager.vfxs.Add(vfx);
            }
        }

        private void SetupHapticPlayers()
        {
            _interactionHapticPlayer = _engine.MakePlayer(new CHHapticPattern(_interactionAHAP));
        }

        public void PlayHaptics()
        {
            _interactionHapticPlayer.Start();
        }
    }
}
