using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DefaultCoreVisualController : MonoBehaviour
{
    public List<MeshRenderer> CoreVisualGroups;
    public  List<VisualEffect> ExplodeVisualGroups;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit(int index)
    {
        CoreVisualGroups[index].enabled = false;
        ExplodeVisualGroups[index].SendEvent("OnExplode");
    }


}
