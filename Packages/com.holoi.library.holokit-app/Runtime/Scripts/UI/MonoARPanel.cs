using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.HoloKit.App
{
    public class MonoARPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _mainWindow;

        [SerializeField] private GameObject _hostConnectionWindow;

        [SerializeField] private GameObject _clientConnectionWindow;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                OpenMainWindow();
            }
            else
            {
                OpenClientConnectionWindow();
            }
        }

        private void OpenMainWindow()
        {
            _mainWindow.SetActive(true);
            _hostConnectionWindow.SetActive(false);
            _clientConnectionWindow.SetActive(false);
        }

        private void OpenHostConnectionWindow()
        {
            _mainWindow.SetActive(false);
            _hostConnectionWindow.SetActive(true);
            _clientConnectionWindow.SetActive(false);
        }

        private void OpenClientConnectionWindow()
        {
            _mainWindow.SetActive(false);
            _hostConnectionWindow.SetActive(false);
            _clientConnectionWindow.SetActive(true);
        }

        public void StartSharingReality()
        {
            HoloKitApp.Instance.StartAdvertising();
            OpenHostConnectionWindow();
        }

        public void Disconnect()
        {

        }
    }
}