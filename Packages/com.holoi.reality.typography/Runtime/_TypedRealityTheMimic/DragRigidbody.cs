using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class DragRigidbody : MonoBehaviour
    {
        [Header("sizheng refine")]
        public Transform fingerJoint;
        public Transform bodyJoint;

        [Tooltip("The spring force applied when dragging rigidbody. The dragging is implemented by attaching an invisible spring joint.")]
        public float Spring = 50.0f;
        public float Damper = 5.0f;
        public float Drag = 10.0f;
        public float AngularDrag = 5.0f;
        public float Distance = 0.2f;
        public float ScrollWheelSensitivity = 5.0f;
        public float RotateSpringSpeed = 10.0f;

        [Tooltip("Pin dragged spring to its current location.")]
        public KeyCode KeyToPinSpring = KeyCode.Space;

        [Tooltip("Delete all pinned springs.")]
        public KeyCode KeyToClearPins = KeyCode.Delete;

        [Tooltip("Twist spring.")]
        public KeyCode KeyToRotateLeft = KeyCode.Z;

        [Tooltip("Twist spring.")]
        public KeyCode KeyToRotateRight = KeyCode.C;

        [Tooltip("Set any LineRenderer prefab to render the used springs for the drag.")]
        public LineRenderer SpringRenderer;

        private int m_SpringCount = 1;
        private SpringJoint m_SpringJoint;
        private LineRenderer m_SpringRenderer;

        private void Start()
        {
            fingerJoint.position = bodyJoint.position;
            //UpdatePinnedSprings();

            if (!m_SpringJoint)
            {
                //fingerJoint.transform.parent = transform;
                //fingerJoint.transform.localPosition = Vector3.zero;
                Rigidbody body = fingerJoint.gameObject.AddComponent<Rigidbody>();
                m_SpringJoint = fingerJoint.gameObject.AddComponent<SpringJoint>();
                body.isKinematic = true;

                if (SpringRenderer)
                {
                    m_SpringRenderer = Instantiate(SpringRenderer.gameObject, m_SpringJoint.transform, true).GetComponent<LineRenderer>();
                }
            }

            m_SpringJoint.spring = Spring;
            m_SpringJoint.damper = Damper;
            m_SpringJoint.maxDistance = Distance;
            m_SpringJoint.connectedBody = bodyJoint.GetComponent<Rigidbody>();
            Debug.Log("connectedBody = BodyJoint");
            m_SpringJoint.transform.position = fingerJoint.position;
            m_SpringJoint.anchor = Vector3.zero;

            if (m_SpringRenderer)
            {
                m_SpringRenderer.enabled = true;
            }

            UpdatePinnedSprings();

            StartCoroutine(DragObject());
        }

        private void Update()
        {

            UpdatePinnedSprings();

            //if (!m_SpringJoint)
            //{
            //    fingerJoint.transform.parent = transform;
            //    fingerJoint.transform.localPosition = Vector3.zero;
            //    Rigidbody body = fingerJoint.gameObject.AddComponent<Rigidbody>();
            //    m_SpringJoint = fingerJoint.gameObject.AddComponent<SpringJoint>();
            //    body.isKinematic = true;

            //    m_SpringJoint.spring = Spring;
            //    m_SpringJoint.damper = Damper;
            //    m_SpringJoint.maxDistance = Distance;
            //    m_SpringJoint.connectedBody = BodyJoint.GetComponent<Rigidbody>();
            //    Debug.Log("connectedBody = BodyJoint");


            //    if (SpringRenderer)
            //    {
            //        m_SpringRenderer = Instantiate(SpringRenderer.gameObject, m_SpringJoint.transform, true).GetComponent<LineRenderer>();
            //    }
            //}

            //m_SpringJoint.transform.position = fingerJoint.position;
            //m_SpringJoint.anchor = Vector3.zero;

            //if (m_SpringRenderer)
            //{
            //    m_SpringRenderer.enabled = true;
            //}

            //UpdatePinnedSprings();

            //StartCoroutine(DragObject());
        }


        private IEnumerator DragObject()
        {
            Debug.Log("DragObject");

            var oldDrag = m_SpringJoint.connectedBody.drag;
            var oldAngularDrag = m_SpringJoint.connectedBody.angularDrag;
            m_SpringJoint.connectedBody.drag = Drag;
            m_SpringJoint.connectedBody.angularDrag = AngularDrag;
            while (true)
            {
                var connectedPosition = m_SpringJoint.connectedBody.transform.position +
                                        m_SpringJoint.connectedBody.rotation * m_SpringJoint.connectedAnchor;

                var axis = m_SpringJoint.transform.position - connectedPosition;
                if (Input.GetKey(KeyToRotateLeft))
                {
                    m_SpringJoint.connectedBody.transform.Rotate(axis, RotateSpringSpeed, Space.World);
                }
                if (Input.GetKey(KeyToRotateRight))
                {
                    m_SpringJoint.connectedBody.transform.Rotate(axis, -RotateSpringSpeed, Space.World);
                }
                yield return null;
            }


            if (m_SpringJoint.connectedBody)
            {
                m_SpringJoint.connectedBody.drag = oldDrag;
                m_SpringJoint.connectedBody.angularDrag = oldAngularDrag;

                //if (Input.GetKeyDown(KeyToPinSpring))
                //{
                //    m_SpringJoint = null;
                //    m_SpringRenderer = null;
                //}
                //else
                //{
                //    m_SpringJoint.connectedBody = null;
                //    if (m_SpringRenderer)
                //    {
                //        m_SpringRenderer.enabled = false;
                //    }
                //}
            }
        }


        private void UpdatePinnedSprings()
        {
            //Debug.Log("UpdatePinnedSprings");

            //foreach (Transform child in transform)
            //{
            var spring = GetComponent<SpringJoint>();
            var renderer = GetComponent<LineRenderer>();

            if (!spring.connectedBody)
                return;

            var connectedPosition = spring.connectedBody.transform.TransformPoint(spring.connectedAnchor);

            if (renderer && renderer.positionCount >= 2)
            {
                renderer.SetPosition(0, spring.transform.position);
                renderer.SetPosition(1, connectedPosition);
            }
            //}
        }
    }
}