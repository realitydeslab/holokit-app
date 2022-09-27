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
                //Debug.Log("Optional BackButton is clicked.");
                PanelManager.Pop();
            });
            UITool.GetOrAddComponentInChildren<Button>("EnterScreenARButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("EnterScreenARButton is clicked.");

                // enter screen ar scene:
                HoloKitApp.Instance.EnterRealityAsHost();
                var sceneName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().SceneName;

                var scene = new ScreenARMainScene();
                scene._sceneName = sceneName;

                GameRoot.Instance.SceneSystem.SetScene(scene);

                // events invoke:
                PanelManager.OnEnteredRealityAsHost?.Invoke(HoloKitApp.Instance.CurrentReality);
            });
            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");

                HoloKitApp.Instance.JoinRealityAsSpectator();
                var sceneName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().SceneName;

                var scene = new ScreenARMainScene();
                scene._sceneName = sceneName;
                scene.openState = ScreenARMainScene.State.spectator;

                GameRoot.Instance.SceneSystem.SetScene(scene);

                // events invoke:
                PanelManager.OnJoinedRealityAsSpectator?.Invoke(HoloKitApp.Instance.CurrentReality);
            });
        }
    }
}

