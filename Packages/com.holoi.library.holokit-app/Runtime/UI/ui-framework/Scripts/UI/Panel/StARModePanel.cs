using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomePage displaly all realities and the entry to settings
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class StARModePanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/StARModePanel";
        public StARModePanel() : base(new UIType(_path)) { }


        public override void OnOpen()
        {
            var stARUIPanel = UITool.GetOrAddComponent<StarUIPanel>();
            stARUIPanel.more.onClick.AddListener(() =>
            {
                Debug.Log("More Button On Click!");
                // trigger action here:

            });
            stARUIPanel.exitBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                    Debug.Log("BackButton is clicked.");
                    PanelManager.Pop();

                    PanelManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Mono);
                }
            });
            stARUIPanel.volumeBar.onValueChanged.AddListener((value) =>
            {
                Debug.Log("current volume: " + value);
            });
            stARUIPanel.recordBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                    PanelManager.OnStartedRecording?.Invoke();
                }
            });
            stARUIPanel.triggerBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                    PanelManager.OnStARTriggered?.Invoke();
                }
            });
            stARUIPanel.boostBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                    PanelManager.OnStARBoosted?.Invoke();
                }
            });
            stARUIPanel.recalibrateBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                }
            });
            stARUIPanel.pauseBar.onValueChanged.AddListener((value) =>
            {
                if (value == 1)
                {
                    // trigger action here:
                    PanelManager.OnStARStartedPause?.Invoke();
                }
            });
        }
    }
}