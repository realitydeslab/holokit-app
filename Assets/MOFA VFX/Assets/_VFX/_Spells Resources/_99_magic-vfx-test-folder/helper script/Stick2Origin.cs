
using UnityEngine;

public class Stick2Origin : MonoBehaviour
{
    Vector3 originPos;
    void Start()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = originPos;
    }
}
