using System;
using UnityEngine;

namespace Holoi.Library.HoloKitAppLib
{
    /// <summary>Each thin project supplies its own catalog and defaults in Resources.</summary>
    public static class HoloKitProductConfiguration
    {
        public const string ResourceName = "HoloKitAppGlobalSettings";

        public static HoloKitAppGlobalSettings LoadRuntimeSettings(HoloKitAppGlobalSettings fallback = null)
        {
            var template = Resources.Load<HoloKitAppGlobalSettings>(ResourceName);
            return CreateRuntimeSettings(template != null ? template : fallback);
        }

        internal static HoloKitAppGlobalSettings CreateRuntimeSettings(HoloKitAppGlobalSettings template)
        {
            if (template == null || template.AppConfig == null || template.RealityList == null ||
                template.AvatarCollectionList == null || template.ObjectCollectionList == null)
                throw new InvalidOperationException("Product settings and catalogs must be assigned in Resources/" + ResourceName + ".asset.");
            if (template.RealityList.List == null || template.RealityList.List.Count == 0 ||
                template.RealityList.List.Exists(reality => reality == null))
                throw new InvalidOperationException("The product catalog must contain at least one valid reality.");
            // Player preferences mutate the runtime copy, never a shared package asset.
            return UnityEngine.Object.Instantiate(template);
        }
    }
}
