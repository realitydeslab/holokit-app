using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class RealityOptionUI : MonoBehaviour
{
    public List<MetaObjectCollection> metaObjectCollections;
    public List<MetaAvatarCollection> metaAvatarCollections;

    protected virtual void OnUIAweak()
    {
    }

    public virtual void Aweak()
    {
        Debug.Log("UI Aweak");
    }
}


