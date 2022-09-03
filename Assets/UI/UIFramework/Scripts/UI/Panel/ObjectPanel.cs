using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ObjectPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ObjectPanel";
        public ObjectPanel() : base(new UIType(_path)) { }

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
}