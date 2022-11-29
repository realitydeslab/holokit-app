using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.ARUX
{
    public class DirectionIndicator : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] Renderer _target;
        DirectionTarget _dT;
        [Tooltip("for debug")]
        //[SerializeField] Transform _realTarget;

        public Renderer Target
        {
            get { return _target; }
            set { _target = value; }
        }

        float _radius = 3f;

        float threshold = 120f;

        Vector3 _realTargetposition;

        float _inTime = 2f;
        bool _isTriggerIn = false;
        float _outTime = 0.25f;

        float _process = 0;

        bool _isVisible = false;


        void Start()
        {
            _dT = _target.GetComponent<DirectionTarget>();

            _dT.OnVisibleEvent += ToVisible;
            _dT.OnInVisibleEvent += ToInVisible;
        }

        void Update()
        {
            if (_isVisible)
            {
                _process += Time.deltaTime;
                if (_process > _inTime) _process = _inTime;
                if(_process == _inTime && !_isTriggerIn)
                {
                    FadeIn();
                }

            }
            else
            {
                _process -= Time.deltaTime;
                if (_process < _outTime) _process = _outTime;


                var direction = (_target.transform.position - _player.position).normalized;

                var SignedAngle = Vector3.SignedAngle(_player.forward, direction, Vector3.up);
                var angle = Mathf.Abs(SignedAngle);


                if (angle < threshold)
                {
                    _realTargetposition = _player.position + direction * _radius;
                }
                else
                {
                    Debug.Log($"> {threshold}");

                    var projectDirection = Vector3.ProjectOnPlane(direction, _player.forward);

                    //Debug.Log(projectDirection);

                    projectDirection = projectDirection.normalized * (0.5f) * (Mathf.Pow(3, 0.5f) * _radius);
                    //projectDirection = projectDirection.normalized * (Mathf.Sin(threshold - 90f) * _radius) * (Mathf.Cos(threshold - 90f) * _radius);
                    Debug.Log("long: " + projectDirection);
                    var cc = -_player.forward * (0.5f * _radius);
                    Debug.Log("short: " + cc);



                    var finalDirection = cc + projectDirection;

                    Debug.Log("final: " + cc);


                    _realTargetposition = _player.position + finalDirection;
                }

                transform.LookAt(_realTargetposition);
            }
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(transform.position, _radius);
        //}

        public void FadeIn()
        {

        }

        public void FadeOut()
        {

        }

        public void ToVisible()
        {
            _isVisible = true;
        }

        public void ToInVisible()
        {
            _isVisible = false;

        }
    }
}
