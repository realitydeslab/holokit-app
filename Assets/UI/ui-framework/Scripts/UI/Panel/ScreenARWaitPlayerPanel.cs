using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARWaitPlayerPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARWaitPlayerPanel";

        public ScreenARWaitPlayerPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ShareButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ShareButton is clicked.");
                var panel = new ScreenARShareQRPanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

        }
    }
}
