using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.HoloKit.App.UI
{
    public class StARModePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/StARModePanel";
        public StARModePanel() : base(new UIType(_path)) { }


        public override void OnEnter()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
            });
        }

        public override void OnExit()
        {
            Debug.Log("StARMode Exit");
        }
    }
}