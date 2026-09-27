using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoloKit.GameTests
{
    public sealed class MOFAMobileInputPrefabTests
    {
        private const string Directory =
            "Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Prefabs/Mobile Input";

        [Test]
        public void AllMobileInputPrefabsLoadWithoutMissingScripts()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { Directory });
            Assert.That(guids.Length, Is.GreaterThan(0));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.That(prefab, Is.Not.Null, path);
                foreach (var transform in prefab.GetComponentsInChildren<Transform>(true))
                    Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject),
                        Is.Zero, path + ": " + transform.name);
            }
        }

        [TestCase("Mobile B Down")]
        [TestCase("Mobile B Up")]
        public void DirectionButtonsKeepTheirUIAndPointerEvents(string name)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Directory}/{name}.prefab");
            Assert.That(prefab, Is.Not.Null);
            var image = prefab.GetComponent<Image>();
            var button = prefab.GetComponent<Button>();
            var trigger = prefab.GetComponent<EventTrigger>();
            var label = prefab.GetComponentInChildren<Text>(true);
            Assert.That(image, Is.Not.Null);
            Assert.That(button, Is.Not.Null);
            Assert.That(trigger, Is.Not.Null);
            Assert.That(label, Is.Not.Null);
            Assert.That(button.targetGraphic, Is.SameAs(image));
            Assert.That(trigger.triggers.Count, Is.EqualTo(2));
        }
    }
}
