using UnityEngine;
using UnityEngine.VFX;

public class FireBreathController : MonoBehaviour
{
    [HideInInspector] public bool IsFollow = true;
    public Transform _followPoint;

    [SerializeField] VisualEffect _breathVFX;

    void Start()
    {
        
    }

    void Update()
    {
        if (IsFollow)
        {
            if (_followPoint != null)
            {
                transform.rotation = _followPoint.rotation;
                transform.position = _followPoint.position;
            }
        }
        else
        {
            // do nothing
        }

    }

    public void OnAttack()
    {
        _breathVFX.SendEvent("OnAttack");
    }
}
