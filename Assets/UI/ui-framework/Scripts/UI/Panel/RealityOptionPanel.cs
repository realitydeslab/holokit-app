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
                //GameRoot.Instance.SceneSystem.SetScene(new ScreenARMainScene());
                var newPanel = new ScreenARModePanel();
                PanelManager.Push(newPanel);

                var samup = newPanel.UITool.GetOrAddComponent<ScreenARModeUIPanel>();
                samup.SetState(ScreenARModeUIPanel.State.idle);

            });
            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");

                var newPanel = new ScreenARModePanel();
                PanelManager.Push(newPanel);
                var samup = newPanel.UITool.GetOrAddComponent<ScreenARModeUIPanel>();
                samup.SetState(ScreenARModeUIPanel.State.scanning);
            });
        }
    }
}

