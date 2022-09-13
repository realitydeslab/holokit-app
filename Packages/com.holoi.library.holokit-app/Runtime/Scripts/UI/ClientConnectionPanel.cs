using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.HoloKit.App
{
    public class ClientConnectionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _connectionText;

        [SerializeField] private GameObject _scanQRCode;

        private void OnEnable()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator += OnConnectedAsSpectator;
            _connectionText.SetActive(true);
            _scanQRCode.SetActive(false);
        }

        private void OnDisable()
        {
            HoloKitApp.Instance.OnConnectedAsSpectator -= OnConnectedAsSpectator;
        }

        private void OnConnectedAsSpectator()
        {
            _connectionText.SetActive(false);
            _scanQRCode.SetActive(true);
        }
    }
}