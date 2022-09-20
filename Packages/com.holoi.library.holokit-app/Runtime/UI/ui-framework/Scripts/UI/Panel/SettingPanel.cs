using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class SettingPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/SettingPanel";
        public SettingPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });

            UITool.GetOrAddComponent<SettingUIPanel>().instruction.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("setting is clicked.");
                // change the setting:

            });

            UITool.GetOrAddComponent<SettingUIPanel>().vibration.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("setting is clicked.");
                // change the setting:

            });

            UITool.GetOrAddComponent<SettingUIPanel>().hdr.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("setting is clicked.");
                // change the setting:

            });

            UITool.GetOrAddComponent<SettingUIPanel>().recordResolution.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("setting is clicked.");
                // change the setting:

            });

            UITool.GetOrAddComponent<SettingUIPanel>().wifi.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("v is clicked.");
                // change the setting:

            });

            UITool.GetOrAddComponent<SettingUIPanel>().showTechInfo.GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("setting is clicked.");
                // change the setting:

            });
        }
    }
}
