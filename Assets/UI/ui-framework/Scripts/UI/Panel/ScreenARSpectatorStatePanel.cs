using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARSpectatorStatePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARSpectatorStatePanel";

        public ScreenARSpectatorStatePanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("DisConnectButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("CheckedButton is clicked.");
                PanelManager.Pop();

                // where should this action go? ask amber;
            });
        }
    }
}
