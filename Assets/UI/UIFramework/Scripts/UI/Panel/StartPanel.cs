using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramwork;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
public class StartPanel : BasePanel
{
    static readonly string _path = "Prefabs/UI/Panels/StartPanel"; 
    public StartPanel(): base(new UIType(_path)) { }

    public override void OnEnter()
    {
        UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
        {
            
            // here we do onclick event of this button
            Debug.Log("HamburgerButton is clicked.");

            var panel = new HamburgerPanel();
            PanelManager.Push(panel);
        });

        UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("PlayButton is clicked.");
            //GameRoot.Instance.SceneSystem.SetScene(new MainScene());
            var panel = new RealityPanel();
            PanelManager.Push(panel);
        });
    }
}
