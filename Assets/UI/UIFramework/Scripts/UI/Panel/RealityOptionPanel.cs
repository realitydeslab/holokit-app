using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class RealityOptionPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/RealityOptionPanel";
        public RealityOptionPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });
            UITool.GetOrAddComponentInChildren<Button>("EnterScreenARButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("EnterScreenARButton is clicked.");

                // enter screen ar scene:
                GameRoot.Instance.SceneSystem.SetScene(new ScreenARMainScene());
            });
            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");

                var panel = new ScreenARScanQRPanel();
                PanelManager.Push(panel);
            });
        }
    }
}

