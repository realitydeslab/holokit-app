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

        HomeUIPanel _hup;

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                var panel = new HamburgerPanel();
                PanelManager.Push(panel);
            });

            _hup = UITool.GetOrAddComponent<HomeUIPanel>();
            _hup.realityThumbnailContainer.clickEvent += EnterRealityDetailPanel;

            //UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            //{

            //});
        }

        void EnterRealityDetailPanel()
        {
            EnterRealityEvents?.Invoke();
            //UITool.GetOrAddComponent<HomeUIPanel>().SwitchToRealityDetailPageLayout();
            var newPanel = new RealityDetailPanel();
            PanelManager.Push(newPanel);
            var newPanelUI = newPanel.UITool.GetOrAddComponent<RealityDetailUIPanel>();
            newPanelUI.reality = _hup.realityCollection.realities[_hup.CurrentIndex];
            newPanelUI.UpdateInformation();
        }

        public override void OnExit()
        {
            _hup.realityThumbnailContainer.clickEvent -= EnterRealityDetailPanel;

            Debug.Log("HomePage Exit");
        }
    }
}