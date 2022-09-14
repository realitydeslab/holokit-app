using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class MainPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/MainPanel";
        public MainPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            //UITool.GetOrAddComponentInChildren<Button>("SettingButton").onClick.AddListener(() =>
            //{
            //    // here we do onclick event of this button
            //    Debug.Log("SettingButton is clicked.");

            //    PanelManager.Push(new SettingPanel());
            //});

            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");
                GameRoot.Instance.SceneSystem.SetScene(new StartScene());
            });
        }
    }
}