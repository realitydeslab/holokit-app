using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HomeUIPanel : HomeUI
{
    protected override void OnHomeUI()
    {
        base.OnHomeUI();

        for (int i = 0; i < RealityListData.realityCollection.Count; i++)
        {
            var go =  RealityListData.realityCollection[i];
        }
    }
}


