using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Utilities/Tools/Use Transform")]

    public class UseTransform : MonoBehaviour
    {
        public enum UpdateMode                                          // The available methods of updating are:
        {
            Update = 1,
            FixedUpdate = 2,                                            // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate = 4,                                             // Update in LateUpdate. (for tracking objects that are moved in Update)
        }


        [Tooltip("Transform to use the Position as Reference")]
        public Transform Reference;
        [Tooltip("Use the Reference's Rotation")]
        public bool rotation = true;  
        public UpdateMode RotationUpdate = UpdateMode.LateUpdate;
        [Tooltip("Use the Reference's Position")]
        public bool position = true;
        public UpdateMode PositionUpdate = UpdateMode.FixedUpdate;



        // Update is called once per frame
        void Update()
        {
            if (PositionUpdate == UpdateMode.Update) SetPositionReference();
            if (RotationUpdate == UpdateMode.Update) SetRotationReference();
        }

        void LateUpdate()
        {
            if (PositionUpdate == UpdateMode.LateUpdate) SetPositionReference();
            if (RotationUpdate == UpdateMode.LateUpdate) SetRotationReference();
        }

        void FixedUpdate()
        {
            if (PositionUpdate == UpdateMode.FixedUpdate) SetPositionReference();
            if (RotationUpdate == UpdateMode.FixedUpdate) SetRotationReference();
        }

        private void SetPositionReference()
        {
            if (!Reference) return;
            if (position) transform.position = Reference.position;
        }


        private void SetRotationReference()
        {
            if (!Reference) return;
            if (rotation) transform.rotation = Reference.rotation;
        }
    }
}