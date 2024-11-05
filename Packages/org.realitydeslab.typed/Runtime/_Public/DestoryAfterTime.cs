using UnityEngine;

public class DestoryAfterTime : MonoBehaviour
{
    public int LifeTime = 3;

    private void Start()
    {
        Destroy(this.gameObject, LifeTime);
    }
}
