﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetStartPanel : BasePanel
{
    static readonly string _path = "Prefabs/UI/Panels/GetStartPanel";
    public GetStartPanel() : base(new UIType(_path)) { }

    public override void OnEnter()
    {
        UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");
            PanelManager.Pop();
        });

    }
}