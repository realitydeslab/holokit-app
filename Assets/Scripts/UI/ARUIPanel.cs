using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject _screenUIPanel;

    [SerializeField] private GameObject _starUIPanel;

    private void Awake()
    {
        _screenUIPanel.SetActive(false);
        _starUIPanel.SetActive(false);

        CoachingOverlaySessionDelegate.OnCoachingOverlayViewEnded += OnCoachOverlayViewEnded;
    }

    private void OnCoachOverlayViewEnded()
    {
        _screenUIPanel.SetActive(true);
    }
}
