using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarUI
{
    public class StarUIManager : MonoBehaviour
    {
        [SerializeField] List<Transform> _baseUIElements;
        [SerializeField] List<Transform> _advanceUIElements;

        [Header("buttons")]
        [SerializeField] Button _more;
        [SerializeField] Button _trigger;

        [Header("scrollbars")]
        [SerializeField] Scrollbar _volumeBar;
        [SerializeField] Scrollbar _recordBar;
        [SerializeField] Scrollbar _exitBar;
        [SerializeField] Scrollbar _boostBar;
        [SerializeField] Scrollbar _spectatorBar;
        [SerializeField] Scrollbar _recalibrateBar;
        [SerializeField] Scrollbar _pauseBar;

        [Header("groups")]
        [SerializeField] Transform _base;
        [SerializeField] Transform _advance;

        // animators
        Animator _volumeAnimator;
        Animator _recordAnimator;
        Animator _exitAnimator;
        Animator _boostAnimator;

        Animator _recalibrateAnimator;
        Animator _spectatorAnimator;
        Animator _pauseAnimator;

        // images
        Image _volumeBG;
        Image _recordBG;
        Image _exitBG_L;
        Image _exitBG_R;
        Image _boostBG;

        Image _recalibrateBG;
        Image _spectatorBG;
        Image _pauseBG;

        // materials
        Material _volumeMaterial;
        Material _recordMaterial;
        Material _exitMaterial_L;
        Material _exitMaterial_R;
        Material _boostMaterial;

        Material _recalibrateMaterial;
        Material _spectatorMaterial;
        Material _pauseMaterial;

        float _value;
        bool _isRecording = false;

        enum UILayerState
        {
            baseState,
            advanceState
        }

        UILayerState _uiLayerState = UILayerState.baseState;

        enum UIState
        {
            idle,
            spectator,
            recalibrate,
            pause
        }

        UIState _uiState = UIState.idle;

        enum RecordState
        {
            notReceivePointerUp,
            receivePointerUp
        }

        RecordState _recordState = RecordState.notReceivePointerUp;

        void Start()
        {
            _volumeAnimator = _volumeBar.GetComponent<Animator>();
            _volumeBG = _volumeBar.GetComponent<ScrollBarHelper>().BackGround;
            _volumeMaterial = _volumeBG.material;

            _recordAnimator = _recordBar.GetComponent<Animator>();
            _recordBG = _recordBar.GetComponent<ScrollBarHelper>().BackGround;
            _recordMaterial = _recordBG.material;

            _boostAnimator = _boostBar.GetComponent<Animator>();
            _boostBG = _boostBar.GetComponent<ScrollBarHelper>().BackGround;
            _boostMaterial = _boostBG.material;

            _exitAnimator = _exitBar.GetComponent<Animator>();
            _exitBG_L = _exitBar.GetComponent<ScrollBarHelper>().BackGround;
            _exitMaterial_L = _exitBG_L.material;
            _exitBG_R = _exitBar.GetComponent<ScrollBarHelper>().ExtraBackGround;
            _exitMaterial_R = _exitBG_R.material;

            _recalibrateAnimator = _recalibrateBar.GetComponent<Animator>();
            _recalibrateBG = _recalibrateBar.GetComponent<ScrollBarHelper>().BackGround;
            _recalibrateMaterial = _recalibrateBG.material;

            _spectatorAnimator = _spectatorBar.GetComponent<Animator>();
            _spectatorBG = _spectatorBar.GetComponent<ScrollBarHelper>().BackGround;
            _spectatorMaterial = _spectatorBG.material;

            _pauseAnimator = _pauseBar.GetComponent<Animator>();
            _pauseBG = _pauseBar.GetComponent<ScrollBarHelper>().BackGround;
            _pauseMaterial = _pauseBG.material;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                SpectatorDone();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                RecalibrateDone();
            }
        }

        public void EnableUILayout()
        {
            switch (_uiLayerState)
            {
                case UILayerState.baseState:
                    for (int i = 0; i < _baseUIElements.Count; i++)
                    {
                        _baseUIElements[i].gameObject.SetActive(true);
                    }
                    break;
                case UILayerState.advanceState:
                    for (int i = 0; i < _advanceUIElements.Count; i++)
                    {
                        _advanceUIElements[i].gameObject.SetActive(true);
                    }
                    break;
            }
        }

        public void ScorllBarOnSelected(Transform scrollbarTransform)
        {
            switch (_uiLayerState)
            {
                case UILayerState.baseState:
                    for (int i = 0; i < _baseUIElements.Count; i++)
                    {
                        if(scrollbarTransform.gameObject == _baseUIElements[i].gameObject)
                        {

                        }
                        else
                        {
                            _baseUIElements[i].gameObject.SetActive(false);
                        }
                    }
                    break;
                case UILayerState.advanceState:
                    for (int i = 0; i < _advanceUIElements.Count; i++)
                    {
                        if (scrollbarTransform.gameObject == _advanceUIElements[i].gameObject)
                        {

                        }
                        else
                        {
                            _advanceUIElements[i].gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }

        public void ScorllBarOnReleased(Transform scrollbarTransform)
        {
            switch (_uiLayerState)
            {
                case UILayerState.baseState:
                    for (int i = 0; i < _baseUIElements.Count; i++)
                    {
                        if (scrollbarTransform.gameObject == _baseUIElements[i].gameObject)
                        {

                        }
                        else
                        {
                            _baseUIElements[i].gameObject.SetActive(true);
                        }
                    }
                    break;
                case UILayerState.advanceState:
                    for (int i = 0; i < _advanceUIElements.Count; i++)
                    {
                        if (scrollbarTransform.gameObject == _advanceUIElements[i].gameObject)
                        {

                        }
                        else
                        {
                            _advanceUIElements[i].gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }

        public void MoreOnClick()
        {
            _uiLayerState = UILayerState.advanceState;
            _base.gameObject.SetActive(false);
            _advance.gameObject.SetActive(true);
        }

        public void BackOnClick()
        {
            _uiLayerState = UILayerState.baseState;
            _base.gameObject.SetActive(true);
            _advance.gameObject.SetActive(false);
        }

        public void TriggerOnClick()
        {
            Debug.Log("Trigger button clicked!");

        }

        public void ScrollBarPointerDown(Scrollbar scrollbar)
        {
            switch (_uiState)
            {
                case UIState.idle:
                    ScorllBarOnSelected(scrollbar.transform);
                    scrollbar.GetComponent<ScrollBarHelper>().BackGround.gameObject.SetActive(true);
                    break;
                case UIState.spectator:
                    break;
                case UIState.recalibrate:
                    break;
                case UIState.pause:
                    break;
            }

        }

        public void ScrollBarPointerUp(Scrollbar scrollbar)
        {
            switch (_uiState)
            {
                case UIState.idle:
                    ScorllBarOnReleased(scrollbar.transform);
                    scrollbar.GetComponent<ScrollBarHelper>().BackGround.gameObject.SetActive(false);
                    scrollbar.value = scrollbar.GetComponent<ScrollBarHelper>().ScrollBarDefaultValue;
                    break;
                case UIState.spectator:
                    break;
                case UIState.recalibrate:
                    break;
                case UIState.pause:
                    break;
            }

        }

        // record pointer
        public void RecordBarPointerDown(Scrollbar scrollbar)
        {
            
            ScorllBarOnSelected(scrollbar.transform);

            scrollbar.GetComponent<ScrollBarHelper>().BackGround.gameObject.SetActive(true);

            if (!_isRecording)
            {

            }
            else
            {
                Debug.Log("OnStopPointerDown");
                _recordAnimator.SetTrigger("OnStopPointerDown");

                switch (_recordState)
                {
                    case RecordState.notReceivePointerUp:
                        _recordState = RecordState.receivePointerUp;
                        break;
                    case RecordState.receivePointerUp:
                        break;
                }

            }
        }

        public void RecordBarPointerUp(Scrollbar scrollbar)
        {
            ScorllBarOnReleased(scrollbar.transform);

            scrollbar.GetComponent<ScrollBarHelper>().BackGround.gameObject.SetActive(false);

            scrollbar.value = scrollbar.GetComponent<ScrollBarHelper>().ScrollBarDefaultValue;

            if (!_isRecording)
            {

            }
            else
            {
                switch (_recordState)
                {
                    case RecordState.notReceivePointerUp:
                        _recordState = RecordState.receivePointerUp;
                        break;
                    case RecordState.receivePointerUp:
                        _recordAnimator.SetTrigger("OnStopPointerUp");
                        break;
                }
            }
        }

        public void ExitBarPointerDown(Scrollbar scrollbar)
        {
            ScorllBarOnSelected(scrollbar.transform);
        }
        public void ExitBarPointerUp(Scrollbar scrollbar)
        {
            ScorllBarOnReleased(scrollbar.transform);

            scrollbar.value = scrollbar.GetComponent<ScrollBarHelper>().ScrollBarDefaultValue;
        }

        // volume bar
        public void VolumeBarOnValueChange()
        {
            _value = _volumeBar.value;
            _volumeMaterial.SetFloat("_Offset", _value);

            // to do: set volume value with _value
            Debug.Log("set volume value to: " + _value);

        }
        // boost bar
        public void BoostBarOnValueChanged()
        {
            _value = _boostBar.value;
            _boostMaterial.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                BoostBarOnLoaded();
            }
        }
        public void BoostBarOnLoaded()
        {
            _boostBG.gameObject.SetActive(false);
            _boostAnimator.SetTrigger("OnLoaded");
        }

        // exit bar
        public void ExitBarOnValueChanged()
        {
            _value = _exitBar.value;
            _exitMaterial_L.SetFloat("_Offset", _value);
            _exitMaterial_R.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                ExitBarOnLoaded();
            }
        }
        public void ExitBarOnLoaded()
        {
            //_exitBG_L.gameObject.SetActive(false);
            //_exitBG_R.gameObject.SetActive(false);
            // do exit the star:
            Debug.Log("Now we exit from star mode");
        }
        // record bar
        public void RecordBarOnValueChanged()
        {
            _value = _recordBar.value;
            _recordMaterial.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                RecordBarOnLoaded();
            }
        }
        public void RecordBarOnLoaded()
        {
            _recordBG.gameObject.SetActive(false);
            if (!_isRecording)
            {
                Debug.Log("begin record");
                _isRecording = true;
                _recordAnimator.SetTrigger("OnLoaded");
            }
            else
            {
                _isRecording = false;
                _recordAnimator.SetTrigger("OnStopRecord");

                _recordState = RecordState.notReceivePointerUp;

            }
        }

        // pause bar
        public void PauseBarOnValueChanged()
        {
            _value = _pauseBar.value;
            _pauseMaterial.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                PauseBarOnLoaded();
            }
        }
        public void PauseBarOnLoaded()
        {
            _pauseBG.gameObject.SetActive(false);
            _pauseAnimator.SetTrigger("OnLoaded");
        }

        // spectator bar
        public void SpectatorBarOnValueChanged()
        {
            _value = _spectatorBar.value;
            _spectatorMaterial.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                SpectatorBarOnLoaded();
            }
        }
        public void SpectatorBarOnLoaded()
        {
            _uiState = UIState.spectator;
            _spectatorBG.gameObject.SetActive(false);
            _spectatorAnimator.SetTrigger("OnLoaded");
        }
        public void SpectatorDone()
        {
            //EnableUILayout();
            _uiState = UIState.idle;
            _spectatorAnimator.SetTrigger("OnDone");
            _spectatorBar.value = 0;
        }
        // recalibrate bar
        public void RecalibrateBarOnValueChanged()
        {
            _value = _recalibrateBar.value;
            _recalibrateMaterial.SetFloat("_Offset", _value);

            if (_value == 1)
            {
                RecalibrateBarOnLoaded();
            }
        }
        public void RecalibrateBarOnLoaded()
        {
            _uiState = UIState.recalibrate;
            _recalibrateBG.gameObject.SetActive(false);
            _recalibrateAnimator.SetTrigger("OnLoaded");
        }
        public void RecalibrateDone()
        {
            //EnableUILayout();
            _uiState = UIState.idle;
            _recalibrateAnimator.SetTrigger("OnDone");
            _recalibrateBar.value = 0;

        }
    }
}