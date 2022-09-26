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
                while(PanelManager.GetActivePanel().UIType.Name != "ScreenARModePanel")
                {
                    PanelManager.Pop();
                }
                PanelManager.OnStoppedSharingReality?.Invoke();
            });

            RealityManager.OnSpectatorDeviceListUpdated += UpdateConnectedDeviceNameUI;
        }

        public void UpdateConnectedDeviceNameUI(List<string> names)
        {
            PanelManager.Instance.GetActivePanel().UITool.GetOrAddComponent<ScreenARShareQRUIPanel>().UpdateConnectedDeviceUI(names);
        }

        public override void OnClose()
        {
            base.OnClose();
            RealityManager.OnSpectatorDeviceListUpdated -= UpdateConnectedDeviceNameUI;

        }
    }
}
