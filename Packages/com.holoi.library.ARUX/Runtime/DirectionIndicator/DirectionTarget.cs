using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holoi.Library.ARUX
{
    public class DirectionTarget : MonoBehaviour
    {
        public event Action OnVisibleEvent;
        public event Action OnInVisibleEvent;

        void Start()
        {

        }

        private void OnBecameVisible()
        {
            Debug.Log($"{gameObject.name} become visible.");
            OnVisibleEvent?.Invoke();
        }

        private void OnBecameInvisible()
        {
            Debug.Log($"{gameObject.name} become invisible.");

            OnInVisibleEvent?.Invoke();
        }
    }
}
