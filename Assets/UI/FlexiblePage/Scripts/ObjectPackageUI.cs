using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class ObjectPackageUI : MonoBehaviour
{
    public List<MetaObjectCollection> CollectionLists;

    protected virtual void OnObjectPackageUIAweak()
    {
    }

    public virtual void Aweak()
    {
        Debug.Log("ItemPackageUI Aweak");
    }
}


