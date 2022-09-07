using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    public class RealityDetailPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/RealityDetailPanel";
        public RealityDetailPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");

                HomeUIPanel HUP = Transform.FindObjectOfType<HomeUIPanel>();
                if (HUP != null)
                {
                    HUP.SwitchToHomePageLayout();
                }
                else
                {
                    Debug.LogError("Not Found HUP");
                }

                PanelManager.Pop();
            });
            UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("PlayButton is clicked.");

                var panel = new RealityOptionPanel();
                PanelManager.Push(panel);

                // filter all object list with tag:
                MetaObjectCollectionList MOCL = new MetaObjectCollectionList();

                // filter all ava list with tag:
                MetaAvatarCollectionList MACL = new MetaAvatarCollectionList();

                panel.UITool.GetOrAddComponent<RealityOptionUIPanel>().metaObjectCollections = MOCL.list;
                panel.UITool.GetOrAddComponent<RealityOptionUIPanel>().metaAvatarCollections = MACL.list;
            });
        }
    }
}