using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class HomeUI : MonoBehaviour
{
    public RealityList realityCollection;

    protected virtual void OnHomeUIAweak()
    {
    }

    public virtual void Aweak()
    {
        Debug.Log("HomeUI Aweak");
    }
}


