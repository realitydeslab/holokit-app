using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HamburgerPanel : BasePanel
{
    static readonly string _path = "Prefabs/UI/Panels/HamburgerPanel";
    public HamburgerPanel() : base(new UIType(_path)) { }

    public override void OnEnter()
    {
        UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");
            PanelManager.Pop();
        });
        UITool.GetOrAddComponentInChildren<Button>("RealityButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("RealityButton is clicked.");

            var panel = new StartPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("ObjectButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("ObjectButton is clicked.");
            //GameRoot.Instance.SceneSystem.SetScene(new MainScene());
            var panel = new ObjectPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("AvatarButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("AvatarButton is clicked.");
            var panel = new AvatarPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("AboutButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("AboutButton is clicked.");
            var panel = new AboutPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("GetHolokitButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("GetHolokitButton is clicked.");
            var panel = new GetHolokitPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("GetStartButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("GetStartButton is clicked.");
            var panel = new GetStartPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("AccountButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("AccountButton is clicked.");
            var panel = new AccountPanel();
            PanelManager.Push(panel);
        });
        UITool.GetOrAddComponentInChildren<Button>("SettingButton").onClick.AddListener(() =>
        {
            // here we do onclick event of this button
            Debug.Log("SettingButton is clicked.");
            var panel = new SettingPanel();
            PanelManager.Push(panel);
        });
    }
}
