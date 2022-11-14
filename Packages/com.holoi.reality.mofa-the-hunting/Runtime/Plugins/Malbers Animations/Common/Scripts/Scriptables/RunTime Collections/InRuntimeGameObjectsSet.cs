using MalbersAnimations.Events;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Scriptables
{
    /// <summary>
    /// Monitors if a gameObject was added or removed from a set
    /// </summary>
    [AddComponentMenu("Malbers/Runtime Vars/In Runtime GameObjects Set")]

    public class InRuntimeGameObjectsSet : MonoBehaviour
    {
        [RequiredField] public RuntimeGameObjects Collection;

        public UnityEvent AddedToSet = new UnityEvent();
        public UnityEvent RemovedFromSet = new UnityEvent();

        private void OnEnable()
        {
            if (Collection != null)
            {
                Collection.OnItemRemoved.AddListener(OnItemAdded);
                Collection.OnItemRemoved.AddListener(OnItemRemoved);
            }

        }
        private void OnDisable()
        {
            if (Collection != null)
            {
                Collection.OnItemRemoved.RemoveListener(OnItemAdded);
                Collection.OnItemRemoved.RemoveListener(OnItemRemoved);
            }
        }

        private void OnItemRemoved(GameObject arg0)
        {
            if (arg0 == gameObject) RemovedFromSet.Invoke();
        }

        private void OnItemAdded(GameObject arg0)
        {
            if (arg0 == gameObject) AddedToSet.Invoke();
        }
    }
}