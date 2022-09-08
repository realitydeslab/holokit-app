using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenARModeUIPanel : MonoBehaviour
{
    public enum State
    {
        idle,
        scanning,
        scanned,
        recording,
    }

    [Header("UI Elements")]
    public Transform scanContainer;
    public Transform spectator;
    public Transform stAR;
    public Transform recorder;

    public void SetState(State state)
    {
        switch (state)
        {
            case State.idle:
                scanContainer.gameObject.SetActive(false);
                spectator.gameObject.SetActive(true);
                stAR.gameObject.SetActive(true);
                recorder.gameObject.SetActive(true);

                recorder.GetComponent<RecordButton>().SetInactive();
                break;
            case State.scanning:
                scanContainer.gameObject.SetActive(true);
                spectator.gameObject.SetActive(true);
                stAR.gameObject.SetActive(false);
                recorder.gameObject.SetActive(false);

                scanContainer.Find("Scanning").gameObject.SetActive(true);
                scanContainer.Find("Scanned").gameObject.SetActive(false);
                break;
            case State.scanned:
                scanContainer.gameObject.SetActive(true);
                spectator.gameObject.SetActive(true);
                stAR.gameObject.SetActive(true);
                recorder.gameObject.SetActive(true);

                scanContainer.Find("Scanning").gameObject.SetActive(false);
                scanContainer.Find("Scanned").gameObject.SetActive(true);
                break;
            case State.recording:
                scanContainer.gameObject.SetActive(false);
                spectator.gameObject.SetActive(true);
                stAR.gameObject.SetActive(false);
                recorder.gameObject.SetActive(true);

                recorder.GetComponent<RecordButton>().SetActive();
                break;
        }
    }

}
