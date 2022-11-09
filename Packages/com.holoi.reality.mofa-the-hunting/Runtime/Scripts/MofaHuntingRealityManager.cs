using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class MofaHuntingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Hunting")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [Header("Scene Set Up")]
        [SerializeField] Transform _dragonContainer;
        [SerializeField] GameObject _portalPrefab;
        [SerializeField] GameObject _dragonPrefab;
        Vector3 _initialTarget;

        [Header("Debug")]
        public bool SceneSetup = false;

        protected override void Start()
        {
            base.Start();

            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
            }
        }

        private void Update()
        {
            if (SceneSetup)
            {
                CreatePortalAndDragon(new Vector3(0, 1f, 6f));
                SceneSetup = false;
            }
        }

        public void CreatePortalAndDragon(Vector3 targetPosOnFloor)
        {
            var portalPos = targetPosOnFloor;
            var portalInstance = Instantiate(_portalPrefab);
            portalInstance.transform.position = portalPos;
            portalInstance.transform.LookAt(portalInstance.transform.position + DirectionHorizental(portalInstance.transform.position, HoloKit.HoloKitCamera.Instance.CenterEyePose.position));

            var dragonPos = portalPos - 2 * portalInstance.transform.forward;
            //var dragonTarget = portalPos + 2 * portalInstance.transform.forward;
            var dragonInstance = Instantiate(_dragonPrefab, _dragonContainer);
            dragonInstance.GetComponent<UnkaDragonController>().ClipPlane = -portalInstance.transform.forward;
            dragonInstance.GetComponent<UnkaDragonController>().ClipPlaneHeihgt = portalInstance.transform.position.magnitude;
            dragonInstance.transform.position = dragonPos;
        }

        void SignedDistance(Vector3 point, Vector4 plane)
        {

        }

        Vector3 DirectionHorizental(Vector3 pos, Vector3 target)
        {
            var horizentalTarget = new Vector3(target.x, pos.y ,target.z);
            return (horizentalTarget - pos).normalized;
        }
    }
}
