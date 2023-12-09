// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp
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
