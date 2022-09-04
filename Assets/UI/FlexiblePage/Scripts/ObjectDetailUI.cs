using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class ObjectDetailUI : MonoBehaviour
{
    public MetaObject metaObject;

    protected virtual void OnObjectDetailUIAweak()
    {
    }

    public virtual void Aweak()
    {
        Debug.Log("ObjectDetailUI Aweak");
    }
}


