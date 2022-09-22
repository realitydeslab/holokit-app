using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARCheckTheMarkPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARCheckTheMarkPanel";

        public ScreenARCheckTheMarkPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("CheckedButton").onClick.AddListener(() =>
            {
                Debug.Log("CheckedButton is clicked.");
                while(PanelManager.GetActivePanel().UIType.Name != "ScreenARModePanel")
                {
                    PanelManager.Pop();
                }
                Debug.Log( "now we are in: " + PanelManager.GetActivePanel().UIType.Name);

                // now we should in ScreenArModePanel:
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.@checked);
                PanelManager.OnAlignmentMarkChecked?.Invoke();
            });

            UITool.GetOrAddComponentInChildren<Button>("RescanButton").onClick.AddListener(() =>
            {
                Debug.Log("RescanButton is clicked.");
                while (PanelManager.GetActivePanel().UIType.Name != "ScreenARShareQRPanel")
                {
                    PanelManager.Pop();
                }
                Debug.Log("now we are in: " + PanelManager.GetActivePanel().UIType.Name);

                // now we should in ScreenArShareQRPanel:
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARShareQRUIPanel>().ClearConnectedDeviceUI();
                UIManager.GetPanel("ScreenARModePanel").GetComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.waittingScanned);

                PanelManager.OnRescanQRCode?.Invoke();
            });
        }
    }
}
