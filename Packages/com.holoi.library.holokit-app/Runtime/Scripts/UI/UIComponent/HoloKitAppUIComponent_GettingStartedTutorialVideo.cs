using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_GettingStartedTutorialVideo : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        [SerializeField] private RawImage _videoRawImage;

        [SerializeField] private GameObject _playButton;

        private RenderTexture _renderTexture;

        private const int VideoWidth = 1170;

        private const int VideoHeight = 2080;

        private void Start()
        {
            _renderTexture = new RenderTexture(VideoWidth, VideoHeight, 16, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            _renderTexture.Create();
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.isLooping = false;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            for (ushort i = 0; i < _videoPlayer.audioTrackCount; i++)
            {
                _videoPlayer.SetDirectAudioMute(i, true);
            }
            _videoRawImage.texture = _renderTexture;

            StartCoroutine(PauseOnFirstFrame());
        }

        private IEnumerator PauseOnFirstFrame()
        {
            yield return null;
            _videoPlayer.Pause();
        }

        private void Update()
        {
            if (_videoPlayer.isPlaying)
            {
                _playButton.SetActive(false);
            }
            else
            {
                _playButton.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            // We release the allocated render texture on destroy
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }
        }

        public void OnToggleVideoPlay()
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Pause();
            }
            else
            {
                _videoPlayer.Play();
            }
        }
    }
}
