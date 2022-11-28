using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.ARUX
{
    public class DirectionIndicator : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] Renderer _target;
        [SerializeField] Transform _realTarget;

        public Renderer Target
        {
            get { return _target;}
            set { _target = value; }
        }

        void Start()
        {
        
        }

        void Update()
        {
            if (_target.isVisible)
            {
                Debug.Log("Object is visible");

            }
            else
            {
                var direction = (_target.transform.position - _player.position).normalized;
                
                var SignedAngle = Vector3.SignedAngle(_player.forward, direction, Vector3.up);
                var angle = Mathf.Abs(SignedAngle);

                if (angle < 135)
                {
                    _realTarget.position = _player.position + direction * 3f;
                }
                else
                {
                    Debug.Log("> 135");

                    var projectDirection = Vector3.ProjectOnPlane(direction, _player.forward);

                    Debug.Log(projectDirection);

                    projectDirection = projectDirection.normalized * 1.5f * 1.632f;

                    var finalDirection = -_player.forward * 1.5f + projectDirection;

                    _realTarget.position = _player.position + finalDirection;
                }

                transform.LookAt(_realTarget.transform.position);

                Debug.Log("Object is no longer visible");
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}
