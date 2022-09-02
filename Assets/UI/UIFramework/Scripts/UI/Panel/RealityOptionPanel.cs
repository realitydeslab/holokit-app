using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealityOptionPanel : BasePanel
{
    static readonly string _path = "Prefabs/UI/Panels/RealityOptionPanel";
    public RealityOptionPanel() : base(new UIType(_path)) { }

    public override void OnEnter()
    {
        UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("ExitButton is clicked.");
            PanelManager.Pop();
        });
        UITool.GetOrAddComponentInChildren<Button>("StarButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("StarButton is clicked.");

            //var panel = new StARModePanel();
            //PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("SpectatorButton is clicked.");

            var panel = new SpectatorOpenComfirmPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("RecordButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("RecordButton is clicked.");

            //var panel = new RealityOptionPanel();
            //PanelManager.Push(panel);
        });

    }
}
