using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARScanQRPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARScanQRPanel";

        public ScreenARScanQRPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");
                PanelManager.Pop();
            });

        }
    }
}
