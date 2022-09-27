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
            var loadingUIPanel = UITool.GetOrAddComponent<LoadingUIPanel>();

            loadingUIPanel.OnAnimationTrigger += LoadingDone;

        }

        public void LoadingDone()
        {
            if (HoloKitAppPermissionsManager.MandatoryPermissionsGranted())
            {
                PanelManager.Pop();
                PanelManager.Push(new StartPanel());
            }
            else
            {
                var panel = new PermissionPanel();
                PanelManager.Push(panel);
            }
        }
    }
}