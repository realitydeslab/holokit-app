using System.Collections;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace HoloKit.Tests
{
    public sealed class HoverableStartButtonTriggerTests
    {
        private sealed class Probe
        {
            public HoverableStartButton Button;
            public int Triggered;
        }

        [UnityTest]
        public IEnumerator StarTriggerStartsVisibleButtonWithoutHand()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            yield return new EnterPlayMode();

            var visible = CreateButton("Visible");
            var hidden = CreateButton("Hidden");
            hidden.Button.gameObject.SetActive(false);
            yield return null;

            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            Assert.That(visible.Triggered, Is.EqualTo(1), "Trigger button must start a visible button without hand dwell.");
            Assert.That(hidden.Triggered, Is.Zero, "A button that is not shown must ignore the Trigger button.");

            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            Assert.That(visible.Triggered, Is.EqualTo(1), "A second press inside the cooldown must not start twice.");

            Object.Destroy(visible.Button.gameObject);
            Object.Destroy(hidden.Button.gameObject);
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator PhoneModeTapOnButtonStartsItButNotElsewhereOrThroughUi()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            yield return new EnterPlayMode();

            var cameraObject = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = cameraObject.AddComponent<Camera>();
            var probe = CreateButton("Start");
            probe.Button.transform.position = camera.transform.position + camera.transform.forward;
            var ui = CreateFullScreenUi();
            yield return null;

            var onButton = (Vector2)camera.WorldToScreenPoint(probe.Button.transform.position);
            Assert.That(probe.Button.TryTriggerFromScreenTap(onButton), Is.False, "A tap on app UI must not start the game.");
            Object.Destroy(ui);
            yield return null;

            Assert.That(probe.Button.TryTriggerFromScreenTap(Vector2.zero), Is.False, "A tap away from the button must not start the game.");
            Assert.That(probe.Triggered, Is.Zero);
            Assert.That(probe.Button.TryTriggerFromScreenTap(onButton), Is.True, "A tap on the button must start the game.");
            Assert.That(probe.Triggered, Is.EqualTo(1));

            Object.Destroy(probe.Button.gameObject);
            Object.Destroy(cameraObject);
            yield return new ExitPlayMode();
        }

        private static GameObject CreateFullScreenUi()
        {
            var root = new GameObject("UI");
            var canvas = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            canvas.transform.SetParent(root.transform);
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var image = new GameObject("Panel", typeof(Image));
            image.transform.SetParent(canvas.transform, false);
            var rect = image.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            new GameObject("EventSystem", typeof(EventSystem)).transform.SetParent(root.transform);
            return root;
        }

        private static Probe CreateButton(string name)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            var probe = new Probe { Button = go.AddComponent<HoverableStartButton>() };
            // Scene instances deserialize this event; give the runtime-built button a constructed one.
            var triggered = new UnityEvent();
            triggered.AddListener(() => probe.Triggered++);
            go.GetComponent<HoverableObject>().OnTriggered = triggered;
            return probe;
        }
    }
}
