using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
public class StartPanel : BasePanel
{
    static readonly string _path = "Prefabs/UI/Panels/StartPanel"; 
    public StartPanel(): base(new UIType(_path)) { }

    public UnityEvent EnterRealityEvents;

    public override void OnEnter()
    {
        UITool.GetOrAddComponentInChildren<Button>("HamburgerButton").onClick.AddListener(() =>
        {
            var panel = new HamburgerPanel();
            PanelManager.Push(panel);
        });

        UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
        {
            EnterRealityEvents?.Invoke();
            //GameRoot.Instance.SceneSystem.SetScene(new MainScene());
            UITool.GetOrAddComponent<HomeUIPanel>().EnterRealityDetailPage();
            var panel = new RealityPanel();
            panel.RealityData = UITool.GetOrAddComponent<HomeUIPanel>().RealityListData.realityCollection[UITool.GetOrAddComponent<HomeUIPanel>().ActiveIndex];
            PanelManager.Push(panel);
        });
    }

    public override void OnExit()
    {
        Debug.Log("HomePage Exit");
    }
}
