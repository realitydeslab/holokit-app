using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class TypedTheDuckRealityManager : RealityManager
    {
        public GameObject DuckPrefab;

        GameObject duckInstance;

        [HideInInspector] public Vector2 DuckMaxSpeed = new(10,10);

        public void OnDragBegin()
        {
            duckInstance = Instantiate(DuckPrefab);
            duckInstance.transform.position =
                HoloKit.HoloKitCamera.Instance.CenterEyePose.position
                + HoloKit.HoloKitCamera.Instance.CenterEyePose.forward * 1f
                - HoloKit.HoloKitCamera.Instance.CenterEyePose.up * 0.5f;
            duckInstance.GetComponent<Rigidbody>().useGravity = false;
        }

        public void OnDrag()
        {
            //duckInstance.transform.position = position;
        }

        public void OnDragEnd()
        {
            Debug.Log("OnDragEnd!!!!");
            var length = FindObjectOfType<DragableUI>().Length;
            var direction = FindObjectOfType<DragableUI>().Direction;
            //Debug.Log("length: " + length);
            //Debug.Log("direction: " + direction);

            duckInstance.GetComponent<Rigidbody>().useGravity = true;
            var fixedYSpeed = MathHelpers.Remap(length,0,600,0, DuckMaxSpeed.y, true);
            var fixedXSpeed = MathHelpers.Remap(length,0,600,0, DuckMaxSpeed.x, true);

            duckInstance.GetComponent<Rigidbody>().velocity = -direction * fixedYSpeed + HoloKit.HoloKitCamera.Instance.CenterEyePose.forward * fixedXSpeed;
        }
    }
}
