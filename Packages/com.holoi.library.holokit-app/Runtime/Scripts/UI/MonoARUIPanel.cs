using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App
{
    public class MonoARUIPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _mainWindow;

        [SerializeField] private GameObject _hostConnectionWindow;

        [SerializeField] private GameObject _clientConnectionWindow;

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
            HoloKitApp.Instance.RealityManager.StartSharingReality();
            OpenHostConnectionWindow();
        }

        public void Disconnect()
        {

        }
    }
}