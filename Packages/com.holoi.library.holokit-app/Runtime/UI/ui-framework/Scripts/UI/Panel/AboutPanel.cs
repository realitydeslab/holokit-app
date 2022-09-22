﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class AboutPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/AboutPanel";
        public AboutPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.FindGameObjectInChindren("BackButton").GetComponent<Button>().onClick.AddListener(() =>
            { 
                // here we do onclick event of this button
                Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });
        }
    }
}