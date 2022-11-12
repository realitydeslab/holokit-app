using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_VideoList : MonoBehaviour
    {
        [SerializeField] private RectTransform _videoListRoot;

        [SerializeField] private RawImage _videoRawImagePrefab;

        private readonly List<GameObject> _videoPlayers = new();

        private readonly List<RenderTexture> _renderTextures = new();

        private const int VideoWidth = 1170;

        private const int VideoHeight = 1872;

        private void OnEnable()
        {
            if (HoloKitApp.Instance.CurrentReality.PreviewVideos.Count == 0)
            {
                Destroy(gameObject);
                return;
            }

            foreach (var videoClip in HoloKitApp.Instance.CurrentReality.PreviewVideos)
            {
                var go = new GameObject();
                go.transform.SetParent(transform);
                var videoPlayer = go.AddComponent<VideoPlayer>();
                videoPlayer.clip = videoClip;
                var renderTexture = new RenderTexture(VideoWidth, VideoHeight, 16, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
                renderTexture.Create();
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                videoPlayer.targetTexture = renderTexture;
                videoPlayer.isLooping = true;
                videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
                for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
                {
                    videoPlayer.SetDirectAudioMute(i, true);
                }

                var videoRawImage = Instantiate(_videoRawImagePrefab);
                videoRawImage.transform.SetParent(_videoListRoot);
                videoRawImage.transform.localScale = Vector3.one;
                videoRawImage.texture = renderTexture;

                _videoPlayers.Add(go);
                _renderTextures.Add(renderTexture);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _videoListRoot.childCount; i++)
            {
                Destroy(_videoListRoot.GetChild(i).gameObject);
                Destroy(_videoPlayers[i]);
                _renderTextures[i].Release();
            }
        }
    }
}
