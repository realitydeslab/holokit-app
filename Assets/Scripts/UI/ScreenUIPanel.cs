using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class ScreenUIPanel : MonoBehaviour
{
    //[SerializeField] private GameObject _back;

    //[SerializeField] private GameObject _star;

    //[SerializeField] private GameObject _spectator;

    //[SerializeField] private GameObject _enterStARMode;

    //[SerializeField] private GameObject _record;

    //[SerializeField] private GameObject _shareYourReality;

    //[SerializeField] private GameObject _scanQRCodeFrame;

    //[SerializeField] private GameObject _scanQRCodeScanningText;

    //[SerializeField] private GameObject _scanQRCodeConnectedText;

    //[SerializeField] private GameObject _showQRCode;

    //[SerializeField] private GameObject _checkMarkWindow;

    //[Space(10)]
    //[SerializeField] private GameObject _starUI;

    ////[Space(16)]
    ////[SerializeField] private QRCodeManager _qrCodeManager;

    //private void Awake()
    //{
    //    App.OnJoinedAsSpectator += OnJoinedAsSpectator;
    //}

    //private void OnDestroy()
    //{
    //    App.OnJoinedAsSpectator -= OnJoinedAsSpectator;
    //}

    //private void OnEnable()
    //{
    //    // Host
    //    if(App.Instance.Runner != null)
    //    {
    //        DefaultHostUI();
    //    }
    //    // Spectator
    //    else
    //    {
    //        StartScanningQRCode();
    //    }
    //}

    //private void StartScanningQRCode()
    //{
    //    _back.SetActive(false);
    //    _star.SetActive(false);
    //    _spectator.SetActive(false);
    //    _enterStARMode.SetActive(false);
    //    _record.SetActive(false);
    //    _shareYourReality.SetActive(false);
    //    _scanQRCodeFrame.SetActive(true);
    //    _scanQRCodeScanningText.SetActive(true);
    //    _showQRCode.SetActive(false);
    //    _checkMarkWindow.SetActive(false);

    //    FindObjectOfType<QRCodeManager>().StartScanningQRCode();
    //}

    //public void OnSpectatorBtnPressed()
    //{
    //    _back.SetActive(false);
    //    _star.SetActive(false);
    //    _spectator.SetActive(false);
    //    _enterStARMode.SetActive(false);
    //    _record.SetActive(false);
    //    _shareYourReality.SetActive(true);
    //    _scanQRCodeFrame.SetActive(false);
    //    _scanQRCodeScanningText.SetActive(false);
    //}

    //public void ShareQRCode()
    //{
    //    _back.SetActive(true);
    //    _star.SetActive(false);
    //    _spectator.SetActive(false);
    //    _enterStARMode.SetActive(false);
    //    _record.SetActive(false);
    //    _shareYourReality.SetActive(false);
    //    _scanQRCodeFrame.SetActive(false);
    //    _scanQRCodeScanningText.SetActive(false);
    //    _showQRCode.SetActive(true);

    //    App.Instance.RealityManager.StartSharingQRCode();
    //    //_qrCodeManager.StartSharingQRCode();
    //}

    //public void OnJoinedAsSpectator()
    //{
    //    _scanQRCodeScanningText.SetActive(false);
    //    _scanQRCodeConnectedText.SetActive(true);
    //    StartCoroutine(ShowCheckMarkWindow());
    //}

    //private IEnumerator ShowCheckMarkWindow()
    //{
    //    yield return new WaitForSeconds(2f);
    //    _scanQRCodeConnectedText.SetActive(false);
    //    _scanQRCodeFrame.SetActive(false);
    //    _checkMarkWindow.SetActive(true);
    //}

    //public void OnMarkChecked()
    //{
    //    App.Instance.RealityManager.RPC_OnMarkChecked();
    //    _checkMarkWindow.SetActive(false);
    //    _back.SetActive(true);
    //    _spectator.SetActive(true);
    //    _record.SetActive(true);
    //}

    //public void OnRescan()
    //{
    //    StartScanningQRCode();
    //}

    //public void DefaultHostUI()
    //{
    //    _back.SetActive(true);
    //    _star.SetActive(true);
    //    _spectator.SetActive(true);
    //    _enterStARMode.SetActive(true);
    //    _record.SetActive(true);
    //    _shareYourReality.SetActive(false);
    //    _scanQRCodeFrame.SetActive(false);
    //    _scanQRCodeScanningText.SetActive(false);
    //    _showQRCode.SetActive(false);
    //}

    //public void SwitchToStAR()
    //{
    //    HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
    //    Screen.orientation = ScreenOrientation.LandscapeLeft;
    //    gameObject.SetActive(false);
    //    _starUI.SetActive(true);
    //}
}
