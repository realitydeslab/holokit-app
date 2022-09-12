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

        HomeUIPanel _hup;

        public override void OnOpen()
        {
            _hup = UITool.GetOrAddComponent<HomeUIPanel>();
            _hup.realityThumbnailContainer.clickEvent += EnterRealityDetailPanel;


            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                _hup.OthersPanelUILayout();
                var panel = new HamburgerPanel();
                PanelManager.Push(panel);
                _hup.OthersPanelUILayout();
            });

            UITool.GetOrAddComponentInChildren<Button>("EnterButton").onClick.AddListener(() =>
            {
                EnterRealityDetailPanel();
            });

            //UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            //{

            //});
        }

        void EnterRealityDetailPanel()
        {
            Debug.Log("EnterRealityDetailPanel");
            var newPanel = new RealityDetailPanel();
            PanelManager.Push(newPanel);
            var newPanelUI = newPanel.UITool.GetOrAddComponent<RealityDetailUIPanel>();
            newPanelUI.reality = _hup.realityCollection.realities[_hup.CurrentIndex];
            newPanelUI.UpdateInformation();

            _hup.OthersPanelUILayout();
        }

        public override void OnClose()
        {
            _hup.realityThumbnailContainer.clickEvent -= EnterRealityDetailPanel;
            Debug.Log("HomePage Exit");
        }
    }
}