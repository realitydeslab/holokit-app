// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.Library.HoloKitApp
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppConfig")]
    public class HoloKitAppConfig : ScriptableObject
    {
        // Debug Mode
        public bool GalleryViewEnabled;

        // Splash Screen Mode
        public bool LandingPageEnabled;

        // Account System Enabled
        // Apple Signin, Cloud Save and Analytics
        public bool UserAccountSystemEnabled;
    }
}
