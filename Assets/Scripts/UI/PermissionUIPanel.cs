using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class PermissionUIPanel : MonoBehaviour
    {
        private void OnApplicationPause(bool pause)
        {
            Debug.Log($"OnApplicationQuit {pause}");
            if (!pause)
            {
                var permissionPanel = PanelManager.Instance.GetActivePanel() as PermissionPanel;
                permissionPanel.UpdateAllPermissionButtons();
            }
        }
    }
}
