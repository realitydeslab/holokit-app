using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class BalloonTypoSceneManager : MonoBehaviour
    {
        public GameObject Ball;
        public GameObject SoftBall;

        void Start()
        {
            FindObjectOfType<ARPlaneManager>(true).enabled = true;
        }

        public void MeshingDone()
        {
            FindObjectOfType<ARPlaneManager>(true).enabled = false;
            //FindObjectOfType<ARMeshManager>(true).enabled = false;
            //FindObjectOfType<ToggleMeshClassification>(true).enabled = false;
        }

        public void ServeTheBall()
        {
            var go = Instantiate(Ball);
            go.transform.position = HoloKitCamera.Instance.CenterEyePose.position + HoloKitCamera.Instance.CenterEyePose.forward * 1f;
            go.transform.LookAt(HoloKitCamera.Instance.CenterEyePose.position + HoloKitCamera.Instance.CenterEyePose.forward * 2f);
            go.GetComponent<Rigidbody>().velocity = new Vector3(0, 1, 1.5f);
        }


        public void ServeTheSoftBall()
        {
            var go = Instantiate(SoftBall);
            go.transform.position = HoloKitCamera.Instance.CenterEyePose.position + HoloKitCamera.Instance.CenterEyePose.forward * 1f;
            go.transform.LookAt(HoloKitCamera.Instance.CenterEyePose.position + HoloKitCamera.Instance.CenterEyePose.forward * 2f);
            go.GetComponent<Rigidbody>().velocity = new Vector3(0, 1, 1.5f);
        }

        public void ClearBalls()
        {
            var gos = FindObjectsOfType<Rigidbody>();

            foreach (var go in gos)
            {
                Destroy(go.gameObject);
            }
        }
    }
}
