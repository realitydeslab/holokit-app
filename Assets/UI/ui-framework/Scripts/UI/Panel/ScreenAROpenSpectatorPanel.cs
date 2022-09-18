using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenAROpenSpectatorPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenAROpenSpectatorPanel";

        public ScreenAROpenSpectatorPanel() : base(new UIType(_path)) { }


        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ShareButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ShareButton is clicked.");
                var openConfirmPanel = PanelManager.GetActivePanel();
                openConfirmPanel.UITool.ActivePanelGO.SetActive(false);

                var panel = new ScreenARShareQRPanel();
                PanelManager.Push(panel, false);
                PanelManager.OnStartedSharingReality?.Invoke();
                UIManager.GetPanel("ScreenARModePanel").GetComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.waittingScanned);
            });

            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
                PanelManager.GetActivePanel().UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.idle);
            });

        }
    }
}
