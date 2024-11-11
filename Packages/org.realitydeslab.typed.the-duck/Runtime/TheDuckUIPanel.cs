// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib.UI;
using UnityEngine.UI;
using TMPro;

namespace RealityDesignLab.Typed.TheDuck
{
    public class TheDuckUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TheDuckUIPanel";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] TMP_InputField SpeedX;
        [SerializeField] TMP_InputField SpeedY;

        void Start()
        {
            SpeedX.onValueChanged.AddListener(delegate { SpeedXValueChanged(); });
            SpeedY.onValueChanged.AddListener(delegate { SpeedYValueChanged(); });
        }

        public void SpeedXValueChanged()
        {
            var result = 0f;
            if (float.TryParse(SpeedX.text, out result))
            {
                FindObjectOfType<TypedTheDuckRealityManager>().DuckMaxSpeed.x = result;
            }
            else
            {
                // Not a number, do something else with it.
            }
        }

        public void SpeedYValueChanged()
        {
            var result = 0f;
            if (float.TryParse(SpeedY.text, out result))
            {
                FindObjectOfType<TypedTheDuckRealityManager>().DuckMaxSpeed.y = result;
            }
            else
            {
                // Not a number, do something else with it.
            }
        }
    }
}
