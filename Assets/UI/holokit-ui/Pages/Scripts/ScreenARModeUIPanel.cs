using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARModeUIPanel : MonoBehaviour
    {
        public enum State
        {
            idle,
            spectatorConfrim,
            waitPlayerEnter,
            showQRcode,
            scanning,
            checkMark,
            scanned,
            recording
        }

        public State state;

        [Header("UI Elements")]
        public Transform scanContainer;
        public Transform spectator;
        public Transform stAR;
        public Transform exitButton;
        public Transform recorder;

        public void SetState(State currentState)
        {
            state = currentState;
            switch (currentState)
            {
                case State.idle:
                    scanContainer.gameObject.SetActive(false);
                    SetToolState(true);

                    recorder.GetComponent<RecordButton>().SetState(false);
                    break;
                case State.spectatorConfrim:
                    SetToolState(false);
                    break;
                case State.waitPlayerEnter:
                    SetToolState(false);
                    break;
                case State.showQRcode:
                    SetToolState(false);
                    break;
                case State.scanning:
                    scanContainer.gameObject.SetActive(true);
                    SetToolState(false);

                    scanContainer.Find("Scanning").gameObject.SetActive(true);
                    scanContainer.Find("Scanned").gameObject.SetActive(false);
                    break;
                case State.checkMark:
                    scanContainer.gameObject.SetActive(false);
                    SetToolState(false);

                    break;
                case State.scanned:
                    scanContainer.gameObject.SetActive(true);
                    spectator.gameObject.SetActive(true);
                    stAR.gameObject.SetActive(true);
                    recorder.gameObject.SetActive(true);

                    scanContainer.Find("Scanning").gameObject.SetActive(false);
                    scanContainer.Find("Scanned").gameObject.SetActive(true);
                    StartCoroutine(ScannedCoroutine());
                    break;
                case State.recording:
                    scanContainer.gameObject.SetActive(false);
                    spectator.gameObject.SetActive(true);
                    stAR.gameObject.SetActive(false);
                    recorder.gameObject.SetActive(true);

                    recorder.GetComponent<RecordButton>().SetState(true);
                    break;
            }
        }

        IEnumerator ScannedCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            SetState(State.idle);
        }

        void SetToolState(bool state)
        {
            spectator.gameObject.SetActive(state);
            stAR.gameObject.SetActive(state);
            exitButton.gameObject.SetActive(state);
            recorder.gameObject.SetActive(state);
        }
    }
}