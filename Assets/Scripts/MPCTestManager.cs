using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class MPCTestManager : MonoBehaviour
{
    [SerializeField] private GameObject _startHostBtn;

    [SerializeField] private GameObject _startClientBtn;

    [SerializeField] private GameObject _connectedText;

    [SerializeField] private GameObject _disconnectBtn;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientConnected(ulong clientID)
    {
        Debug.Log($"OnClientConnected {clientID}");
        _startClientBtn.SetActive(false);
        _startHostBtn.SetActive(false);
        _connectedText.SetActive(true);
        _disconnectBtn.SetActive(true);
    }

    private void OnClientDisconnect(ulong clientID)
    {
        //throw new NotImplementedException();
        Debug.Log($"OnClientDisconnect {clientID}");
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        _startClientBtn.SetActive(true);
        _startHostBtn.SetActive(true);
        _connectedText.SetActive(false);
        _disconnectBtn.SetActive(false);
    }
}
