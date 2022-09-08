using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARCheckTheMarkPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARCheckTheMarkPanel";

        public ScreenARCheckTheMarkPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("CheckedButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("CheckedButton is clicked.");
                PanelManager.Pop();

                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.scanned);
            });

            UITool.GetOrAddComponentInChildren<Button>("RescanButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("RescanButton is clicked.");
                PanelManager.Pop();

                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.scanning);

            });

        }
    }
}
