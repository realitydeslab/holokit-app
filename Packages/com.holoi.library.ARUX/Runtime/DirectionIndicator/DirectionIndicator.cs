using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.ARUX
{
    public class DirectionIndicator : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] Renderer _target;
        [SerializeField] VisualEffect _vfx;
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
        bool _isTriggerOut = false;

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
                //FadeOut();

                //_process += Time.deltaTime;
                //if (_process > _inTime) _process = _inTime;
                //if (_process == _inTime && !_isTriggerIn)
                //{
                //    _isTriggerIn = true;
                //    FadeIn();
                //}
            }
            else
            {
                //FadeIn();
                //_process -= Time.deltaTime;
                //if (_process < _outTime) _process = _outTime;
                //if (_process == _outTime && !_isTriggerOut)
                //{
                //    _isTriggerOut = false;
                //    FadeOut();
                //}


                var direction = (_target.transform.position - _player.position).normalized;

                var SignedAngle = Vector3.SignedAngle(_player.forward, direction, Vector3.up);

                var angle = Mathf.Abs(SignedAngle);


                if (angle < threshold)
                {
                    _realTargetposition = _player.position + direction * _radius;
                }
                else
                {
                    //Debug.Log($"> {threshold}");

                    var projectDirection = Vector3.ProjectOnPlane(direction, _player.forward);

                    projectDirection = projectDirection.normalized * (0.5f) * (Mathf.Pow(3, 0.5f) * _radius);

                    var cc = -_player.forward * (0.5f * _radius);

                    var finalDirection = cc + projectDirection;

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
            Debug.Log("FadeIn");
            _vfx.enabled = true;
        }

        public void FadeOut()
        {
            Debug.Log("FadeOut");

            _vfx.enabled = false;
        }

        public void ToVisible()
        {
            _isVisible = true;
            FadeOut();
        }

        public void ToInVisible()
        {
            _isVisible = false;
            FadeIn();


        }
    }
}
