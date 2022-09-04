using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ObjectPackagePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ObjectPackagePanel";
        public ObjectPackagePanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("HamburgerButton is clicked.");
                PanelManager.Pop();
            });
        }

        public void CustomFunction(Button button, string PanelName)
        {
            button.onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("HamburgerButton is clicked.");
                //var panel = new SpectatorOpenComfirmPanel();
                //PanelManager.Push(panel);
            });
        }
    }
}