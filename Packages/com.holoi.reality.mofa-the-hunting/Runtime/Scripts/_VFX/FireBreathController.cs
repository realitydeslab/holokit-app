using UnityEngine;
using UnityEngine.VFX;

public class FireBreathController : MonoBehaviour
{
    [HideInInspector] public bool IsFollow = true; // fireball follow at first and then unfollow and shoot
                                                    // firebreath always follow

    public Transform followPoint;

    [SerializeField] VisualEffect _breathVFX;


    void Start()
    {
        if (IsFollow)
        {
            if (followPoint != null)
            {
                transform.rotation = followPoint.rotation;
                transform.position = followPoint.position;
            }
        }
        else
        {
            // do nothing
        }
    }

    void Update()
    {
        if (IsFollow)
        {
            if (followPoint != null)
            {
                transform.rotation = followPoint.rotation;
                transform.position = followPoint.position;
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
