using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// 1. when user slider screen to video container, the first video plays
/// 2. video play and stop base on user's interaction, always play the active one and others stop
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class VideoScrollViewUI : MonoBehaviour
    {
        [Header("Data")]
        List<string> _videoUrl;
        [Header("Prefabs")]
        [SerializeField] GameObject _videoObject;
        [Header("UI Elements")]
        List<GameObject> _videoObjectList = new List<GameObject>();
        [SerializeField] Transform _content;
        [SerializeField] Scrollbar _scrollBar;
        [SerializeField] Scrollbar _parentScrollBar;

        [Header("Debug")]
        [SerializeField] int _videoCount;
        int _readyCount;

        float _scrollValue;
        int _activeIndex;

        enum VisibleState
        {
            visible,
            inVisible 
        }

        enum VideoState
        {
            notReady,
            ready,
        }

        VisibleState _visibleState = VisibleState.inVisible;
        VideoState _videoState = VideoState.notReady;

        private void OnEnable()
        {
            transform.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.AddListener(UpdateVideoState);
        }

        private void OnDisable()
        {
            transform.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.RemoveListener(UpdateVideoState);
        }

        private void Awake()
        {
            DeletePreviousElement(_content);

            UpdateVideos();
        }

        private void Start()
        {

        }

        private void Update()
        {
            switch (_videoState)
            {
                case VideoState.notReady:
                    foreach (var video in _videoObjectList)
                    {
                        var vp = video.GetComponent<VideoPlayer>();
                        if (vp.frame == 1)
                        {
                            vp.Stop();
                            _readyCount ++;
                        }
                        else
                        {

                        }
                    }

                    if(_readyCount == _videoCount)
                    {
                        _videoState = VideoState.ready;
                    }

                    break;
                case VideoState.ready:

                    UpdateContainerState();
                    UpdateVideoState();

                    break;
            }
        }

        void UpdateVideos()
        {
            _videoObjectList.Clear();
            for (int i = 0; i < _videoCount; i++)
            {
                var go = Instantiate(_videoObject, _content);
                //go.GetComponent<VideoPlayer>().url = _videoUrl[i];
                _videoObjectList.Add(go);
                go.GetComponent<VideoPlayer>().Play();
            }

            _scrollBar.GetComponent<ScrollBarSlidingAreaStyle>().Init(_videoCount);
        }

        private void UpdateContainerState()
        {
            if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
            {
                if (_parentScrollBar.value < 0.25f)
                {
                    _visibleState = VisibleState.visible;
                }
                else
                {
                    _visibleState = VisibleState.inVisible;
                }
            }
            else
            {
                _visibleState = VisibleState.inVisible;
            }

        }

        void DeletePreviousElement(Transform content)
        {
            var tempList = new List<Transform>();
            for (int i = 0; i < content.childCount; i++)
            {
                tempList.Add(content.GetChild(i));
            }
            foreach (var child in tempList)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void UpdateVideoState()
        {
            switch (_visibleState)
            {
                case VisibleState.visible:
                    _scrollValue = _scrollBar.value;
                    _scrollValue = Mathf.Clamp01(_scrollValue);
                    _activeIndex = Mathf.RoundToInt(_scrollValue * (_videoCount - 1));

                    for (int i = 0; i < _videoCount; i++)
                    {
                        if (_activeIndex == i)
                        {
                            if(!_videoObjectList[i].GetComponent<VideoPlayer>().isPlaying) _videoObjectList[i].GetComponent<VideoPlayer>().Play();
                        }
                        else
                        {
                            if (_videoObjectList[i].GetComponent<VideoPlayer>().isPlaying) _videoObjectList[i].GetComponent<VideoPlayer>().Stop();
                        }
                    }
                    break;
                case VisibleState.inVisible:
                    for (int i = 0; i < _videoCount; i++)
                    {
                        _videoObjectList[i].GetComponent<VideoPlayer>().Stop();
                    }
                    break;
            }

        }
    }
}
