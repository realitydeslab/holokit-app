using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    public class CompatiableRealityPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/CompatiableRealityPanel";
        public CompatiableRealityPanel() : base(new UIType(_path)) { }
        public MetaObject _metaObject;

        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });

            // create button by script object:
            InitialUI();

            // add button listenser:
            //UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            //{
            //    // here we do onclick event of this button
            //    Debug.Log("HamburgerButton is clicked.");
            //    var panel = new SpectatorOpenComfirmPanel();
            //    PanelManager.Push(panel);
            //});
        }

        void InitialUI()
        {

        }
    }
}