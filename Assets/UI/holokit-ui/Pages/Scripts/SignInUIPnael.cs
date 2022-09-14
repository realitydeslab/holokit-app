using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class SignInUIPnael : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Transform _idleContainer;
        [SerializeField] Transform _successContainer;

        public enum State
        {
            idle,
            success
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.idle:
                    _idleContainer.gameObject.SetActive(true);
                    _successContainer.gameObject.SetActive(false);
                    break;
                case State.success:
                    _idleContainer.gameObject.SetActive(false);
                    _successContainer.gameObject.SetActive(true);
                    break;
            }
        }
    }
}