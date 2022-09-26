using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class StartPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/StartPanel";
        public StartPanel() : base(new UIType(_path)) { }

        HomeUIPanel _homeUIPanel;

        public override void OnOpen()
        {
            _homeUIPanel = UITool.GetOrAddComponent<HomeUIPanel>();
            _homeUIPanel.realityThumbnailContainer.OnThumbnailClickedEvent += EnterRealityDetailPanel;


            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                var panel = new HamburgerPanel();
                PanelManager.Push(panel);
            });
        }

        void EnterRealityDetailPanel()
        {
            var currentReality = _homeUIPanel.realityCollection.realities[_homeUIPanel.CurrentIndex];
            // set CurrentReality to this selected reality
            HoloKitApp.Instance.CurrentReality = currentReality;

            var realityDetailPanel = new RealityDetailPanel();

            PanelManager.Push(realityDetailPanel);
        }

        public override void OnClose()
        {
            _homeUIPanel.realityThumbnailContainer.OnThumbnailClickedEvent -= EnterRealityDetailPanel;
            Debug.Log("HomePage Exit");
        }
    }
}