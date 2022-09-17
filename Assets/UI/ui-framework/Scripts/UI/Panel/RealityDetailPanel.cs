using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class RealityDetailPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/RealityDetailPanel";
        public RealityDetailPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
                PanelManager.Pop();
            });

            UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("PlayButton is clicked.");

                RealityDetailUIPanel realityDetailUI = PanelManager.Instance.GetActivePanel().UITool.GetOrAddComponent<RealityDetailUIPanel>();

                var realityOptionPanel = new RealityOptionPanel();
                PanelManager.Push(realityOptionPanel);

                var realityOptionUI = realityOptionPanel.UITool.GetOrAddComponent<RealityOptionUIPanel>();
                realityOptionUI.realityMetaObjectCollections = realityDetailUI.realityMetaObjectCollectionList;
                realityOptionUI.realityMetaAvatarCollections = realityDetailUI.realityMetaAvatarCollectionList;
                realityOptionUI.SetUIInfo();
                realityOptionUI.SetUIButtons();
            });
        }
    }
}