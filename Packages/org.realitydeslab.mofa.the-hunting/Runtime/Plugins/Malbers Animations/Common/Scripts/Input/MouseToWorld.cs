using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Input/Mouse World Position")]

    public class MouseToWorld : MonoBehaviour 
    {
        [Tooltip("Reference to the camera")]
        public TransformReference MainCamera;
        [Tooltip("Reference to the Mouse Point Transform")]
        public TransformReference MousePoint;
        [Tooltip("Reference to the Mouse Point Transform")]
        public LayerReference layer = new LayerReference(-1);
        public QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal;
        public FloatReference MaxDistance = new FloatReference( 100f);

        //[Space]
        //public bool Snap = true;
        //[Tooltip("Reference to the Mouse Point Transform")]
        //public LayerReference Snaplayer = new LayerReference(0);
        //public Tag[] tags;

        private Camera m_camera;

        private void Start()
        {
            if (MainCamera.Value == null)
            {
                m_camera = MTools.FindMainCamera();

                if (m_camera)
                {
                    MainCamera = m_camera.transform;
                }
                else
                {
                    Debug.LogWarning("There's no Main Camera on the Scene");
                    enabled = false;
                }
            }
            else
            {
                m_camera = MainCamera.Value.GetComponent<Camera>();
                if (m_camera == null)
                {
                    Debug.LogWarning("There's no Main Camera on the Scene");
                    enabled = false;
                }
            }

            if (MousePoint.Value == null) MousePoint.Value = transform;
             
        }


        private void Update()
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, layer, interaction))
            {
                if (MousePoint.Value == null)
                {
                    MousePoint.Value = transform; //ReCheck that the Mouse Point is Never Null
                }

                if (MousePoint.Value == transform)
                {
                    MousePoint.Value.position = hit.point; //Only Update the Point if the Mouse Point is This Transform
                }
                
                //if (hit.transform != HitTransform)
                //{
                //    HitTransform = hit.transform; //Store the hit transform.

                //    FindCenter();
                //}

                //if (Snap && Snaplayer != 0 && MTools.Layer_in_LayerMask(HitTransform.gameObject.layer, Snaplayer))
                //{
                //    MousePoint.Value.position = TransformCenter;
                //}
            }
        }

        //private void FindCenter()
        //{
        //    TransformCenter = Vector3.zero;

        //    var MR = HitTransform.GetComponentsInChildren<MeshRenderer>();

        //    foreach (var item in MR)
        //        TransformCenter += item.bounds.center;

        //    var SMR = HitTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        //    foreach (var item in SMR)
        //        TransformCenter += item.bounds.center;

        //    if (MR.Length + MR.Length > 0)
        //        TransformCenter /= MR.Length + MR.Length;
        //}

        public Transform HitTransform { get; set; }
        public Vector3 TransformCenter { get; set; }

        private void Reset()
        {
            MousePoint.Value = transform;
        }
    }
}