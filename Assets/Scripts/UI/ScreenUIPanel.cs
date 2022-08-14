using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject _back;

    [SerializeField] private GameObject _star;

    [SerializeField] private GameObject _spectator;

    [SerializeField] private GameObject _enterStARMode;

    [SerializeField] private GameObject _record;

    [SerializeField] private GameObject _shareYourReality;

    [SerializeField] private GameObject _scanQRCodeFrame;

    [SerializeField] private GameObject _scanQRCodeText;

    [SerializeField] private GameObject _showQRCode;

    [Space(16)]
    [SerializeField] private QRCodeManager _qrCodeManager;

    private void OnEnable()
    {
        // Host
        if(App.Instance.Runner != null)
        {
            _back.SetActive(true);
            _star.SetActive(true);
            _spectator.SetActive(true);
            _enterStARMode.SetActive(true);
            _record.SetActive(true);
            _shareYourReality.SetActive(false);
            _scanQRCodeFrame.SetActive(false);
            _scanQRCodeText.SetActive(false);
            _showQRCode.SetActive(false);
        }
        // Spectator
        else
        {
            _back.SetActive(true);
            _star.SetActive(false);
            _spectator.SetActive(false);
            _enterStARMode.SetActive(false);
            _record.SetActive(false);
            _shareYourReality.SetActive(false);
            _scanQRCodeFrame.SetActive(false);
            _scanQRCodeText.SetActive(false);
            _showQRCode.SetActive(true);

            _qrCodeManager.StartSharingQRCode();
        }
    }

    public void ShareYourReality()
    {
        _back.SetActive(false);
        _star.SetActive(false);
        _spectator.SetActive(false);
        _enterStARMode.SetActive(false);
        _record.SetActive(false);
        _shareYourReality.SetActive(true);
        _scanQRCodeFrame.SetActive(false);
        _scanQRCodeText.SetActive(false);
    }

    public void ScanQRCode()
    {
        _shareYourReality.SetActive(false);
        _scanQRCodeFrame.SetActive(true);
        _scanQRCodeText.SetActive(true);

        _qrCodeManager.StartScanningQRCode();
    }
}
