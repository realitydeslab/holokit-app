using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class AboutPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/AboutPanel";
        public AboutPanel() : base(new UIType(_path)) { }

        public override void OnEnter()
        {
            UITool.FindTransform("PageHeader-backbutton/BackButton").GetComponent<Button>().onClick.AddListener(() =>
            { 
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");
            PanelManager.Pop();
            });

            //UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            //{
            //// here we do onclick event of this button
            //Debug.Log("BackButton is clicked.");
            //    PanelManager.Pop();
            //});

        }
    }
}
