using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEngine;

namespace HoloKit.GameTests
{
    /// <summary>
    /// Tapping the mono-to-stereo icon goes straight into stereo unless the player turns instructions on
    /// in Settings: a fresh install must start with instructions off.
    /// </summary>
    public sealed class InstructionDefaultTests
    {
        private static HoloKitAppGlobalSettings ShippedSettings()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null, "The product has no global settings asset.");
            return settings;
        }

        [Test]
        public void ShippedSettingsHaveInstructionsOff()
        {
            Assert.That(ShippedSettings().InstructionEnabled, Is.False);
        }

        [Test]
        public void DefaultSettingsHaveInstructionsOff()
        {
            var settings = Object.Instantiate(ShippedSettings());
            try
            {
                settings.InstructionEnabled = true;
                settings.LoadDefaultSettings();
                Assert.That(settings.InstructionEnabled, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(settings);
            }
        }
    }
}
