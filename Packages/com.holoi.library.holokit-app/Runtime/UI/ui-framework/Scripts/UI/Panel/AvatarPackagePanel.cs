using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class AvatarPackagePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/AvatarPackagePanel";
        public AvatarPackagePanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("HamburgerButton is clicked.");
                PanelManager.Pop();
            });

            List<GameObject> enterButtons = UITool.FindGameObjectsInChildren("EnterDetailButton");

            foreach (var button in enterButtons)
            {
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // here we do onclick event of this button
                    Debug.Log("EnterDetailButton is clicked.");

                    var panel = new AvatarDetailPanel();
                    PanelManager.Push(panel);
                    panel.UITool.GetOrAddComponent<ObjectDetailUIPanel>().metaAvatar = button.GetComponent<CollectionContainer>().activeAvatar;
                });
            }
        }
    }
}