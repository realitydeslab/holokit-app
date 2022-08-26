using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QRFoundation;
using UnityEngine.XR.ARFoundation;
using HoloKit;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField] private QRAnchorSender _sender;

    [SerializeField] private QRAnchorReceiver _receiver;

    [SerializeField] private Image _qrCodeImage;

    public void StartSharingQRCode()
    {
        Debug.Log("Start sharing QR code");

        _sender.enabled = true;
        _sender.metaData = App.Instance.CurrentSessionCode;
        _sender.drawOnGui = false;
        _sender.onCodeUpdate.RemoveAllListeners();
        _sender.onCodeUpdate.AddListener((Texture2D texture) =>
        {
            _qrCodeImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
        _sender.StartSharing();
    }

    public void StopSharingQRCode()
    {
        _sender.StopSharing();
        _sender.enabled = false;
    }

    public void StartScanningQRCode()
    {
        Debug.Log("Start scanning QR code");

        if (App.Instance.Runner != null)
        {
            App.Instance.RealityManager.RPC_OnSpectatorStartScanningAgain();
        }

        _receiver.enabled = true;
        _receiver.onCodeDetected.RemoveAllListeners();
        _receiver.onCodeDetected.AddListener((string call) =>
        {
            Debug.Log($"On code detected {call}");
        });
        _receiver.onStabilizeFailure.RemoveAllListeners();
        _receiver.onStabilizeFailure.AddListener((string call) =>
        {
            Debug.Log($"On stabilize failure {call}");
        });
        _receiver.onAnchorReceived.RemoveAllListeners();
        _receiver.onAnchorReceived.AddListener((int anchorId, string metadata, ARAnchor anchor) =>
        {
            Debug.Log($"On anchor received {metadata}");
  
            _receiver.enabled = false;
            ResetOrigin(anchor.transform.position, anchor.transform.rotation);
            if (App.Instance.Runner == null)
            {
                Debug.Log("Attempting joining Fusion room");
                App.Instance.JoinReality(metadata);
            }
            else
            {
                Debug.Log("Already connected to the Fusion room");
                App.Instance.RealityManager.RPC_OnSpectatorScannedQRCode();
            }
        });
    }

    private void ResetOrigin(Vector3 originPosition, Quaternion originRotation)
    {
        HoloKitARSessionControllerAPI.ResetOrigin(originPosition, originRotation);
    }
}
