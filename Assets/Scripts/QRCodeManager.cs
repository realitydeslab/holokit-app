using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QRFoundation;
using UnityEngine.UI;
using HoloKitAppNativePlugin;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField] private QRAnchorSender _sender;

    [SerializeField] private QRAnchorReceiver _receiver;

    [SerializeField] private Image _qrCodeImage;

    public void StartSharingQRCode()
    {
        Debug.Log("Start sharing QR code");
        // Generate a random password for MPC
        string password = Utils.GetRandomMPCPassword();
        MPCSessionControllerAPI.StartAdvertising(password);

        _sender.enabled = true;
        _sender.metaData = password;
        _sender.onCodeUpdate.RemoveAllListeners();
        _sender.onCodeUpdate.AddListener((Texture2D texture) =>
        {
            _qrCodeImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
        _sender.StartSharing();
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
        _receiver.onPoseReceived.RemoveAllListeners();
        _receiver.onPoseReceived.AddListener((int anchorId, string metadata, Pose pose) =>
        {
            Debug.Log($"On pose received {anchorId} {metadata} {pose}");
            App.Instance.RealityManager.SetPose(pose);
            
            //HoloKit.HoloKitARSessionControllerAPI.ResetOrigin(pose.position, pose.rotation);
            _receiver.enabled = false;
            // TODO: Start browse the advertiser with the same metadata
            MPCSessionControllerAPI.StartBrowsing(metadata, App.Instance.CurrentSessionCode);
        });
    }
}
