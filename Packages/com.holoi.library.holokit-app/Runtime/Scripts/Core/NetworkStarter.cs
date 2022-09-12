using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App
{
    public class NetworkStarter : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StartNetworkWithDelay(3f));
        }

        private IEnumerator StartNetworkWithDelay(float t)
        {
            yield return new WaitForSeconds(t);
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitApp.Instance.StartHost();
            }
            else
            {
                HoloKitApp.Instance.StartClient();
            }
        }
    }
}