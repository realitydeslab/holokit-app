using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARScanQRPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARScanQRPanel";

        public ScreenARScanQRPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("OpenSpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("CheckMarkButton").onClick.AddListener(() =>
            {
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.checkMark);
                // here we do onclick event of this button
                Debug.Log("DebugButton is clicked.");
                var panel = new ScreenARCheckTheMarkPanel();
                PanelManager.Push(panel);
            });

        }
    }
}
