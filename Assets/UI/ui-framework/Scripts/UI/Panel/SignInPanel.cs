using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.HoloKit.App.UI
{
    public class SignInPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/SignInPanel";
        public SignInPanel() : base(new UIType(_path)) { }


        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                var panel = new HamburgerPanel();
                PanelManager.Push(panel);
            });

            UITool.GetOrAddComponentInChildren<Button>("SignInButton").onClick.AddListener(() =>
            {
                PanelManager.Pop();
                PanelManager.Pop();
                PanelManager.Pop();
                var panel = new StartPanel();
                PanelManager.Push(panel);
            });
        }

    }
}