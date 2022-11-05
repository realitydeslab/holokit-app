using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.ARUX
{
    public class RotateOverTime : MonoBehaviour
    {
        public float RotateSpeed;
        public Vector3 RotateVector;

        Vector3 _angle;
        //Quaternion _rotationOffset;
        Vector3 _positionOffset;

        Transform child;

        [SerializeField] bool _isSineWave = false;
        [SerializeField] float _amp = 1f;
        [SerializeField] float _fre = 1f;

        void Start()
        {
            child = transform.GetChild(0);
            //_rotationOffset = child.rotation;
            _positionOffset = child.position;
        }

        void Update()
        {
            _angle += RotateVector * RotateSpeed * Time.deltaTime;

            if (_isSineWave)
            {
                var x = child.position.x;
                var y = _positionOffset.y + (_amp * Mathf.Sin(Time.time * Mathf.PI * _fre));
                var z = child.position.z;

                child.position = new Vector3(x, y, z);
            }

            //transform.rotation = _rotationOffset * Quaternion.Euler(_angle.x, _angle.y, _angle.z);
            transform.rotation = Quaternion.Euler(_angle);
        }
    }
}
