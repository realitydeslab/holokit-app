using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientConnectionPanel : MonoBehaviour
{
    [SerializeField] private GameObject _connectionText;

    [SerializeField] private GameObject _scanQRCode;

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        _connectionText.SetActive(true);
        _scanQRCode.SetActive(false);
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            _connectionText.SetActive(false);
            _scanQRCode.SetActive(true);
        }
    }
}
