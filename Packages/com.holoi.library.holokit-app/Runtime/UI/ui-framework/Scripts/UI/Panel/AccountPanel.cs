using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class AccountPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/AccountPanel";
        public AccountPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
                Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });

            UITool.FindGameObjectInChindren("LogButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("LogButton is clicked.");
            });

            UITool.FindGameObjectInChindren("ScanButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("ScanButton is clicked.");
            });
        }
    }

}