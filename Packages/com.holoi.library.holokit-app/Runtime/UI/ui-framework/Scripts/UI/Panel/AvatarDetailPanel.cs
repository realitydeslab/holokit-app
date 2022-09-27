using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class AvatarDetailPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/AvatarDetailPanel";
        public AvatarDetailPanel() : base(new UIType(_path)) { }
        public MetaAvatar metaAvatar;

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });


            UITool.GetOrAddComponentInChildren<Button>("OpenSeaButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("OpenSeaButton is clicked.");
                //var panel = new SpectatorOpenComfirmPanel();
                //PanelManager.Push(panel);
            });


            UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("PlayButton is clicked.");
                // open compatiable reality page
                var panel = new CompatiableRealityPanel();
                //panel._metaObject = 
                PanelManager.Push(panel);
            });

        }
    }
}