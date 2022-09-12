using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.HoloKit.App.UI
{
    public class StartPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/StartPanel";
        public StartPanel() : base(new UIType(_path)) { }

        public UnityEvent EnterRealityEvents;

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                var panel = new HamburgerPanel();
                PanelManager.Push(panel);
            });

            UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            {
                EnterRealityEvents?.Invoke();
                //GameRoot.Instance.SceneSystem.SetScene(new MainScene());
                //UITool.GetOrAddComponent<HomeUIPanel>().SwitchToRealityDetailPageLayout();
                var panel = new RealityDetailPanel();
                panel.UITool.GetOrAddComponent<RealityDetailUIPanel>().reality = UITool.GetOrAddComponent<HomeUIPanel>().realityCollection.Realities[UITool.GetOrAddComponent<HomeUIPanel>().CurrentIndex];
                PanelManager.Push(panel);
            });
        }

        public override void OnExit()
        {
            Debug.Log("HomePage Exit");
        }
    }
}