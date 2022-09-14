using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class PermissionPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/PermissionPanel";
        public PermissionPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            var cameraButton = UITool.GetOrAddComponentInChildren<Button>("CameraButton");
            cameraButton.onClick.AddListener(() =>
            {
                //if (cameraButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.State.Checked)
                //{
                //    cameraButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Uncheck;
                //}
                //else
                //{
                //    cameraButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Checked;
                //}
            });

            var microphoneButton = UITool.GetOrAddComponentInChildren<Button>("MicrophoneButton");
            microphoneButton.onClick.AddListener(() =>
            {
                //if (microphoneButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.State.Checked)
                //{
                //    microphoneButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Uncheck;
                //}
                //else
                //{
                //    microphoneButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Checked;
                //}
            });

            var photoButton = UITool.GetOrAddComponentInChildren<Button>("PhotoButton");
            photoButton.onClick.AddListener(() =>
            {
                //if (photoButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.State.Checked)
                //{
                //    photoButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Uncheck;
                //}
                //else
                //{
                //    photoButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Checked;
                //}
            });

            var LocationButton = UITool.GetOrAddComponentInChildren<Button>("LocationButton");
            LocationButton.onClick.AddListener(() =>
            {
                //if (LocationButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.State.Checked)
                //{
                //    LocationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Uncheck;
                //}
                //else
                //{
                //    LocationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.State.Checked;
                //}
            });

            var NetworkButton = UITool.GetOrAddComponentInChildren<Button>("NetworkButton");
            NetworkButton.onClick.AddListener(() =>
            {
                if (NetworkButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.GoOnState.Checked)
                {
                    NetworkButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
                }
                else
                {
                    NetworkButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
                }
            });

            var NotificationButton = UITool.GetOrAddComponentInChildren<Button>("NotificationButton");
            NotificationButton.onClick.AddListener(() =>
            {
                if (NotificationButton.GetComponent<FlexibleUIPermissionButton>().state == FlexibleUIPermissionButton.GoOnState.Checked)
                {
                    NotificationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
                }
                else
                {
                    NotificationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
                }
            });

            var DoneButton = UITool.GetOrAddComponentInChildren<Button>("DoneButton");
            DoneButton.onClick.AddListener(() =>
            {
                if (DoneButton.GetComponent<FlexibleUIButton>().state == FlexibleUIButton.State.Active)
                {
                    DoneButton.GetComponent<FlexibleUIButton>().state = FlexibleUIButton.State.Inactive;
                }
                else
                {
                    DoneButton.GetComponent<FlexibleUIButton>().state = FlexibleUIButton.State.Active;
                }

                var panel = new SignInPanel();
                PanelManager.Push(panel);
            });

        }

    }
}