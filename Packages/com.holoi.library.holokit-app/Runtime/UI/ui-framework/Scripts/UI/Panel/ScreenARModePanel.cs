using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Holoi.Library.HoloKitApp.UI
{
    public class ScreenARModePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/ScreenARModePanel";
        public ScreenARModePanel() : base(new UIType(_path)) { }


        public override void OnOpen()
        {
            UITool.GetOrAddComponentInChildren<Button>("ExitButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("ExitButton is clicked. Exit from ScreenAR Mode to RealityOptionPanel.");
                // exit to start scene
                GameRoot.Instance.SceneSystem.SetScene(new StartScene());
                PanelManager.OnExitReality?.Invoke();
            });

            UITool.GetOrAddComponentInChildren<Button>("SpectatorButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("SpectatorButton is clicked.");

                var panelUI = UITool.GetOrAddComponent<ScreenARModeUIPanel>();

                panelUI.SetState(ScreenARModeUIPanel.State.comfirmingOpenSpectator);

                var panel = new ScreenAROpenSpectatorPanel();
                PanelManager.Push(panel);

                //PanelManager.OnStartedSharingReality?.Invoke();
            });

            UITool.GetOrAddComponentInChildren<Button>("StARButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("StARButton is clicked.");

                // switch to stAR mode
                //GameRoot.Instance.SceneSystem.SetScene(new StARMainScene());

                var panel = new StARModePanel();
                PanelManager.Push(panel, false);

                PanelManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Stereo);
            });

            UITool.GetOrAddComponentInChildren<Button>("RecordButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("RecordButton is clicked.");
                // switch record button style:
                if(UITool.GetOrAddComponent<ScreenARModeUIPanel>().state == ScreenARModeUIPanel.State.recording)
                {
                    UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.idle);
                    PanelManager.OnStoppedRecording?.Invoke();
                }
                else
                {
                    UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.recording);
                    PanelManager.OnStartedRecording?.Invoke();
                }
                // start recording:

            });

            // debug for scanning -> check mark
            UITool.GetOrAddComponentInChildren<Button>("CheckMarkButton").onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("CheckMarkButton is clicked.");

                GoToCheckTheMarkPanel();
            });

            // check mark event here
            // HoloKitApp.Instance. 
        }

        public void GoToCheckTheMarkPanel()
        {
            UITool.GetOrAddComponent<ScreenARModeUIPanel>().SetState(ScreenARModeUIPanel.State.checkingMark);
            var panel = new ScreenARCheckTheMarkPanel();
            PanelManager.Push(panel);
        }
    }
}
