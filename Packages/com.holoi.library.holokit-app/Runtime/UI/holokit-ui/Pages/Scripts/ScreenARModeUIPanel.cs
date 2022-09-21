using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class ScreenARModeUIPanel : MonoBehaviour
    {
        public enum State
        {
            idle = 0,
            // host:
            comfirmingOpenSpectator = 1,
            showingQRcode = 2,
            waittingScanned = 3,

            // client: 
            waittingPlayerEnter = 6,
            scanningQRcode = 7,
            checkingMark = 4,
            @checked = 5,

            // others"
            recording = 8
            // notRecording = idle
        }

        public enum ShareState
        {
            idle = 0,
            share = 1
        }

        //[HideInInspector]
        public State state;
        public ShareState shareState = ShareState.idle;

        [Header("UI Elements")]
        public Transform scanContainer;
        public Transform spectator;
        public Transform stAR;
        public Transform exitButton;
        public Transform recorder;

        private void Awake()
        {
            SetState(State.idle);
        }

        public void SetState(State currentState)
        {
            state = currentState;
            switch (currentState)
            {
                case State.idle:
                    // rest ui state to default state:
                    scanContainer.gameObject.SetActive(false);
                    EnableARTool(true);
                    recorder.GetComponent<RecordButton>().StartRecording(false);
                    break;
                case State.comfirmingOpenSpectator:
                    EnableARTool(false);
                    break;
                case State.showingQRcode:
                    EnableARTool(false);
                    break;
                case State.waittingScanned:
                    EnableARTool(false);
                    break;

                case State.waittingPlayerEnter:
                    EnableARTool(false);
                    break;
                case State.scanningQRcode:
                    scanContainer.gameObject.SetActive(true);
                    EnableARTool(false);

                    scanContainer.Find("Scanning").gameObject.SetActive(true);
                    scanContainer.Find("Scanned").gameObject.SetActive(false);
                    break;
                case State.checkingMark:
                    scanContainer.gameObject.SetActive(false);
                    EnableARTool(false);

                    break;
                case State.@checked:
                    EnableARTool(true);
                    //scanContainer.Find("Scanning").gameObject.SetActive(false);
                    //scanContainer.Find("Scanned").gameObject.SetActive(true);
                    StartCoroutine(ScannedCoroutine());
                    break;
                case State.recording:
                    scanContainer.gameObject.SetActive(false);

                    spectator.gameObject.SetActive(true);
                    stAR.gameObject.SetActive(false);
                    exitButton.gameObject.SetActive(true);
                    recorder.gameObject.SetActive(true);

                    recorder.GetComponent<RecordButton>().StartRecording(true);
                    break;
            }
        }

        IEnumerator ScannedCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            SetState(State.idle);
        }

        void EnableARTool(bool state)
        {
            spectator.gameObject.SetActive(state);
            stAR.gameObject.SetActive(state);
            exitButton.gameObject.SetActive(state);
            recorder.gameObject.SetActive(state);
        }
    }
}