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
    }

    public void StartScanningQRCode()
    {
        Debug.Log("Start scanning QR code");
        _receiver.enabled = true;
        _receiver.onCodeDetected.RemoveAllListeners();
        _receiver.onCodeDetected.AddListener((string call) =>
        {
            Debug.Log($"On code detected {call}");
        });
        //_receiver.onPoseReceived.RemoveAllListeners();
        //_receiver.onPoseReceived.AddListener((int anchorId, string metadata, Pose pose) =>
        //{
        //    Debug.Log($"On pose received {metadata}");
        //    App.Instance.JoinReality(metadata);
        //    _receiver.enabled = false;
        //    App.Instance.RealityManager.ResetOrigin(pose.position, pose.rotation);
        //});
        _receiver.onAnchorReceived.RemoveAllListeners();
        _receiver.onAnchorReceived.AddListener((int anchorId, string metadata, ARAnchor anchor) =>
        {
            Debug.Log($"On anchor received {metadata}");
            ResetOrigin(anchor.transform.position, anchor.transform.rotation);
            App.Instance.JoinReality(metadata);
        });
    }

    private void ResetOrigin(Vector3 originPosition, Quaternion originRotation)
    {
        HoloKitARSessionControllerAPI.ResetOrigin(originPosition, originRotation);
    }
}
