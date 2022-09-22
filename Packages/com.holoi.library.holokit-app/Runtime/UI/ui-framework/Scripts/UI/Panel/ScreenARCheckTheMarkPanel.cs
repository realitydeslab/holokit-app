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

            // this panel is only for client to confirm the connection
            UITool.GetOrAddComponentInChildren<Button>("RescanButton").onClick.AddListener(() =>
            {
                Debug.Log("RescanButton is clicked.");

                while (PanelManager.GetActivePanel().UIType.Name != "ScreenARModePanel")
                {
                    //Debug.Log($"pop {PanelManager.GetActivePanel().UIType.Name} panel");
                    PanelManager.Pop();
                }
                Debug.Log("now we are in: " + PanelManager.GetActivePanel().UIType.Name);

                // now we should in ScreenArShareQRPanel:
                //PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARShareQRUIPanel>().ClearConnectedDeviceUI();
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.scanningQRcode);

                PanelManager.OnRescanQRCode?.Invoke();
            });
        }
    }
}
