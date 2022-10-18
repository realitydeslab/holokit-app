using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEventWithLayer : MonoBehaviour
{
    public enum Activation
    {
        OnEnter,
        OnExit,
        OnStay
    }
    public LayerMask selectedLayer;
    public Activation activation = Activation.OnEnter;
    public UnityEvent Event;

    private void OnTriggerEnter(Collider other)
    {
        if (activation != Activation.OnEnter) return;
        if (other.gameObject.layer != selectedLayer.value) return;

        Event?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (activation != Activation.OnExit) return;
        if (other.gameObject.layer != selectedLayer.value) return;

        Event?.Invoke();

    }

    private void OnTriggerStay(Collider other)
    {
        if (activation != Activation.OnStay) return;
        if (other.gameObject.layer != selectedLayer.value) return;

        Event?.Invoke();

    }
}
