using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class ObjectPackagePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ObjectPackagePanel";
        public ObjectPackagePanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("HamburgerButton is clicked.");
                PanelManager.Pop();
            });


            List<GameObject> enterButtons = UITool.FindChildrenGameObject("EnterDetailButton");

            foreach (var button in enterButtons)
            {
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // here we do onclick event of this button
                    Debug.Log("EnterDetailButton is clicked.");
                    var panel = new ObjectDetailPanel();
                    //if(panel.UITool.GetOrAddComponent<ObjectDetailUIPanel>() == null)
                    //{
                    //    Debug.Log("not found!!!!");
                    //}
                    //if (panel.UITool.GetOrAddComponent<ObjectDetailUIPanel>().metaObject == null)
                    //{
                    //    Debug.Log("not found 2 !!!!");
                    //}
                    //if (button.GetComponent<ObjectPackageObjectButtonDescription>() == null)
                    //{
                    //    Debug.Log("not found 3 !!!!");
                    //}
                    PanelManager.Push(panel);
                    panel.UITool.GetOrAddComponent<ObjectDetailUIPanel>().metaObject = button.GetComponent<ObjectPackageObjectButtonDescription>().metaObject;

                });
            }


        }

        public void CustomFunction(Button button, string PanelName)
        {
            button.onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("HamburgerButton is clicked.");
                //var panel = new SpectatorOpenComfirmPanel();
                //PanelManager.Push(panel);
            });
        }
    }
}