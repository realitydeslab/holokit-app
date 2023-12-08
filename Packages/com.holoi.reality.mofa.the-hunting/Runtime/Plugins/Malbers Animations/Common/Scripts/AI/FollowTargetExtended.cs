using System.Collections;
using UnityEngine;

/// <summary>
/// This is an updated version to the original script from Malbers. It allows you to:
/// * set a random offset to the target position
/// * change the random offset at a given time interval
/// </summary>
namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/AI/Follow Target Extended")]

    /// <summary> Simple follow target for the animal </summary>
    public class FollowTargetExtended : MonoBehaviour
    {

        public enum TargetType
        {
            [Tooltip("Use the specific target gameobject as target")]
            GameObject,

            [Tooltip("Use the first found gameobject as target which has the specified target tag. Usually the Player tag")]
            Tag
        }

        [Header("Selection")]

        public TargetType type = TargetType.GameObject;

        [Tooltip("An optional target tag. If no target is explicitly set, then the target with this specified target name will be used")]
        public string targetTag;

        [Tooltip("The target gameobject. Either explicitly specified or defined at start depending on the target tag")]
        public Transform targetGameObject;

        [Tooltip("In case the target is instantiated at startup, setting this to true will try to find the target with the specified tag during the Update method")]
        public bool delayedTargetSearch = false;

        [Header("Settings")]

        public float stopDistance = 3;
        ICharacterMove animal;

        [Tooltip("Activate having random positions around the target")]
        public bool targetRandomness = false;

        [Tooltip("Number of seconds at which the target position changes. Use 0 for keeping the initial position")]
        public float updateInterval = 0f;

        [Tooltip("Minimum random distance from the target")]
        [Range(0f,10f)]
        public float randomDistanceMin = 1f;

        [Tooltip("Maximum random distance from the target")]
        [Range(0f, 10f)]
        public float randomDistanceMax = 3f; 

        private Vector3 targetDistanceOffset = Vector3.zero;

        /// <summary>
		/// If a key is defined, then this key will be used for toggling the following.
		/// </summary>
        public string followToogleKey;

        /// <summary>
		/// Whether following is enabled or not
		/// </summary>
        public bool followEnabled = true;

        // Use this for initialization
        void Start()
        {
            animal = GetComponentInParent<ICharacterMove>();

            // randomize target position
            if (targetRandomness)
            {
                // initial target position
                CalculateTargetPositionOffset();

                // periodic target position changes
                if(updateInterval > 0) {

                    StartCoroutine(UpdateTargetPositionOffset());

                }
            }

            // find target in case it should be found by tag
            FindTargetByTag();
        }

        private void FindTargetByTag()
        {
            if (!targetGameObject && type == TargetType.Tag && targetTag != null)
            {
                targetTag = targetTag.Trim();

                if (!string.IsNullOrEmpty(targetTag))
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);
                    foreach (GameObject player in players)
                    {
                        if (player.activeInHierarchy)
                        {
                            targetGameObject = player.transform;

                            Debug.Log("Target found for " + transform.name + ": " + targetGameObject.transform.name);
                            break;
                        }

                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!targetGameObject)
            {
                // search until a target is found
                if (delayedTargetSearch)
                {
                    FindTargetByTag();

                    Debug.Log("Delayed target update. Target = " + targetGameObject);
                }

                // still no target found, abort
                if( !targetGameObject)
                    return;
            }

            if (Input.anyKeyDown)
            {
                if (followToogleKey.Length > 0 && Input.inputString == followToogleKey)
                {
                        followEnabled = !followEnabled;
                }
            }

            if (!followEnabled)
                return;

            Vector3 targetPosition = targetGameObject.position;
            if( targetRandomness) {

                // calculate the new target position depending on the offset from the local position
                // targetPosition = target.transform.TransformPoint( target.localPosition + targetDistanceOffset);
                targetPosition = targetGameObject.localPosition + targetDistanceOffset;

                // Debug.DrawLine(transform.position, targetPosition, Color.yellow);

            }

            Vector3 Direction = targetPosition - transform.position;               //Calculate the direction from the animal to the target
            float distance = Vector3.Distance(transform.position, targetGameObject.position); //Calculate the distance..
            animal.Move(distance > stopDistance ? Direction : Vector3.zero);        //Move the Animal if we are not on the Stop Distance Radius
        }

        private void OnDisable()
        {
            animal.Move(Vector3.zero);      //In case this script gets disabled stop the movement of the Animal
        }

        private IEnumerator UpdateTargetPositionOffset() {

            while (true)
            {
                yield return new WaitForSeconds(updateInterval);

                CalculateTargetPositionOffset();

            }

        }

        /// <summary>
        /// Calculate a random local offset from the target
        /// </summary>
        private void CalculateTargetPositionOffset()
        {
            float distance = Random.Range(randomDistanceMin, randomDistanceMax);
            targetDistanceOffset = Random.insideUnitSphere.normalized * distance;

        }
    }
}