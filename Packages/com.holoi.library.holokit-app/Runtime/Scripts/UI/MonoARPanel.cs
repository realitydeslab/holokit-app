using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.HoloKit.App
{
    public class MonoARPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _hostMainWindow;

        [SerializeField] private GameObject _spectatorMainWindow;

        [SerializeField] private GameObject _hostConnectionWindow;

        [SerializeField] private GameObject _spectatorConnectionWindow;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                OpenMainWindow();
            }
            else
            {
                OpenSpectatorConnectionWindow();
            }
        }

        private void OpenMainWindow()
        {
            _hostMainWindow.SetActive(true);
            _hostConnectionWindow.SetActive(false);
            _spectatorConnectionWindow.SetActive(false);
        }

        private void OpenHostConnectionWindow()
        {
            _hostMainWindow.SetActive(false);
            _hostConnectionWindow.SetActive(true);
            _spectatorConnectionWindow.SetActive(false);
        }

        private void OpenSpectatorConnectionWindow()
        {
            _hostMainWindow.SetActive(false);
            _hostConnectionWindow.SetActive(false);
            _spectatorConnectionWindow.SetActive(true);
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