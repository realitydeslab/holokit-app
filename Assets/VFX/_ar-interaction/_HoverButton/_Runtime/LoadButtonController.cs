using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

namespace HolokitApp.ARUI
{
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class LoadButtonController : MonoBehaviour
    {
        public UnityEvent OnLoadedEvents;
        public event Action OnDisableEvent;

        //animator
        Animator _animator;
        MeshRenderer _mr;
        Material _mat;

        // loading
        float _process;
        bool _triggered;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mr = GetComponent<MeshRenderer>();
            _mat = _mr.material;
        }
        private void OnEnable()
        {
            // reset all 
            _animator.Rebind();
            _animator.Update(0f);
            _triggered = false;
            _process = 0;
        }
        private void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        void Update()
        {
            if (!_triggered)
            {
                _process = GetComponent<HoverableObject>().Process;
            }

            if (_process == 1 && !_triggered)
            {
                StartCoroutine(PlayDieAndDisable());
                OnLoadedEvents?.Invoke();
                _triggered = true;
            }
            else
            {
                _mat.SetFloat("_Process", _process);
            }
        }

        IEnumerator PlayDieAndDisable()
        {
            PlayDie();
            yield return new WaitForSecondsRealtime(1f);
            gameObject.SetActive(false);
        }
        public void SetTexture(Texture2D tex)
        {
            _mat.SetTexture("_Texture", tex);
        }

        void PlayDie()
        {
            _animator.SetTrigger("Die");

        }
    }
}