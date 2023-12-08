using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Utilities/Laser")] 
    public class MLaser : MonoBehaviour
    {
        [RequiredField] public TrailRenderer trail;
        [RequiredField] public Transform StartPoint;
        [RequiredField] public Transform EndPoint;

        private void OnEnable()
        {
            trail.AddPosition(StartPoint.position);
            trail.AddPosition(EndPoint.position);
            trail.enabled = true;
            trail.time = Mathf.Infinity; //Make the laser last forever
            trail.minVertexDistance = Mathf.Infinity; //Make the laser last forever
        }

        private void OnDisable()
        {
            trail.Clear();
        }

        private void Update()
        {
            trail.SetPosition(0, StartPoint.position);
            trail.SetPosition(1, EndPoint.position);
        }

        private void Reset()
        {
            trail = GetComponent<TrailRenderer>();

            if (trail == null) trail = gameObject.AddComponent<TrailRenderer>();
            StartPoint = transform;

            trail.time = Mathf.Infinity; //Make the laser last forever
            trail.minVertexDistance = Mathf.Infinity; //Make the laser last forever
            trail.material = MTools.GetInstance<Material>("ParticleSpark");
            trail.widthMultiplier = 0.1f;
        }

       
    }
}
