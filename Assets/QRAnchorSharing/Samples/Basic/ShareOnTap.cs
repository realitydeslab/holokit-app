using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QRFoundation
{
    public class ShareOnTap : MonoBehaviour
    {
        private QRAnchorSender sender;
        private bool sending = false;

        void Start()
        {
            sender = GetComponent<QRAnchorSender>();
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                if (!sending)
                {
                    sending = true;
                    sender.StartSharing();
                }
            }
            else
            {
                if (sending)
                {
                    sending = false;
                    sender.StopSharing();
                }
            }
        }
    }
}
