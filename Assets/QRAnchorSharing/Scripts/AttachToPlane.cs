using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace QRFoundation
{
    public class AttachToPlane : MonoBehaviour
    {
        // Direction in which to look for the plane to snap to.
        // Hit test will be performed from -diretion*tolerance till +direction*tolerance
        public Vector3 direction = new Vector3(0, -1, 0);
        public float tolerance = 0.2f;
        // If set to true, the GameObject is aligned with the normal of the plane.
        // Otherwise only the position snaps.
        public bool adjustOrientation = true;

        // Dependencies
        public ARRaycastManager rayCastManager;
        public ARPlaneManager planeManager;

        // Start is called before the first frame update
        void Start()
        {
        }

        void Update()
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (this.rayCastManager.Raycast(
                new Ray(this.transform.position - this.direction * tolerance, this.direction),
                hits
            ))
            {
                ARRaycastHit closestHit = new ARRaycastHit();
                float closestDistance = tolerance;

                foreach (ARRaycastHit hit in hits)
                {
                    if ((hit.hitType & UnityEngine.XR.ARSubsystems.TrackableType.Planes) != 0)
                    {
                        float distance = Vector3.Distance(hit.pose.position, this.transform.position);
                        if (distance <= closestDistance)
                        {
                            closestDistance = distance;
                            closestHit = hit;
                        }
                    }
                }


                this.transform.position = closestHit.pose.position;
                if (this.adjustOrientation)
                {
                    ARPlane plane = planeManager.GetPlane(closestHit.trackableId);
                    this.transform.rotation = Quaternion.LookRotation(plane.normal, Vector3.up);
                }
                this.enabled = false;
            }
        }
    }
}
