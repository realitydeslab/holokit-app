using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class SignInPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/SignInPanel";
        public SignInPanel() : base(new UIType(_path)) { }


        public override void OnOpen()
        {
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