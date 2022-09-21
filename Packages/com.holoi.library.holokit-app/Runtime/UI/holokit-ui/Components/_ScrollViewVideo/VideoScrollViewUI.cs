using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.IO;

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

        float _scrollValue;
        int _activeIndex;

        enum State
        {
            visible,
            inVisible 
        }

        State _state = State.inVisible;

        private void OnEnable()
        {
            transform.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.AddListener(SetVideoState);
        }

        private void OnDisable()
        {
            transform.GetComponent<HorizontalScrollSnap>().OnNextScreenEvent.RemoveListener(SetVideoState);
        }

        private void Awake()
        {
            DeletePreviousElement(_content);

            UpdateVideos();
        }


        private void Update()
        {
            SetContainerState();
            SetVideoState();
        }

        void UpdateVideos()
        {
            _videoObjectList.Clear();
            for (int i = 0; i < _videoCount; i++)
            {
                var go = Instantiate(_videoObject, _content);
                //go.GetComponent<VideoPlayer>().url = _videoUrl[i];
                _videoObjectList.Add(go);
            }

            _scrollBar.GetComponent<ScrollBarSlidingAreaStyle>().Init(_videoCount);
        }

        private void SetContainerState()
        {
            if (_parentScrollBar.value < 0.25f)
            {
                _state = State.visible;
            }else
            {
                _state = State.inVisible;
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

        public void SetVideoState()
        {
            switch (_state)
            {
                case State.visible:
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
                case State.inVisible:
                    for (int i = 0; i < _videoCount; i++)
                    {
                        _videoObjectList[i].GetComponent<VideoPlayer>().Stop();
                    }
                    break;
            }

        }
    }
}
