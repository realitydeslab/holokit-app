using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class SpectatorOpenComfirmPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/SpectatorOpenComfirmPanel";

        public SpectatorOpenComfirmPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ShareButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ShareButton is clicked.");
                var panel = new SpectatorOpenComfirmPanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

        }
    }
}
