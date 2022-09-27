using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class TestSceneHomePagePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/TestSceneHomePagePanel";
        public TestSceneHomePagePanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            var buttonList = UITool.GetComponentsInChildren<Button>("RealityDetailButton");

            foreach (var button in buttonList)
            {
                button.onClick.AddListener(() =>
                {

                });
            }
        }
    }
}