using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARShareQRPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARShareQRPanel";

        public ScreenARShareQRPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();

                PanelManager.OnStoppedSharingReality?.Invoke();
            });

            // debug for scanning -> check mark
            UITool.GetOrAddComponentInChildren<Button>("Fake Connect").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("CheckMarkButton is clicked.");
                // create a debug list:
                List<string> names = new List<string>();
                names.Add("cool guy's iphone 14");
                UpdateConnectedDeviceNameUI(names);
            });

            UITool.GetOrAddComponentInChildren<Button>("Check Mark").onClick.AddListener(() =>
            {
                //PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.checkMark);
                // here we do onclick event of this button
                Debug.Log("DebugButton is clicked.");
                var panel = new ScreenARCheckTheMarkPanel();
                PanelManager.Push(panel, false);
            });
        }

        public void UpdateConnectedDeviceNameUI(List<string> names)
        {
            PanelManager.Instance.GetActivePanel().UITool.GetOrAddComponent<ScreenARShareQRUIPanel>().connectedDeviceNames = names;
            PanelManager.Instance.GetActivePanel().UITool.GetOrAddComponent<ScreenARShareQRUIPanel>().UpdateConnectedDeviceUI();
        }
    }
}
