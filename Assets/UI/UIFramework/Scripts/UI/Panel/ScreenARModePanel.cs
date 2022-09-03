using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Holoi.HoloKit.App.UI
{
    public class ScreenARModePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARModePanel";
        public ScreenARModePanel() : base(new UIType(_path)) { }

        public UnityEvent EnterRealityEvents;

        public override void OnEnter()
        {

            UITool.GetOrAddComponentInChildren<Button>("OpenSpectatorButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("OpenSpectatorButton is clicked.");

                var panel = new ScreenAROpenSpectatorPanel();
                PanelManager.Push(panel);
            });
            UITool.GetOrAddComponentInChildren<Button>("SwitchToStARButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("SwitchToStARButton is clicked.");

                // switch to stAR mode
                GameRoot.Instance.SceneSystem.SetScene(new StARMainScene());


            });
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("ExitButton is clicked. Exit from ScreenAR Mode to RealityOptionPanel.");
                PanelManager.Pop();

                // exit to start scene
                GameRoot.Instance.SceneSystem.SetScene(new StartScene());

            });

            UITool.GetOrAddComponentInChildren<Button>("StartRecordButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("StartRecordButton is clicked.");
                // switch record button style:

                // start recording:

            });
        }

        public override void OnExit()
        {
            Debug.Log("ScreenARMode Exit");
        }
    }
}
