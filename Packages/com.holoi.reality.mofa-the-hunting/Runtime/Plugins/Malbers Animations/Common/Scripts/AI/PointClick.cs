using UnityEngine;
using UnityEngine.EventSystems;
using MalbersAnimations.Events;
using UnityEngine.AI;
using UnityEngine.Serialization;
using MalbersAnimations.Controller.AI;

namespace MalbersAnimations 
{
    [AddComponentMenu("Malbers/AI/Point Click")]
    public class PointClick : MonoBehaviour
    {
        public PointClickData pointClickData;
        [Tooltip ("UI to intantiate on the Hit Point ")]
        public GameObject PointUI;
        [Tooltip ("Radius to find <AI Targets> on the Hit Point")]
        public float radius = 0.2f;
        private const float navMeshSampleDistance = 4f;
       
        [Tooltip("If its hit a point on an empty space, it will clear the Current Target")]
        public bool ClearTarget = true;

        [Header("Events")]
        public Vector3Event OnPointClick = new Vector3Event();
        [FormerlySerializedAs("OnInteractableClick")]
        public TransformEvent OnAITargetClick = new TransformEvent();

        protected Collider[] AITargets;

        public IAIControl AIControl;

        void OnEnable()
        {
            if (pointClickData) pointClickData.baseDataPointerClick += OnGroundClick;

            AIControl = GetComponent<IAIControl>();
        }


        void OnDisable()
        {
            if (pointClickData) pointClickData.baseDataPointerClick -= OnGroundClick;
        }

        Vector3 destinationPosition;

        public virtual void OnGroundClick(BaseEventData data)
        {
            PointerEventData pData = (PointerEventData)data;
            if (NavMesh.SamplePosition(pData.pointerCurrentRaycast.worldPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                destinationPosition = hit.position;
            }
            else
                destinationPosition = pData.pointerCurrentRaycast.worldPosition;

            MTools.DrawWireSphere(destinationPosition, Color.red, radius, 1);

            AITargets = Physics.OverlapSphere(destinationPosition, radius); //Find all the AI TARGETS on a Radius

            foreach (var inter in AITargets)
            {
                if (inter.transform.root == this.transform.root) continue;  //Don't click on yourself

                if (inter.transform.root.FindInterface<IAITarget>() != null)
                {
                    OnAITargetClick.Invoke(inter.transform.root); //Invoke only the first interactable found
                    if (PointUI)
                        Instantiate(PointUI, inter.transform.position, Quaternion.FromToRotation(PointUI.transform.up, pData.pointerCurrentRaycast.worldNormal));

                    return;
                }
            }

            if (PointUI)
                Instantiate(PointUI, destinationPosition, Quaternion.FromToRotation(PointUI.transform.up, pData.pointerCurrentRaycast.worldNormal));


            if (ClearTarget) AIControl?.SetTarget(null, true);
            OnPointClick.Invoke(destinationPosition);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(destinationPosition, 0.1f);
                Gizmos.DrawSphere(destinationPosition, 0.1f);
            }
        }

        private void Reset()
        {
            pointClickData = MTools.GetInstance<PointClickData>("PointClickData");
            PointUI = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Malbers Animations/Common/Prefabs/Interactables/ClickPoint.prefab");
            MTools.SetDirty(this); 

            var SetDestination = this.GetUnityAction<Vector3>("MAnimalAIControl", "SetDestination");
            if (SetDestination != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(OnPointClick, SetDestination);

            var SetTarget = this.GetUnityAction<Transform>("MAnimalAIControl", "SetTarget");
            if (SetTarget != null) UnityEditor.Events.UnityEventTools.AddPersistentListener(OnAITargetClick, SetTarget);  
        }

#endif
    }
}