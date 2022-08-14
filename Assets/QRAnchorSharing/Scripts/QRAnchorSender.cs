using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QRFoundation
{
    [Serializable]
    public class OnCodeUpdateEvent : UnityEvent<Texture2D> { };

    public class QRAnchorSender : MonoBehaviour
    {
        public static readonly float INCH_TO_METERS = 0.0254f;

        [HideInInspector]
        public GameObject target;
        [HideInInspector]
        public float updateInterval = 0.3f;
        [HideInInspector]
        public string metaData = "";
        [HideInInspector]
        public int anchorId = 0;
        [HideInInspector]
        public float codeScale = 1;
        [HideInInspector]
        public bool drawOnGui = true;
        [HideInInspector]
        public OnCodeUpdateEvent onCodeUpdate;

        [HideInInspector]
        public Texture2D qrCodeAsTexture2D;
        private float lastWidth = 1;
        private float lastQrTime = 0;

        public bool sharing { get; private set; } = false;
        private Pose lastPose;

        void Start()
        {
        }

        void Update()
        {
            if (!sharing || lastQrTime > Time.time - updateInterval || !target)
            {
                return;
            }

            CreateCode();
            onCodeUpdate.Invoke(qrCodeAsTexture2D);
        }

        private Texture2D CreateCode()
        {
            Pose targetPose = new Pose(target.transform.position, target.transform.rotation);
            // Rotate own pose to "lean back" 90 degrees, because the system
            // interprets a QR code's orientation as laying flat, not like a
            // window as desired in this case.
            Pose camPose = new Pose(transform.position, Quaternion.LookRotation(transform.up, -transform.forward));

            Pose diffPose = new Pose();
            diffPose.position = Quaternion.Inverse(camPose.rotation) * (target.transform.position - camPose.position);
            diffPose.rotation = Quaternion.Inverse(camPose.rotation) * targetPose.rotation;
            lastPose = diffPose;

            EncodeDecode.EncodePose(diffPose, lastWidth, anchorId, metaData, out string base64);

            var enc = new ZXing.QrCode.QRCodeWriter();
            var mat = enc.encode(base64, ZXing.BarcodeFormat.QR_CODE, 0, 0);
            qrCodeAsTexture2D = CodeToTexture(mat);

            lastWidth = CalcQRWidth(mat);
            //EncodeDecode.DecodePose(base64, out Pose decodedPose, out float decodedWidth, out int decodedAnchorId, out string rest);

            lastQrTime = Time.time;

            //Debug.Log(decodedPose);
            //Debug.Log("width: " + lastWidth);

            return qrCodeAsTexture2D;
        }

        public void StartSharing()
        {
            sharing = true;
            lastQrTime = 0;
            CreateCode();
            onCodeUpdate.Invoke(qrCodeAsTexture2D);
        }

        public void StopSharing()
        {
            sharing = false;
        }

        private void OnGUI()
        {
            if (sharing && drawOnGui && qrCodeAsTexture2D)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                // Choose the smaller screen dimension to support landscape view.
                float codeSize = Mathf.Min(Screen.width, Screen.height);
                GUI.DrawTexture(
                    new Rect(
                        (Screen.width - codeSize) / 2,
                        (Screen.height - codeSize) / 2,
                        codeSize,
                        codeSize
                    ),
                    qrCodeAsTexture2D
                );

                //GUI.TextField(new Rect(30, 10, 200, 30), lastWidth + "");
            }
        }

        private Texture2D CodeToTexture(ZXing.Common.BitMatrix mat)
        {
            var width = mat.Width;
            var res = new Texture2D(width, width, TextureFormat.RGB24, false, false);
            res.filterMode = FilterMode.Point;
            var colors = new Color[width * width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    colors[j * width + i] = mat[i, width - j - 1] ? Color.black : Color.white;
                }
            }
            res.SetPixels(colors);
            res.Apply();
            return res;
        }

        private float CalcQRWidth(ZXing.Common.BitMatrix mat)
        {
            float pixelWidth = 0;
            for (int i = 0; i < mat.Width; i++)
            {
                if (mat[i, i])
                {
                    pixelWidth = mat.Width - i * 2;
                    break;
                }
            }
            float screenWidth = (Screen.width / Screen.dpi) * INCH_TO_METERS;
            return (pixelWidth / mat.Width) * screenWidth * codeScale;
        }
    }
}
