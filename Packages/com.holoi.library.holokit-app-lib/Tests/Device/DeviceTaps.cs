#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Taps on-screen buttons the way a finger does: the tap lands on whatever the event system finds on top at the
    /// button's centre, so a covered button cannot be tapped, and the click reaches the button's own onClick wiring,
    /// not a method the test picked.
    /// </summary>
    public static class DeviceTaps
    {
        private static readonly string[] JoinPanels =
            { "MonoAR_WaitingForConnection", "MonoAR_ScanQRCode", "MonoAR_CheckAlignmentMark" };

        /// <summary>What was on top of the last button that could not be tapped because something covered it.</summary>
        public static string LastBlocker { get; private set; }

        /// <summary>
        /// Clicks the shown, interactable, uncovered button under <paramref name="root"/> whose onClick calls
        /// <paramref name="method"/>. Returns false when there is no such button.
        /// </summary>
        public static bool Tap(Component root, string method)
        {
            var button = root.GetComponentsInChildren<Button>().FirstOrDefault(b => b.IsInteractable() &&
                Enumerable.Range(0, b.onClick.GetPersistentEventCount()).Any(i => b.onClick.GetPersistentMethodName(i) == method));
            if (button == null || EventSystem.current == null)
                return false;
            var rect = (RectTransform)button.transform;
            var canvas = button.GetComponentInParent<Canvas>().rootCanvas;
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center))
            };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);
            if (hits.Count == 0 || !hits[0].gameObject.transform.IsChildOf(button.transform))
            {
                LastBlocker = hits.Count == 0 ? "nothing" : hits[0].gameObject.name;
                return false;
            }
            ExecuteEvents.Execute(button.gameObject, eventData, ExecuteEvents.pointerClickHandler);
            return true;
        }

        /// <summary>
        /// Taps the check button on the panel a client sees after scanning the host's QR code, confirming that the
        /// alignment marker sits on the host. Returns false while that panel is not shown.
        /// </summary>
        public static bool TapCheckMark()
        {
            var panel = Object.FindObjectsByType<HoloKitAppUIPanel_MonoAR_CheckAlignmentMark>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.isActiveAndEnabled);
            return panel != null && Tap(panel, nameof(HoloKitAppUIPanel_MonoAR_CheckAlignmentMark.OnCheckedButtonPressed));
        }

        /// <summary>
        /// Taps the close button on the host's QR code panel, as the host does once everyone has scanned it.
        /// Returns false while that panel is not shown.
        /// </summary>
        public static bool CloseShareQRCode()
        {
            var panel = Object.FindObjectsByType<HoloKitAppUIPanel_MonoAR_ShareQRCode>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.isActiveAndEnabled);
            return panel != null && Tap(panel, nameof(HoloKitAppUIPanel_MonoAR_ShareQRCode.OnExitButtonPressed));
        }

        /// <summary>The app's panel stack, top first, with the hidden ones marked.</summary>
        public static string PanelStack()
        {
            var manager = HoloKitApp.Instance != null ? HoloKitApp.Instance.UIPanelManager : null;
            var field = typeof(HoloKitAppUIPanelManager).GetField("_uiPanelStack", BindingFlags.NonPublic | BindingFlags.Instance);
            if (manager == null || field?.GetValue(manager) is not IEnumerable<HoloKitAppUIPanel> stack)
                return "";
            return string.Join(" > ", stack.Where(p => p != null)
                .Select(p => p.UIPanelName + (p.gameObject.activeInHierarchy ? "" : " (hidden)")));
        }

        /// <summary>
        /// A connection, scanning or alignment-check panel still shown on a client that has confirmed the alignment,
        /// covering the reality; null when there is none.
        /// </summary>
        public static string LeftoverJoinPanel() => Object.FindObjectsByType<HoloKitAppUIPanel>(FindObjectsSortMode.None)
            .Where(p => p.isActiveAndEnabled && JoinPanels.Contains(p.UIPanelName))
            .Select(p => p.UIPanelName).FirstOrDefault();
    }
}
#endif
