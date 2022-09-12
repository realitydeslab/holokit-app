using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class HamburgerPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/HamburgerPanel";
        public HamburgerPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked.");
                PanelManager.Pop();
                //PanelManager.GetActivePanel().UITool.GetOrAddComponent<HomeUIPanel>().HomePanelUIlayout();


            });
            UITool.GetOrAddComponentInChildren<Button>("RealityButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("RealityButton is clicked.");
                PanelManager.Pop();
                //PanelManager.GetActivePanel().UITool.GetOrAddComponent<HomeUIPanel>().HomePanelUIlayout();
            });
            UITool.GetOrAddComponentInChildren<Button>("ObjectButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ObjectButton is clicked.");
            var panel = new ObjectPackagePanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("AvatarButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("AvatarButton is clicked.");
                var panel = new AvatarPackagePanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("AboutButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("AboutButton is clicked.");
                var panel = new AboutPanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("GetButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("GetHolokitButton is clicked.");
                var panel = new GetHolokitPanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("GettingButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("GettingStartButton is clicked.");
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
}