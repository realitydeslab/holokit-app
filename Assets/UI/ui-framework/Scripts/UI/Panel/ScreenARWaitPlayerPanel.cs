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
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("DebugButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("DebugButton is clicked.");

                PlayerEnteredReality();
            });
        }

        public void PlayerEnteredReality()
        {
            PanelManager.Pop();
            var panel = PanelManager.Instance.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().state = ScreenARModeUIPanel.State.waittingScanned;
        }
    }
}
