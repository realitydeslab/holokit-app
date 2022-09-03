using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class ItemPackageUI : MonoBehaviour
{
    public List<MetaObjectCollection> CollectionLists;

    protected virtual void OnItemPackageUIAweak()
    {
    }

    public virtual void Aweak()
    {
        Debug.Log("ItemPackageUI Aweak");
    }
}


