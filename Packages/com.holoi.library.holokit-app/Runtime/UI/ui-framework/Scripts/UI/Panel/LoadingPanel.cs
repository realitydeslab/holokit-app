using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class LoadingPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/LoadingPanel";
        public LoadingPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("DebugButton").onClick.AddListener(() =>
            {
                // yc todo:
                if (true)
                {
                    var panel = new StartPanel();
                    PanelManager.Push(panel);
                }
                else
                {
                    var panel = new PermissionPanel();
                    PanelManager.Push(panel);
                }
            });
        }

    }
}