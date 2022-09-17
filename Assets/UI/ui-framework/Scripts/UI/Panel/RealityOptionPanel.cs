using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.HoloKitApp.UI
{
    public class RealityOptionPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/RealityOptionPanel";
        public RealityOptionPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("Optional BackButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("EnterScreenARButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("EnterScreenARButton is clicked.");

                //Debug.Log(PanelManager.Instance._panelStack.Count + " panels found.");

                // enter screen ar scene:
                HoloKitApp.Instance.EnterRealityAsHost();
                var sceneName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().SceneName;
                GameRoot.Instance.SceneSystem.SetScene(new ScreenARMainScene());
            });
            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");

                //var newPanel = new ScreenARModePanel();
                //PanelManager.Push(newPanel);
                //var screenARUI = newPanel.UITool.GetOrAddComponent<ScreenARModeUIPanel>();
                //screenARUI.SetState(ScreenARModeUIPanel.State.waitPlayerEnter);

                //var panel = new ScreenARWaitPlayerPanel();
                //PanelManager.Push(panel);

                HoloKitApp.Instance.JoinRealityAsSpectator();
                var sceneName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().SceneName;
                GameRoot.Instance.SceneSystem.SetScene(new ScreenARMainScene());
            });
        }
    }
}

