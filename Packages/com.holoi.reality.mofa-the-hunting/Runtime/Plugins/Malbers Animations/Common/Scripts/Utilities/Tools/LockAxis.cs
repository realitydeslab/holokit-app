using UnityEngine;

[AddComponentMenu("Malbers/Utilities/Tools/Lock Axis")]

public class LockAxis : MonoBehaviour
{
    public bool LockX = true;
    public bool LockY = false;
    public bool LockZ = false;

    public Vector3 LockOffset;

    void Update()
    {
        Vector3 pos = transform.position;

        if (LockX) pos.x = LockOffset.x;
        if (LockY) pos.y = LockOffset.y;
        if (LockZ) pos.z = LockOffset.z; 

        transform.position = pos;
    }
}
