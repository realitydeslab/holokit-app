using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App.UI
{
    public class SignInUIPnael : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Transform _idleContainer;
        [SerializeField] Transform _successContainer;

        public enum State
        {
            notSignIn = 0,
            signedIn = 1
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.notSignIn:
                    _idleContainer.gameObject.SetActive(true);
                    _successContainer.gameObject.SetActive(false);
                    break;
                case State.signedIn:
                    _idleContainer.gameObject.SetActive(false);
                    _successContainer.gameObject.SetActive(true);
                    break;
            }
        }
    }
}