using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARShareQRPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARShareQRPanel";

        public ScreenARShareQRPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            // debug for scanning -> check mark
            UITool.GetOrAddComponentInChildren<Button>("CheckMarkButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("CheckMarkButton is clicked.");

                var panel = new ScreenARCheckTheMarkPanel();
                PanelManager.Push(panel);
            });

        }
    }
}
