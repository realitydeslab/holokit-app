using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class HomeUI : MonoBehaviour
{
    public RealityCollection RealityListData;

    protected virtual void OnHomeUI()
    {
    }

    public virtual void Aweak()
    {
        OnHomeUI();
    }

    public virtual void Update()
    {
        if (Application.isEditor)
        {
            OnHomeUI();
        }
    }
}


