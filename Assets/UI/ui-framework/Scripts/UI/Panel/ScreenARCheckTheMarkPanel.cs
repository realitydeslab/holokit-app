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
                PanelManager.Pop();

                //Debug.Log(PanelManager.GetActivePanel().UIType.Name);
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.scanned);
            });

            UITool.GetOrAddComponentInChildren<Button>("RescanButton").onClick.AddListener(() =>
            {
                Debug.Log("RescanButton is clicked.");
                PanelManager.Pop();

                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.scanning);
            });

        }
    }
}
